using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using CoreLib.Infrastructure.DateTime;
using PagedList;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class RequestFormController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        public RequestFormController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        // GET: Admin/ContactUs
        [CorrectArabianLetter(new string[] { "FullNameString", "FullNameFilter" })]
        public virtual ActionResult Index(string sortOrder, string FullNameString, string FullNameFilter, string LanguagenameString, string LanguagenameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, int? page)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 17);
                if (p.Where(x => x == true).Any())
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    #region search
                    if (string.IsNullOrEmpty(FullNameString))
                        FullNameString = FullNameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
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

                    ViewBag.FullNameFilter = FullNameString;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.InsertDateStartFilter = InsertDateStart;
                    ViewBag.InsertDateEndFilter = InsertDateEnd;

                    var contactUs = uow.FormRequestRepository.Get(x => x, null, null, "FormRequestCategory");

                    if (!String.IsNullOrEmpty(FullNameString))
                        contactUs = contactUs.Where(s => s.Family.Contains(FullNameString) || s.Name.Contains(FullNameString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        contactUs = contactUs.Where(s => s.FormRequestCategory.LanguageId == langId);
                    }

                    DateTime dtInsertDateStart = DateTime.Now.Date, dtInsertDateEnd = DateTime.Now.Date;
                    if (!String.IsNullOrEmpty(InsertDateStart))
                        dtInsertDateStart = DateTimeConverter.ChangeShamsiToMiladi(InsertDateStart);
                    if (!String.IsNullOrEmpty(InsertDateEnd))
                        dtInsertDateEnd = DateTimeConverter.ChangeShamsiToMiladi(InsertDateEnd);


                    if (!String.IsNullOrEmpty(InsertDateStart) && !String.IsNullOrEmpty(InsertDateEnd))
                        contactUs = contactUs.Where(s => s.InsertDate >= dtInsertDateStart && s.InsertDate <= dtInsertDateEnd);
                    else if (!String.IsNullOrEmpty(InsertDateStart))
                        contactUs = contactUs.Where(s => s.InsertDate >= dtInsertDateStart);
                    else if (!String.IsNullOrEmpty(InsertDateEnd))
                        contactUs = contactUs.Where(s => s.InsertDate <= dtInsertDateEnd);

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "FullName":
                            contactUs = contactUs.OrderBy(s => s.Name).ThenBy(s => s.Family);
                            ViewBag.CurrentSort = "FullName";
                            break;
                        case "FullName_desc":
                            contactUs = contactUs.OrderByDescending(s => s.Name).ThenBy(s => s.Family);
                            ViewBag.CurrentSort = "FullName_desc";
                            break;
                        case "IsVisit":
                            contactUs = contactUs.OrderBy(s => s.IsVisit);
                            ViewBag.CurrentSort = "IsVisit";
                            break;
                        case "IsVisit_desc":
                            contactUs = contactUs.OrderByDescending(s => s.IsVisit);
                            ViewBag.CurrentSort = "IsVisit_desc";
                            break;
                        case "InsertDate":
                            contactUs = contactUs.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            contactUs = contactUs.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        default:  // Name ascending 
                            contactUs = contactUs.OrderBy(x => x.IsVisit).ThenBy(x => x.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "گزارش تماس با ما", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ContactUs", "Index", true, 200, " نمایش صفحه گزارش های تماس با ما", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(contactUs.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ContactUs/Details/5
        public virtual ActionResult Details(int? id)
        {

            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 17);
                if (p.Where(x => x == true).Any())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    FormRequest contactUs = uow.FormRequestRepository.Get(x => x, x => x.Id == id, null, "FormRequestCategory").FirstOrDefault();
                    if (contactUs == null)
                    {
                        return HttpNotFound();
                    }
                    contactUs.IsVisit = true;
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ContactUs", "Details", true, 200, " نمایش پیام تماس با ما از طرفِ " + contactUs.Family, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(contactUs);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ContactUs/Edit/5
        public virtual ActionResult Edit(int? id)
        {

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 17, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    FormRequest contactUs = uow.FormRequestRepository.GetByID(id);
                    if (contactUs == null)
                    {
                        return HttpNotFound();
                    }
                    contactUs.IsVisit = true;
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ContactUs", "Edit", true, 200, " نمایشِ صفحه ویرایشِ پیام تماس با ما از طرفِ " + contactUs.Family, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(contactUs);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ContactUs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(FormRequest contactUs)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    contactUs.Family = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(contactUs.Family);
                    contactUs.Name = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(contactUs.Name);
                    uow.FormRequestRepository.Update(contactUs);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ContactUs", "Edit", false, 200, " ویرایش پیام تماس با ما از طرفِ  " + contactUs.Family, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                return View(contactUs);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ContactUs/Delete/5
        public virtual ActionResult Delete(int? id)
        {

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 17, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    FormRequest FormRequest = uow.FormRequestRepository.GetByID(id);
                    if (FormRequest == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ContactUs", "Delete", true, 200, " نمایش صفحه حذفِ پیام تماس با ما از طرفِ " + FormRequest.Family, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(FormRequest);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/ContactUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {

            try
            {
                FormRequest contactUs = uow.FormRequestRepository.GetByID(id);
                uow.FormRequestRepository.Delete(contactUs);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ContactUs", "DeleteConfirmed", false, 200, " حذف پیام تماس با ما از طرفِ  " + contactUs.Family, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public virtual async Task<JsonResult> SendSms(string[] Tele, string Message)
        {
            try
            {

                SmsService ss = new SmsService();
                await ss.SendMultiDestinationAsync(Message, Tele.ToList());
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ContactUs", "SmsSend", false, 200, "ارسال پیامک " + Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    success = 1
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "SmsSend", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    success = 3
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public virtual async Task<JsonResult> SendEmail(string BodyContent, string Email, string Subject)
        {
            try
            {

                EmailService es = new EmailService();
                await es.SendAsync(new IdentityMessage() { Body = BodyContent, Destination = Email, Subject = Subject });
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "ContactUs", "SendEmail", false, 200, " ارسال ایمیل پاسخ تماس با ما " + BodyContent, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    success = 1
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ContactUs", "SendEmail", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    success = 3
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
