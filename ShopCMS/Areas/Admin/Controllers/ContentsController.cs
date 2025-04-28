using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using PagedList;
using CoreLib.Infrastructure.DateTime;
using CoreLib.ViewModel.Xml;
using ahmadi.Infrastructure.Helper;
using CoreLib.Infrastructure.ModelBinder;
using UnitOfWork;
using Microsoft.AspNet.Identity;
using ahmadi.ViewModels.Slider;
using ahmadi.ViewModels.Content;
using CoreLib.Infrastructure;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class ContentsController : Controller
    {
        private UnitOfWorkClass uow = new UnitOfWorkClass();

        // GET: Admin/Contents
        [CorrectArabianLetter(new string[] { "TitleString", "TitleFilter" })]
        public virtual ActionResult Index(string sortOrder, string TitleString, string TitleFilter, string LanguagenameString, string LanguagenameFilter, string CatId, string CatFilter, string IsActive, string IsActiveFilter, string ContentTypeId, string ContentTypeIdFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, string UpdateDateStart, string UpdateDateStartFilter, string UpdateDateEnd, string UpdateDateEndFilter, string DeleteDateStart, string DeleteDateStartFilter, string DeleteDateEnd, string DeleteDateEndFilter, int? page)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                if (ContentTypeId != null || ContentTypeIdFilter != null)
                {
                    int contenttypeid = 0;
                    if (ContentTypeId != null)
                        contenttypeid = Convert.ToInt32(ContentTypeId);
                    else
                        contenttypeid = Convert.ToInt32(ContentTypeIdFilter);
                    ViewBag.ContentTypeName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeShortName(contenttypeid, setting.StaticContentDomain);
                    ViewBag.ContentTypeShortName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeShortName(contenttypeid, setting.StaticContentDomain);
                    ViewBag.contenttypeid = contenttypeid;
                    if (contenttypeid == 0)
                        return Redirect("~/Admin/BaseContents");

                    var p = ModulePermission.check(User.Identity.GetUserId(), 4);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        #region Load DropDown List

                        List<SelectListItem> IsDeleteSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "حذف شده ها", Value = "True" }, new SelectListItem() { Text = "حذف نشده ها", Value = "False" } };
                        ViewBag.IsDeleteSelectListItem = IsDeleteSelectListItem;

                        List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                        ViewBag.IsActive = IsActiveSelectListItem;

                        ViewBag.Categories = uow.CategoryRepository.Get(x => x, x => x.ParentCat == null && x.ContentTypeId.Value == contenttypeid).OrderBy(x => x.Sort).ToList();

                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.ContentType = new SelectList(readXML.ListOfXContentType(), "Id", "Name");
                        ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");


                        ViewBag.ContentTypes = readXML.ListOfXContentType();

                        #endregion

                        #region search
                        if (string.IsNullOrEmpty(TitleString))
                            TitleString = TitleFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(LanguagenameString))
                            LanguagenameString = LanguagenameFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(CatId))
                            CatId = CatFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(IsActive))
                            IsActive = IsActiveFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(ContentTypeId))
                            ContentTypeId = ContentTypeIdFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(InsertDateStart))
                            InsertDateStart = InsertDateStartFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(InsertDateEnd))
                            InsertDateEnd = InsertDateEndFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(UpdateDateStart))
                            UpdateDateStart = UpdateDateStartFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(UpdateDateEnd))
                            UpdateDateEnd = UpdateDateEndFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(DeleteDateStart))
                            DeleteDateStart = DeleteDateStartFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(DeleteDateEnd))
                            DeleteDateEnd = DeleteDateEndFilter;
                        else
                            page = 1;

                        ViewBag.TitleFilter = TitleString;
                        ViewBag.LanguagenameFilter = LanguagenameString;
                        ViewBag.CatFilter = CatId;
                        ViewBag.IsActiveFilter = IsActive;
                        ViewBag.ContentTypeIdFilter = ContentTypeId;
                        ViewBag.InsertDateStartFilter = InsertDateStart;
                        ViewBag.InsertDateEndFilter = InsertDateEnd;
                        ViewBag.UpdateDateStartFilter = UpdateDateStart;
                        ViewBag.UpdateDateEndFilter = UpdateDateEnd;
                        ViewBag.DeleteDateStart = DeleteDateStart;
                        ViewBag.DeleteDateEnd = DeleteDateEnd;

                        var contents = uow.ContentRepository.GetByReturnQueryable(x => x, x => x.IsAbout == false && x.IsRegister == false && x.IsDefault == false && x.IsContact == false, null, "attachment,Category,User,attachment");

                        if (!String.IsNullOrEmpty(TitleString))
                            contents = contents.Where(s => s.Title.Contains(TitleString));
                        if (!String.IsNullOrEmpty(LanguagenameString))
                        {
                            int langId = Convert.ToInt32(LanguagenameString);
                            contents = contents.Where(s => s.LanguageId == langId);
                        }
                        if (!String.IsNullOrEmpty(CatId))
                        {
                            int ctId = Convert.ToInt32(CatId);
                            if (ctId == 0)
                                contents = contents.Where(s => s.CatId == null);
                            else
                                contents = contents.Where(s => s.CatId == ctId);
                        }
                        if (!String.IsNullOrEmpty(ContentTypeId))
                        {
                            int CtId = Convert.ToInt32(ContentTypeId);
                            contents = contents.Where(s => s.ContentTypeId == CtId);
                        }
                        if (!String.IsNullOrEmpty(IsActive))
                        {
                            bool isActive = Convert.ToBoolean(IsActive);
                            contents = contents.Where(s => s.IsActive == isActive);
                        }


                        DateTime dtInsertDateStart = DateTime.Now.Date, dtInsertDateEnd = DateTime.Now.Date, dtUpdateDateStart = DateTime.Now.Date, dtUpdateDateEnd = DateTime.Now.Date, dtDeleteDateStart = DateTime.Now.Date, dtDeleteDateEnd = DateTime.Now.Date;
                        if (!String.IsNullOrEmpty(InsertDateStart))
                            dtInsertDateStart = DateTimeConverter.ChangeShamsiToMiladi(InsertDateStart);
                        if (!String.IsNullOrEmpty(InsertDateEnd))
                            dtInsertDateEnd = DateTimeConverter.ChangeShamsiToMiladi(InsertDateEnd);
                        if (!String.IsNullOrEmpty(UpdateDateStart))
                            dtUpdateDateStart = DateTimeConverter.ChangeShamsiToMiladi(UpdateDateStart);
                        if (!String.IsNullOrEmpty(UpdateDateEnd))
                            dtUpdateDateEnd = DateTimeConverter.ChangeShamsiToMiladi(UpdateDateEnd);
                        if (!String.IsNullOrEmpty(DeleteDateStart))
                            dtDeleteDateStart = DateTimeConverter.ChangeShamsiToMiladi(DeleteDateStart);
                        if (!String.IsNullOrEmpty(DeleteDateEnd))
                            dtDeleteDateEnd = DateTimeConverter.ChangeShamsiToMiladi(DeleteDateEnd);

                        if (!String.IsNullOrEmpty(InsertDateStart) && !String.IsNullOrEmpty(InsertDateEnd))
                            contents = contents.Where(s => s.InsertDate >= dtInsertDateStart && s.InsertDate <= dtInsertDateEnd);
                        else if (!String.IsNullOrEmpty(InsertDateStart))
                            contents = contents.Where(s => s.InsertDate >= dtInsertDateStart);
                        else if (!String.IsNullOrEmpty(InsertDateEnd))
                            contents = contents.Where(s => s.InsertDate <= dtInsertDateEnd);

                        if (!String.IsNullOrEmpty(UpdateDateStart) && !String.IsNullOrEmpty(UpdateDateEnd))
                            contents = contents.Where(s => s.UpdateDate >= dtUpdateDateStart && s.UpdateDate <= dtUpdateDateEnd);
                        else if (!String.IsNullOrEmpty(UpdateDateStart))
                            contents = contents.Where(s => s.UpdateDate >= dtUpdateDateStart);
                        else if (!String.IsNullOrEmpty(UpdateDateEnd))
                            contents = contents.Where(s => s.UpdateDate <= dtUpdateDateEnd);


                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {
                            case "Title":
                                contents = contents.OrderBy(s => s.Title);
                                ViewBag.CurrentSort = "Title";
                                break;
                            case "Title_desc":
                                contents = contents.OrderByDescending(s => s.Title);
                                ViewBag.CurrentSort = "Title_desc";
                                break;
                            case "CatTitle":
                                contents = contents.OrderBy(s => s.Category.Title);
                                ViewBag.CurrentSort = "CatTitle";
                                break;
                            case "CatTitle_desc":
                                contents = contents.OrderByDescending(s => s.Category.Title);
                                ViewBag.CurrentSort = "CatTitle_desc";
                                break;
                            case "ContentType":
                                contents = contents.OrderBy(s => s.ContentTypeId);
                                ViewBag.CurrentSort = "ContentType";
                                break;
                            case "ContentType_desc":
                                contents = contents.OrderByDescending(s => s.ContentTypeId);
                                ViewBag.CurrentSort = "ContentType_desc";
                                break;
                            case "Visits":
                                contents = contents.OrderBy(s => s.Visits);
                                ViewBag.CurrentSort = "Visits";
                                break;
                            case "Visits_desc":
                                contents = contents.OrderByDescending(s => s.Visits);
                                ViewBag.CurrentSort = "Visits_desc";
                                break;
                            case "InsertDate":
                                contents = contents.OrderBy(s => s.InsertDate);
                                ViewBag.CurrentSort = "InsertDate";
                                break;
                            case "InsertDate_desc":
                                contents = contents.OrderByDescending(s => s.InsertDate);
                                ViewBag.CurrentSort = "InsertDate_desc";
                                break;
                            case "IsActive":
                                contents = contents.OrderBy(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive";
                                break;
                            case "IsActive_desc":
                                contents = contents.OrderByDescending(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive_desc";
                                break;
                            case "Language":
                                contents = contents.OrderBy(s => s.LanguageId);
                                ViewBag.CurrentSort = "Language";
                                break;
                            case "Language_desc":
                                contents = contents.OrderByDescending(s => s.LanguageId);
                                ViewBag.CurrentSort = "Language_desc";
                                break;
                            default:  // Name ascending 
                                contents = contents.OrderByDescending(s => s.Id);
                                break;
                        }

                        #endregion

                        int pageSize = 8;
                        int pageNumber = (page ?? 1);

                        ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "محتواها", null, "HelpModuleSections").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Contents", "Index", true, 200, " نمایش صفحه مدیریت محتوا", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(contents.ToPagedList(pageNumber, pageSize));

                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

                }
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }


        public virtual JsonResult FilterCategory(int ContentTypeId)
        {
            uow = new UnitOfWorkClass();
            var Categories = uow.CategoryRepository.Get(x => x, x => x.ContentTypeId == ContentTypeId && x.IsVideo == false && x.ParrentId == null).OrderBy(x => x.Sort).AsEnumerable();
            string HTMLString = CaptureHelper.RenderViewToString("_TreeCategory", Categories, this.ControllerContext);
            return Json(new
            {
                success = 1,
                data = HTMLString
            }, JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/Contents/Sort
        public virtual ActionResult Sort(int? page, int? ContentTypeId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                #region Check License


                #endregion

                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                if (ContentTypeId.HasValue)
                {
                    int contenttypeid = 0;
                    if (ContentTypeId != null)
                        contenttypeid = Convert.ToInt32(ContentTypeId);
                    ViewBag.ContentTypeName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeShortName(contenttypeid, setting.StaticContentDomain);


                    var p = ModulePermission.check(User.Identity.GetUserId(), 4);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        var contents = uow.ContentRepository.Get(x => x, x => x.ContentTypeId == contenttypeid, null, "attachment,Category,User").OrderBy(x => x.DisplaySort);

                        int pageSize = 100;
                        int pageNumber = (page ?? 1);

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 14 && x.Name == "مرتب سازی").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Contents", "Sort", true, 200, " نمایش صفحه مرتب سازی محتواها", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(contents.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتواها" }));

                }
                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Contents/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uow = new UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var content = uow.ContentRepository.GetByID(id);
                    content.DisplaySort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Contents", "Sort", false, 200, " مرتب سازی محتواها", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Contents/Details/5
        public virtual ActionResult Details(int? id)
        {
            uow = new UnitOfWorkClass();

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 4).Where(x => x == true).Any())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Content content = uow.ContentRepository.GetByID(id);
                    if (content == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Contents", "Details", true, 200, " نمایش جزئیات محتوایِ " + content.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(content);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // GET: Admin/Contents/Create
        public virtual ActionResult Create(int? ContentTypeId)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ContentTypeId.HasValue)
                {
                    int contenttypeid = 0;
                    if (ContentTypeId != null)
                        contenttypeid = Convert.ToInt32(ContentTypeId.Value);
                    ViewBag.ContentTypeName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeShortName(contenttypeid, setting.StaticContentDomain);
                    ViewBag.ContentTypeId = contenttypeid;



                    if (ModulePermission.check(User.Identity.GetUserId(), 4, 1))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                        ViewBag.CatId = uow.CategoryRepository.Get(x => x, x => x.ParentCat == null && x.ContentTypeId == contenttypeid).OrderBy(x => x.Sort);

                        ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                        ViewBag.TagId = uow.TagRepository.GetByReturnQueryable(x => x).OrderByDescending(x => x.Id);

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 14 && x.Name == "ایجاد", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Contents", "Create", true, 200, "نمایش صفحه ایجاد محتوا", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));
                }
                else
                    return RedirectToAction("Index", "Home");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Contents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create(Content content, string AutoSave, string[] TagId, string[] SourceTitle, string[] SourceLink, string[] JanebiId, string[] JanebiTitle)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModelState.IsValid)
                {
                    content.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(content.Title);
                    if (content.IsDefault && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsDefault == true).Any())
                        ViewBag.Error = " صفحه اصلی وجود دارد و نمی توانید بیش از یک صفحه اصلی ایجاد کنید. ";
                    else if (content.IsAbout && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsAbout == true).Any())
                        ViewBag.Error = " صفحه درباره ما وجود دارد و نمی توانید بیش از یک صفحه درباره ما ایجاد کنید. ";
                    else if (content.IsRegister && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsRegister == true).Any())
                        ViewBag.Error = " صفحه محتوای شرایط ثبت نام وجود دارد و نمی توانید بیش از یک صفحه محتوای شرایط ثبت نام ایجاد کنید. ";
                    else if (content.IsContact && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsContact == true).Any())
                        ViewBag.Error = " صفحه تماس با ما وجود دارد و نمی توانید بیش از یک صفحه تماس با ما ایجاد کنید. ";
                    else
                    {
                        #region AddContent

                        CheckSearchEngineTypeExist(content.ContentTypeId);
                        content.PageAddress = CommonFunctions.NormalizeAddressWithSpace(content.PageAddress);

                        IdentityManager im = new IdentityManager();
                        content.UserId = User.Identity.GetUserId();
                        content.InsertDate = DateTime.Now;
                        if (uow.ContentRepository.GetByReturnQueryable(x => x).Any())
                            content.DisplaySort = uow.ContentRepository.Max(x => x.DisplaySort) + 1;

                        #region Add Source 
                        List<Source> LstSource = new List<Source>();
                        if (SourceTitle != null)
                        {
                            for (int i = 0; i < SourceTitle.Length; i++)
                            {
                                LstSource.Add(new Source(SourceTitle[i], (SourceLink[i] == "unknown" ? "" : SourceLink[i])));
                            }
                            content.Sources = LstSource;
                        }
                        #endregion
                        #region Add OtherImage
                        List<OtherImageViewModel> JanebiList = new List<OtherImageViewModel>();
                        if (JanebiId != null)
                        {
                            content.OtherImages = new List<OtherImage>();
                            for (int i = 0; i < JanebiId.Length; i++)
                            {
                                content.OtherImages.Add(new OtherImage(JanebiTitle[i], i, new Guid(JanebiId[i])));
                            }
                        }
                        #endregion
                        #region Add Tag
                        List<string> CurrentTags = new List<string>();
                        content.Tags = new List<Tag>();
                        if (TagId != null)
                        {
                            foreach (string item in TagId)
                            {
                                int id = int.Parse(item);
                                content.Tags.Add(uow.TagRepository.GetByID(id));
                            }
                        }
                        #endregion
                        uow.ContentRepository.Insert(content);
                        uow.Save();

                        #endregion



                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Contents", "Create", false, 200, " ایجاد محتوای " + content.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        if (Convert.ToBoolean(AutoSave))
                            return RedirectToAction("Edit", new { Id = content.Id, message = "ثبت شد." });
                        else
                            return RedirectToAction("Index", new { ContentTypeId = content.ContentTypeId });
                    }
                }
                ViewBag.CatId = uow.CategoryRepository.Get(x => x, x => x.ParentCat == null && x.ContentTypeId == content.ContentTypeId).OrderBy(x => x.Sort);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.ContentTypeId = new SelectList(readXML.ListOfXContentType(), "Id", "Name");
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                ViewBag.TagId = uow.TagRepository.GetByReturnQueryable(x => x).OrderByDescending(x => x.Id);
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 14 && x.Name == "ایجاد", null, "HelpModuleSectionFields").FirstOrDefault();
                return View(content);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Contents/Edit/5
        public virtual ActionResult Edit(int? id, string message = "")
        {
            uow = new UnitOfWorkClass();
            try
            {

                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModulePermission.check(User.Identity.GetUserId(), 4, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ContentViewModel content = new ContentViewModel(uow.ContentRepository.Get(x => x, x => x.Id == id, null, "VideoAttachment,attachment,Blogattachment,Tags,Sources,OtherImages.attachment,Category").FirstOrDefault());

                    if (content == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.Message = message;

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.masterTheme = readXML.DetailOfXMasterTheme(Convert.ToInt16(content.LanguageId));

                    ViewBag.CatId = uow.CategoryRepository.Get(x => x, x => x.ParentCat == null && x.ContentTypeId == content.ContentTypeId).OrderBy(x => x.Sort);

                    ViewBag.ContentTypeName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeShortName(content.ContentTypeId, setting.StaticContentDomain);

                    ViewBag.ContentTypeId = new SelectList(readXML.ListOfXContentType(), "Id", "Name", content.ContentTypeId);
                    var contentTypes = readXML.ListOfXContentType().OrderBy(x => x.Name);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", content.LanguageId);
                    ViewBag.TagId = uow.TagRepository.Get(x => x).OrderByDescending(x => x.Id);


                    #region Load Sources
                    List<SourceViewModel> Sources = new List<SourceViewModel>();
                    foreach (Source item in content.Sources)
                    {
                        SourceViewModel oSource = new SourceViewModel();
                        oSource.SourceLink = item.Link;
                        oSource.Source = item.Title;
                        Sources.Add(oSource);
                    }
                    ViewBag.Sources = Sources;
                    #endregion
                    #region Load Other Images
                    List<OtherImageViewModel> JanebiList = new List<OtherImageViewModel>();
                    foreach (OtherImage item in content.OtherImages)
                    {
                        OtherImageViewModel oOtherImage = new OtherImageViewModel();
                        oOtherImage.Cover = item.Cover;
                        oOtherImage.DisplaySort = item.DisplaySort;
                        oOtherImage.Title = item.Title;
                        oOtherImage.Src = item.attachment.FileName;
                        JanebiList.Add(oOtherImage);
                    }
                    ViewBag.JanebiList = JanebiList.OrderBy(x => x.Id).ToList();
                    #endregion
                    #region Load Tags
                    List<string> CurrentTags = new List<string>();
                    List<Tag> CurrentTagItems = new List<Tag>();
                    if (content.Tags.Any())
                    {
                        foreach (Tag item in content.Tags)
                        {
                            CurrentTags.Add(item.Id.ToString());
                            Tag oTag = new Tag();
                            oTag.Id = item.Id;
                            oTag.TagName = item.TagName;
                            CurrentTagItems.Add(oTag);
                        }
                        Session["CurrentTags"] = CurrentTags;
                        ViewBag.CurrentTagItems = CurrentTagItems;
                    }
                    #endregion

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 14 && x.Name == "ایجاد", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Contents", "Edit", true, 200, "نمایش صفحه ویرایش محتوای " + content.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(content);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Contents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Edit(ContentViewModel content, string AutoSave, string[] TagId, string[] SourceTitle, string[] SourceLink, string[] JanebiId, string[] JanebiTitle, string FooterGoogleMapLongitude, string FooterGoogleMapLatitude, string FooterGoogleMapZoom)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                if (ModelState.IsValid)
                {

                    content.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(content.Title);
                    if (content.IsDefault && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsDefault == true && x.Id != content.Id).Any())
                        ViewBag.Error = " صفحه اصلی وجود دارد و نمی توانید بیش از یک صفحه اصلی ایجاد کنید. ";
                    else if (content.IsAbout && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsAbout == true && x.Id != content.Id).Any())
                        ViewBag.Error = " صفحه درباره ما وجود دارد و نمی توانید بیش از یک صفحه درباره ما ایجاد کنید. ";
                    else if (content.IsRegister && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsRegister == true && x.Id != content.Id).Any())
                        ViewBag.Error = " صفحه محتوای شرایط ثبت نام وجود دارد و نمی توانید بیش از یک صفحه محتوای شرایط ثبت نام ایجاد کنید. ";
                    else if (content.IsContact && uow.ContentRepository.Get(x => x, x => x.LanguageId == content.LanguageId && x.IsContact == true && x.Id != content.Id).Any())
                        ViewBag.Error = " صفحه تماس با ما وجود دارد و نمی توانید بیش از یک صفحه تماس با ما ایجاد کنید. ";
                    else
                    {


                        #region Delete Old Tag
                        List<string> lstTags = new List<string>();
                        if (TagId != null)
                        {
                            lstTags = TagId.ToList();
                            var ContentTags = uow.ContentRepository.Get(x => x, x => x.Id == content.Id, null, "Tags").FirstOrDefault().Tags;
                            List<Tag> DeleteTags = new List<Tag>();
                            foreach (Tag item in ContentTags)
                            {
                                var current = lstTags.Where(x => x.Equals(item.Id.ToString())).FirstOrDefault();
                                if (current != null)
                                    lstTags.Remove(current);
                                else
                                    DeleteTags.Add(item);
                            }
                            foreach (Tag item in DeleteTags)
                            {

                                Content Currentcontent = uow.ContentRepository.Get(x => x, x => x.Id == content.Id, null, "Tags").FirstOrDefault();
                                uow.ContentRepository.Load(Currentcontent, "Tags");
                                Currentcontent.Tags.Remove(item);
                                uow.Save();
                            }

                        }
                        else
                        {
                            var ContentTags = uow.ContentRepository.Get(x => x, x => x.Id == content.Id, null, "Tags").FirstOrDefault().Tags;
                            List<Tag> DeleteOtherImages = new List<Tag>();
                            foreach (Tag item in ContentTags)
                            {
                                DeleteOtherImages.Add(item);
                            }
                            foreach (Tag item in DeleteOtherImages)
                            {
                                Content Currentcontent = uow.ContentRepository.Get(x => x, x => x.Id == content.Id, null, "Tags").FirstOrDefault();
                                uow.ContentRepository.Load(Currentcontent, "Tags");
                                Currentcontent.Tags.Remove(item);
                                uow.Save();
                            }

                        }
                        #endregion

                        #region Delete Old Source
                        List<Source> lstSource = new List<Source>();
                        if (SourceTitle != null)
                        {
                            for (int i = 0; i < SourceTitle.Length; i++)
                            {
                                lstSource.Add(new Source(SourceTitle[i], (SourceLink[i] == "unknown" ? "" : SourceLink[i])));
                            }
                            var ContentSources = uow.SourceRepository.Get(x => x, x => x.ContentId == content.Id);
                            List<Source> DeleteSources = new List<Source>();
                            foreach (Source item in ContentSources)
                            {
                                var current = lstSource.Where(x => x.Title.Equals(item.Title)).FirstOrDefault();
                                if (current != null)
                                    lstSource.Remove(current);
                                else
                                    DeleteSources.Add(item);
                            }
                            foreach (Source item in DeleteSources)
                            {
                                uow.SourceRepository.Delete(item);
                                uow.Save();
                            }

                        }
                        else
                        {
                            var ContentSources = uow.SourceRepository.Get(x => x, x => x.ContentId == content.Id);
                            uow.SourceRepository.Delete(ContentSources);
                            uow.Save();
                        }
                        #endregion

                        #region Delete Old Other Image
                        List<OtherImage> lstOtherImage = new List<OtherImage>();
                        if (JanebiId != null)
                        {
                            for (int i = 0; i < JanebiId.Length; i++)
                            {
                                lstOtherImage.Add(new OtherImage(JanebiTitle[i], i, new Guid(JanebiId[i])));
                            }

                            var ContentlstOtherImages = uow.OtherImageRepository.Get(x => x, x => x.ContentId == content.Id);
                            List<OtherImage> DeleteOtherImages = new List<OtherImage>();
                            foreach (OtherImage item in ContentlstOtherImages)
                            {
                                var current = lstOtherImage.Where(x => x.Cover == item.Cover).FirstOrDefault();
                                if (current != null)
                                    lstOtherImage.Remove(current);
                                else
                                    DeleteOtherImages.Add(item);
                            }
                            foreach (OtherImage item in DeleteOtherImages)
                            {
                                uow.OtherImageRepository.Delete(item);
                                uow.Save();
                            }

                        }
                        else
                        {
                            var ContentlstOtherImages = uow.OtherImageRepository.Get(x => x, x => x.ContentId == content.Id);
                            List<OtherImage> DeleteOtherImages = new List<OtherImage>();
                            foreach (OtherImage item in ContentlstOtherImages)
                            {
                                DeleteOtherImages.Add(item);
                            }
                            foreach (OtherImage item in DeleteOtherImages)
                            {
                                uow.OtherImageRepository.Delete(item);
                                uow.Save();
                            }

                        }
                        #endregion

                        #region EdtiContent
                        CheckSearchEngineTypeExist(content.ContentTypeId);

                        var Editcontent = uow.ContentRepository.Get(x => x, x => x.Id == content.Id, null, "VideoAttachment,attachment,Blogattachment,Tags,Sources,OtherImages.attachment,Category").FirstOrDefault();
                        string OldTitle = Editcontent.Title;
                        int OldContentTypeId = Editcontent.ContentTypeId;
                        IdentityManager im = new IdentityManager();
                        Editcontent.UserId = User.Identity.GetUserId();
                        Editcontent.UpdateDate = DateTime.Now;
                        Editcontent.Title = content.Title;
                        Editcontent.PageAddress = CommonFunctions.NormalizeAddressWithSpace(content.PageAddress);
                        Editcontent.Abstract = content.Abstract;
                        Editcontent.Descr = content.Descr;
                        Editcontent.Data = content.Data;
                        Editcontent.LanguageId = content.LanguageId;
                        Editcontent.Cover = content.Cover;
                        Editcontent.BlogCover = content.BlogCover;
                        Editcontent.CatId = content.CatId;
                        Editcontent.ContentTypeId = content.ContentTypeId;
                        Editcontent.IsActive = content.IsActive;
                        Editcontent.IsDefault = content.IsDefault;
                        Editcontent.IsAbout = content.IsAbout;
                        Editcontent.IsContact = content.IsContact;
                        Editcontent.HasContact = content.HasContact;
                        Editcontent.IsRegister = content.IsRegister;
                        Editcontent.IsSuperDeal = content.IsSuperDeal;
                        Editcontent.Icon = content.Icon;
                        Editcontent.Video = content.Video;
                        Editcontent.BlogMain = content.BlogMain;
                        Editcontent.ReadMinuts = content.ReadMinuts;
                        if (SourceTitle != null)
                        {
                            foreach (var item in lstSource)
                            {
                                Editcontent.Sources.Add(item);
                            }
                        }
                        if (JanebiId != null)
                        {
                            foreach (var item in lstOtherImage)
                            {

                                Editcontent.OtherImages.Add(item);
                            }
                        }
                        List<string> CurrentTags = new List<string>();
                        if (TagId != null)
                        {
                            foreach (string item in lstTags)
                            {
                                int id = int.Parse(item);
                                Editcontent.Tags.Add(uow.TagRepository.GetByID(id));
                            }
                        }
                        uow.ContentRepository.Update(Editcontent);
                        uow.Save();

                        if (OldContentTypeId != content.ContentTypeId)
                        {
                            var sef = uow.SearchEngineFactRepository.Get(x => x, x => x.ElementID == content.Id && x.ElementTypeID == OldContentTypeId);
                            uow.SearchEngineFactRepository.Delete(sef);
                            uow.Save();
                        }

                        #endregion

                        #region Update Google Map Of Contact Us Page
                        if (content.IsContact)
                        {
                            readXML.EditXMasterThemeGoogleMap(Convert.ToInt16(content.LanguageId), FooterGoogleMapLongitude, FooterGoogleMapLatitude, FooterGoogleMapZoom);
                        }
                        #endregion

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(3, "Contents", "Edit", false, 200, " ویرایش محتوای " + content.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        if (Convert.ToBoolean(AutoSave))
                        {
                            return RedirectToAction("Edit", new { message = "ثبت شد." });
                        }
                        else
                            return RedirectToAction("Index", new { ContentTypeId = content.ContentTypeId });


                    }
                }
                ViewBag.ContentTypeName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeShortName(content.ContentTypeId, setting.StaticContentDomain);
                ViewBag.CatId = uow.CategoryRepository.Get(x => x, x => x.ParentCat == null && x.ContentTypeId == content.ContentTypeId).OrderBy(x => x.Sort);
                ViewBag.ContentTypeId = new SelectList(readXML.ListOfXContentType(), "Id", "Name", content.ContentTypeId);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", content.LanguageId);
                ViewBag.TagId = uow.TagRepository.GetByReturnQueryable(x => x).OrderByDescending(x => x.Id);


                #region Load Sources
                if (content.Sources != null)
                {
                    List<SourceViewModel> Sources = new List<SourceViewModel>();
                    foreach (Source item in content.Sources)
                    {
                        SourceViewModel oSource = new SourceViewModel();
                        oSource.SourceLink = item.Link;
                        oSource.Source = item.Title;
                        Sources.Add(oSource);
                    }
                    Session["Sources"] = Sources;
                }
                #endregion
                #region Load Other Images
                if (content.OtherImages != null)
                {
                    List<OtherImageViewModel> JanebiList = new List<OtherImageViewModel>();
                    foreach (OtherImage item in content.OtherImages)
                    {
                        OtherImageViewModel oOtherImage = new OtherImageViewModel();
                        oOtherImage.Cover = item.Cover;
                        oOtherImage.DisplaySort = item.DisplaySort;
                        oOtherImage.Title = item.Title;
                        oOtherImage.Src = item.attachment.FileName;
                        JanebiList.Add(oOtherImage);
                    }
                    Session["JanebiList"] = JanebiList.OrderBy(x => x.Id).ToList();
                }
                #endregion
                #region Load Tags
                if (content.Tags != null)
                {
                    List<string> CurrentTags = new List<string>();
                    List<Tag> CurrentTagItems = new List<Tag>();
                    if (content.Tags.Any())
                    {
                        foreach (Tag item in content.Tags)
                        {
                            CurrentTags.Add(item.Id.ToString());
                            Tag oTag = new Tag();
                            oTag.Id = item.Id;
                            oTag.TagName = item.TagName;
                            CurrentTagItems.Add(oTag);
                        }
                        Session["CurrentTags"] = CurrentTags;
                        ViewBag.CurrentTagItems = CurrentTagItems;
                    }
                }
                #endregion
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 14 && x.Name == "ایجاد", null, "HelpModuleSectionFields").FirstOrDefault();

                ViewBag.masterTheme = readXML.DetailOfXMasterTheme(Convert.ToInt16(content.LanguageId));
                return View(content);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Contents/Delete/5
        [HttpPost]
        public virtual JsonResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                Content content = uow.ContentRepository.GetByID(id);
                if (content.IsDefault)
                {
                    return Json(new
                    {
                        Message = "اجازه حذف صفحه اصلی وجود ندارد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (content.IsAbout)
                {
                    return Json(new
                    {
                        Message = "اجازه حذف صفحه درباره ما وجود ندارد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (content.IsRegister)
                {
                    return Json(new
                    {
                        Message = "اجازه حذف صفحه محتوای ثبت نام وجود ندارد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (content.IsContact)
                {
                    return Json(new
                    {
                        Message = "اجازه حذف صفحه تماس با ما وجود ندارد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);

                }
                else if (content.IsSuperDeal)
                {
                    return Json(new
                    {
                        Message = "اجازه حذف صفحه تخفیف وجود ندارد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    uow.ContentRepository.Delete(content);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "Contents", "DeleteConfirmed", false, 200, " حذف محتوای " + content.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        Message = "حذف شد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Tag
        [CorrectArabianLetter(new string[] { "TagName" })]
        public virtual JsonResult SearchTag(string TagName)
        {
            uow = new UnitOfWorkClass();
            var tag = uow.TagRepository.Get(x => x, x => x.TagName.Contains(TagName)).OrderByDescending(x => x.Id);
            return Json(new
            {
                data = tag.Select(x => new { Id = x.Id, TagName = x.TagName, LanguageId = x.LanguageId }).ToList(),
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }

        public virtual JsonResult AllTag()
        {
            uow = new UnitOfWorkClass();
            var tag = uow.TagRepository.Get(x => x).OrderByDescending(x => x.Id);
            return Json(new
            {
                data = tag.Select(x => new { Id = x.Id, TagName = x.TagName, LanguageId = x.LanguageId }).ToList(),
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual JsonResult SelectTag(string ids)
        {
            try
            {
                string[] TagIds = ids.Split(',');
                List<string> CurrentTags = new List<string>();
                for (int i = 0; i < TagIds.Length; i++)
                {
                    CurrentTags.Add(TagIds[i]);
                }

                Session["CurrentTags"] = CurrentTags;
                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public virtual JsonResult DeleteSelectedTag(string id)
        {
            try
            {
                List<string> CurrentTags = new List<string>();
                if (Session["CurrentTags"] != null)
                {
                    CurrentTags = (List<string>)Session["CurrentTags"];
                    var DeletedTag = CurrentTags.Where(x => x.Equals(id)).SingleOrDefault();
                    if (CurrentTags.Remove(DeletedTag))
                    {
                        Session["CurrentTags"] = CurrentTags;
                        return Json(new
                        {
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            message = "خطایی رخداد",
                            statusCode = 400
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        message = "خطایی رخداد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public virtual JsonResult ClearSelectedTags()
        {
            Session["CurrentTags"] = null;
            return Json(new
            {
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        [CorrectArabianLetter(new string[] { "tagName" })]
        public virtual JsonResult EditTags(string id, string tagName)
        {
            uow = new UnitOfWorkClass();
            try
            {
                int ID = int.Parse(id);
                Tag oTag = uow.TagRepository.GetByID(ID);
                oTag.TagName = tagName.Trim();
                uow.Save();
                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Message = "خطا.برچسب وارد شده وجود دارد",
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public virtual JsonResult DeleteTags(string ids)
        {
            uow = new UnitOfWorkClass();
            try
            {
                string msg = "";
                List<int> deletedTagId = new List<int>();
                string[] id = ids.Split(',');
                for (int i = 0; i < id.Length; i++)
                {
                    int ID = int.Parse(id[i]);
                    Tag oTag = uow.TagRepository.Get(x => x, x => x.Id == ID, null, "Content").FirstOrDefault();
                    if (!oTag.Content.Any())
                    {
                        uow.TagRepository.Delete(oTag);
                        uow.Save();
                        msg += string.Format("{0} حذف شد.", oTag.TagName);
                        deletedTagId.Add(oTag.Id);
                    }
                    else
                        msg += string.Format("{0} ، {1} محتوا دارد و حذف نشد.", oTag.TagName, oTag.Content.Count);
                }

                return Json(new
                {
                    deletedTagId = deletedTagId,
                    Message = msg,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        [CorrectArabianLetter(new string[] { "tagName" })]
        public virtual JsonResult AddTags(string tagName, string LanguageId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                Int16 langId = Int16.Parse(LanguageId);
                Tag oTag = new Tag();
                oTag.TagName = tagName.Trim();
                oTag.LanguageId = langId;
                oTag.MetaDescription = tagName;
                oTag.Title = tagName;
                uow.TagRepository.Insert(oTag);
                uow.Save();
                return Json(new
                {
                    Id = oTag.Id,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new
                {
                    Message = "خطا.برچسب وارد شده وجود دارد",
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion

        #region Batch Operation

        [HttpPost]
        public virtual JsonResult RemoveContents(string ids)
        {
            uow = new UnitOfWorkClass();
            try
            {
                string[] ContentIds = ids.Split(',');
                for (int i = 0; i < ContentIds.Length; i++)
                {
                    int id = Convert.ToInt32(ContentIds[i]);
                    Content DeleteContent = uow.ContentRepository.GetByID(id);
                    uow.ContentRepository.Delete(DeleteContent);
                    uow.Save();
                }

                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public virtual JsonResult ActiveContents(string ids)
        {
            uow = new UnitOfWorkClass();
            try
            {
                string[] ContentIds = ids.Split(',');
                for (int i = 0; i < ContentIds.Length; i++)
                {
                    int id = Convert.ToInt32(ContentIds[i]);
                    Content DeleteContent = uow.ContentRepository.GetByID(id);
                    DeleteContent.IsActive = true;
                    uow.Save();
                }

                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        public virtual JsonResult DeActiveContents(string ids)
        {
            uow = new UnitOfWorkClass();
            try
            {
                string[] ContentIds = ids.Split(',');
                for (int i = 0; i < ContentIds.Length; i++)
                {
                    int id = Convert.ToInt32(ContentIds[i]);
                    Content DeleteContent = uow.ContentRepository.GetByID(id);
                    DeleteContent.IsActive = false;
                    uow.Save();
                }

                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }


        }
        #endregion

        protected void CheckSearchEngineTypeExist(int ContentTypeId)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            XMLReader readXml = new XMLReader(setting.StaticContentDomain);
            XContentType xct = readXml.DetailOfXContentType(ContentTypeId);
            if (xct != null)
            {
                if (uow.SearchEngineElementTypeRepository.GetByID(ContentTypeId) == null)
                {
                    SearchEngineElementType sete = new SearchEngineElementType(ContentTypeId, xct.Name, xct.Abstract, 1, 1);
                    uow.SearchEngineElementTypeRepository.Insert(sete);
                    uow.Save();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
    }
}
