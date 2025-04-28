using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ahmadi.Infrastructure.Security;
using CoreLib.ViewModel.Xml;
using System.Data.Entity;
using System.Net;
using CoreLib.Infrastructure.ModelBinder;
using Microsoft.AspNet.Identity;
using UnitOfWork;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class WorkTimesController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow;

        public WorkTimesController()
        {
            uow = new UnitOfWorkClass();

        }
        public virtual ActionResult Index(int? id)
        {
            try
            {

                
                    var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.settings =new SelectList(uow.SettingRepository.Get(x => x),"Id", "SettingName");
                        var ShoppingWorkTimes = uow.ShoppingWorkTimeRepository.Get(x => x);
                        if (id.HasValue)
                            ShoppingWorkTimes = ShoppingWorkTimes.Where(x => x.SettingId == id.Value);

                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.AddPermission = p.First();
                        ViewBag.DeletePermission = p.Skip(2).First();


                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "WorkTimes", "Index", true, 200, "نمایش صفحه زمان های کاری فروشگاه ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ShoppingWorkTimes);


                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));
              
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "WorkTimes", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public virtual ActionResult Create(int id)
        {
            try
            {

               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                    {
                        var Setting = uow.SettingRepository.GetByID(id);
                        if (Setting != null)
                        {
                            ViewBag.Setting = Setting;
                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "WorkTimes", "Create", true, 200, "نمایش صفحه درج ساعت کاری برای فروشگاه" + Setting.SettingName, DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View();
                        }
                        else
                            return RedirectToAction("WorkTimes", new { id });
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "WorkTimes", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Create(ShoppingWorkTime ShoppingWorkTime)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                ViewBag.Setting = uow.SettingRepository.GetByID(ShoppingWorkTime.SettingId);
                if (ShoppingWorkTime.StartTime >= ShoppingWorkTime.EndTime || ShoppingWorkTime.EndTime <= ShoppingWorkTime.StartTime)
                {

                    ViewBag.Erorr = "زمان شروع و پایان درست وارد نشده است !";
                    return View(ShoppingWorkTime);
                }
                bool CompeleteDuplicate = false;
                if (uow.ShoppingWorkTimeRepository.Get(x => x, x => x.SettingId == ShoppingWorkTime.SettingId && x.WeekDay == ShoppingWorkTime.WeekDay && x.StartTime == ShoppingWorkTime.StartTime && x.EndTime == ShoppingWorkTime.EndTime).Any())
                    CompeleteDuplicate = true;

                if (CompeleteDuplicate == false && !uow.ShoppingWorkTimeRepository.Get(x => x, x => x.SettingId == ShoppingWorkTime.SettingId && x.WeekDay == ShoppingWorkTime.WeekDay && ((x.StartTime <= ShoppingWorkTime.StartTime && x.EndTime > ShoppingWorkTime.StartTime) || (x.StartTime < ShoppingWorkTime.EndTime && x.EndTime >= ShoppingWorkTime.EndTime))).Any())
                {

                    uow.ShoppingWorkTimeRepository.Insert(ShoppingWorkTime);
                    uow.Save();
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "WorkTimes", "Create", true, 200, "ایجاد ساعت کاری برای فروشگاه " + ShoppingWorkTime.SettingId, DateTime.Now, User.Identity.GetUserId());

                    return RedirectToAction("Index");
                }
                else
                {

                    ViewBag.Erorr = "این بازه زمانی  انتخاب شده است !";
                    return View(ShoppingWorkTime);
                }
            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "WorkTimes", "Create", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(ShoppingWorkTime);
            }
        }

        public virtual ActionResult Edit(int id)
        {
            try
            {

                #region Check License
                
                
                #endregion

               
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                    {
                        var ShoppingWorkTime = uow.ShoppingWorkTimeRepository.GetByID(id);
                        if (ShoppingWorkTime != null)
                        {
                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "WorkTimes", "Edit", true, 200, "نمایش صفحه ویرایش زمان کاری" + ShoppingWorkTime.Id, DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View(ShoppingWorkTime);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));
               
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "WorkTimes", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(ShoppingWorkTime ShoppingWorkTime)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                bool CompeleteDuplicate = false;
                if (uow.ShoppingWorkTimeRepository.Get(x => x, x => x.SettingId == ShoppingWorkTime.SettingId && x.WeekDay == ShoppingWorkTime.WeekDay && x.StartTime == ShoppingWorkTime.StartTime && x.EndTime == ShoppingWorkTime.EndTime && x.Id != ShoppingWorkTime.Id).Any())
                    CompeleteDuplicate = true;

                if (CompeleteDuplicate == false && !uow.ProductSendWayWorkTimeRepository.Get(x => x, x => x.ProductSendWayId == ShoppingWorkTime.SettingId && x.WeekDay == ShoppingWorkTime.WeekDay && ((x.StartTime <= ShoppingWorkTime.StartTime && x.EndTime > ShoppingWorkTime.StartTime) || (x.StartTime < ShoppingWorkTime.EndTime && x.EndTime >= ShoppingWorkTime.EndTime)) && x.Id != ShoppingWorkTime.Id).Any())
                {

                    uow.ShoppingWorkTimeRepository.Update(ShoppingWorkTime);
                    uow.Save();
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "WorkTimes", "Edit", true, 200, "ویرایش ساعت کاری برای برای " + ShoppingWorkTime.SettingId, DateTime.Now, User.Identity.GetUserId());

                    return RedirectToAction("Index");
                }
                else
                {

                    ViewBag.Erorr = "این بازه زمانی  برای این روش انتخاب شده است !";
                    return View(ShoppingWorkTime);
                }
            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "WorkTimes", "Edit", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(ShoppingWorkTime);
            }
        }



        public virtual ActionResult Delete(int id)
        {
            try
            {

                
                    if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                    {
                        var ShoppingWorkTime = uow.ShoppingWorkTimeRepository.GetByID(id);
                        if (ShoppingWorkTime != null)
                        {
                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "WorkTimes", "Delete", true, 200, "نمایش صفحه حذف زمان کاری ", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View(ShoppingWorkTime);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));
                
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "WorkTimes", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Delete(ShoppingWorkTime ShoppingWorkTime)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                uow.ShoppingWorkTimeRepository.Delete(ShoppingWorkTime);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "WorkTimes", "Delete", true, 200, "حذف زمان کاری ", DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("Index");

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "WorkTimes", "Delete", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " زمان کاری انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(ShoppingWorkTime);
            }
        }


    }
}