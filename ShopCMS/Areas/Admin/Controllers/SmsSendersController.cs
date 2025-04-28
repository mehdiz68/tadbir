using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using System.Configuration;
using Microsoft.Ajax;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperUser")]
    public partial class SmsSendersController : Controller
    {
        UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/SmsSenders
        public virtual ActionResult Index()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                IdentityManager im = new IdentityManager();

                var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    var SmsSenders = uow.SmssenderRepository.Get(x => x, null, s => s.OrderBy(x => x.DisplaySort), "Setting");

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت سرویس های sms اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SmsSenders", "Index", true, 200, " نمایش صفحه مدیریت سرویس ها sms", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(SmsSenders.ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/SmsSender/Sort/5
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
                    var SmsSenders = uow.SmssenderRepository.GetByID(id);
                    SmsSenders.DisplaySort = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SmsSenders", "Sort", true, 200, " نمایش صفحه مرتب سازی ایمیل های اطلاع رسان", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: Admin/SmsSenders/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                {
                    ViewBag.SettingId = new SelectList(uow.SettingRepository.Get(x => x), "Id", "SettingName");
                    List<SelectListItem> CompanyId = new List<SelectListItem>() { new SelectListItem() { Text = "نیک اس ام اس", Value = "1" }, new SelectListItem() { Text = "پیامک پنل", Value = "2" }, new SelectListItem() { Text = "کاوه نگار", Value = "3" } };
                    ViewBag.CompanyId = CompanyId;

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت سرویس های sms اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SmsSenders", "Create", true, 200, " نمایش صفحه ایجاد ایمیلِ اطلاع رسان ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/SmsSenders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SmsSender smsSender)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (smsSender.IsActive)
                {
                    uow.SmssenderRepository.Get(x => x).ForEach(x => x.IsActive = false);
                    uow.Save();
                }
                if (ModelState.IsValid)
                {
                    smsSender.EncryptPassword();
                    if (uow.SmssenderRepository.Get(x => x).Any())
                        smsSender.DisplaySort = uow.SmssenderRepository.Max(x => x.DisplaySort) + 1;
                    uow.SmssenderRepository.Insert(smsSender);
                    uow.Save();
                    UpdateAppSetting(smsSender.SettingId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "SmsSenders", "Create", false, 200, "   ایجاد  اس ام اس  اطلاع رسان   " + smsSender.Username, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت سرویس های sms اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                ViewBag.SettingId = new SelectList(uow.SettingRepository.Get(x => x), "Id", "SettingName");
                List<SelectListItem> CompanyId = new List<SelectListItem>() { new SelectListItem() { Text = "نیک اس ام اس", Value = "1" }, new SelectListItem() { Text = "پیامک پنل", Value = "2" }, new SelectListItem() { Text = "کاوه نگار", Value = "3" } };

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Create", false, 500, "   ایجاد  اس ام اس  اطلاع رسان   " + smsSender.Username, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(smsSender);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Create", false, 500, dbEx.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                throw;  // You can also choose to handle the exception here...
            }

        }

        // GET: Admin/SmsSenders/Edit/5
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
                    SmsSender SmsSender = uow.SmssenderRepository.GetByID(id);
                    if (SmsSender == null)
                    {
                        return HttpNotFound();
                    }
                    SmsSender.DecryptPassword();
                    ViewBag.SettingId = new SelectList(uow.SmssenderRepository.Get(x => x), "Id", "SettingName", SmsSender.SettingId);
                    List<SelectListItem> CompanyId = new List<SelectListItem>() { new SelectListItem() { Text = "نیک اس ام اس", Value = "1", Selected = (SmsSender.CompanyId == 1 ? true : false) }, new SelectListItem() { Text = "پیامک پنل", Value = "2", Selected = (SmsSender.CompanyId == 2 ? true : false) }, new SelectListItem() { Text = "کاوه نگار", Value = "3", Selected = (SmsSender.CompanyId == 3 ? true : false) } };
                    ViewBag.CompanyId = CompanyId;

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت سرویس های sms اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SmsSenders", "Edit", true, 200, " نمایش صفحه ویرایش  اس ام اس  اطلاع رسان   " + SmsSender.Username, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(SmsSender);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/SmsSenders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( SmsSender smsSender)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (smsSender.IsPublic)
                {
                    uow.SmssenderRepository.GetByReturnQueryable(x => x, x => x.Id != smsSender.Id).ForEach(x => x.IsPublic = false);
                    uow.Save();
                }

                if (ModelState.IsValid)
                {
                    smsSender.EncryptPassword();
                    uow.SmssenderRepository.Update(smsSender);
                    uow.Save();
                    UpdateAppSetting(smsSender.SettingId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "SmsSenders", "Edit", false, 200, "   ویرایش  اس ام اس  اطلاع رسان   " + smsSender.Username, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت سرویس های sms اطلاع رسان", null, "HelpModuleSectionFields").FirstOrDefault();
                ViewBag.SettingId = new SelectList(uow.SmssenderRepository.Get(x => x), "Id", "SettingName", smsSender.SettingId);
                List<SelectListItem> CompanyId = new List<SelectListItem>() { new SelectListItem() { Text = "نیک اس ام اس", Value = "1" }, new SelectListItem() { Text = "پیامک پنل", Value = "2" }, new SelectListItem() { Text = "کاوه نگار", Value = "3" } };

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Edit", false, 500, "   ویرایش  اس ام اس  اطلاع رسان   " + smsSender.Username, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(smsSender);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/SmsSenders/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    SmsSender SmsSender = uow.SmssenderRepository.GetByID(id);
                    if (SmsSender == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SmsSenders", "Delete", true, 200, " نمایش صفحه حذف  اس ام اس  اطلاع رسان   " + SmsSender.Username, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(SmsSender);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تنظیمات سایت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/SmsSenders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                SmsSender smsSender = uow.SmssenderRepository.GetByID(id);
                uow.SmssenderRepository.Delete(smsSender);
                uow.Save();
                UpdateAppSetting(smsSender.SettingId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "SmsSenders", "DeleteConfirmed", false, 200, "   حذف  اس ام اس  اطلاع رسان   " + smsSender.Username, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SmsSenders", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        private void UpdateAppSetting(int SettingId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var model = uow.SettingRepository.Get(x => x, x => x.Id == SettingId).FirstOrDefault();
            if (model.SmsSenders != null)
            {
                var ActiveSMSsender = model.SmsSenders.Where(x => x.IsActive == true && x.IsPublic == false).OrderBy(x => x.DisplaySort);
                var ActivePublicSMSsender = model.SmsSenders.Where(x => x.IsActive == true && x.IsPublic==true).OrderBy(x => x.DisplaySort);
                if (ActiveSMSsender.Any())
                {
                    Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    
                    myConfiguration.AppSettings.Settings["SMSUsername"].Value = ActiveSMSsender.First().Username;
                    myConfiguration.AppSettings.Settings["SMSPassword"].Value = ActiveSMSsender.First().DecryptedPassword;
                    if (ActiveSMSsender.First().DomainName != null)
                        myConfiguration.AppSettings.Settings["SMSDomainName"].Value = ActiveSMSsender.First().DomainName;
                    myConfiguration.AppSettings.Settings["SMSSenderNumber"].Value = ActiveSMSsender.First().SenderNumber;
                    myConfiguration.AppSettings.Settings["SMSCompany"].Value = ActiveSMSsender.First().CompanyId.ToString();

                    myConfiguration.AppSettings.Settings["SMSUsername2"].Value = ActivePublicSMSsender.First().Username;
                    myConfiguration.AppSettings.Settings["SMSPassword2"].Value = ActivePublicSMSsender.First().DecryptedPassword;
                    if (ActivePublicSMSsender.First().DomainName != null)
                        myConfiguration.AppSettings.Settings["SMSDomainName2"].Value = ActivePublicSMSsender.First().DomainName;
                    myConfiguration.AppSettings.Settings["SMSSenderNumber2"].Value = ActivePublicSMSsender.First().SenderNumber;
                    myConfiguration.AppSettings.Settings["SMSCompany2"].Value = ActivePublicSMSsender.First().CompanyId.ToString();

                    myConfiguration.Save();
                }
                else
                {
                    Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    myConfiguration.AppSettings.Settings["SMSUsername"].Value = "09128605712";
                    myConfiguration.AppSettings.Settings["SMSPassword"].Value = "123456";
                    myConfiguration.AppSettings.Settings["SMSDomainName"].Value = "";
                    myConfiguration.AppSettings.Settings["SMSSenderNumber"].Value = "3000";
                    myConfiguration.AppSettings.Settings["SMSCompany"].Value = "1";

                    myConfiguration.AppSettings.Settings["SMSUsername2"].Value = "09128605712";
                    myConfiguration.AppSettings.Settings["SMSPassword2"].Value = "123456";
                    myConfiguration.AppSettings.Settings["SMSDomainName2"].Value = "";
                    myConfiguration.AppSettings.Settings["SMSSenderNumber2"].Value = "3000";
                    myConfiguration.AppSettings.Settings["SMSCompany2"].Value = "1";

                    myConfiguration.Save();
                }
            }
            else
            {
                Configuration myConfiguration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                myConfiguration.AppSettings.Settings["SMSUsername"].Value = "09128605712";
                myConfiguration.AppSettings.Settings["SMSPassword"].Value = "123456";
                myConfiguration.AppSettings.Settings["SMSDomainName"].Value = "";
                myConfiguration.AppSettings.Settings["SMSSenderNumber"].Value = "3000";
                myConfiguration.AppSettings.Settings["SMSCompany"].Value = "1";

                myConfiguration.AppSettings.Settings["SMSUsername2"].Value = "09128605712";
                myConfiguration.AppSettings.Settings["SMSPassword2"].Value = "123456";
                myConfiguration.AppSettings.Settings["SMSDomainName2"].Value = "";
                myConfiguration.AppSettings.Settings["SMSSenderNumber2"].Value = "3000";
                myConfiguration.AppSettings.Settings["SMSCompany2"].Value = "1";

                myConfiguration.Save();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
