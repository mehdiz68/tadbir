using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain;
using PagedList;
using ahmadi.Infrastructure.Security;
using CoreLib.Infrastructure.DateTime;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class EventLogsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uof = null;

        // GET: Admin/EventLogs
        public virtual ActionResult Index(string sortOrder, string LogTypeFilter, string LogTypeString, string ControllerNameFilter, string ControllerNameString, string ActionNameFilter, string ActionNameString, string RequestTypeFilter, string RequestTypeString, string StatusCodeFilter, string StatusCodeString, string LogDateTimeStartFilter, string LogDateTimeStartString, string LogDateTimeEndFilter, string LogDateTimeEndString, string UserIdFilter, string UserIdString, int? page)
        {
            uof = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 13);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    List<SelectListItem> LogTypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "نمایش اطلاعات", Value = "1" }, new SelectListItem() { Text = "درج اطلاعات", Value = "2" }, new SelectListItem() { Text = "ویرایش اطلاعات", Value = "3" }, new SelectListItem() { Text = "حذف اطلاعات", Value = "4" }, new SelectListItem() { Text = "خطا", Value = "5" } };
                    ViewBag.LogTypeString = LogTypeSelectListItem;

                    List<SelectListItem> RequestTypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "Get", Value = "True" }, new SelectListItem() { Text = "Post", Value = "False" } };
                    ViewBag.RequestTypeString = RequestTypeSelectListItem;

                    #region search
                    if (string.IsNullOrEmpty(LogTypeString))
                        LogTypeString = LogTypeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ControllerNameString))
                        ControllerNameString = ControllerNameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ActionNameString))
                        ActionNameString = ActionNameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(RequestTypeString))
                        RequestTypeString = RequestTypeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StatusCodeString))
                        StatusCodeString = StatusCodeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LogDateTimeStartString))
                        LogDateTimeStartString = LogDateTimeStartFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LogDateTimeEndString))
                        LogDateTimeEndString = LogDateTimeEndFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(UserIdString))
                        UserIdString = UserIdFilter;
                    else
                        page = 1;

                    ViewBag.LogTypeFilter = LogTypeFilter;
                    ViewBag.ControllerNameFilter = ControllerNameFilter;
                    ViewBag.ActionNameFilter = ActionNameFilter;
                    ViewBag.RequestTypeFilter = RequestTypeFilter;
                    ViewBag.StatusCodeFilter = StatusCodeFilter;
                    ViewBag.LogDateTimeStartFilter = LogDateTimeStartFilter;
                    ViewBag.LogDateTimeEndString = LogDateTimeEndFilter;
                    ViewBag.UserIdFilter = UserIdFilter;

                    var eventLogs = uof.EventLogRepository.Get(x => x, null, null, "User");

                    if (!String.IsNullOrEmpty(LogTypeString))
                    {
                        int Lts = Convert.ToInt16(LogTypeString);
                        eventLogs = eventLogs.Where(s => s.LogType == Lts);
                    }
                    if (!String.IsNullOrEmpty(ControllerNameString))
                    {
                        eventLogs = eventLogs.Where(s => s.ControllerName.Contains(ControllerNameString));
                    }
                    if (!String.IsNullOrEmpty(ActionNameString))
                    {
                        eventLogs = eventLogs.Where(s => s.ActionName.Contains(ActionNameString));
                    }
                    if (!String.IsNullOrEmpty(RequestTypeString))
                    {
                        bool Rts = Convert.ToBoolean(RequestTypeString);
                        eventLogs = eventLogs.Where(s => s.RequestType == Rts);
                    }
                    if (!String.IsNullOrEmpty(StatusCodeString))
                    {
                        int Sc = Convert.ToInt32(StatusCodeString);
                        eventLogs = eventLogs.Where(s => s.StatusCode == Sc);
                    }
                    if (!String.IsNullOrEmpty(UserIdString))
                    {
                        eventLogs = eventLogs.Where(s => s.User.FirstName.Contains(UserIdString) || s.User.LastName.Contains(UserIdString));
                    }

                    DateTime dtInsertDateStart = DateTime.Now.Date, dtInsertDateEnd = DateTime.Now.Date;
                    if (!String.IsNullOrEmpty(LogDateTimeStartString))
                        dtInsertDateStart = DateTimeConverter.ChangeShamsiToMiladi(LogDateTimeStartString);
                    if (!String.IsNullOrEmpty(LogDateTimeEndString))
                        dtInsertDateEnd = DateTimeConverter.ChangeShamsiToMiladi(LogDateTimeEndString);


                    if (!String.IsNullOrEmpty(LogDateTimeStartString) && !String.IsNullOrEmpty(LogDateTimeEndString))
                        eventLogs = eventLogs.Where(s => s.LogDateTime >= dtInsertDateStart && s.LogDateTime <= dtInsertDateEnd);
                    else if (!String.IsNullOrEmpty(LogDateTimeStartString))
                        eventLogs = eventLogs.Where(s => s.LogDateTime >= dtInsertDateStart);
                    else if (!String.IsNullOrEmpty(LogDateTimeEndString))
                        eventLogs = eventLogs.Where(s => s.LogDateTime <= dtInsertDateEnd);

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "LogType":
                            eventLogs = eventLogs.OrderBy(s => s.LogType);
                            ViewBag.CurrentSort = "LogType";
                            break;
                        case "LogType_desc":
                            eventLogs = eventLogs.OrderByDescending(s => s.LogType);
                            ViewBag.CurrentSort = "LogType_desc";
                            break;
                        case "ControllerName":
                            eventLogs = eventLogs.OrderBy(s => s.ControllerName);
                            ViewBag.CurrentSort = "ControllerName";
                            break;
                        case "ControllerName_desc":
                            eventLogs = eventLogs.OrderByDescending(s => s.ControllerName);
                            ViewBag.CurrentSort = "ControllerName_desc";
                            break;
                        case "ActionName":
                            eventLogs = eventLogs.OrderBy(s => s.ActionName);
                            ViewBag.CurrentSort = "ActionName";
                            break;
                        case "ActionName_desc":
                            eventLogs = eventLogs.OrderByDescending(s => s.ActionName);
                            ViewBag.CurrentSort = "ActionName_desc";
                            break;
                        case "RequestType":
                            eventLogs = eventLogs.OrderBy(s => s.RequestType);
                            ViewBag.CurrentSort = "RequestType";
                            break;
                        case "RequestType_desc":
                            eventLogs = eventLogs.OrderByDescending(s => s.RequestType);
                            ViewBag.CurrentSort = "RequestType_desc";
                            break;
                        case "StatusCode":
                            eventLogs = eventLogs.OrderBy(s => s.StatusCode);
                            ViewBag.CurrentSort = "StatusCode";
                            break;
                        case "StatusCode_desc":
                            eventLogs = eventLogs.OrderByDescending(s => s.StatusCode);
                            ViewBag.CurrentSort = "StatusCode_desc";
                            break;
                        case "LogDateTime":
                            eventLogs = eventLogs.OrderBy(s => s.LogDateTime);
                            ViewBag.CurrentSort = "LogDateTime";
                            break;
                        case "LogDateTime_desc":
                            eventLogs = eventLogs.OrderByDescending(s => s.LogDateTime);
                            ViewBag.CurrentSort = "LogDateTime_desc";
                            break;
                        case "UserId":
                            eventLogs = eventLogs.OrderBy(s => s.User.FirstName + " " + s.User.LastName);
                            ViewBag.CurrentSort = "UserId";
                            break;
                        case "UserId_desc":
                            eventLogs = eventLogs.OrderByDescending(s => s.User.FirstName + " " + s.User.LastName);
                            ViewBag.CurrentSort = "UserId_desc";
                            break;
                        default:  // Name ascending 
                            eventLogs = eventLogs.OrderByDescending(x => x.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 50;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uof.HelpModuleRepository.Get(x => x, x => x.Name == "رویدادها", null, "HelpModuleSections").FirstOrDefault();

                    return View(eventLogs.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت رویدادها" }));

            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
