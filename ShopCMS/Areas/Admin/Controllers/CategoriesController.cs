using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using PagedList;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class CategoriesController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;

        // GET: Admin/Categories
        [CorrectArabianLetter(new string[] { "CatTitleString", "CatTitleFilter" })]
        public virtual ActionResult Index(string LanguageId, string sortOrder, string CatTitleFilter, string CatTitleString, string LanguagenameFilter, string LanguagenameString, string ParrentCatFilter, string ParrentCatString, string IsActive, string IsActiveFilter, string ContentTypeId, string ContentTypeIdFilter, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ContentTypeId != null || ContentTypeIdFilter != null)
                {
                    int contenttypeid = 0;
                    if (ContentTypeId != null)
                        contenttypeid = Convert.ToInt32(ContentTypeId);
                    else
                        contenttypeid = Convert.ToInt32(ContentTypeIdFilter);
                    ViewBag.ContentTypeName = CoreLib.Infrastructure.CommonFunctions.GetContentTypeName(contenttypeid, setting.StaticContentDomain);
                    ViewBag.contenttypeid = contenttypeid;




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
                        if (string.IsNullOrEmpty(ContentTypeId))
                            ContentTypeId = ContentTypeIdFilter;

                        ViewBag.IsActiveFilter = IsActive;
                        ViewBag.CatTitleFilter = CatTitleString;
                        ViewBag.LanguagenameFilter = LanguagenameString;
                        ViewBag.ParrentCatFilter = ParrentCatString;
                        ViewBag.ContentTypeIdFilter = ContentTypeId;

                        var Categories = uow.CategoryRepository.Get(x => x, null, null, "attachment,ParentCat");
                        if (!String.IsNullOrEmpty(CatTitleString))
                            Categories = Categories.Where(s => s.Title.Contains(CatTitleString));
                        if (!String.IsNullOrEmpty(LanguagenameString))
                        {
                            int langId = Convert.ToInt32(LanguagenameString);
                            Categories = Categories.Where(s => s.LanguageId == langId);
                        }
                        if (!String.IsNullOrEmpty(ParrentCatString))
                        {
                            int parrentId = Convert.ToInt32(ParrentCatString);
                            Categories = Categories.Where(s => s.ParrentId == parrentId);
                        }
                        if (!String.IsNullOrEmpty(IsActive))
                        {
                            bool active = Convert.ToBoolean(IsActive);
                            Categories = Categories.Where(s => s.IsActive == active);
                        }
                        if (!String.IsNullOrEmpty(ContentTypeId))
                        {
                            int ContentTypeid = Convert.ToInt32(ContentTypeId);
                            Categories = Categories.Where(s => s.ContentTypeId == ContentTypeid);
                        }

                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {
                            case "CatTitle":
                                Categories = Categories.OrderBy(s => s.Title);
                                ViewBag.CurrentSort = "CatTitle";
                                break;
                            case "CatTitle_desc":
                                Categories = Categories.OrderByDescending(s => s.Title);
                                ViewBag.CurrentSort = "CatTitle_desc";
                                break;
                            case "Languagename":
                                Categories = Categories.OrderBy(s => s.LanguageId);
                                ViewBag.CurrentSort = "Languagename";
                                break;
                            case "Languagename_desc":
                                Categories = Categories.OrderByDescending(s => s.LanguageId);
                                ViewBag.CurrentSort = "Languagename_desc";
                                break;
                            case "ParrentCat":
                                Categories = Categories.OrderBy(s => s.ParentCat.Title);
                                ViewBag.CurrentSort = "ParrentCat";
                                break;
                            case "ParrentCat_desc":
                                Categories = Categories.OrderByDescending(s => s.ParentCat.Title);
                                ViewBag.CurrentSort = "ParrentCat_desc";
                                break;
                            case "IsActive":
                                Categories = Categories.OrderBy(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive";
                                break;
                            case "IsActive_desc":
                                Categories = Categories.OrderByDescending(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive_desc";
                                break; 
                            case "Defalut":
                                Categories = Categories.OrderBy(s => s.ShowInDefault);
                                ViewBag.CurrentSort = "Defalut";
                                break;
                            case "Defalut_desc":
                                Categories = Categories.OrderByDescending(s => s.ShowInDefault);
                                ViewBag.CurrentSort = "Defalut_desc";
                                break;
                            case "ContentTypeId":
                                Categories = Categories.OrderBy(s => s.ContentTypeId);
                                ViewBag.CurrentSort = "ContentTypeId";
                                break;
                            case "ContentTypeId_desc":
                                Categories = Categories.OrderByDescending(s => s.ContentTypeId);
                                ViewBag.CurrentSort = "ContentTypeId_desc";
                                break;
                            default:  // Name ascending 
                                Categories = Categories.OrderByDescending(s => s.Id);
                                break;
                        }

                        #endregion

                        int pageSize = 8;
                        int pageNumber = (page ?? 1);

                        #region Tree

                        var ContetTypes = readXML.ListOfXContentType();

                        if (!String.IsNullOrEmpty(LanguageId))
                        {
                            ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", LanguageId);
                            int langId = Convert.ToInt32(LanguageId);
                            var TreeCategories = uow.CategoryRepository.Get(x => x, c => c.ParrentId == null && c.ContentTypeId == contenttypeid && c.LanguageId == langId, c => c.OrderBy(s => s.Sort), "attachment,ParentCat");

                            ViewBag.TreeCategories = TreeCategories.ToList();
                        }
                        else
                        {
                            ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                            var TreeCategories = uow.CategoryRepository.Get(x => x, c => c.ParrentId == null && c.ContentTypeId == contenttypeid, c => c.OrderBy(s => s.Sort), "attachment,ParentCat");

                            ViewBag.TreeCategories = TreeCategories.ToList();
                        }
                        #endregion

                        ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "دسته بندی ها", null, "HelpModuleSections").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Categories", "Index", true, 200, " نمایش صفحه مدیریت دسته بندی ها ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Categories.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتواها" }));

                }

                else
                    return RedirectToAction("Index", "Home");
            }
            catch (Exception x)
            {
                #region EventLogger 
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        public virtual JsonResult FilterCategory(int? ContentTypeId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            if (ContentTypeId.HasValue)
            {
                var Categories = new SelectList(uow.CategoryRepository.Get(x => x, x => x.ContentTypeId == ContentTypeId && x.IsVideo == false), "Id", "Title");
                return Json(new
                {
                    success = 1,
                    data = Categories
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var Categories = new SelectList(uow.CategoryRepository.Get(x => x, x => x.ContentTypeId == null && x.IsVideo == true), "Id", "Title");
                return Json(new
                {
                    success = 1,
                    data = Categories
                }, JsonRequestBehavior.AllowGet);
            }
        }


        // POST: Admin/Categories/Sort/5
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
                    var category = uow.CategoryRepository.GetByID(id);
                    category.Sort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Categories", "Sort", false, 200, "مرتب سازی دسته بندی ها", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Admin/Categories/Details/5
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
                    Category category = uow.CategoryRepository.GetByID(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Categories", "Details", true, 200, " نمایش جزئیاتِ دسته بندیِ  " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(category);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتواها" }));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Categories/Create
        public virtual ActionResult Create(int? ContentTypeId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ContentTypeId.HasValue)
                {



                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                    {
                        ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                        ViewBag.ParrentId = new SelectList(uow.CategoryRepository.Get(x => x, x => x.IsVideo == true && !x.ContentTypeId.HasValue), "Id", "Title");
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                        var ContetTypes = readXML.ListOfXContentType();
                        ViewBag.ContentTypeId = ContentTypeId;

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Categories", "Create", true, 200, "نمایش صفحه ایجاد دسته بندی", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتواها" }));

                }
                else
                    return RedirectToAction("Index", "Home");

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create( Category category)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModelState.IsValid)
                {
                    category.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(category.Title);

                    if (!uow.CategoryRepository.Get(x => x, x => x.Title == category.Title && x.ContentTypeId == category.ContentTypeId).Any())
                    {
                        if (category.ContentTypeId == null)
                            category.IsVideo = true;
                        else
                            category.IsVideo = false;
                        if (uow.CategoryRepository.Get(x => x).Any())
                            category.Sort = uow.CategoryRepository.Max(x => x.Sort) + 1;
                        uow.CategoryRepository.Insert(category);
                        uow.Save();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Categories", "Create", false, 200, "ایجاد دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index", "Categories", new { ContentTypeId = category.ContentTypeId });
                    }
                    else
                        ViewBag.Error = "دسته بندی با این عنوان وجود دارد.";
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Create", false, 500, "خطا در ایجاد دسته بندیِ" + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                ViewBag.ParrentId = new SelectList(uow.CategoryRepository.Get(x => x), "Id", "Title", category.ParrentId);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                ViewBag.ContentTypeId = category.ContentTypeId;
                return View(category);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Categories/Create
        public virtual ActionResult SubCreate(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (id.HasValue)
                {



                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                    {
                        ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Categories", "Create", true, 200, "نمایش صفحه ایجاد دسته بندی", DateTime.Now, User.Identity.GetUserId());
                        #endregion

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                        ViewBag.category = uow.CategoryRepository.GetByID(id.Value);
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتواها" }));

                }
                else
                    return RedirectToAction("Index", "Home");

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult SubCreate( Category category)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    category.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(category.Title);
                    if (!uow.CategoryRepository.Get(x => x, x => x.Title == category.Title && x.ContentTypeId == category.ContentTypeId).Any())
                    {
                        if (category.ContentTypeId == null)
                            category.IsVideo = true;
                        else
                            category.IsVideo = false;
                        if (uow.CategoryRepository.Get(x => x).Any())
                            category.Sort = uow.CategoryRepository.Max(x => x.Sort) + 1;
                        uow.CategoryRepository.Insert(category);
                        uow.Save();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Categories", "Create", false, 200, "ایجاد دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index", "Categories", new { ContentTypeId = category.ContentTypeId });
                    }
                    else
                        ViewBag.Error = "دسته بندی با این عنوان وجود دارد.";
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Create", false, 500, "خطا در ایجاد دسته بندیِ" + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                ViewBag.ParrentId = new SelectList(uow.CategoryRepository.Get(x => x), "Id", "Title", category.ParrentId);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.category = uow.CategoryRepository.GetByID(category.ParrentId.Value);
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                ViewBag.ContentTypeId = category.ContentTypeId;
                return View(category);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Categories/Edit/5
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
                    Category category = uow.CategoryRepository.Get(x=>x,x=>x.Id==id,null, "attachment").Single();
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    ViewBag.ParrentId = new SelectList(uow.CategoryRepository.Get(x => x, x => x.ContentTypeId == category.ContentTypeId), "Id", "Title", category.ParrentId);
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    var ContetTypes = readXML.ListOfXContentType();
                    ViewBag.ContentTypeId = new SelectList(ContetTypes, "Id", "Name", category.ContentTypeId);


                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Categories", "Edit", true, 200, "نمایش صفحه ویرایش دسته بندیِ   " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(category);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

                return RedirectToAction("Index", "Home");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Edit(Category category)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    category.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(category.Title);
                    if (!uow.CategoryRepository.Get(x => x, x => x.Title == category.Title && x.Id != category.Id && x.ContentTypeId == category.ContentTypeId).Any())
                    {
                        uow.CategoryRepository.Update(category);
                        uow.Save();

                        //Active/DeActive Content Of Categories
                        if (category.IsActive)
                        {
                            var contents = uow.ContentRepository.Get(x => x, x => x.CatId == category.Id);
                            foreach (var item in contents)
                                item.IsActive = true;
                            uow.Save();
                        }
                        else
                        {
                            var contents = uow.ContentRepository.Get(x => x, x => x.CatId == category.Id);
                            foreach (var item in contents)
                                item.IsActive = false;
                            uow.Save();

                        }

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(3, "Categories", "Edit", false, 200, "ویرایش دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index", "Categories", new { ContentTypeId = category.ContentTypeId });
                    }
                    else
                        ViewBag.Error = "دسته بندی با این عنوان وجود دارد.";
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Edit", false, 500, "خطا در ویرایش دسته بندیِ" + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", category.LanguageId);
                ViewBag.ParrentId = new SelectList(uow.CategoryRepository.Get(x => x), "Id", "Title", category.ParrentId);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var ContetTypes = readXML.ListOfXContentType();

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                ViewBag.ContentTypeId = new SelectList(ContetTypes, "Id", "Name", category.ContentTypeId);
                return View(category);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Categories/Delete/5
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
                    Category category = uow.CategoryRepository.GetByID(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Categories", "Delete", true, 200, "نمایش صفحه حذف دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(category);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            Category category = uow.CategoryRepository.Get(x=>x,x=>x.Id==id,null, "attachment,Contents").Single();
            try
            {
                if (category.ChildCategory.Any())
                {
                    ViewBag.Erorr = "این دسته بندی ، دارای" + category.ChildCategory.Count + " زیردسته بندی است. ابتدا باید آنها را پاک نمایید.";
                    return View(category);
                }
                if (category.Contents.Any())
                {
                    ViewBag.Erorr = "این دسته بندی ، دارای" + category.Contents.Count + " محتوا است. ابتدا باید آنها را پاک نمایید.";
                    return View(category);
                }
                int ContentTypeId = category.ContentTypeId.Value;
                uow.CategoryRepository.Delete(category);
                uow.Save();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Categories", "DeleteConfirmed", false, 200, "حذف دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Categories", new { ContentTypeId = ContentTypeId });
            }
            catch (Exception s)
            {
                ViewBag.Erorr = "این دسته بندی ، دارای زیر دسته بندی های دیگری است. ابتدا باید آنها را پاک نمایید.";
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Categories", "DeleteConfirmed", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(category);
            }
        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
