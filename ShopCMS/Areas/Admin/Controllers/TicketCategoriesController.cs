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
    public partial class TicketCategoriesController : Controller
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

                    var Categories = uow.TicketCategoryRepository.Get(x => x);
                    if (!String.IsNullOrEmpty(CatTitleString))
                        Categories = Categories.Where(s => s.Title.Contains(CatTitleString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        Categories = Categories.Where(s => s.LanguageId == langId);
                    }

                    if (!String.IsNullOrEmpty(IsActive))
                    {
                        bool active = Convert.ToBoolean(IsActive);
                        Categories = Categories.Where(s => s.IsActive == active);
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

                        case "IsActive":
                            Categories = Categories.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            Categories = Categories.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;

                        default:  // Name ascending 
                            Categories = Categories.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);



                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "دسته بندی ها", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "TicketCategories", "Index", true, 200, " نمایش صفحه مدیریت دسته بندی ها ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Categories.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتواها" }));


            }
            catch (Exception x)
            {
                #region EventLogger 
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
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
                    var category = uow.TicketCategoryRepository.GetByID(id);
                    category.Sort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "TicketCategories", "Sort", false, 200, "مرتب سازی دسته بندی ها", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
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

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    ViewBag.ParrentId = new SelectList(uow.TicketCategoryRepository.Get(x => x), "Id", "Title");
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    var ContetTypes = readXML.ListOfXContentType();
                    ViewBag.ContentTypeId = ContentTypeId;

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "TicketCategories", "Create", true, 200, "نمایش صفحه ایجاد دسته بندی", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتواها" }));



            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
        public virtual ActionResult Create(TicketCategory category)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModelState.IsValid)
                {
                    category.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(category.Title);

                    if (!uow.TicketCategoryRepository.Get(x => x, x => x.Title == category.Title).Any())
                    {
                       
                        if (uow.TicketCategoryRepository.Get(x => x).Any())
                            category.Sort = uow.TicketCategoryRepository.Max(x => x.Sort) + 1;
                        uow.TicketCategoryRepository.Insert(category);
                        uow.Save();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "TicketCategories", "Create", false, 200, "ایجاد دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index", "TicketCategories");
                    }
                    else
                        ViewBag.Error = "دسته بندی با این عنوان وجود دارد.";
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Create", false, 500, "خطا در ایجاد دسته بندیِ" + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                return View(category);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                    TicketCategory category = uow.TicketCategoryRepository.Get(x => x, x => x.Id == id).Single();
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "TicketCategories", "Edit", true, 200, "نمایش صفحه ویرایش دسته بندیِ   " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(category);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
        public virtual ActionResult Edit(TicketCategory category)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    category.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(category.Title);
                    if (!uow.TicketCategoryRepository.Get(x => x, x => x.Title == category.Title && x.Id != category.Id).Any())
                    {
                        uow.TicketCategoryRepository.Update(category);
                        uow.Save();


                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(3, "TicketCategories", "Edit", false, 200, "ویرایش دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index", "TicketCategories");
                    }
                    else
                        ViewBag.Error = "دسته بندی با این عنوان وجود دارد.";
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Edit", false, 500, "خطا در ویرایش دسته بندیِ" + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", category.LanguageId);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var ContetTypes = readXML.ListOfXContentType();

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 13 && x.Name == "ایجاد دسته بندی", null, "HelpModuleSectionFields").FirstOrDefault();
                return View(category);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                    TicketCategory category = uow.TicketCategoryRepository.GetByID(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "TicketCategories", "Delete", true, 200, "نمایش صفحه حذف دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(category);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
            TicketCategory category = uow.TicketCategoryRepository.Get(x => x, x => x.Id == id, null, "Tickets").Single();
            try
            {
                if (category.Tickets.Any())
                {
                    ViewBag.Erorr = "این دسته بندی ، دارای" + category.Tickets.Count + " محتوا است. ابتدا باید آنها را پاک نمایید.";
                    return View(category);
                }
                uow.TicketCategoryRepository.Delete(category);
                uow.Save();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "TicketCategories", "DeleteConfirmed", false, 200, "حذف دسته بندیِ " + category.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "TicketCategories");
            }
            catch (Exception s)
            {
                ViewBag.Erorr = "این دسته بندی ، دارای زیر دسته بندی های دیگری است. ابتدا باید آنها را پاک نمایید.";
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "TicketCategories", "DeleteConfirmed", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());
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
