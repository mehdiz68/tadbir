using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using PagedList;
using CoreLib.Infrastructure.DateTime;
using CoreLib.ViewModel.Xml;
using ahmadi.Infrastructure.Helper;
using CoreLib.Infrastructure.ModelBinder;
using UnitOfWork;
using Microsoft.AspNet.Identity;
using ahmadi.ViewModels.Slider;
using ahmadi.ViewModels.Content;
using CoreLib.Infrastructure;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class LawController : Controller
    {
        private UnitOfWorkClass uow = null;
        public LawController()
        {
            uow = new UnitOfWorkClass();
        }

        // GET: Admin/Contents
        [CorrectArabianLetter(new string[] { "TitleString", "TitleFilter" })]
        public virtual ActionResult Index(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                var Law = uow.LawRepository.GetQueryList().AsNoTracking();
                if (id.HasValue)
                {
                    var law = uow.LawRepository.GetByID(id.Value);
                    if (law == null)
                        return RedirectToAction("Index");
                    else
                        ViewBag.law = law;

                    Law = Law.Include("ChildLaws.ChildLaws.ChildLaws.ChildLaws.ChildLaws.ChildLaws.ChildLaws").Include("ParentLaw").Where(x => x.ParrentId == id.Value);
                }
                else
                    Law = Law.Where(x => x.ParrentId == null);
                return View(Law.OrderBy(x => x.DisplaySort).ThenBy(x => x.Id));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Law", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }

        // GET: Admin/Contents/Create
        public virtual ActionResult Create(int? id)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 4, 1))
                {

                    ViewBag.ParrentId = id;
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Law", "Create", true, 200, "نمایش صفحه ایجاد قانون", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));


            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Law", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create(Law Law)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModelState.IsValid)
                {
                    if (Law.ParrentId.HasValue)
                        Law.ParentLaw = uow.LawRepository.GetByID(Law.ParrentId);
                    Law.Title = CommonFunctions.CorrectArabianLetter(Law.Title);
                    uow.LawRepository.Insert(Law);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Law", "Create", false, 200, " ایجاد قانون " + Law.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    if (Law.ParrentId.HasValue)
                        return RedirectToAction("Index", new { id = Law.ParrentId });
                    else
                        return RedirectToAction("Index");
                }
                ViewBag.ParrentId = Law.ParrentId;
                return View(Law);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Law", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                throw;
            }
        }


        // GET: Admin/Contents/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModulePermission.check(User.Identity.GetUserId(), 4, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Law Law = uow.LawRepository.GetByID(id.Value);

                    if (Law == null)
                    {
                        return HttpNotFound();
                    }


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Law", "Edit", true, 200, "نمایش صفحه ویرایش قانون " + Law.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Law);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Law", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Edit(Law Law)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModelState.IsValid)
                {

                    Law.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Law.Title);
                    uow.LawRepository.Update(Law);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Law", "Edit", false, 200, " ویرایش قانون " + Law.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    if (Law.ParrentId.HasValue)
                        return RedirectToAction("Index", new { id = Law.ParrentId });
                    else
                        return RedirectToAction("Index");


                }



                return View(Law);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Law", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                    Law Law = uow.LawRepository.GetByID(id);
                    if (Law == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Law", "Delete", true, 200, "نمایش صفحه حذف قانون " + Law.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Law);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Law", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
            Law law = uow.LawRepository.Get(x => x, x => x.Id == id, null, "ChildLaws").Single();
            int? ParrentId = law.ParrentId;
            try
            {
                if (law.ChildLaws.Any())
                {
                    ViewBag.Erorr = "این دسته بندی ، دارای" + law.ChildLaws.Count + " آیتم است. ابتدا باید آنها را پاک نمایید.";
                    return View(law);
                }
                
                uow.LawRepository.Delete(law);
                uow.Save();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Law", "DeleteConfirmed", false, 200, "حذف آیتم" + law.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                if (ParrentId.HasValue)
                    return RedirectToAction("Index", new { id = ParrentId });
                else
                    return RedirectToAction("Index");
            }
            catch (Exception s)
            {
                ViewBag.Erorr = "این قانون، دارای آیتم های دیگری است. ابتدا باید آنها را پاک نمایید.";
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Law", "DeleteConfirmed", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(law);
            }
        }

        protected override void Dispose(bool disposing)
        {

            base.Dispose(disposing);
        }
    }
}
