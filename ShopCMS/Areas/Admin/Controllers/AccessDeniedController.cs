using Microsoft.AspNet.Identity;
using System;
using System.Web.Mvc;

namespace ahmadi.Areas.Admin.Controllers
{
    public partial class AccessDeniedController : Controller
    {
        // GET: Admin/AccessDenied
        public virtual ActionResult Index(string MouleName)
        {
            try
            {
                ViewBag.MouleName = MouleName;
                
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "AccessDenied", "Index", true, 200, " نمایش صفحه عدم دسترسی به ماژولِ " + MouleName, DateTime.Now,User.Identity.GetUserId());
                #endregion

                return View();

            }
            catch (Exception x)
            {
                #region EventLogger 
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "AccessDenied", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion


                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}