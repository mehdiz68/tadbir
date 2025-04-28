using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;
using UnitOfWork;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class SocialsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uof = null;

        // GET: Admin/Socials
        [CorrectArabianLetter(new string[] { "TitleString", "TitleFilter" })]
        public virtual ActionResult Index(string sortOrder, string TitleFilter, string TitleString, string LanguagenameFilter, string LanguagenameString, string PlaceShowFilter)
        {
            uof = new UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 3);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    XMasterTheme master = readXML.ListOfXMasterTheme().First();
                    List<SelectListItem> PlaceShowSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "هدر (بالا) سایت", Value = "1" }, new SelectListItem() { Text = "فوتر (پایین) سایت", Value = "2" }, new SelectListItem() { Text = "هر دو", Value = "3" } };
                    if (!master.HeaderSocialNetworksVisibility)
                        PlaceShowSelectListItem.Remove(PlaceShowSelectListItem.Where(x => x.Value == "1").First());
                    if (!master.FooterSocialNetworksVisibility)
                        PlaceShowSelectListItem.Remove(PlaceShowSelectListItem.Where(x => x.Value == "2").First());
                    if (!master.HeaderSocialNetworksVisibility || !master.FooterSocialNetworksVisibility)
                    {
                        PlaceShowSelectListItem.Remove(PlaceShowSelectListItem.Where(x => x.Value == "3").First());
                    }
                    ViewBag.PlaceShow = PlaceShowSelectListItem;

                    #region search
                    if (string.IsNullOrEmpty(TitleString))
                        TitleString = TitleFilter;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;

                    ViewBag.TitleFilter = TitleString;
                    ViewBag.LanguagenameFilter = LanguagenameString;

                    var socials = uof.SocialRepository.Get(x => x, null, null, "attachment");
                    if (!String.IsNullOrEmpty(TitleString))
                        socials = socials.Where(s => s.Title.Contains(TitleString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        socials = socials.Where(s => s.LanguageId == langId);
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "Title":
                            socials = socials.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Title";
                            break;
                        case "Title_desc":
                            socials = socials.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Title_desc";
                            break;
                        case "Languagename":
                            socials = socials.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename";
                            break;
                        case "Languagename_desc":
                            socials = socials.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Languagename_desc";
                            break;
                        case "DisplaySort":
                            socials = socials.OrderBy(s => s.DisplaySort);
                            ViewBag.CurrentSort = "DisplaySort";
                            break;
                        case "DisplaySort_desc":
                            socials = socials.OrderByDescending(s => s.DisplaySort);
                            ViewBag.CurrentSort = "DisplaySort_desc";
                            break;
                        case "IsActive":
                            socials = socials.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            socials = socials.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;
                        default:  // Name ascending 
                            socials = socials.OrderBy(s => s.DisplaySort);
                            break;
                    }
                    #endregion

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, "Socials", "Index", true, 200, " نمایش صفحه مدیریت شبکه های اجتماعی", DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    ViewBag.HelpModule = uof.HelpModuleRepository.Get(x => x, x => x.Name == "شبکه های اجتماعی", null, "HelpModuleSections").FirstOrDefault();
                    return View(socials.ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت شبکه های اجتماعی" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // GET: Admin/Socials/Create
        public virtual ActionResult Create()
        {
            uof = new UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 3, 1))
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");


                    ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 7 && x.Name == "ایجاد شبکه جدید", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, " Socials", "Create", true, 200, " نمایش صفحه ایجاد شبکه اجتماعی", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت شبکه های اجتماعی" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Categories/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uof = new UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var social = uof.SocialRepository.GetByID(id);
                    social.DisplaySort = i;
                    uof.Save();
                }
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(1, "Socials", "Sort", false, 200, "   مرتب سازی شبکه های اجتماعی", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        // POST: Admin/Socials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Cover,Title,Link,DisplaySort,IsActive,LanguageId,Icon")] Social social)
        {
            uof = new UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid && (social.Cover != null || social.Icon != null))
                {
                    social.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(social.Title);
                    if (uof.SocialRepository.GetByReturnQueryable(x => x).Any())
                        social.DisplaySort = uof.SocialRepository.Max(x => x.DisplaySort) + 1;
                    uof.SocialRepository.Insert(social);
                    uof.Save();

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "Socials", "Create", false, 200, "   ایجاد شبکه ی " + social.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                if (social.Cover == null)
                    ViewBag.message = " تصویر یا آیکن انتخاب نشده است. ";


                ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 7 && x.Name == "ایجاد شبکه جدید", null, "HelpModuleSectionFields").FirstOrDefault();

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(2, "Socials", "Create", false, 500, "   خطا در ایجاد شبکه ی " + social.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", social.LanguageId);
                return View(social);

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Socials/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uof = new UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 3, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Social social = uof.SocialRepository.Get(x => x, x => x.Id == id, null, "attachment").Single();
                    if (social == null)
                    {
                        return HttpNotFound();
                    }

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", social.LanguageId);

                    ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 7 && x.Name == "ایجاد شبکه جدید", null, "HelpModuleSectionFields").FirstOrDefault();

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, "Socials", "Edit", true, 200, " نمایش صفحه ویرایش شبکه ی " + social.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(social);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت شبکه های اجتماعی" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Socials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Cover,Title,Link,DisplaySort,IsActive,LanguageId,Icon")] Social social)
        {
            uof = new UnitOfWorkClass();
            var setting = uof.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    social.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(social.Title);
                    uof.SocialRepository.Update(social);
                    uof.Save();

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(3, "Socials", "Edit", false, 200, "   ویرایش شبکه ی" + social.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                ViewBag.Cover = new SelectList(uof.AttachmentRepository.GetByReturnQueryable(x => x), "Id", "Title", social.Cover);
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name", social.LanguageId);

                ViewBag.HelpModuleSection = uof.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 7 && x.Name == "ایجاد شبکه جدید", null, "HelpModuleSectionFields").FirstOrDefault();

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(3, "Socials", "Edit", false, 500, "   خطا در ویرایش شبکه ی" + social.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(social);

            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Socials/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uof = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 3, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Social social = uof.SocialRepository.Get(x => x, x => x.Id == id, null, "attachment").Single();
                    if (social == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(1, "Socials", "Delete", true, 200, " نمایش صفحه حذف شبکه ی" + social.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(social);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت شبکه های اجتماعی" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Socials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uof = new UnitOfWorkClass();
            try
            {
                Social social = uof.SocialRepository.GetByID(id);
                uof.SocialRepository.Delete(social);
                uof.Save();

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(4, "Socials", "DeleteConfirmed", false, 200, "   حذف شبکه ی" + social.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Socials", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
    }
}
