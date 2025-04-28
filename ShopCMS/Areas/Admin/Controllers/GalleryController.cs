using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ahmadi.Infrastructure.Security;
using CoreLib.ViewModel.Xml;
using System.Data.Entity;
using System.Net;
using CoreLib.Infrastructure.ModelBinder;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class GalleryController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow ;
        //
        // GET: /Admin/Gallery/
        [CorrectArabianLetter(new string[] { "CatTitleString", "CatTitleFilter" })]
        public virtual ActionResult Index(string LanguageId, string sortOrder, string CatTitleFilter, string CatTitleString, string LanguagenameFilter, string LanguagenameString, string ParrentCatFilter, string ParrentCatString, string IsActive, string IsActiveFilter, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
               
                    var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();


                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                        List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                        ViewBag.IsActive = IsActiveSelectListItem;

                        #region search
                        if (string.IsNullOrEmpty(CatTitleString))
                            CatTitleString = CatTitleFilter;
                        if (string.IsNullOrEmpty(LanguagenameString))
                            LanguagenameString = LanguagenameFilter;
                        if (string.IsNullOrEmpty(ParrentCatString))
                            ParrentCatString = ParrentCatFilter;
                        if (string.IsNullOrEmpty(IsActive))
                            IsActive = IsActiveFilter;

                        ViewBag.IsActiveFilter = IsActive;
                        ViewBag.CatTitleFilter = CatTitleString;
                        ViewBag.LanguagenameFilter = LanguagenameString;
                        ViewBag.ParrentCatFilter = ParrentCatString;

                        var Galleries = uow.GalleryCategoryRepository.Get(x=>x,null,null,"attachment");
                        if (!String.IsNullOrEmpty(CatTitleString))
                            Galleries = Galleries.Where(s => s.Title.Contains(CatTitleString));
                        if (!String.IsNullOrEmpty(LanguagenameString))
                        {
                            int langId = Convert.ToInt32(LanguagenameString);
                            Galleries = Galleries.Where(s => s.LanguageId == langId);
                        }
                        if (!String.IsNullOrEmpty(IsActive))
                        {
                            bool active = Convert.ToBoolean(IsActive);
                            Galleries = Galleries.Where(s => s.IsActive == active);
                        }

                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {
                            case "CatTitle":
                                Galleries = Galleries.OrderBy(s => s.Title);
                                ViewBag.CurrentSort = "CatTitle";
                                break;
                            case "CatTitle_desc":
                                Galleries = Galleries.OrderByDescending(s => s.Title);
                                ViewBag.CurrentSort = "CatTitle_desc";
                                break;
                            case "Languagename":
                                Galleries = Galleries.OrderBy(s => s.LanguageId);
                                ViewBag.CurrentSort = "Languagename";
                                break;
                            case "Languagename_desc":
                                Galleries = Galleries.OrderByDescending(s => s.LanguageId);
                                ViewBag.CurrentSort = "Languagename_desc";
                                break;
                       
                            case "IsActive":
                                Galleries = Galleries.OrderBy(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive";
                                break;
                            case "IsActive_desc":
                                Galleries = Galleries.OrderByDescending(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive_desc";
                                break;
                            default:  // Name ascending 
                                Galleries = Galleries.OrderByDescending(s => s.Id);
                                break;
                        }

                        #endregion

                        int pageSize = 8;
                        int pageNumber = (page ?? 1);

                        

                        ViewBag.HelpModule = uow.HelpModuleRepository.Get(x=>x,x => x.Name == "گالری",null, "HelpModuleSections").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "Index", true, 200, " نمایش صفحه مدیریت گالری ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Galleries.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گالری" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Gallery/Sort
        public virtual ActionResult Sort(int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                
                    var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        var Galleries = uow.GalleryCategoryRepository.Get(x=>x,null,x => x.OrderBy(o=>o.Sort), "attachment");

                        int pageSize = 100;
                        int pageNumber = (page ?? 1);

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 26 && x.Name == "مرتب سازی", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "Sort", true, 200, " نمایش صفحه مرتب سازی گالری ها ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Galleries.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی گالری ها" }));
                
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Gallery/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var gallery = uow.GalleryCategoryRepository.GetByID(id);
                    gallery.Sort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Galleries", "Sort", false, 200, "مرتب سازی گالری ها", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Gallery/Details/5
        public virtual ActionResult Details(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16).Where(x => x == true).Any())
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        var gallery = uow.GalleryCategoryRepository.GetByID(id);
                        if (gallery == null)
                        {
                            return HttpNotFound();
                        }
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "Details", true, 200, " نمایش جزئیاتِ گالری  " + gallery.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(gallery);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گالری ها" }));
                
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Galleries/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                    {
                        ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x=>x), "LanguageId", "SettingName");

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x=>x,x => x.ModuleId == 26 && x.Name == "ایجاد گالری",null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "Create", true, 200, "نمایش صفحه ایجاد گالری", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گالری ها" }));
               
            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Galleries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create([Bind(Include = "Id,Title,PageAddress,Descr,Abstract,Data,LanguageId,Cover,IsActive")] GalleryCategory Gallery)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (Gallery.Cover != null)
                {
                    if (ModelState.IsValid)
                    {
                        Gallery.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Gallery.Title);
                        if (uow.GalleryCategoryRepository.Get(x=>x).Any())
                            Gallery.Sort = uow.GalleryCategoryRepository.Get(x=>x).Max(x => x.Sort) + 1;
                        uow.GalleryCategoryRepository.Insert(Gallery);
                        uow.Save();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Galleries", "Create", false, 200, "ایجاد گالری " + Gallery.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
                }

                ViewBag.Error = " کاور ( تصویر ) انتخاب نشده است! ";
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 26 && x.Name == "ایجاد گالری", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Create", false, 500, "خطا در ایجاد گالری" + Gallery.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x=>x), "LanguageId", "SettingName");
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                
                return View(Gallery);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Galleries/Create
        public virtual ActionResult SubCreate(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (id.HasValue)
                {
                  
                        if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                        {
                            ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x=>x), "LanguageId", "SettingName");

                            ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 26 && x.Name == "ایجاد گالری", null, "HelpModuleSectionFields").FirstOrDefault();
                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "SubCreate", true, 200, "نمایش صفحه ایجاد گالری", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            ViewBag.Gallery = uow.GalleryCategoryRepository.GetByID(id.Value);

                            return View();
                        }
                        else
                            return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گالری ها" }));
                    }
                    else
                        return RedirectToAction("Index", "Home");
                
            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "SubCreate", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Gallery/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 2))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        var gallery = uow.GalleryCategoryRepository.Get(x=>x,x=>x.Id==id,null, "attachment").Single();
                        if (gallery == null)
                        {
                            return HttpNotFound();
                        }
                        ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x=>x), "LanguageId", "SettingName");

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x=>x,x => x.ModuleId == 26 && x.Name == "ایجاد گالری",null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "Edit", true, 200, "نمایش صفحه ویرایش گالری   " + gallery.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(gallery);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Gallery/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Edit([Bind(Include = "Id,Title,PageAddress,Descr,Abstract,Data,LanguageId,Cover,Sort,IsActive")] GalleryCategory gallery)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    gallery.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(gallery.Title);
                    uow.GalleryCategoryRepository.Update(gallery);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Galleries", "Edit", false, 200, "ویرایش گالری " + gallery.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x=>x,x => x.ModuleId == 26 && x.Name == "ایجاد گالری",null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Edit", false, 500, "خطا در ویرایش گالری" + gallery.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x=>x), "LanguageId", "SettingName", gallery.LanguageId);
                
                return View(gallery);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Gallery/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 3))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        var gallery = uow.GalleryCategoryRepository.GetByID(id);
                        if (gallery == null)
                        {
                            return HttpNotFound();
                        }
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "Delete", true, 200, "نمایش صفحه حذف گالری " + gallery.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(gallery);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گالری ها" }));
              
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Galleries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                var gallery = uow.GalleryCategoryRepository.GetByID(id);
                uow.GalleryCategoryRepository.Delete(gallery);
                uow.Save();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Galleries", "DeleteConfirmed", false, 200, "حذف گالری " + gallery.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                var gallery = uow.GalleryCategoryRepository.GetByID(id);
                ViewBag.Erorr = "این گالری ، دارای زیر گالری های دیگری است. ابتدا باید آنها را پاک نمایید.";
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(gallery);
            }
        }

        [CorrectArabianLetter(new string[] { "TitleString", "TitleFilter" })]
        public virtual ActionResult ImagesManage(int id,string sortOrder, string TitleFilter, string TitleString, string CatIdFilter, string CatIdString, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
              
                    var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();
        
                        #region search
                        if (string.IsNullOrEmpty(TitleString))
                            TitleString = TitleFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(CatIdString))
                            CatIdString = CatIdFilter;
                        else
                            page = 1;

                        ViewBag.TitleFilter = TitleString;
                        ViewBag.CatIdFilter = CatIdString;

                        ViewBag.id = id;

                        ViewBag.CatIdSelectListItem = new SelectList(uow.GalleryCategoryRepository.Get(x=>x), "Id", "Title", id);

                        var GalleryImages = uow.GalleryImageRepository.Get(x => x, x => x.CatId == id, null,"attachment");
                        if (!String.IsNullOrEmpty(TitleString))
                            GalleryImages = GalleryImages.Where(s => s.Title.Contains(TitleString));
                        if (!String.IsNullOrEmpty(CatIdString))
                        {
                            int CatId = Convert.ToInt32(CatIdString);
                            GalleryImages = GalleryImages.Where(s => s.CatId == CatId);
                        }
                        
                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {
                            case "Title":
                                GalleryImages = GalleryImages.OrderBy(s => s.Title);
                                ViewBag.CurrentSort = "Title";
                                break;
                            case "Title_desc":
                                GalleryImages = GalleryImages.OrderByDescending(s => s.Title);
                                ViewBag.CurrentSort = "Title_desc";
                                break;
                            case "CatId":
                                GalleryImages = GalleryImages.OrderBy(s => s.CatId);
                                ViewBag.CurrentSort = "CatId";
                                break;
                            case "CatId_desc":
                                GalleryImages = GalleryImages.OrderByDescending(s => s.CatId);
                                ViewBag.CurrentSort = "CatId_desc";
                                break;
                            default:  // Name ascending 
                                GalleryImages = GalleryImages.OrderByDescending(s => s.Id);
                                break;
                        }

                        #endregion

                        int pageSize = 8;
                        int pageNumber = (page ?? 1);

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x=>x,x => x.ModuleId == 26 && x.Name == "مدیریت تصاویر",null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "ImagesManage", true, 200, " نمایش صفحه مدیریت تصاویر ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(GalleryImages.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تصاویر" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "ImagesManage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Galleries/CreateImage
        public virtual ActionResult CreateImage(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                    {
                        ViewBag.CatId = new SelectList(uow.GalleryCategoryRepository.Get(x=>x), "Id", "Title",id);
                        ViewBag.id = id;

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "CreateImage", true, 200, "نمایش صفحه ایجاد تصویر", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تصاویر" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "CreateImage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Galleries/CreateImage
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult CreateImage([Bind(Include = "Id,Title,CatId,Cover")] GalleryImage GalleryImage)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    GalleryImage.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(GalleryImage.Title);
                    if (uow.GalleryImageRepository.Get(x=>x).Any())
                        GalleryImage.DisplaySort = uow.GalleryImageRepository.Max(x => x.DisplaySort) + 1;
                    uow.GalleryImageRepository.Insert(GalleryImage);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Galleries", "CreateImage", false, 200, "ایجاد تصویر " + GalleryImage.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("ImagesManage", new {id=GalleryImage.CatId});
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "CreateImage", false, 500, "خطا در ایجاد تصویر" + GalleryImage.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.CatId = new SelectList(uow.GalleryCategoryRepository.Get(x=>x), "Id", "Title", GalleryImage.CatId);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                return View(GalleryImage);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "CreateImage", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Gallery/EditImage/5
        public virtual ActionResult EditImage(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 2))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        var galleryImage = uow.GalleryImageRepository.Get(x => x, x => x.Id == id, null, "attachment").Single();
                        if (galleryImage == null)
                        {
                            return HttpNotFound();
                        }
                        ViewBag.CatId = new SelectList(uow.GalleryCategoryRepository.Get(x=>x), "Id", "Title", galleryImage.CatId);

                        ViewBag.id = id;

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "EditImage", true, 200, "نمایش صفحه ویرایش تصویر   " + galleryImage.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(galleryImage);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تصاویر" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "EditImage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Gallery/EditImage/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult EditImage([Bind(Include = "Id,Title,CatId,Cover,Sort")] GalleryImage galleryImage)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    galleryImage.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(galleryImage.Title);
                    uow.GalleryImageRepository.Update(galleryImage);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Galleries", "EditImage", false, 200, "ویرایش تصویر " + galleryImage.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("ImagesManage", new {id=galleryImage.CatId });
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "EditImage", false, 500, "خطا در ویرایش تصویر" + galleryImage.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.CatId = new SelectList(uow.GalleryCategoryRepository.Get(x=>x), "Id", "Title", galleryImage.CatId);

                return View(galleryImage);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "EditImage", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public virtual ActionResult DeleteImage(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                var GalleryImage = uow.GalleryImageRepository.GetByID(id);
                if (GalleryImage != null)
                {
                    uow.GalleryImageRepository.Delete(GalleryImage);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Gallery", "DeleteImage", false, 200, "حذف تصویر " + GalleryImage.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("ImagesManage", new {id=GalleryImage.CatId });
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Gallery", "DeleteImage", false, 500, "خطا در حذف تصویر" + GalleryImage.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(GalleryImage);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Gallery", "DeleteImage", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Gallery/SortImages
        public virtual ActionResult SortImages(int? id,int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                
                    var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        if (id != null)
                        {
                            var GalleryImage = uow.GalleryImageRepository.Get(x=>x,x => x.CatId == id,s=>s.OrderBy(x => x.DisplaySort), "attachment");
                            ViewBag.id = id;

                            int pageSize = 100;
                            int pageNumber = (page ?? 1);

                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "Galleries", "SortImages", true, 200, " نمایش صفحه مرتب سازی تصاویر ", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View(GalleryImage.ToPagedList(pageNumber, pageSize));
                        }
                        else
                        {
                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "SortImages", true, 500, "خطا در نمایش صفحه مرتب سازی تصاویر ", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View("Index");
                        }
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی تصاویر" }));
                
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "SortImages", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Gallery/SortImages/5
        [HttpPost]
        public virtual JsonResult SortImages(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var galleryImage = uow.GalleryImageRepository.GetByID(id);
                    galleryImage.DisplaySort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Galleries", "SortImages", false, 200, "مرتب سازی تصاویر", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "SortImages", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
	}
}