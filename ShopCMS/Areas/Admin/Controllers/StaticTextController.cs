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
    public partial class StaticTextController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/ContactUs
        [CorrectArabianLetter(new string[] { "FullNameString", "FullNameFilter" })]
        public virtual ActionResult Index(string LanguagenameString, string LanguagenameFilter)
        {
            uow = new UnitOfWork.UnitOfWorkClass();


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


                    ViewBag.LanguagenameFilter = LanguagenameString;

                    var contactUs = uow.StaticTextCategoryRepository.Get(x => x, null, null, "attachment");


                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        contactUs = contactUs.Where(s => s.LanguageId == langId);
                    }



                    #endregion


                    return View(contactUs.OrderBy(x => x.LanguageId).ToList());
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Admin/ContactUs/Details/5
        public virtual ActionResult Details(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 17);
                if (p.Where(x => x == true).Any())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var cat = uow.StaticTextCategoryRepository.Get(x => x, x => x.Id == id, null, "attachment").FirstOrDefault();
                    var StaticTextContents = uow.StaticTextContentRepository.Get(x => x, x => x.CatId == id.Value, null, "attachment");
                    if (cat == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.cat = cat;

                    return View(StaticTextContents);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محتوا" }));

            }
            catch (Exception x)
            {

                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ContactUs/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 17, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    StaticTextContent contactUs = uow.StaticTextContentRepository.Get(x => x, x => x.Id == id, null, "attachment").FirstOrDefault();
                    if (contactUs == null)
                    {
                        return HttpNotFound();
                    }


                    return View(contactUs);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت دسته بندی محتوا ها" }));

            }
            catch (Exception x)
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(StaticTextContent contactUs)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    uow.StaticTextContentRepository.Update(contactUs);
                    uow.Save();


                    return RedirectToAction("Details", new { id = contactUs.CatId });
                }
                return View(contactUs);
            }
            catch (Exception x)
            {

                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public virtual JsonResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                StaticTextCategory content = uow.StaticTextCategoryRepository.GetByID(id);
                if (content.IsActive)
                {
                    content.IsActive = false;
                    uow.StaticTextCategoryRepository.Update(content);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "StaticText", "DeleteConfirmed", false, 200, " حذف محتوای " + content.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        message = "غیر فعال شد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    content.IsActive = true;
                    uow.StaticTextCategoryRepository.Update(content);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "StaticText", "DeleteConfirmed", false, 200, " حذف محتوای " + content.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        message = " فعال شد",
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);
                }

             

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "StaticText", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    Message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
