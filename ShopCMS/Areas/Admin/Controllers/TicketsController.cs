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
using System.Data.SqlClient;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class TicketsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;
        public TicketsController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        // GET: Admin/ContactUs
        [CorrectArabianLetter(new string[] { "FullNameString", "FullNameFilter" })]
        public virtual ActionResult Index(string sortOrder, string FullNameString, string FullNameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, int? page)
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
                    if (string.IsNullOrEmpty(InsertDateStart))
                        InsertDateStart = InsertDateStartFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(InsertDateEnd))
                        InsertDateEnd = InsertDateEndFilter;
                    else
                        page = 1;

                    ViewBag.FullNameFilter = FullNameString;
                    ViewBag.InsertDateStartFilter = InsertDateStart;
                    ViewBag.InsertDateEndFilter = InsertDateEnd;

                    var contactUs = uow.TicketRepository.GetQueryList().Include("User").Include("TicketCategory").Include("ChildTickets").Where(x => x.ParrentId == null).AsNoTracking();

                    if (!String.IsNullOrEmpty(FullNameString))
                        contactUs = contactUs.Where(s => s.User.FirstName.Contains(FullNameString) || s.User.LastName.Contains(FullNameString));

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
                            contactUs = contactUs.OrderBy(s => s.User.FirstName).ThenBy(s => s.User.LastName);
                            ViewBag.CurrentSort = "FullName";
                            break;
                        case "FullName_desc":
                            contactUs = contactUs.OrderByDescending(s => s.User.FirstName).ThenByDescending(s => s.User.LastName);
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
                            contactUs = contactUs.OrderBy(x => x.IsVisit).ThenByDescending(x => x.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "گزارش تماس با ما", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tickets", "Index", true, 200, " نمایش صفحه تیکت ها", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(contactUs.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tickets", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                    Ticket contactUs = uow.TicketRepository.Get(x => x, x => x.Id == id,null, "User,TicketCategory,ChildTickets").FirstOrDefault();
                    if (contactUs == null)
                    {
                        return HttpNotFound();
                    }
                    contactUs.IsVisit = true;
                    uow.Save();


                    List<int> CatIds = uow.ContentRepository.SqlQuery("exec [GetTicketSubCats] @CatId", new SqlParameter("@CatId", id)).ToList();
                    int lastId = CatIds.FirstOrDefault();
                    Ticket lastTicket = uow.TicketRepository.Get(x => x, x => x.Id == lastId).FirstOrDefault();
                    lastTicket.IsVisit = true;
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tickets", "Details", true, 200, " نمایش پرسش از طرفِ " + contactUs.User.FirstName + " " + contactUs.User.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    ViewBag.lastId = CatIds.FirstOrDefault();

                    return View(contactUs);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tickets", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                    Ticket Ticket = uow.TicketRepository.Get(x => x, x => x.Id == id, null, "User").FirstOrDefault();
                    if (Ticket == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Tickets", "Delete", true, 200, " نمایش صفحه حذفِ پیام تیکت " + Ticket.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Ticket);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tickets", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
                Ticket Ticket = uow.TicketRepository.Get(x => x, x => x.Id == id, null, "ChildTickets").FirstOrDefault();
                List<int> CatIds = uow.ContentRepository.SqlQuery("exec [GetTicketSubCats] @CatId", new SqlParameter("@CatId", id)).ToList();
                foreach (var item in uow.TicketRepository.Get(x => x, x => CatIds.Contains(x.Id)))
                {
                    uow.TicketRepository.Delete(item);
                }
                uow.Save();

                uow.TicketRepository.Delete(Ticket);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Tickets", "DeleteConfirmed", false, 200, " حذف تیکت", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Tickets", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
