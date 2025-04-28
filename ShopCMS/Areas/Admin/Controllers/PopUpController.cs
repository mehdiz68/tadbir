using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.ViewModels.Setting;
using ahmadi.Infrastructure.Security;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class PopUpController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;


        // GET: Admin/PopUp
        public virtual ActionResult Index()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
              
                    var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.AddPermission = p.First();
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.DeletePermission = p.Skip(2).First();
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        var languages = readXml.ListOfXLanguage();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "PopUp", "Index", true, 200, " نمایش صفحه مدیریت پاپ آپ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        if (languages.Count <= 1)
                        {
                            var currentLanguage = languages.First();
                            setting = uow.SettingRepository.Get(x=>x,x => x.LanguageId == currentLanguage.Id).FirstOrDefault();
                            if (setting != null)
                                return Redirect("~/Admin/PopUp/Edit/" + setting.Id);
                            else
                                return RedirectToAction("~/Admin");
                        }
                        else
                        {
                            ViewBag.HelpModule = uow.HelpModuleRepository.Get(x=>x,x => x.Name == "اعلانات (پاپ آپ)",null, "HelpModuleSections").FirstOrDefault();
                            return View(languages);
                        }

                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PopUp", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/PopUp/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.Id == id, null, "attachment").SingleOrDefault();
            try
            {
                
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        SettingViewModels currentSetting = new SettingViewModels(setting);
                        if (setting == null)
                        {
                            return HttpNotFound();
                        }
                        XMLReader readXml = new XMLReader(setting.StaticContentDomain);
                        List<short> languages = readXml.ListOfXLanguage().Select(x=>x.Id).ToList();
                        var settings = uow.SettingRepository.Get(x=>x,x => languages.Contains(x.LanguageId.Value));
                        ViewBag.settingCount = settings.Count();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "PopUp", "Edit", true, 200, " نمایش صفحه ویرایش پاپ آپ" , DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(currentSetting);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PopUp", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/PopUp/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public virtual ActionResult Edit([Bind(Include = "PopUpMessage,PopUpActive,PopUpType,PopUpEditVersion")] SettingViewModels Setting,int SettingId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                Setting Osetting = uow.SettingRepository.GetByID(SettingId);

                Osetting.PopUpMessage = Setting.PopUpMessage;
                Osetting.PopUpActive = Setting.PopUpActive;
                Osetting.PopUpEditVersion = Osetting.PopUpEditVersion + 1;
                Osetting.PopUpType = Setting.PopUpType;
                uow.SettingRepository.Update(Osetting);
                uow.Save();

                HttpContext.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Response.AddHeader("Expires", "0");
                Session["settingPersian"] = null;
                Session["Languages"] = null;
                Session["HomePage"] = null;

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Settings", "Edit", false, 200, "   ویرایش تنظیم سایتِ " + Setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");



            }
            catch (Exception x)
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(y=>y,y => y.Name == "ایجاد تنظیم جدید",null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Settings", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
