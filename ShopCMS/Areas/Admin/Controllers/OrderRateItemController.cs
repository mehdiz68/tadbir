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
    public partial class OrderRateItemController : Controller
    {
        private UnitOfWorkClass uow = null;
        public OrderRateItemController()
        {
            uow = new UnitOfWorkClass();
        }

        // GET: Admin/FileTypes
        public virtual ActionResult Index()
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 25);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "OrderRateItem", "Index", true, 200, " نمایش صفحه مدیریت ایتم های نظرسنجی سفارش", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.OrderRateItemRepository.Get(x => x, null, x => x.OrderByDescending(c => c.Title)).ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "سفارشات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRateItem", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/FileTypes/Create
        public virtual ActionResult Create()
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 25, 1))
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "OrderRateItem", "Create", true, 200, " نمایش صفحه ایجاد پسوند فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "سفارشات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRateItem", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/FileTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create( OrderRateItem OrderRateItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    uow.OrderRateItemRepository.Insert(OrderRateItem);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "OrderRateItem", "Create", false, 200, "   ایجاد آیتم نظرسجنی " + OrderRateItem.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                return View(OrderRateItem);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRateItem", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.Error = x.Message;
                return View(OrderRateItem);
            }
        }

        // GET: Admin/FileTypes/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 25, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    OrderRateItem OrderRateItem = uow.OrderRateItemRepository.GetByID(id);
                    if (OrderRateItem == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "OrderRateItem", "Edit", true, 200, " نمایش صفحه ویرایش آیتم نظرسجنی " + OrderRateItem.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(OrderRateItem);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "سفارشات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRateItem", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/FileTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(OrderRateItem OrderRateItem)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    uow.OrderRateItemRepository.Update(OrderRateItem);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "OrderRateItem", "Edit", false, 200, "   ویرایش آیتم نظرسجنی" + OrderRateItem.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "OrderRateItem", "Edit", false, 500, "   خطا در ویرایش آیتم نظرسجنی" + OrderRateItem.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(OrderRateItem);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRateItem", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/FileTypes/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 25, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    OrderRateItem OrderRateItem = uow.OrderRateItemRepository.Get(x => x, x => x.Id == id).SingleOrDefault();
                    if (OrderRateItem == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "OrderRateItem", "Delete", true, 200, " نمایش صفحه حذف آیتم نظرسجنی" + OrderRateItem.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(OrderRateItem);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "سفارشات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRateItem", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/FileTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            try
            {
                OrderRateItem OrderRateItem = uow.OrderRateItemRepository.GetByID(id);
                uow.OrderRateItemRepository.Delete(OrderRateItem);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "OrderRateItem", "DeleteConfirmed", false, 200, "   حذف آیتم نظرسجنی" + OrderRateItem.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRateItem", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
