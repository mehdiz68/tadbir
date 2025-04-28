using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CoreLib.Infrastructure.DateTime;
using PagedList;
using System.IO;
using ahmadi.Infrastructure.Helper;
using ahmadi.Infrastructure.Security;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;
using UnitOfWork;
using Microsoft.AspNet.Identity;
using Domain;
using ahmadi.ViewModels.Slider;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class FileManagerController : Controller
    {
        private UnitOfWorkClass uow = null;
        public FileManagerController()
        {
            uow = new UnitOfWorkClass();
        }

        //GET: Admin/FileManager
        #region FileManager

        [CorrectArabianLetter(new string[] { "searchString", "currentFilter" })]
        public virtual ActionResult Index(string sortOrder, string currentFilter, string searchString, string currentStartDateInputFiltering, string StartDateInput, string currentEndDateInputFiltering, string EndDateInput, string currentFileTypeIdFiltering, string FileTypeId, string currentCapacityListFiltering, string CapacityList, string currentFolderIdFiltering, string FolderId, string currentLanguageIdFiltering, string LanguageId, string currentIsActiveIdFiltering, string IsActive, int? page)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();


                    #region LoadDropdown
                    var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null).ToList();
                    ViewBag.Folders = folders;

                    ViewBag.FolderId = new SelectList(folders, "Id", "FolderName");
                    ViewBag.FileTypeId = new SelectList(uow.FiletypeRepository.Get(x => x), "Id", "FileTypeName");
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    List<SelectListItem> CapacitySelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نمایید--", Value = "All" }, new SelectListItem() { Text = "کوچک کمتر از 100 kb", Value = "small" }, new SelectListItem() { Text = "متوسط کمتر از 1 mb", Value = "medium" }, new SelectListItem() { Text = "حجیم بیشتر از 1 mb", Value = "large" } };
                    ViewBag.CapacityList = CapacitySelectListItem;

                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                    ViewBag.IsActive = IsActiveSelectListItem;
                    #endregion

                    #region Search
                    if (!string.IsNullOrEmpty(searchString))
                        page = 1;
                    else
                        searchString = currentFilter;
                    if (!string.IsNullOrEmpty(StartDateInput))
                        page = 1;
                    else
                        StartDateInput = currentStartDateInputFiltering;
                    if (!string.IsNullOrEmpty(EndDateInput))
                        page = 1;
                    else
                        EndDateInput = currentEndDateInputFiltering;
                    if (!string.IsNullOrEmpty(FileTypeId))
                        page = 1;
                    else
                        FileTypeId = currentFileTypeIdFiltering;
                    if (!string.IsNullOrEmpty(FolderId))
                        page = 1;
                    else
                        FolderId = currentFolderIdFiltering;
                    if (!string.IsNullOrEmpty(CapacityList))
                        page = 1;
                    else
                        CapacityList = currentCapacityListFiltering;
                    if (!string.IsNullOrEmpty(LanguageId))
                        page = 1;
                    else
                        LanguageId = currentLanguageIdFiltering;
                    if (!string.IsNullOrEmpty(IsActive))
                        page = 1;
                    else
                        IsActive = currentIsActiveIdFiltering;

                    ViewBag.CurrentFilter = searchString;
                    ViewBag.currentStartDateInputFiltering = StartDateInput;
                    ViewBag.currentEndDateInputFiltering = EndDateInput;
                    ViewBag.currentFileTypeIdFiltering = FileTypeId;
                    ViewBag.currentFolderIdFiltering = FolderId;
                    ViewBag.currentLanguageIdFiltering = LanguageId;
                    ViewBag.currentIsActiveIdFiltering = IsActive;
                    ViewBag.currentCapacityListFiltering = CapacityList;

                    DateTime startDate = DateTime.Now, endDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(StartDateInput))
                        startDate = DateTimeConverter.ChangeShamsiToMiladi(StartDateInput);
                    if (!String.IsNullOrEmpty(EndDateInput))
                        endDate = DateTimeConverter.ChangeShamsiToMiladi(EndDateInput);
                    int fId = Convert.ToInt32(FolderId);
                    int fiId = Convert.ToInt32(FileTypeId);
                    int lngId = Convert.ToInt32(LanguageId);
                    bool active = Convert.ToBoolean(IsActive);

                    var Attachments = uow.AttachmentRepository.GetQueryList().AsNoTracking();

                    if (!String.IsNullOrEmpty(searchString))
                        Attachments = Attachments.Where(s => s.FileName.Contains(searchString) || s.Title.Contains(searchString));

                    if (!String.IsNullOrEmpty(StartDateInput) && !String.IsNullOrEmpty(EndDateInput))
                        Attachments = Attachments.Where(s => s.InsertDate >= startDate && s.InsertDate <= endDate);
                    else if (!String.IsNullOrEmpty(StartDateInput))
                        Attachments = Attachments.Where(s => s.InsertDate >= startDate);
                    else if (!String.IsNullOrEmpty(EndDateInput))
                        Attachments = Attachments.Where(s => s.InsertDate <= endDate);

                    if (!String.IsNullOrEmpty(FolderId))
                    {
                        if (fId == 0)
                            Attachments = Attachments.Where(s => s.FolderId == null);
                        else
                            Attachments = Attachments.Where(s => s.FolderId == fId);
                    }
                    if (!String.IsNullOrEmpty(LanguageId))
                    {
                        if (lngId > 0)
                            Attachments = Attachments.Where(s => s.LanguageId == lngId);
                    }
                    if (!String.IsNullOrEmpty(IsActive))
                        Attachments = Attachments.Where(s => s.IsActive == active);
                    if (!String.IsNullOrEmpty(FileTypeId))
                        Attachments = Attachments.Where(s => s.FileTypeId == fiId);
                    if (!String.IsNullOrEmpty(CapacityList))
                    {
                        switch (CapacityList.ToLower())
                        {
                            case "small": Attachments = Attachments.Where(s => s.Capacity <= 100); break;
                            case "meduim": Attachments = Attachments.Where(s => s.Capacity <= 1024); break;
                            case "large": Attachments = Attachments.Where(s => s.Capacity > 1024); break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Title":
                            Attachments = Attachments.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Title";
                            break;
                        case "Title_desc":
                            Attachments = Attachments.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Title_desc";
                            break;
                        case "InsertDate":
                            Attachments = Attachments.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            Attachments = Attachments.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        case "FileTypeName":
                            Attachments = Attachments.OrderBy(s => s.FileName);
                            ViewBag.CurrentSort = "FileTypeName";
                            break;
                        case "FileTypeName_desc":
                            Attachments = Attachments.OrderByDescending(s => s.FileName);
                            ViewBag.CurrentSort = "FileTypeName_desc";
                            break;
                        case "Capacity":
                            Attachments = Attachments.OrderBy(s => s.Capacity);
                            ViewBag.CurrentSort = "Capacity";
                            break;
                        case "Capacity_desc":
                            Attachments = Attachments.OrderByDescending(s => s.Capacity);
                            ViewBag.CurrentSort = "Capacity_desc";
                            break;
                        case "UseCount":
                            Attachments = Attachments.OrderBy(s => s.UseCount);
                            ViewBag.CurrentSort = "UseCount";
                            break;
                        case "UseCount_desc":
                            Attachments = Attachments.OrderByDescending(s => s.UseCount);
                            ViewBag.CurrentSort = "UseCount_desc";
                            break;
                        case "HasWatermark":
                            Attachments = Attachments.OrderBy(s => s.HasWatermark);
                            ViewBag.CurrentSort = "HasWatermark";
                            break;
                        case "HasWatermark_desc":
                            Attachments = Attachments.OrderByDescending(s => s.HasWatermark);
                            ViewBag.CurrentSort = "HasWatermark_desc";
                            break;
                        case "HasMultiSize":
                            Attachments = Attachments.OrderBy(s => s.HasMultiSize);
                            ViewBag.CurrentSort = "HasMultiSize";
                            break;
                        case "HasMultiSize_desc":
                            Attachments = Attachments.OrderByDescending(s => s.HasMultiSize);
                            ViewBag.CurrentSort = "HasMultiSize_desc";
                            break;
                        default:  // Name ascending 
                            Attachments = Attachments.OrderByDescending(s => s.InsertDate);
                            break;
                    }

                    #endregion

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "فایل ها", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, "FileManager", "Index", true, 200, " نمایش صفحه مدیریت فایل ها", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Attachments.Select(x => new Domain.ViewModel.Attachement(){Id = x.Id,Capacity = x.Capacity,FileName = x.FileName,HasMultiSize = x.HasMultiSize,HasWatermark = x.HasWatermark,Title = x.Title,UseCount = x.UseCount}).ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/FileManager/Details/5
        public virtual ActionResult Details(Guid id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, null))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    attachment oattachment = uow.AttachmentRepository.Get(x=>x,x=>x.Id== id,null, "FileType").FirstOrDefault();
                    if (oattachment == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == oattachment.LanguageId).FirstOrDefault();

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, "FileManager", "Details", true, 200, " نمایش صفحه جزئیات فایلِ  " + oattachment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(oattachment);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }


        // GET: Admin/FileManager/Delete/5
        public virtual ActionResult Delete(Guid id)
        {
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    attachment oattachment = uow.AttachmentRepository.Get(x => x, x => x.Id == id, null, "FileType").FirstOrDefault();
                    if (oattachment == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == oattachment.LanguageId).FirstOrDefault();

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, "FileManager", "Delete", true, 200, " نمایش صفحه حذف فایلِ  " + oattachment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(oattachment);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }


        // POST: Admin/FileTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(Guid id)
        {

            attachment attachment = uow.AttachmentRepository.GetByID(id);
            try
            {
                bool OldMultiSize = attachment.HasMultiSize;
                string OldFileName = attachment.FileName;
                uow.AttachmentRepository.Delete(attachment);
                uow.Save();
                System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName));
                if (OldMultiSize == true)
                {
                    int startIndex = OldFileName.IndexOf("/") + 3;
                    System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName.Replace("LG_", "MD_")));
                    System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName.Replace("LG_", "SM_")));
                    System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName.Replace("LG_", "XS_")));
                }

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(4, "FileManager", "DeleteConfirmed", false, 200, "   حذف فایلِ" + attachment.FileName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                ViewBag.setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == attachment.LanguageId).FirstOrDefault();
                ViewBag.Message = ex.Message;
                return View(attachment);
            }
        }

        // GET: /TestAttachementController/Upload
        [OutputCache(Duration = 0)]
        public virtual ActionResult Upload(int? LanguageId)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "افزودن فایل", null, "HelpModuleSectionFields").FirstOrDefault();
                if (Request.IsAjaxRequest())
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.Languages = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    if (LanguageId.HasValue)
                    {
                        var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null && x.LanguageId == LanguageId).ToList();
                        ViewBag.Setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == LanguageId).FirstOrDefault();

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "FileManager", "Upload", true, 200, "باز کردن آپلود تکی فایل", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return PartialView("_UploadSingle", folders);
                    }
                    else
                    {
                        var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null && x.LanguageId == 1).ToList();
                        ViewBag.Setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1).FirstOrDefault();

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "FileManager", "Upload", true, 200, "باز کردن آپلود تکی فایل", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return PartialView("_UploadSingle", folders);
                    }
                }
                else
                {
                    return new HttpNotFoundResult();
                }

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "Upload", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }
        //
        // POST: /TestAttachementController/Upload
        [HttpPost]
        public virtual JsonResult Upload(HttpPostedFileBase uploadedFile, string Title, string UseWaterMark, string WaterMarkType, string HasMultiSize, string UseCompression, Int16 compressionLevel, string FolderId, string LanguageId, bool PopUpAttachements, string controllerName)
        {

            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "افزودن فایل", null, "HelpModuleSectionFields").FirstOrDefault();

                #region Get Extention
                string extention = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf("."));
                var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                #endregion
                if (oFiletype != null)//Check Extension is Valid?
                {
                    if (uploadedFile != null && uploadedFile.ContentLength > 0) //Check File Is Selected ?
                    {
                        #region Get File From Uploader
                        byte[] FileByteArray = new byte[uploadedFile.ContentLength];
                        uploadedFile.InputStream.Read(FileByteArray, 0, uploadedFile.ContentLength);
                        #endregion

                        #region Save To Database And Load Retated Object
                        string userid = User.Identity.GetUserId();
                        attachment newAttchment = new attachment();
                        newAttchment.UseCount = 0;
                        newAttchment.InsertDate = DateTime.Now;
                        newAttchment.IsActive = true;
                        newAttchment.DisplaySort = 1;
                        newAttchment.LanguageId = Convert.ToInt16(LanguageId);
                        //newAttchment.FileType = uploadedFile.ContentType.ToString();
                        //newAttchment.FileContent = FileByteArray;
                        newAttchment.FileName = uploadedFile.FileName;
                        newAttchment.FileTypeId = oFiletype.Id;
                        newAttchment.Capacity = uploadedFile.ContentLength / 1024;
                        newAttchment.UserId = userid;
                        if (!string.IsNullOrEmpty(Title))
                            newAttchment.Title = Title;
                        else
                            newAttchment.Title = " بی نام ";
                        if (FolderId != "0")
                            newAttchment.FolderId = Convert.ToInt32(FolderId);
                        if (UseWaterMark == "on")
                            newAttchment.HasWatermark = true;
                        else
                            newAttchment.HasWatermark = false;
                        if (HasMultiSize == "on")
                            newAttchment.HasMultiSize = true;
                        else
                            newAttchment.HasMultiSize = false;
                        uow.AttachmentRepository.Insert(newAttchment);
                        uow.Save();


                        attachment upAttachement = uow.AttachmentRepository.GetByID(newAttchment.Id);
                        string fileName = "LG_" + newAttchment.Id.ToString() + uow.FiletypeRepository.GetByID(newAttchment.FileTypeId).FileTypeName;

                        if (controllerName != "filemanager")
                            upAttachement.FileName = controllerName + "/" + fileName;
                        else
                            upAttachement.FileName = fileName;
                        uow.Save();

                        #endregion

                        #region Upload File To Folder
                        string targetFolder = "";
                        if (controllerName != "filemanager")
                            targetFolder = Server.MapPath("~/Content/UploadFiles/" + controllerName);
                        else
                            targetFolder = Server.MapPath("~/Content/UploadFiles");
                        string targetPath = Path.Combine(targetFolder, fileName);
                        uploadedFile.SaveAs(targetPath);
                        #endregion

                        #region Watermark Compression MultiSize
                        string UploadPath = "~/Content/UploadFiles/" + (controllerName != "filemanager" ? controllerName + "/" : "");
                        if (CoreLib.Infrastructure.Image.ImageClass.IsFileAnImage(UploadPath + fileName))
                        {
                            int langId = int.Parse(LanguageId);
                            Setting oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == langId, null, "attachment,Waterattachment").SingleOrDefault();
                            if (newAttchment.HasWatermark)
                            {
                                string result = "";
                                if (newAttchment.HasMultiSize)
                                {
                                    if (UseCompression == "on")
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    else
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                }
                                else
                                {
                                    if (UseCompression == "on")
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    else
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                }
                            }
                            else
                            {
                                string result = "";
                                if (newAttchment.HasMultiSize)
                                {
                                    if (UseCompression == "on")
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, true, compressionLevel, oSetting.LargeImageWaremark);
                                    else
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, false, compressionLevel, oSetting.LargeImageWaremark);
                                }
                                else
                                {
                                    if (UseCompression == "on")
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, true, compressionLevel, oSetting.LargeImageWaremark);
                                    else
                                        result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, false, compressionLevel, oSetting.LargeImageWaremark);
                                }

                            }
                        }
                        #endregion

                        #region Return Json Of Added File

                        var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        string HTMLString = "";
                        if (PopUpAttachements)
                            HTMLString = CaptureHelper.RenderViewToString("_AttachmentSimpleItem", newAttchment, this.ControllerContext);
                        else
                            HTMLString = CaptureHelper.RenderViewToString("_AttachmentItem", newAttchment, this.ControllerContext);

                        FileInfo fi = new FileInfo(Server.MapPath("~/Content/UploadFiles/" + newAttchment.FileName));
                        newAttchment.Capacity = Convert.ToInt32(fi.Length / 1024);
                        uow.AttachmentRepository.Update(newAttchment);
                        uow.Save();

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(2, "FileManager", "Upload", false, 200, "ایجاد فایلِ " + newAttchment.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return Json(new
                        {
                            statusCode = 200,
                            status = "فایل با موفقیت ثبت شد و هم اکنون می توانید از آن استفاده نمایید",
                            successCounter = 1,
                            NewRow = HTMLString
                        }, JsonRequestBehavior.AllowGet);
                        #endregion

                    }
                    else
                    {
                        #region File Not Selected
                        return Json(new
                        {
                            statusCode = 400,
                            status = "فایل خود را انتخاب نکرید. یا فایل انتخابی مشکل دارد.",
                            successCounter = 0,
                            file = string.Empty
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }
                }
                else
                {
                    #region File Extention Not Valid
                    return Json(new
                    {
                        statusCode = 400,
                        status = "پسوند فایل مجاز نیست. برای کسب اطلاعات بیشتر به مدیریت پسوند ها مراجعه کنید",
                        successCounter = 0,
                        file = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                    #endregion
                }
            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "Upload", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                #region Unexpected Error
                return Json(new
                {
                    statusCode = 400,
                    status = x.Message,
                    successCounter = 0,
                    file = uploadedFile.FileName
                }, JsonRequestBehavior.AllowGet);
                #endregion
            }

        }

        // GET: /TestAttachementController/Upload
        public virtual ActionResult UplodMultiple(int? LanguageId)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "افزودن فایل", null, "HelpModuleSectionFields").FirstOrDefault();

                if (Request.IsAjaxRequest())
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.Languages = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    if (LanguageId.HasValue)
                    {
                        var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null && x.LanguageId == LanguageId).ToList();
                        ViewBag.Setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == LanguageId).FirstOrDefault();

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "FileManager", "UplodMultiple", true, 200, "باز کردن آپلود گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return PartialView("_UplodMultiple", folders);
                    }
                    else
                    {
                        var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null && x.LanguageId == 1).ToList();
                        ViewBag.Setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1).FirstOrDefault();

                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(1, "FileManager", "UplodMultiple", true, 200, "باز کردن آپلود گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return PartialView("_UplodMultiple", folders);
                    }
                }
                else
                {
                    return new HttpNotFoundResult();
                }

            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "UplodMultiple", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }


        }

        [HttpPost]
        public virtual JsonResult UplodMultiple(HttpPostedFileBase[] uploadedFiles, string Title, string UseWaterMark, string WaterMarkType, string HasMultiSize, string UseCompression, Int16 compressionLevel, string FolderId, string LanguageId, bool PopUpAttachements, string controllerName)
        {

            try
            {
                int? folderid = null;
                if (FolderId != "0")
                    folderid = Convert.ToInt32(FolderId);
                string messages = "";
                List<attachment> newAttachmentList = new List<attachment>();
                string result = UploadMultipleFile(uploadedFiles, new string[] { Title }, UseWaterMark, WaterMarkType, HasMultiSize, UseCompression, compressionLevel, folderid, LanguageId, PopUpAttachements, controllerName, out messages, out newAttachmentList);

                if (result != "")
                {
                    var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    string HTMLString = "";
                    if (PopUpAttachements)
                        HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulkSimple", newAttachmentList, this.ControllerContext);
                    else
                        HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulk", newAttachmentList, this.ControllerContext);


                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "FileManager", "UplodMultiple", false, 200, "ایجاد گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 200,
                        successCounter = 1,
                        status = messages,
                        NewRow = HTMLString
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 400,
                        successCounter = 0,
                        status = messages
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "UplodMultiple", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                #region Unexpected Error
                return Json(new
                {
                    statusCode = 400,
                    successCounter = 0,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);
                #endregion
            }


        }
        [HttpPost]
        public virtual JsonResult UplodMultipleFileFromComputer(HttpPostedFileBase[] uploadedFiles, string UseWaterMark, int ProductCatId, string controllerName)
        {

            try
            {
                int? folderId = null;
                if (ProductCatId != 0)
                {
                    var productcat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == ProductCatId, null, "ParentCat").First();
                    var folder = uow.FolderRepository.Get(x => x, x => x.FolderName == productcat.Name).FirstOrDefault();
                    if (folder != null)
                        folderId = folder.Id;
                    else
                    {
                        Folder newfolder = new Folder();
                        newfolder.FolderName = productcat.Name;
                        if (productcat.ParrentId.HasValue)
                        {
                            var parentfolder = uow.FolderRepository.Get(x => x, x => x.FolderName == productcat.ParentCat.Name);
                            if (parentfolder.Any())
                                newfolder.FolderID = parentfolder.FirstOrDefault().Id;
                        }
                        newfolder.LanguageId = uow.SettingRepository.Get(x => x).First().LanguageId;
                        uow.FolderRepository.Insert(newfolder);
                        uow.Save();
                        folderId = newfolder.Id;
                    }
                }
                string messages = "";
                List<attachment> newAttachmentList = new List<attachment>();
                var setting = uow.SettingRepository.Get(x => x).First();
                string result = UploadMultipleFile(uploadedFiles, uploadedFiles.Select(x => x.FileName.Substring(0, x.FileName.LastIndexOf("."))).ToArray(), UseWaterMark, setting.WaterMarkPosition.ToString(), "on", "off", 9, folderId, setting.LanguageId.Value.ToString(), true, controllerName, out messages, out newAttachmentList);

                if (result != "")
                {
                    var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    string HTMLString = "";
                    HTMLString = CaptureHelper.RenderViewToString("_AttachmentJanebi", newAttachmentList, this.ControllerContext);


                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "FileManager", "UplodMultiple", false, 200, "ایجاد گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 200,
                        successCounter = 1,
                        status = messages,
                        NewRow = HTMLString
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 400,
                        successCounter = 0,
                        status = messages
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "UplodMultiple", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                #region Unexpected Error
                return Json(new
                {
                    statusCode = 400,
                    successCounter = 0,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);
                #endregion
            }


        }

        protected string UploadMultipleFile(HttpPostedFileBase[] uploadedFiles, string[] Title, string UseWaterMark, string WaterMarkType, string HasMultiSize, string UseCompression, Int16 compressionLevel, int? FolderId, string LanguageId, bool PopUpAttachements, string controllerName, out string messages, out List<attachment> newAttachmentList)
        {
            messages = "";
            string HTMLString = "";
            newAttachmentList = new List<attachment>();
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "افزودن فایل", null, "HelpModuleSectionFields").FirstOrDefault();

                int success = uploadedFiles.Count();
                int i = 1;
                foreach (var File in uploadedFiles)
                {
                    if (File != null && File.ContentLength > 0)//Check File Is Selected ?
                    {
                        #region Get Extention
                        string extention = File.FileName.Substring(File.FileName.LastIndexOf("."));
                        var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                        #endregion
                        if (oFiletype != null)//Check Extension is Valid?
                        {
                            #region Get File From Uploader
                            byte[] FileByteArray = new byte[File.ContentLength];
                            File.InputStream.Read(FileByteArray, 0, File.ContentLength);
                            #endregion

                            #region Save To Database And Load Retated Object
                            string userid = User.Identity.GetUserId();
                            attachment newAttchment = new attachment();
                            newAttchment.UseCount = 0;
                            newAttchment.InsertDate = DateTime.Now;
                            newAttchment.IsActive = true;
                            newAttchment.DisplaySort = 1;
                            newAttchment.LanguageId = Convert.ToInt16(LanguageId);
                            //newAttchment.FileType = uploadedFile.ContentType.ToString();
                            //newAttchment.FileContent = FileByteArray;
                            newAttchment.FileName = File.FileName;
                            newAttchment.FileTypeId = oFiletype.Id;
                            newAttchment.Capacity = File.ContentLength / 1024;
                            newAttchment.UserId = userid;
                            if (Title.Length > 1)
                            {
                                if (!string.IsNullOrEmpty(Title[(i - 1)]))
                                    newAttchment.Title = Title[(i - 1)];
                                else
                                    newAttchment.Title = " بی نام";
                            }
                            else
                            {
                                newAttchment.Title = Title[0];
                            }
                            if (FolderId.HasValue)
                                newAttchment.FolderId = FolderId.Value;
                            if (UseWaterMark == "on")
                                newAttchment.HasWatermark = true;
                            else
                                newAttchment.HasWatermark = false;
                            if (HasMultiSize == "on")
                                newAttchment.HasMultiSize = true;
                            else
                                newAttchment.HasMultiSize = false;
                            uow.AttachmentRepository.Insert(newAttchment);
                            uow.Save();
                            newAttachmentList.Add(newAttchment);

                            attachment upAttachement = uow.AttachmentRepository.GetByID(newAttchment.Id);
                            string fileName = "LG_" + newAttchment.Id.ToString() + uow.FiletypeRepository.GetByID(newAttchment.FileTypeId).FileTypeName;

                            if (controllerName != "filemanager")
                                upAttachement.FileName = controllerName + "/" + fileName;
                            else
                                upAttachement.FileName = fileName;
                            uow.Save();


                            //uow.Entry(newAttchment).Reference(c => c.FileType).Load();
                            //uow.Entry(newAttchment).Reference(c => c.Folder).Load();
                            #endregion

                            #region Upload File To Folder
                            string targetFolder = "";
                            if (controllerName != "filemanager")
                                targetFolder = Server.MapPath("~/Content/UploadFiles/" + controllerName);
                            else
                                targetFolder = Server.MapPath("~/Content/UploadFiles");
                            string targetPath = Path.Combine(targetFolder, fileName);
                            File.SaveAs(targetPath);
                            #endregion

                            #region Watermark Compression MultiSize
                            string UploadPath = "~/Content/UploadFiles/" + (controllerName != "filemanager" ? controllerName + "/" : "");
                            if (CoreLib.Infrastructure.Image.ImageClass.IsFileAnImage(UploadPath + fileName))
                            {
                                int langId = int.Parse(LanguageId);
                                Setting oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == langId, null, "Waterattachment,attachment").SingleOrDefault();
                                if (newAttchment.HasWatermark)
                                {
                                    string result = "";
                                    if (newAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                }
                                else
                                {
                                    string result = "";
                                    if (newAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }

                                }
                            }
                            #endregion


                            FileInfo fi = new FileInfo(Server.MapPath("~/Content/UploadFiles/" + newAttchment.FileName));
                            newAttchment.Capacity = Convert.ToInt32(fi.Length / 1024);
                            uow.AttachmentRepository.Update(newAttchment);
                            uow.Save();

                            messages += i.ToString() + "- " + "فایلِ " + File.FileName + " با موفقیت ذخیره شد. ";
                        }
                        else
                        {
                            success--;
                            messages += i.ToString() + "- " + " پسوندِ فایلِ  " + File.FileName + " مجاز نیست. برای کسب اطلاعات بیشتر به مدیریت پسوند ها مراجعه کنید، ";
                        }
                    }
                    else
                    {
                        success--;
                        messages += i.ToString() + "- " + "  فایل خراب است،  ";
                    }
                    i++;



                }//for each

                if (success > 0)
                {
                    var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    if (PopUpAttachements)
                        HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulkSimple", newAttachmentList, this.ControllerContext);
                    else
                        HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulk", newAttachmentList, this.ControllerContext);


                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "FileManager", "UplodMultiple", false, 200, "ایجاد گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion

                }

                return HTMLString;
            }
            catch (Exception x)
            {
                return "";
            }
        }

        // GET: Admin/FileManager/Edit/5
        public virtual ActionResult Edit(Guid? id)
        {

            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "ویرایش فایل", null, "HelpModuleSectionFields").FirstOrDefault();


                if (ModulePermission.check(User.Identity.GetUserId(), 8, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    attachment oattachment = uow.AttachmentRepository.Get(x => x, x => x.Id == id, null, "FileType").FirstOrDefault();
                    if (oattachment == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.Folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null).ToList();

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, "FileManager", "Edit", true, 200, " نمایش صفحه ویرایش فایلِ" + oattachment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(oattachment);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/FileManager/Edit/5
        [HttpPost]
        public virtual ActionResult Edit([Bind(Include = "Id,Title,IsActive,DisplaySort")] attachment attachment, int FolderId)
        {

            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "ویرایش فایل", null, "HelpModuleSectionFields").FirstOrDefault();

                if (ModelState.IsValid)
                {
                    attachment.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(attachment.Title);
                    attachment Oattachment = uow.AttachmentRepository.GetByID(attachment.Id);
                    Oattachment.Title = attachment.Title;
                    Oattachment.IsActive = attachment.IsActive;
                    Oattachment.DisplaySort = attachment.DisplaySort;
                    if (FolderId == 0)
                        Oattachment.FolderId = null;
                    else
                        Oattachment.FolderId = FolderId;

                    uow.AttachmentRepository.Update(Oattachment);
                    uow.Save();

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(3, "FileManager", "Edit", false, 200, "   ویرایش فایلِ " + attachment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "Edit", false, 500, "   خطا در ویرایش فایلِ " + attachment.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(attachment);
            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #endregion

        #region ShowPopup
        [CorrectArabianLetter(new string[] { "searchString", "currentFilter" })]
        public virtual ActionResult OpenAttachement(string sortOrder, string currentFilter, string searchString, string currentStartDateInputFiltering, string StartDateInput, string currentEndDateInputFiltering, string EndDateInput, string currentFileTypeIdFiltering, string FileTypeId, string currentCapacityListFiltering, string CapacityList, string currentFolderIdFiltering, string FolderId, string currentLanguageIdFiltering, string LanguageId, string currentIsActiveIdFiltering, string IsActive, int? page)
        {



            ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "انتخاب یا آپلود فایل", null, "HelpModuleSectionFields").FirstOrDefault();

            if (Request.IsAjaxRequest())
            {
                var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();


                    #region LoadDropdown
                    var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null).ToList();
                    ViewBag.Folders = folders;

                    ViewBag.FileTypeId = new SelectList(uow.FiletypeRepository.Get(x => x), "Id", "FileTypeName");
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    List<SelectListItem> CapacitySelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "--انتخاب نمایید--", Value = "All" }, new SelectListItem() { Text = "کوچک کمتر از 100 kb", Value = "small" }, new SelectListItem() { Text = "متوسط کمتر از 1 mb", Value = "medium" }, new SelectListItem() { Text = "حجیم بیشتر از 1 mb", Value = "large" } };
                    ViewBag.CapacityList = CapacitySelectListItem;

                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                    ViewBag.IsActive = IsActiveSelectListItem;
                    #endregion

                    #region Search
                    if (!string.IsNullOrEmpty(searchString))
                        page = 1;
                    else
                        searchString = currentFilter;
                    if (!string.IsNullOrEmpty(StartDateInput))
                        page = 1;
                    else
                        StartDateInput = currentStartDateInputFiltering;
                    if (!string.IsNullOrEmpty(EndDateInput))
                        page = 1;
                    else
                        EndDateInput = currentEndDateInputFiltering;
                    if (!string.IsNullOrEmpty(FileTypeId))
                        page = 1;
                    else
                        FileTypeId = currentFileTypeIdFiltering;
                    if (!string.IsNullOrEmpty(FolderId))
                        page = 1;
                    else
                        FolderId = currentFolderIdFiltering;
                    if (!string.IsNullOrEmpty(CapacityList))
                        page = 1;
                    else
                        CapacityList = currentCapacityListFiltering;
                    if (!string.IsNullOrEmpty(LanguageId))
                        page = 1;
                    else
                        LanguageId = currentLanguageIdFiltering;
                    if (!string.IsNullOrEmpty(IsActive))
                        page = 1;
                    else
                        IsActive = currentIsActiveIdFiltering;

                    ViewBag.CurrentFilter = searchString;
                    ViewBag.currentStartDateInputFiltering = StartDateInput;
                    ViewBag.currentEndDateInputFiltering = EndDateInput;
                    ViewBag.currentFileTypeIdFiltering = FileTypeId;
                    ViewBag.currentFolderIdFiltering = FolderId;
                    ViewBag.currentLanguageIdFiltering = LanguageId;
                    ViewBag.currentIsActiveIdFiltering = IsActive;
                    ViewBag.currentCapacityListFiltering = CapacityList;

                    DateTime startDate = DateTime.Now, endDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(StartDateInput))
                        startDate = DateTimeConverter.ChangeShamsiToMiladi(StartDateInput);
                    if (!String.IsNullOrEmpty(EndDateInput))
                        endDate = DateTimeConverter.ChangeShamsiToMiladi(EndDateInput);
                    int fId = Convert.ToInt32(FolderId);
                    int fiId = Convert.ToInt32(FileTypeId);
                    int lngId = Convert.ToInt32(LanguageId);
                    bool active = Convert.ToBoolean(IsActive);

                    var Attachments = uow.AttachmentRepository.GetQueryList().AsNoTracking();

                    if (!String.IsNullOrEmpty(searchString))
                        Attachments = Attachments.Where(s => s.FileName.Contains(searchString) || s.Title.Contains(searchString));

                    if (!String.IsNullOrEmpty(StartDateInput) && !String.IsNullOrEmpty(EndDateInput))
                        Attachments = Attachments.Where(s => s.InsertDate >= startDate && s.InsertDate <= endDate);
                    else if (!String.IsNullOrEmpty(StartDateInput))
                        Attachments = Attachments.Where(s => s.InsertDate >= startDate);
                    else if (!String.IsNullOrEmpty(EndDateInput))
                        Attachments = Attachments.Where(s => s.InsertDate <= endDate);

                    if (!String.IsNullOrEmpty(FolderId))
                    {
                        List<int?> ids = new List<int?>();
                        ids.Add(fId);
                        var folder = uow.FolderRepository.Get(x => x, x => x.Id == fId, null, "ChildFolder").First();
                        foreach (var item in folder.ChildFolder)
                        {
                            ids.Add(item.Id);
                            var subfolder = uow.FolderRepository.Get(x => x, x => x.Id == fId, null, "ChildFolder").First();
                            foreach (var subfolderitem in subfolder.ChildFolder)
                            {
                                ids.Add(subfolderitem.Id);
                                var subfolder2 = uow.FolderRepository.Get(x => x, x => x.Id == fId, null, "ChildFolder").First();
                                foreach (var subfolder2item in subfolder2.ChildFolder)
                                {
                                    ids.Add(subfolder2.Id);
                                    var subfolder3 = uow.FolderRepository.Get(x => x, x => x.Id == fId, null, "ChildFolder").First();
                                    foreach (var subfolder3item in subfolder3.ChildFolder)
                                    {
                                        ids.Add(subfolder3item.Id);
                                    }
                                }
                            }

                        }
                        if (!ids.Any())
                            Attachments = Attachments.Where(s => s.FolderId == null);
                        else
                            Attachments = Attachments.Where(s => ids.Contains(s.FolderId));
                    }
                    if (!String.IsNullOrEmpty(LanguageId))
                    {
                        if (lngId > 0)
                        {
                            Attachments = Attachments.Where(s => s.LanguageId == lngId);
                            ViewBag.StaticContentDomain = uow.SettingRepository.Get(x => x, x => x.LanguageId == lngId).Single().StaticContentDomain;
                        }
                        else
                            ViewBag.StaticContentDomain = uow.SettingRepository.Get(x => x).FirstOrDefault().StaticContentDomain;
                    }
                    else
                        ViewBag.StaticContentDomain = uow.SettingRepository.Get(x => x).FirstOrDefault().StaticContentDomain;

                    if (!String.IsNullOrEmpty(IsActive))
                        Attachments = Attachments.Where(s => s.IsActive == active);
                    if (!String.IsNullOrEmpty(FileTypeId))
                        Attachments = Attachments.Where(s => s.FileTypeId == fiId);
                    if (!String.IsNullOrEmpty(CapacityList))
                    {
                        switch (CapacityList.ToLower())
                        {
                            case "small": Attachments = Attachments.Where(s => s.Capacity <= 100); break;
                            case "meduim": Attachments = Attachments.Where(s => s.Capacity <= 1024); break;
                            case "large": Attachments = Attachments.Where(s => s.Capacity > 1024); break;
                            default:
                                break;
                        }
                    }
                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Title":
                            Attachments = Attachments.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Title";
                            break;
                        case "Title_desc":
                            Attachments = Attachments.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Title_desc";
                            break;
                        case "InsertDate":
                            Attachments = Attachments.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            Attachments = Attachments.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        case "FileTypeName":
                            Attachments = Attachments.OrderBy(s => s.FileName);
                            ViewBag.CurrentSort = "FileTypeName";
                            break;
                        case "FileTypeName_desc":
                            Attachments = Attachments.OrderByDescending(s => s.FileName);
                            ViewBag.CurrentSort = "FileTypeName_desc";
                            break;
                        case "Capacity":
                            Attachments = Attachments.OrderBy(s => s.Capacity);
                            ViewBag.CurrentSort = "Capacity";
                            break;
                        case "Capacity_desc":
                            Attachments = Attachments.OrderByDescending(s => s.Capacity);
                            ViewBag.CurrentSort = "Capacity_desc";
                            break;
                        case "UseCount":
                            Attachments = Attachments.OrderBy(s => s.UseCount);
                            ViewBag.CurrentSort = "UseCount";
                            break;
                        case "UseCount_desc":
                            Attachments = Attachments.OrderByDescending(s => s.UseCount);
                            ViewBag.CurrentSort = "UseCount_desc";
                            break;
                        case "HasWatermark":
                            Attachments = Attachments.OrderBy(s => s.HasWatermark);
                            ViewBag.CurrentSort = "HasWatermark";
                            break;
                        case "HasWatermark_desc":
                            Attachments = Attachments.OrderByDescending(s => s.HasWatermark);
                            ViewBag.CurrentSort = "HasWatermark_desc";
                            break;
                        case "HasMultiSize":
                            Attachments = Attachments.OrderBy(s => s.HasMultiSize);
                            ViewBag.CurrentSort = "HasMultiSize";
                            break;
                        case "HasMultiSize_desc":
                            Attachments = Attachments.OrderByDescending(s => s.HasMultiSize);
                            ViewBag.CurrentSort = "HasMultiSize_desc";
                            break;
                        default:  // Name ascending 
                            Attachments = Attachments.OrderByDescending(s => s.InsertDate);
                            break;
                    }

                    #endregion

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return PartialView("_FileManager", Attachments.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            else
            {
                return new HttpNotFoundResult();
            }

        }
        public virtual ActionResult AddAttachement(int? LanguageId)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "افزودن فایل", null, "HelpModuleSectionFields").FirstOrDefault();

            if (Request.IsAjaxRequest())
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.Languages = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                if (LanguageId.HasValue)
                {
                    var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null && x.LanguageId == LanguageId).ToList();
                    ViewBag.Setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == LanguageId).FirstOrDefault();
                    return PartialView("_UplodMultiple", folders);
                }
                else
                {
                    var folders = uow.FolderRepository.Get(x => x, x => x.ParrentFolder == null && x.LanguageId == 1).ToList();
                    ViewBag.Setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1).FirstOrDefault();
                    return PartialView("_UplodMultiple", folders);
                }
            }
            else
            {
                return new HttpNotFoundResult();
            }
        }
        public virtual JsonResult UpdateUseCount(Guid AttachementId)
        {

            try
            {
                var attachement = uow.AttachmentRepository.GetByID(AttachementId);
                attachement.UseCount++;
                uow.Save();
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public virtual JsonResult AddToJanebi(string ID, string Src, string Title)
        {

            try
            {
                List<OtherImageViewModel> JanebiList = new List<OtherImageViewModel>();
                if (Session["JanebiList"] != null)
                    JanebiList = (List<OtherImageViewModel>)Session["JanebiList"];
                OtherImageViewModel Newjanebi = new OtherImageViewModel();
                Newjanebi.Cover = new Guid(ID);
                Newjanebi.Src = Src;
                Newjanebi.Title = Title;
                Newjanebi.DisplaySort = (JanebiList.Count() + 1);
                JanebiList.Add(Newjanebi);
                Session["JanebiList"] = JanebiList;
                return Json(new
                {
                    message = "انجام شد",
                    data = JanebiList,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }
        public virtual JsonResult RemoveFromJanebi(string Cover)
        {

            try
            {
                List<OtherImageViewModel> JanebiList = new List<OtherImageViewModel>();
                if (Session["JanebiList"] != null)
                {
                    JanebiList = (List<OtherImageViewModel>)Session["JanebiList"];
                    OtherImageViewModel Found = null;
                    foreach (var item in JanebiList)
                    {
                        if (item.Cover == new Guid(Cover))
                        {
                            Found = item;
                        }
                    }
                    JanebiList.Remove(Found);
                    Session["JanebiList"] = JanebiList;
                    if (JanebiList.Count == 0)
                        Session["JanebiList"] = null;

                    return Json(new
                    {
                        message = "انجام شد",
                        data = JanebiList,
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        message = "لیست تصاویر جانبی خالی است",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region UploadFileForEdit
        [HttpPost]
        [CorrectArabianLetter(new string[] { "Title" })]
        public virtual JsonResult UploadFileForEdit(HttpPostedFileBase uploadedFile, string Title, string UseWaterMark, string WaterMarkType, string HasMultiSize, string UseCompression, Int16 compressionLevel, string FolderId, string LanguageId, string controllerName, string AttachementId, bool EditOrAdd)
        {

            try
            {
                #region Get Extention
                string extention = uploadedFile.FileName.Substring(uploadedFile.FileName.LastIndexOf("."));
                var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                #endregion
                if (oFiletype != null)//Check Extension is Valid?
                {
                    if (uploadedFile != null && uploadedFile.ContentLength > 0) //Check File Is Selected ?
                    {
                        if (EditOrAdd == true)
                        {
                            #region Get File From Uploader
                            byte[] FileByteArray = new byte[uploadedFile.ContentLength];
                            uploadedFile.InputStream.Read(FileByteArray, 0, uploadedFile.ContentLength);
                            #endregion

                            #region Update Database Field And Load Retated Object

                            string userid = User.Identity.GetUserId();
                            attachment UpAttchment = uow.AttachmentRepository.Get(x => x, x => x.Id == new Guid(AttachementId), null, "FileType").SingleOrDefault();
                            string OldFileName = UpAttchment.FileName;
                            bool OldMultiSize = UpAttchment.HasMultiSize;

                            UpAttchment.UpdateDate = DateTime.Now;
                            UpAttchment.LanguageId = Convert.ToInt16(LanguageId);
                            UpAttchment.FileTypeId = oFiletype.Id;
                            UpAttchment.Capacity = uploadedFile.ContentLength / 1024;
                            UpAttchment.UserId = userid;
                            if (!string.IsNullOrEmpty(Title))
                                UpAttchment.Title = Title;
                            if (FolderId != "0")
                                UpAttchment.FolderId = Convert.ToInt32(FolderId);
                            if (UseWaterMark == "on")
                                UpAttchment.HasWatermark = true;
                            else
                                UpAttchment.HasWatermark = false;
                            if (HasMultiSize == "on")
                                UpAttchment.HasMultiSize = true;
                            else
                                UpAttchment.HasMultiSize = false;

                            string fileName = "LG_" + UpAttchment.Id.ToString() + UpAttchment.FileType.FileTypeName;
                            UpAttchment.FileName = fileName;
                            uow.Save();

                            //db.Entry(UpAttchment).Reference(c => c.FileType).Load();
                            //db.Entry(UpAttchment).Reference(c => c.Folder).Load();
                            #endregion

                            #region Delete Old File And Upload New File To Folder
                            //Delete
                            System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName));
                            if (OldMultiSize == true)
                            {
                                int startIndex = OldFileName.IndexOf("/") + 3;
                                System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName.Replace("LG_", "MD_")));
                                System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName.Replace("LG_", "SM_")));
                                System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + OldFileName.Replace("LG_", "XS_")));
                            }
                            //Upload
                            string targetFolder, UploadPath = "";
                            if (UpAttchment.FileName.IndexOf("/") > 0)
                            {
                                targetFolder = Server.MapPath("~/Content/UploadFiles/" + UpAttchment.FileName.Substring(0, UpAttchment.FileName.IndexOf("/")));
                                UploadPath = "~/Content/UploadFiles/" + UpAttchment.FileName.Substring(0, UpAttchment.FileName.IndexOf("/")) + "/";
                            }
                            else
                            {
                                targetFolder = Server.MapPath("~/Content/UploadFiles");
                                UploadPath = "~/Content/UploadFiles/";
                            }
                            string targetPath = Path.Combine(targetFolder, fileName);
                            uploadedFile.SaveAs(targetPath);
                            #endregion

                            #region Watermark Compression MultiSize
                            if (CoreLib.Infrastructure.Image.ImageClass.IsFileAnImage(UploadPath + fileName))
                            {
                                int langId = int.Parse(LanguageId);
                                Setting oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == langId, null, "attachment,Waterattachment").SingleOrDefault();
                                if (UpAttchment.HasWatermark)
                                {
                                    string result = "";
                                    if (UpAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                }
                                else
                                {
                                    string result = "";
                                    if (UpAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }

                                }
                            }
                            #endregion

                            #region Clear Cache AND Return Json Of Added File 


                            FileInfo fi = new FileInfo(Server.MapPath("~/Content/UploadFiles/" + UpAttchment.FileName));
                            UpAttchment.Capacity = Convert.ToInt32(fi.Length / 1024);
                            uow.AttachmentRepository.Update(UpAttchment);
                            uow.Save();

                            HttpContext.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                            HttpContext.Response.AddHeader("Pragma", "no-cache");
                            HttpContext.Response.AddHeader("Expires", "0");
                            Session["settingPersian"] = null;
                            Session["Languages"] = null;
                            Session["HomePage"] = null;
                            Session["submenu"] = null;

                            var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                            ViewBag.AddPermission = p.First();
                            ViewBag.EditPermission = p.Skip(1).First();
                            ViewBag.DeletePermission = p.Skip(2).First();

                            return Json(new
                            {
                                statusCode = 200,
                                status = "فایل فایل با موفقیت ویرایش شد",
                                src = UpAttchment.FileName,
                                ID = UpAttchment.Id,
                                EditOrAdd = EditOrAdd
                            }, JsonRequestBehavior.AllowGet);
                            #endregion
                        }
                        else
                        {
                            #region Get File From Uploader
                            byte[] FileByteArray = new byte[uploadedFile.ContentLength];
                            uploadedFile.InputStream.Read(FileByteArray, 0, uploadedFile.ContentLength);
                            #endregion

                            #region Save To Database And Load Retated Object
                            string userid = User.Identity.GetUserId();
                            attachment newAttchment = new attachment();
                            newAttchment.UseCount = 0;
                            newAttchment.InsertDate = DateTime.Now;
                            newAttchment.IsActive = true;
                            newAttchment.DisplaySort = 1;
                            newAttchment.LanguageId = Convert.ToInt16(LanguageId);
                            //newAttchment.FileType = uploadedFile.ContentType.ToString();
                            //newAttchment.FileContent = FileByteArray;
                            newAttchment.FileName = uploadedFile.FileName;
                            newAttchment.FileTypeId = oFiletype.Id;
                            newAttchment.Capacity = uploadedFile.ContentLength / 1024;
                            newAttchment.UserId = userid;
                            if (!string.IsNullOrEmpty(Title))
                                newAttchment.Title = Title;
                            else
                                newAttchment.Title = " بی نام ";
                            if (FolderId != "0")
                                newAttchment.FolderId = Convert.ToInt32(FolderId);
                            if (UseWaterMark == "on")
                                newAttchment.HasWatermark = true;
                            else
                                newAttchment.HasWatermark = false;
                            if (HasMultiSize == "on")
                                newAttchment.HasMultiSize = true;
                            else
                                newAttchment.HasMultiSize = false;
                            uow.AttachmentRepository.Insert(newAttchment);
                            uow.Save();

                            attachment upAttachement = uow.AttachmentRepository.GetByID(newAttchment.Id);
                            string fileName = "LG_" + newAttchment.Id.ToString() + uow.FiletypeRepository.GetByID(newAttchment.FileTypeId).FileTypeName;

                            if (controllerName != "filemanager")
                                upAttachement.FileName = controllerName + "/" + fileName;
                            else
                                upAttachement.FileName = fileName;
                            uow.Save();

                            //uow.Entry(newAttchment).Reference(c => c.FileType).Load();
                            //uow.Entry(newAttchment).Reference(c => c.Folder).Load();
                            #endregion

                            #region Upload File To Folder
                            string targetFolder = "";
                            if (controllerName != "filemanager")
                                targetFolder = Server.MapPath("~/Content/UploadFiles/" + controllerName);
                            else
                                targetFolder = Server.MapPath("~/Content/UploadFiles");
                            string targetPath = Path.Combine(targetFolder, fileName);
                            uploadedFile.SaveAs(targetPath);
                            #endregion

                            #region Watermark Compression MultiSize
                            string UploadPath = "~/Content/UploadFiles/" + (controllerName != "filemanager" ? controllerName + "/" : "");
                            if (CoreLib.Infrastructure.Image.ImageClass.IsFileAnImage(UploadPath + fileName))
                            {
                                int langId = int.Parse(LanguageId);
                                Setting oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == langId, null, "attachment,Waterattachment").SingleOrDefault();
                                if (newAttchment.HasWatermark)
                                {
                                    string result = "";
                                    if (newAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                    }
                                }
                                else
                                {
                                    string result = "";
                                    if (newAttchment.HasMultiSize)
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }
                                    else
                                    {
                                        if (UseCompression == "on")
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, true, compressionLevel, oSetting.LargeImageWaremark);
                                        else
                                            result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, false, compressionLevel, oSetting.LargeImageWaremark);
                                    }

                                }
                            }
                            #endregion

                            #region Clear Cache AND Return Json Of Added File 
                            attachment UpAttchment = uow.AttachmentRepository.Get(x => x, x => x.Id == new Guid(AttachementId), null, "FileType").SingleOrDefault();
                            uow.AttachmentRepository.Update(UpAttchment);
                            uow.Save();

                            HttpContext.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                            HttpContext.Response.AddHeader("Pragma", "no-cache");
                            HttpContext.Response.AddHeader("Expires", "0");
                            Session["settingPersian"] = null;
                            Session["Languages"] = null;
                            Session["HomePage"] = null;
                            Session["submenu"] = null;

                            var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                            ViewBag.AddPermission = p.First();
                            ViewBag.EditPermission = p.Skip(1).First();
                            ViewBag.DeletePermission = p.Skip(2).First();

                            return Json(new
                            {
                                statusCode = 200,
                                status = "فایل دوم با موفقیت درج شد",
                                src = newAttchment.FileName,
                                ID = newAttchment.Id,
                                EditOrAdd = EditOrAdd
                            }, JsonRequestBehavior.AllowGet);
                            #endregion
                        }

                    }
                    else
                    {
                        #region File Not Selected
                        return Json(new
                        {
                            statusCode = 400,
                            status = "فایل خود را انتخاب نکرید. یا فایل انتخابی مشکل دارد.",
                            successCounter = 0,
                            file = string.Empty
                        }, JsonRequestBehavior.AllowGet);
                        #endregion
                    }
                }
                else
                {
                    #region File Extention Not Valid
                    return Json(new
                    {
                        statusCode = 400,
                        status = "پسوند فایل مجاز نیست. برای کسب اطلاعات بیشتر به مدیریت پسوند ها مراجعه کنید",
                        successCounter = 0,
                        file = string.Empty
                    }, JsonRequestBehavior.AllowGet);
                    #endregion
                }
            }
            catch (Exception x)
            {
                #region Unexpected Error
                return Json(new
                {
                    statusCode = 400,
                    status = x.Message,
                    successCounter = 0,
                    file = uploadedFile.FileName
                }, JsonRequestBehavior.AllowGet);
                #endregion
            }

        }

        #endregion

        [HttpPost]
        [CorrectArabianLetter(new string[] { "folderName" })]
        public virtual JsonResult AddFolder(string folderName, int languageId, int parrentId)
        {

            try
            {
                Folder folder = new Folder();
                folder.FolderName = folderName.Trim();
                folder.LanguageId = Convert.ToInt16(languageId);
                if (parrentId != 0)
                    folder.FolderID = parrentId;
                uow.FolderRepository.Insert(folder);
                uow.Save();
                return Json(new
                {
                    id = folder.Id,
                    statusCode = 200,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    statusCode = 500,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
