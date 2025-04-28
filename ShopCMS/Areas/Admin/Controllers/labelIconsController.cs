using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using System.Collections.Generic;
using CoreLib.ViewModel.Xml;
using ahmadi.Infrastructure.Security;
using Microsoft.AspNet.Identity;
using Stimulsoft.Base.Context.Animation;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class labelIconsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        public labelIconsController()
        {

            uow = new UnitOfWork.UnitOfWorkClass();
        }
        // GET: Admin/Folders
        public virtual ActionResult Index(string LanguageId)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "labelIcons", "Index", true, 200, "نمایش صفحه مدیریت آیکن های ارسال مرسوله", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.LabelIconRepository.Get(x => x, null, null, "attachment"));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "labelIcons", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Folders/Create
        public virtual ActionResult Create()
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguageId = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    ViewBag.FolderID = new SelectList(uow.FolderRepository.Get(x => x), "Id", "FolderName");

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "labelIcons", "Create", true, 200, " نمایش صفحه ایجاد پوشه", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "labelIcons", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(labelIcon labelIcon)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                if (ModelState.IsValid)
                {
                    labelIcon.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(labelIcon.Title);
                    uow.LabelIconRepository.Insert(labelIcon);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "labelIcons", "Create", false, 200, "   ایجاد آیکن" + labelIcon.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }




                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "labelIcons", "Create", false, 500, "   خطا در ایجاد ایکن" + labelIcon.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(labelIcon);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "labelIcons", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Folders/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    labelIcon labelIcon = uow.LabelIconRepository.Get(x => x, x => x.Id == id.Value).FirstOrDefault();
                    if (labelIcon == null)
                    {
                        return HttpNotFound();
                    }
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "labelIcons", "Edit", true, 200, " نمایش صفحه ویرایش آیکن" + labelIcon.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(labelIcon);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "labelIcons", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(labelIcon labelIcon)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                if (ModelState.IsValid)
                {
                    labelIcon.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(labelIcon.Title);
                    uow.LabelIconRepository.Update(labelIcon);
                    uow.Save();
                    return RedirectToAction("Index");
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "labelIcons", "Edit", false, 200, "   ویرایش آیکن" + labelIcon.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(labelIcon);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "labelIcons", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Folders/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    labelIcon labelIcon = uow.LabelIconRepository.GetByID(id);
                    if (labelIcon == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "labelIcons", "Delete", true, 200, " نمایش صفحه حذف آیکن" + labelIcon.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(labelIcon);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "labelIcons", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            try
            {
                labelIcon labelIcon = uow.LabelIconRepository.Get(x => x, x => x.Id == id).FirstOrDefault();

                uow.LabelIconRepository.Delete(labelIcon);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "labelIcons", "DeleteConfirmed", false, 200, "   حذف آیکن" + labelIcon.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "labelIcons", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
