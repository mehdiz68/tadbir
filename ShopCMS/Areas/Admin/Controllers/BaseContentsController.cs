using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain;
using ahmadi.Infrastructure.Security;
//using CoreLib.ViewModel.Entity;
using PagedList;
using CoreLib.Infrastructure.DateTime;
using CoreLib.ViewModel.Xml;
using ahmadi.Infrastructure.Helper;
using UnitOfWork;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class BaseContentsController : Controller
    {
        private UnitOfWorkClass uow = null;

        // GET: Admin/Contents
        public virtual ActionResult Index(int? page)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 4);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    #region search

                    var contents = uow.ContentRepository.Get(x => x, x => x.ContentTypeId == 0 || x.ContentTypeId == 9, null, "attachment,Category,User,attachment");


                    #endregion


                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "صفحات پایه ای سایت", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "BaseContents", "Index", true, 200, " نمایش صفحه مدیریت صفحات پایه ای سایت", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(contents.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));


            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Contents", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
