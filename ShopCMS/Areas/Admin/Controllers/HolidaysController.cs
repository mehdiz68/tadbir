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

namespace ahmadi.Areas.Admin.Controllers
{
   [Authorize(Roles = "Admin,Security,SuperUser")]
    public partial class HolidaysController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        // GET: Admin/Folders
        public virtual ActionResult Index(string LanguageId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
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

                        var Holiday = uow.HolidayRepository.Get(f=>f);

                        

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Holiday", "Index", true, 200, " نمایش صفحه مدیریت روزهای تعطیل", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Holiday.ToList());
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Holiday", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Folders/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                
               
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 1))
                    {
                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Holiday", "Create", true, 200, " نمایش صفحه ایجاد روز تعطیل", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Holiday", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create( Holiday Holiday, string InsertDate)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                Holiday.InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(InsertDate);
                if (ModelState.IsValid)
                    {
                        uow.HolidayRepository.Insert(Holiday);
                        uow.Save();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "Holiday", "Create", false, 200, "   ایجاد روز تعطیل" + Holiday.InsertDate, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index");
                    }
               

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "Holiday", "Create", false, 500, " خطا در ایجاد روز تعطیل " + Holiday.InsertDate, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(Holiday);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Holiday", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Folders/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
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
                        Holiday Holiday = uow.HolidayRepository.GetByID(id);
                        if (Holiday == null)
                        {
                            return HttpNotFound();
                        }

                        ViewBag.InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(Holiday.InsertDate);


                        XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Holiday", "Edit", true, 200, " نمایش صفحه ویرایش روز تعطیل" + Holiday.Id, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Holiday);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Holiday", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(Holiday Holiday, string InsertDate)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                Holiday.InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(InsertDate);
                if (ModelState.IsValid)
                    {
                        uow.HolidayRepository.Update(Holiday);
                        uow.Save();
                        return RedirectToAction("Index");
                    }
                
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "Holiday", "Edit", false, 200, "   ویرایش روز تعطیل" + Holiday.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(Holiday);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Holiday", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Folders/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                
                
                    if (ModulePermission.check(User.Identity.GetUserId(), 1, 3))
                    {
                        if (id == null)
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                        Holiday Holiday = uow.HolidayRepository.GetByID(id);
                        if (Holiday == null)
                        {
                            return HttpNotFound();
                        }

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "Holiday", "Delete", true, 200, " نمایش صفحه حذفِ روز تعطیل" + Holiday.Id, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Holiday);
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "فایل ها" }));
                
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Holiday", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                Holiday Holiday = uow.HolidayRepository.Get(x=>x,x=>x.Id==id).FirstOrDefault();
               
                    uow.HolidayRepository.Delete(Holiday);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "Holiday", "DeleteConfirmed", false, 200, "   حذف روز تعطیل" + Holiday.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Holiday", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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
