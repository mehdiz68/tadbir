using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
using System.Collections.Generic;
using UnitOfWork;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class FileTypesController : Controller
    {
        private UnitOfWorkClass uow = null;

        // GET: Admin/FileTypes
        public virtual ActionResult Index()
        {
            uow = new UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 1);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت پسوندهای مجاز", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "FileTypes", "Index", true, 200, " نمایش صفحه مدیریت پسوندهای مجاز", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.FiletypeRepository.Get(x => x, null, x => x.OrderByDescending(c => c.Id), "attachments").ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/FileTypes/Details/5
        public virtual ActionResult Details(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, null))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    FileType fileType = uow.FiletypeRepository.GetByID(id);
                    if (fileType == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "FileTypes", "Details", true, 200, " نمایش صفحه جزئیات پسوندِ " + fileType.FileTypeName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(fileType);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/FileTypes/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                {
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت پسوندهای مجاز", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "FileTypes", "Create", true, 200, " نمایش صفحه ایجاد پسوند فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/FileTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create([Bind(Include = "Id,FileTypeName")] FileType fileType)
        {
            uow = new UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    uow.FiletypeRepository.Insert(fileType);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "FileTypes", "Create", false, 200, "   ایجاد پسوندِ " + fileType.FileTypeName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت پسوندهای مجاز", null, "HelpModuleSectionFields").FirstOrDefault();
                return View(fileType);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.Error = x.Message;
                return View(fileType);
            }
        }

        // GET: Admin/FileTypes/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    FileType fileType = uow.FiletypeRepository.GetByID(id);
                    if (fileType == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت پسوندهای مجاز", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "FileTypes", "Edit", true, 200, " نمایش صفحه ویرایش پسوندِ " + fileType.FileTypeName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(fileType);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/FileTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit([Bind(Include = "Id,FileTypeName")] FileType fileType)
        {
            uow = new UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    uow.FiletypeRepository.Update(fileType);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "FileTypes", "Edit", false, 200, "   ویرایش پسوندِ" + fileType.FileTypeName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 1 && x.Name == "مدیریت پسوندهای مجاز", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "FileTypes", "Edit", false, 500, "   خطا در ویرایش پسوندِ" + fileType.FileTypeName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(fileType);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/FileTypes/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    FileType fileType = uow.FiletypeRepository.Get(x => x, x => x.Id == id, null, "attachments").SingleOrDefault();
                    if (fileType == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "FileTypes", "Delete", true, 200, " نمایش صفحه حذف پسوندِ" + fileType.FileTypeName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(fileType);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/FileTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                FileType fileType = uow.FiletypeRepository.GetByID(id);
                uow.FiletypeRepository.Delete(fileType);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "FileTypes", "DeleteConfirmed", false, 200, "   حذف پسوندِ" + fileType.FileTypeName, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "FileTypes", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
