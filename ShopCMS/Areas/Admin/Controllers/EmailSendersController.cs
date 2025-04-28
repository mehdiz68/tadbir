using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class EmailSendersController : Controller
    {
        UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/EmailSenders
        public virtual ActionResult Index()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    var emailSenders = uow.EmailsenderRepository.Get(x => x, null, x => x.OrderBy(s => s.DisplaySort), "Setting");

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت ایمیل های اطلاع رسان").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "EmailSenders", "Index", true, 200, " نمایش صفحه مدیریت ایمیل های اطلاع رسان", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(emailSenders.ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POST: Admin/EmailSender/Sort/5
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
                    var EmailSender = uow.EmailsenderRepository.GetByID(id);
                    EmailSender.DisplaySort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "EmailSenders", "Sort", true, 200, " نمایش صفحه مرتب سازی ایمیل های اطلاع رسان", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Admin/EmailSenders/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                var EmailSenders = uow.EmailsenderRepository.Get(x => x, null, x => x.OrderBy(s => s.DisplaySort), "Setting");
                //if (EmailSenders.Select(x => x.Setting).Count() > EmailSenders.Count())
                {


                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                    {
                        ViewBag.SettingId = new SelectList(uow.SettingRepository.Get(x => x, null, null), "Id", "SettingName");

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت ایمیل های اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "EmailSenders", "Create", true, 200, " نمایش صفحه ایجاد ایمیلِ اطلاع رسان ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

                }
                //else
                //    return RedirectToAction("Index", "EmailSenders");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/EmailSenders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,Email,Password,MailServer,SettingId,IsActive,Port")] EmailSender emailSender)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    emailSender.EncryptPassword();
                    if (uow.EmailsenderRepository.Get(x => x).Any())
                        emailSender.DisplaySort = uow.EmailsenderRepository.Max(x => x.DisplaySort) + 1;
                    uow.EmailsenderRepository.Insert(emailSender);
                    uow.Save();
                    UpdateAppSetting(emailSender.SettingId);

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت ایمیل های اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "EmailSenders", "Create", false, 200, "   ایجاد ایمیلِ اطلاع رسانِ " + emailSender.Email, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Create", false, 500, "   خطا در ایجاد ایمیلِ اطلاع رسانِ " + emailSender.Email, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.SettingId = new SelectList(uow.SettingRepository.Get(x => x, null, null), "Id", "SettingName", emailSender.SettingId);
                return View(emailSender);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/EmailSenders/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    EmailSender emailSender = uow.EmailsenderRepository.GetByID(id);
                    if (emailSender == null)
                    {
                        return HttpNotFound();
                    }
                    emailSender.DecryptPassword();
                    ViewBag.SettingId = new SelectList(uow.SettingRepository.Get(x => x), "Id", "SettingName", emailSender.SettingId);

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت ایمیل های اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "EmailSenders", "Edit", true, 200, " نمایش صفحه ویرایش ایمیلِ اطلاع رسانِ " + emailSender.Email, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(emailSender);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/EmailSenders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,Email,Password,MailServer,SettingId,IsActive,DisplaySort,Port")] EmailSender emailSender)
        {
            uow = new UnitOfWork.UnitOfWorkClass();

            try
            {
                if (ModelState.IsValid)
                {
                    emailSender.EncryptPassword();
                    uow.EmailsenderRepository.Update(emailSender);
                    uow.Save();
                    UpdateAppSetting(emailSender.SettingId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "EmailSenders", "Edit", false, 200, "   ویرایش ایمیلِ اطلاع رسانِ " + emailSender.Email, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت ایمیل های اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Edit", false, 500, "   خطا در ویرایش ایمیلِ اطلاع رسانِ " + emailSender.Email, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.SettingId = new SelectList(uow.SettingRepository.Get(x => x), "Id", "SettingName", emailSender.SettingId);
                return View(emailSender);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "EmailSenders", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        private void UpdateAppSetting(int SettingId)
        {
            var model = uow.SettingRepository.Get(x => x, x => x.Id == SettingId, null).FirstOrDefault();
            if (model.EmailSenders != null)
            {
                var Activeemailsender = model.EmailSenders.Where(x => x.IsActive == true).OrderBy(x => x.DisplaySort);
                if (Activeemailsender.Any())
                {
                    Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["credentialUserName"].Value = Activeemailsender.First().Email;
                    myConfiguration.AppSettings.Settings["pwd"].Value = Activeemailsender.First().DecryptedPassword;
                    myConfiguration.AppSettings.Settings["smtp"].Value = Activeemailsender.First().MailServer;
                    myConfiguration.AppSettings.Settings["Port"].Value = Activeemailsender.First().Port.ToString();
                    myConfiguration.AppSettings.Settings["siteName"].Value = model.WebSiteName;
                    myConfiguration.Save();
                }
                else
                {
                    Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["credentialUserName"].Value = "info@site1.com";
                    myConfiguration.AppSettings.Settings["pwd"].Value = "123456";
                    myConfiguration.AppSettings.Settings["smtp"].Value = "mail.site1.com";
                    myConfiguration.AppSettings.Settings["Port"].Value = "25";
                    myConfiguration.AppSettings.Settings["siteName"].Value = "site1";
                    myConfiguration.Save();
                }
            }
            else
            {
                Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["credentialUserName"].Value = "info@site1.com";
                myConfiguration.AppSettings.Settings["pwd"].Value = "123456";
                myConfiguration.AppSettings.Settings["smtp"].Value = "mail.site1.com";
                myConfiguration.AppSettings.Settings["Port"].Value = "25";
                myConfiguration.AppSettings.Settings["siteName"].Value = "site1";
                myConfiguration.Save();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


    }
}
