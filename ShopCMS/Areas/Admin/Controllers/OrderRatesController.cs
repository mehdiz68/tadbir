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
using PagedList;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class OrderRatesController : Controller
    {
        private UnitOfWorkClass uow = null;

        // GET: Admin/OrderRates
        public virtual ActionResult Index(int? page)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 25);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    int pageNumber = (page ?? 1);
                    ViewBag.OrderRateItems = uow.OrderRateItemRepository.Get(x => x);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "OrderRates", "Index", true, 200, " نمایش صفحه نظرسنجی سفارشات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.OrderRepository.GetQueryList().Include("OrderRates").Include("OrderRates.orderRateItem").Include("User").AsNoTracking().Where(x=>x.OrderRates.Any()).OrderBy(x=>x.BankOrderId).ToPagedList(pageNumber, 20));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "سفارشات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "OrderRates", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/OrderRates/Details/5
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
