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
using Microsoft.Ajax.Utilities;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class BrandController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        //
        // GET: /Admin/Brand/
        [CorrectArabianLetter(new string[] { "NameString", "NameFilter" })]
        public virtual ActionResult Index(string LanguageId, string sortOrder, string NameFilter, string NameString, string LanguagenameFilter, string LanguagenameString, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
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
                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;

                    ViewBag.NameFilter = NameString;
                    ViewBag.LanguagenameFilter = LanguagenameString;

                    var Brands = uow.BrandRepository.Get(x => x, null, null, "attachment,attachmentHomePage");
                    if (!String.IsNullOrEmpty(NameString))
                        Brands = Brands.Where(s => s.Name.ToLower().Contains(NameString.ToLower()) || s.Title.ToLower().Contains(NameString.ToLower()) || s.PersianName.ToLower().Contains(NameString.ToLower()));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        Brands = Brands.Where(s => s.LanguageId == langId);
                    }


                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Name":
                            Brands = Brands.OrderBy(s => s.Name);
                            ViewBag.CurrentSort = "Name";
                            break;
                        case "Name_desc":
                            Brands = Brands.OrderByDescending(s => s.Name);
                            ViewBag.CurrentSort = "Name_desc";
                            break;
                        case "Languagename":
                            Brands = Brands.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename";
                            break;
                        case "Languagename_desc":
                            Brands = Brands.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename_desc";
                            break;
                        default:  // Name ascending 
                            Brands = Brands.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);



                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت برندها", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Brands", "Index", true, 200, " نمایش صفحه مدیریت برندها ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Brands.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Galleries", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Brand/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 32 && x.Name == "ایجاد برند", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Brand", "Create", true, 200, "نمایش صفحه ایجاد برند", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت برند ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brand", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create( Brand Brand)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModelState.IsValid)
                {
                    Brand.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Brand.Name);
                    Brand.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Brand.Title);
                    Brand.Name = CoreLib.Infrastructure.CommonFunctions.NormalizeAddressWithSpace(Brand.Name);
                    uow.BrandRepository.Insert(Brand);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Brand", "Create", false, 200, "ایجاد برند " + Brand.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }


                ViewBag.Error = " خطایی رخ داد ";
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 32 && x.Name == "ایجاد برند", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brand", "Create", false, 500, "خطا در ایجاد برند" + Brand.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                return View(Brand);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brand", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                

               
                    if (ModulePermission.check(User.Identity.GetUserId(), 22, 2))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        var brand = uow.BrandRepository.Get(x=>x,x=>x.Id==id,null, "attachment,attachmentHomePage").Single();
                        if (brand == null)
                        {
                            return HttpNotFound();
                        }
                        ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 26 && x.Name == "ایجاد گالری", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "brand", "Edit", true, 200, "نمایش صفحه ویرایش برند   " + brand.Name, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(brand);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "brand", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POST: Admin/Gallery/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Edit( Brand Brand)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    Brand.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Brand.Title);
                    Brand.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Brand.Name);
                    Brand.Name = CoreLib.Infrastructure.CommonFunctions.NormalizeAddressWithSpace(Brand.Name);
                    uow.BrandRepository.Update(Brand);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Brand", "Edit", false, 200, "ویرایش گالری " + Brand.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 32 && x.Name == "ایجاد برند", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brand", "Edit", false, 500, "خطا در ویرایش گالری" + Brand.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", Brand.LanguageId);

                return View(Brand);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brand", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                

               
                    if (ModulePermission.check(User.Identity.GetUserId(), 22, 3))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        var Brand = uow.BrandRepository.Get(x=>x,x=>x.Id==id,null, "attachment,attachmentHomePage").Single();
                        if (Brand == null)
                        {
                            return HttpNotFound();
                        }
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Brand", "Delete", true, 200, "نمایش صفحه حذف برند " + Brand.Name, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Brand);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت گالری ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brand", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                var Brand = uow.BrandRepository.GetByID(id);
                uow.BrandRepository.Delete(Brand);
                uow.Save();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Brand", "DeleteConfirmed", false, 200, "حذف گالری " + Brand.Name, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                var Brand = uow.BrandRepository.GetByID(id);
                ViewBag.Erorr = "خطایی رخ داد";
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Brand", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(Brand);
            }
        }
    }
}