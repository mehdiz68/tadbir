using CoreLib.ViewModel.Xml;
using Domain;
using ahmadi.Infrastructure.Security;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using UnitOfWork;
using PagedList;
using Domain.ViewModels;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class SendWaysController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;
        // GET: Admin/SendWays

        public ActionResult Index()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {

                    var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                    ViewBag.setting = setting;

                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.AddPermission = p.First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    var ProductSendWay = uow.ProductSendWayRepository.Get(x => x, x => x.IsActive, null, "Image,Tax").ToList();

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "روش های ارسال", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "Index", true, 200, " نمایش صفحه مدیریت روشهای ارسال ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductSendWay);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #region SendWay

        public ActionResult Create()
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    ViewBag.TaxList = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title");
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 24 && x.Name == "ایجاد", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "Create", true, 200, "نمایش صفحه ایجاد روشهای ارسال", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Domain.ProductSendWay SendWay)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModelState.IsValid)
                {
                    SendWay.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(SendWay.Title);

                    uow.ProductSendWayRepository.Insert(SendWay);
                    uow.Save();


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "SendWays", "Create", false, 200, " ایجاد روش ارسال " + SendWay.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");

                }

                ViewBag.TaxList = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title", SendWay.TaxId.HasValue ? SendWay.TaxId.Value : 0);
                return View();

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        public ActionResult Edit(int id)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var Sendway = uow.ProductSendWayRepository.Get(x => x, x => x.Id == id, null, "Image").FirstOrDefault();
                    if (Sendway != null)
                    {
                        ViewBag.TaxList = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title", Sendway.TaxId.HasValue ? Sendway.TaxId.Value : 0);
                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 24 && x.Name == "ایجاد", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "Create", true, 200, "نمایش صفحه ایجاد روشهای ارسال", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Sendway);
                    }
                    else
                        return RedirectToAction("Index", "SendWays");
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Domain.ProductSendWay SendWay)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModelState.IsValid)
                {
                    SendWay.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(SendWay.Title);

                    uow.ProductSendWayRepository.Update(SendWay);
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "SendWays", "Edit", false, 200, " ویرایش روش ارسال " + SendWay.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");

                }

                ViewBag.TaxList = new SelectList(uow.TaxRepository.Get(x => x), "Id", "Title", SendWay.TaxId.HasValue ? SendWay.TaxId.Value : 0);
                return View();

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        public ActionResult Delete(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var sendWayBox = uow.ProductSendWayRepository.GetByID(id);
                    if (sendWayBox != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "Delete", true, 200, "نمایش صفحه حذف روش ارسال", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(sendWayBox);
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
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ProductSendWay ProductSendWay)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                uow.ProductSendWayRepository.Delete(ProductSendWay);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "Delete", true, 200, "حذف روش ارسال" + ProductSendWay.Title, DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("Index");

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Delete", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " روش ارسال انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(ProductSendWay);
            }
        }

        #endregion

        #region SendWayWorkTimes
        public ActionResult ProductSendWayWorkTimes(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {


                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    var productSendWay = uow.ProductSendWayRepository.Get(x => x, x => x.Id == id, null, "ProductSendWayWorkTimes", 0, 0, true).FirstOrDefault();
                    if (productSendWay != null)
                    {

                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.AddPermission = p.First();
                        ViewBag.DeletePermission = p.Skip(2).First();


                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "ProductSendWayWorkTimes", true, 200, "نمایش صفحه زمان های کاری روش ارسالِ " + productSendWay.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(productSendWay);
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
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "ProductSendWayWorkTimes", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult CreateProductSendWayWorkTime(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var ProductSendWay = uow.ProductSendWayRepository.GetByID(id);
                    if (ProductSendWay != null)
                    {
                        ViewBag.SendBoxes = new SelectList(uow.SendwayBoxRepository.Get(x => x), "Id", "Title");
                        ViewBag.ProductSendWay = ProductSendWay;
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "CreateSendWayWorkTime", true, 200, "نمایش صفحه درج ساع کاری برای روش ارسال " + ProductSendWay.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("ProductSendWayBoxes", new { id });
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreateSendWayWorkTime", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductSendWayWorkTime(ProductSendWayWorkTime ProductSendWayWorkTime)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                ViewBag.ProductSendWay = uow.ProductSendWayRepository.GetByID(ProductSendWayWorkTime.ProductSendWayId);
                if (ProductSendWayWorkTime.StartTime >= ProductSendWayWorkTime.EndTime || ProductSendWayWorkTime.EndTime <= ProductSendWayWorkTime.StartTime)
                {

                    ViewBag.Erorr = "زمان شروع و پایان درست وارد نشده است !";
                    return View(ProductSendWayWorkTime);
                }
                bool CompeleteDuplicate = false;
                if (uow.ProductSendWayWorkTimeRepository.Get(x => x, x => x.ProductSendWayId == ProductSendWayWorkTime.ProductSendWayId && x.WeekDay == ProductSendWayWorkTime.WeekDay && x.StartTime == ProductSendWayWorkTime.StartTime && x.EndTime == ProductSendWayWorkTime.EndTime).Any())
                    CompeleteDuplicate = true;

                if (CompeleteDuplicate == false && !uow.ProductSendWayWorkTimeRepository.Get(x => x, x => x.ProductSendWayId == ProductSendWayWorkTime.ProductSendWayId && x.WeekDay == ProductSendWayWorkTime.WeekDay && ((x.StartTime <= ProductSendWayWorkTime.StartTime && x.EndTime > ProductSendWayWorkTime.StartTime) || (x.StartTime < ProductSendWayWorkTime.EndTime && x.EndTime >= ProductSendWayWorkTime.EndTime))).Any())
                {

                    uow.ProductSendWayWorkTimeRepository.Insert(ProductSendWayWorkTime);
                    uow.Save();
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "CreateSendWayWorkTime", true, 200, "ایجاد ساعت کاری برای برای " + ProductSendWayWorkTime.ProductSendWayId, DateTime.Now, User.Identity.GetUserId());

                    return RedirectToAction("ProductSendWayWorkTimes", new { id = ProductSendWayWorkTime.ProductSendWayId });
                }
                else
                {

                    ViewBag.Erorr = "این بازه زمانی  برای این روش انتخاب شده است !";
                    return View(ProductSendWayWorkTime);
                }
            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreateSendWayWorkTime", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(ProductSendWayWorkTime);
            }
        }

        public ActionResult EditProductSendWayWorkTime(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var ProductSendWayWorkTime = uow.ProductSendWayWorkTimeRepository.GetByID(id);
                    if (ProductSendWayWorkTime != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditProductSendWayWorkTime", true, 200, "نمایش صفحه ویرایش زمان ارسا لانتخابیِ" + ProductSendWayWorkTime.Id, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductSendWayWorkTime);
                    }
                    else
                    {
                        return RedirectToAction("EditProductSendWayWorkTime", new { id });
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditProductSendWayWorkTime", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductSendWayWorkTime(ProductSendWayWorkTime ProductSendWayWorkTime)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                ViewBag.ProductSendWay = uow.ProductSendWayRepository.GetByID(ProductSendWayWorkTime.ProductSendWayId);
                bool CompeleteDuplicate = false;
                if (uow.ProductSendWayWorkTimeRepository.Get(x => x, x => x.ProductSendWayId == ProductSendWayWorkTime.ProductSendWayId && x.WeekDay == ProductSendWayWorkTime.WeekDay && x.StartTime == ProductSendWayWorkTime.StartTime && x.EndTime == ProductSendWayWorkTime.EndTime && x.Id != ProductSendWayWorkTime.Id).Any())
                    CompeleteDuplicate = true;

                if (CompeleteDuplicate == false && !uow.ProductSendWayWorkTimeRepository.Get(x => x, x => x.ProductSendWayId == ProductSendWayWorkTime.ProductSendWayId && x.WeekDay == ProductSendWayWorkTime.WeekDay && ((x.StartTime <= ProductSendWayWorkTime.StartTime && x.EndTime > ProductSendWayWorkTime.StartTime) || (x.StartTime < ProductSendWayWorkTime.EndTime && x.EndTime >= ProductSendWayWorkTime.EndTime)) && x.Id != ProductSendWayWorkTime.Id).Any())
                {

                    uow.ProductSendWayWorkTimeRepository.Update(ProductSendWayWorkTime);
                    uow.Save();
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditProductSendWayWorkTime", true, 200, "ویرایش ساعت کاری برای برای " + ProductSendWayWorkTime.ProductSendWayId, DateTime.Now, User.Identity.GetUserId());

                    return RedirectToAction("ProductSendWayWorkTimes", new { id = ProductSendWayWorkTime.ProductSendWayId });
                }
                else
                {

                    ViewBag.Erorr = "این بازه زمانی  برای این روش انتخاب شده است !";
                    return View(ProductSendWayWorkTime);
                }
            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditProductSendWayWorkTime", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(ProductSendWayWorkTime);
            }
        }



        public ActionResult DeleteProductSendWayWorkTime(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var ProductSendWayWorkTime = uow.ProductSendWayWorkTimeRepository.GetByID(id);
                    if (ProductSendWayWorkTime != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "DeleteProductSendWayWorkTime", true, 200, "نمایش صفحه حذف زمان کاری ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(ProductSendWayWorkTime);
                    }
                    else
                    {
                        return RedirectToAction("ProductSendWayWorkTimes", new { id = ProductSendWayWorkTime.ProductSendWayId });
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "DeleteProductSendWayWorkTime", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductSendWayWorkTime(ProductSendWayWorkTime ProductSendWayWorkTime)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                int id = ProductSendWayWorkTime.ProductSendWayId;
                uow.ProductSendWayWorkTimeRepository.Delete(ProductSendWayWorkTime);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "DeleteProductSendWayWorkTime", true, 200, "حذف زمان کاری " + ProductSendWayWorkTime.Id, DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("ProductSendWayWorkTimes", new { id = id });

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "DeleteProductSendWayWorkTime", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " زمان کاری انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(ProductSendWayWorkTime);
            }
        }

        #endregion

        #region SendWayBoxes
        public ActionResult SendWayBoxes()
        {

            uow = new UnitOfWork.UnitOfWorkClass();


            var p = ModulePermission.check(User.Identity.GetUserId(), 16);
            if (p.Where(x => x == true).Any())
            {

                ViewBag.EditPermission = p.Skip(1).First();
                ViewBag.AddPermission = p.First();
                ViewBag.DeletePermission = p.Skip(2).First();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "SendWayBoxes", true, 200, " نمایش صفحه مدیریت باکس های ارسال", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(uow.SendwayBoxRepository.GetByReturnQueryable(x => x));
            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای ارسال" }));




        }


        public ActionResult CreateSendWayBox()
        {
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "CreateSendWayBoxe", true, 200, "نمایش صفحه ایجاد باکس ارسال", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreateSendWayBoxe", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSendWayBox(SendwayBox SendwayBox)
        {

            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                uow.SendwayBoxRepository.Insert(SendwayBox);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "CreateSendWayBox", true, 200, "ایجاد باکس ارسال", DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("SendWayBoxes");

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreateSendWayBox", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(SendwayBox);
            }
        }



        public ActionResult EditSendWayBox(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var sendWayBox = uow.SendwayBoxRepository.GetByID(id);
                    if (sendWayBox != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditSendWayBox", true, 200, "نمایش صفحه ویرایش باکس ارسال", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(sendWayBox);
                    }
                    else
                    {
                        return RedirectToAction("SendWayBoxes");
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditSendWayBox", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSendWayBox(SendwayBox SendwayBox)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                uow.SendwayBoxRepository.Update(SendwayBox);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditSendWayBox", true, 200, "ویرایش باکس ارسال" + SendwayBox.Title, DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("SendWayBoxes");

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditSendWayBox", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(SendwayBox);
            }
        }


        public ActionResult DeleteSendWayBox(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var sendWayBox = uow.SendwayBoxRepository.GetByID(id);
                    if (sendWayBox != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "DeleteSendWayBox", true, 200, "نمایش صفحه حذف باکس ارسال", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(sendWayBox);
                    }
                    else
                    {
                        return RedirectToAction("SendWayBoxes");
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "DeleteSendWayBox", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSendWayBox(SendwayBox SendwayBox)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                uow.SendwayBoxRepository.Delete(SendwayBox);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditSendWayBox", true, 200, "حذف باکس ارسال" + SendwayBox.Title, DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("SendWayBoxes");

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditSendWayBox", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " باکس انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(SendwayBox);
            }
        }

        #endregion

        #region State
        public ActionResult States()
        {

            uow = new UnitOfWork.UnitOfWorkClass();


            var p = ModulePermission.check(User.Identity.GetUserId(), 16);
            if (p.Where(x => x == true).Any())
            {

                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");
                ViewBag.CityList = new SelectList(uow.CityRepository.GetByReturnQueryable(x => x), "Id", "Name");

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "States", true, 200, " نمایش صفحه مدیریت موقعیت جفرافیایی", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(setting);
            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای ارسال" }));


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult States(int ProvinceId, int CityId)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                setting.ProvinceId = ProvinceId;
                setting.CityId = CityId;
                uow.SettingRepository.Update(setting);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "States", true, 200, "تغییر موقعیت جغرافیایی فروشگاه", DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("Index");

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "States", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");
                ViewBag.CityList = new SelectList(uow.CityRepository.GetByReturnQueryable(x => x), "Id", "Name");
                ViewBag.error = s.Message;
                return View(setting);
            }
        }

        public JsonResult GetCities(int ProvinceId)
        {
            uow = new UnitOfWorkClass();

            var jsonResult = Json(new
            {
                data = uow.CityRepository.GetByReturnQueryable(x => x, x => x.ProvinceId == ProvinceId, x => x.OrderBy(s => s.Name), "", 0, 0).Select(x => new { x.Id, x.Name }).ToList(),
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult InnerState()
        {

            uow = new UnitOfWork.UnitOfWorkClass();


            var p = ModulePermission.check(User.Identity.GetUserId(), 16);
            if (p.Where(x => x == true).Any())
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "InnerState", true, 200, "مدیریت استان های همجوار", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(uow.SettingRepository.Get(x => x));
            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای ارسال" }));


        }

        public ActionResult InnerStateList(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();


            var p = ModulePermission.check(User.Identity.GetUserId(), 16);
            if (p.Where(x => x == true).Any())
            {
                ViewBag.id = id;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "InnerState", true, 200, "مدیریت استان های همجوار", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(uow.SettingStateRepository.Get(x => x, x => x.SettingId == id, x => x.OrderByDescending(s => s.Id), "Province,Setting"));
            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای ارسال" }));


        }

        public ActionResult CreateInnerState(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            ViewBag.Id = id;
            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateInnerState(SettingState settingState)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name", settingState.ProvinceId);
                if (uow.SettingStateRepository.Get(x => x.Id, x => x.SettingId == settingState.SettingId && x.ProvinceId == settingState.ProvinceId).Any())
                {
                    ViewBag.Error = " این استان قبلا انتخاب شده است ";
                    return View(settingState);
                }
                uow.SettingStateRepository.Insert(settingState);
                uow.Save();
                return RedirectToAction("InnerStateList", new { id = settingState.SettingId });

            }
            catch (Exception ex)
            {
                ViewBag.Error = " خطایی رخ داد. " + ex.Message;
                return View(settingState);
            }
        }
        public ActionResult InnerStateRemove(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            ViewBag.Id = id;
            var settingstate = uow.SettingStateRepository.Get(x => x, x => x.Id == id, null, "Province").FirstOrDefault();
            ViewBag.Province = settingstate.Province;
            return View(settingstate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult InnerStateRemove(SettingState SettingState)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                int id = SettingState.SettingId;
                uow.SettingStateRepository.Delete(SettingState);
                uow.Save();
                return RedirectToAction("InnerStateList", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.Error = " خطایی رخ داد. " + ex.Message;
                return View(SettingState);
            }
        }

        #endregion

        #region ProductSendWayBoxes

        public ActionResult ProductSendWayBoxes(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    var productSendWay = uow.ProductSendWayRepository.Get(x => x, x => x.Id == id, null, "ProductSendWayBoxes.SendwayBox", 0, 0, true).FirstOrDefault();
                    if (productSendWay != null)
                    {

                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.AddPermission = p.First();
                        ViewBag.DeletePermission = p.Skip(2).First();


                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "ProductSendWayBoxes", true, 200, "نمایش صفحه باکس های ارسالی" + productSendWay.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(productSendWay);
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
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreateSendWayBoxe", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }



        public ActionResult IrPostDetail(int id, short? status)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    var productSendWay = uow.ProductSendWayRepository.Get(x => x, x => x.Id == id, null, "ProductSendwayIrPostDetail", 0, 0, true).FirstOrDefault();

                    if (productSendWay == null)
                        return Redirect("~/Admin/SendWays");
                    else if (productSendWay.FreeOff)
                        return Redirect("~/Admin/SendWays");
                    else if (!productSendWay.IsIrPost)
                        return Redirect("~/Admin/SendWays/ShippingSetting/" + id);

                    if (productSendWay != null)
                    {

                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.AddPermission = p.First();
                        ViewBag.DeletePermission = p.Skip(2).First();

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "IrPostDetail", true, 200, "نمایش صفحه تنظیمات حمل و نقل" + productSendWay.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        ViewBag.title = productSendWay.Title;
                        ViewBag.Id = productSendWay.Id;
                        ViewBag.status = status;
                        return View(productSendWay.ProductSendwayIrPostDetail);
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
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "IrPostDetail", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult IrPostDetail(Domain.ProductSendwayIrPostDetail ProductSendwayIrPostDetail)
        {
            uow = new UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    if (uow.ProductSendwayIrPostDetailRepository.Any(x => x.ProductSendWayId == ProductSendwayIrPostDetail.ProductSendWayId))
                    {
                        uow.ProductSendwayIrPostDetailRepository.Update(ProductSendwayIrPostDetail);
                        uow.Save();
                    }
                    else
                    {
                        uow.ProductSendwayIrPostDetailRepository.Insert(ProductSendwayIrPostDetail);
                        uow.Save();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "SendWays", "IrPostDetail", false, 200, " ویرایش تنظیمات حمل و نقل" + ProductSendwayIrPostDetail.ProductSendWayId, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("IrPostDetail", new { id = ProductSendwayIrPostDetail.ProductSendWayId, status = 1 });

                }

                return RedirectToAction("IrPostDetail", new { id = ProductSendwayIrPostDetail.ProductSendWayId, status = 0 });

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "IrPostDetail", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult ShippingSetting(int id, int? ProvinceId, int? CityId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {

                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name");
                    var sendway = uow.ProductSendWayRepository.GetByID(id);
                    if (sendway == null)
                        return Redirect("~/Admin/SendWays");
                    else if (sendway.FreeOff)
                        return Redirect("~/Admin/SendWays");
                    else if(sendway.IsIrPost)
                        return Redirect("~/Admin/SendWays/IrPostDetail/"+id);

                    int? ProvId = uow.SettingRepository.Get(x => x.ProvinceId, x => x.LanguageId == 1).Single();
                    if (ProvinceId.HasValue)
                        ProvId = ProvinceId.Value;
                    if (!ProvId.HasValue)
                        ProvId = 8;
                    var SendwayCosts = uow.ProductSendWayDetailRepository.GetByReturnQueryable(x => new ShippingSetting
                    {
                        Id = x.Id,
                        SendWayBoxID = x.ProductSendWayBoxId,
                        sendwayId = x.ProductSendWayBox.ProductSendWayId,
                        sendwayName = x.ProductSendWayBox.ProductSendWay.Title,
                        cityId = x.CityId,
                        cityName = x.City.Name,
                        provienceName = x.City.Province.Name,
                        provienceId = x.City.ProvinceId,
                        boxId = x.ProductSendWayBox.SendWayBoxID,
                        boxName = x.ProductSendWayBox.SendwayBox.Title,
                        Cost = x.Value,
                        isActive = x.IsActive,
                        limitation = x.Limitation
                    }, x => x.ProductSendWayBox.ProductSendWayId == id && x.City.ProvinceId == ProvId, x => x.OrderBy(s => s.City.Province.Name).ThenBy(s => s.City.Name), "ProductSendWayBox.SendwayBox,ProductSendWayBox.ProductSendWay,City.Province,");

                    if (SendwayCosts != null)
                    {
                        ViewBag.SendwayName = uow.ProductSendWayRepository.Get(x => x.Title, x => x.Id == id).Single();
                        ViewBag.SendwayId = id;
                        ViewBag.EditPermission = p.Skip(1).First();
                        ViewBag.AddPermission = p.First();
                        ViewBag.DeletePermission = p.Skip(2).First();


                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "ShippingSetting", true, 200, "نمایش صفحه تنظیمات حمل و نقل " + id, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(SendwayCosts);
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
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "ShippingSetting", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        public ActionResult CreateProductSendWayBox(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var ProductSendWay = uow.ProductSendWayRepository.GetByID(id);
                    if (ProductSendWay != null)
                    {
                        ViewBag.SendBoxes = new SelectList(uow.SendwayBoxRepository.Get(x => x), "Id", "Title");
                        ViewBag.ProductSendWay = ProductSendWay;
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "CreateProductSendWayBox", true, 200, "نمایش صفحه انتخاب باکس ارسال برای روش ارسال " + ProductSendWay.Title, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View();
                    }
                    else
                        return RedirectToAction("ProductSendWayBoxes", new { id });
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreateProductSendWayBox", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProductSendWayBox(ProductSendWayBox ProductSendWayBox)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                ViewBag.ProductSendWay = uow.ProductSendWayRepository.GetByID(ProductSendWayBox.ProductSendWayId);
                ViewBag.SendBoxes = new SelectList(uow.SendwayBoxRepository.Get(x => x), "Id", "Title");

                if (!uow.ProductSendWayBoxRepository.Get(x => x, x => x.ProductSendWayId == ProductSendWayBox.ProductSendWayId && x.SendWayBoxID == ProductSendWayBox.SendWayBoxID).Any())
                {

                    uow.ProductSendWayBoxRepository.Insert(ProductSendWayBox);
                    uow.Save();
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "CreateProductSendWayBox", true, 200, "انختاب باکس ارسال برای " + ProductSendWayBox.ProductSendWayId, DateTime.Now, User.Identity.GetUserId());

                    return RedirectToAction("ProductSendWayBoxes", new { id = ProductSendWayBox.ProductSendWayId });
                }
                else
                {

                    ViewBag.Erorr = "این باکس برای این روش انتخاب شده است !";
                    return View(ProductSendWayBox);
                }
            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreateProductSendWayBox", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(ProductSendWayBox);
            }
        }



        public ActionResult EditProductSendWayBox(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var sendWayBox = uow.ProductSendWayBoxRepository.GetByID(id);
                    if (sendWayBox != null)
                    {
                        ViewBag.SendBoxes = new SelectList(uow.SendwayBoxRepository.Get(x => x), "Id", "Title", sendWayBox.SendWayBoxID);
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditProductSendWayBox", true, 200, "نمایش صفحه ویرایش باکس ارسال انتخابیِ" + sendWayBox.Id, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(sendWayBox);
                    }
                    else
                    {
                        return RedirectToAction("ProductSendWayBoxes", new { id });
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditProductSendWayBox", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProductSendWayBox(ProductSendWayBox ProductSendWayBox)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.SendBoxes = new SelectList(uow.SendwayBoxRepository.Get(x => x), "Id", "Title", ProductSendWayBox.SendWayBoxID);
            try
            {
                if (!uow.ProductSendWayBoxRepository.Get(x => x, x => x.ProductSendWayId == ProductSendWayBox.ProductSendWayId && x.SendWayBoxID == ProductSendWayBox.SendWayBoxID && x.Id != ProductSendWayBox.Id).Any())
                {
                    uow.ProductSendWayBoxRepository.Update(ProductSendWayBox);
                    uow.Save();
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditProductSendWayBox", true, 200, "ویرایش باکس ارسال انتخابیِ" + ProductSendWayBox.Id, DateTime.Now, User.Identity.GetUserId());

                    return RedirectToAction("ProductSendWayBoxes", new { id = ProductSendWayBox.ProductSendWayId });
                }
                else
                {
                    ViewBag.Erorr = "این باکس برای این روش انتخاب شده است !";
                    return View(ProductSendWayBox);

                }
            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditProductSendWayBox", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = s.Message;
                return View(ProductSendWayBox);
            }
        }


        public ActionResult DeleteProductSendWayBox(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var sendWayBox = uow.ProductSendWayBoxRepository.GetByID(id);
                    if (sendWayBox != null)
                    {
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "DeleteProductSendWayBox", true, 200, "نمایش صفحه حذف باکس ارسال انتخابی", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(sendWayBox);
                    }
                    else
                    {
                        return RedirectToAction("ProductSendWayBoxes", new { id = sendWayBox.ProductSendWayId });
                    }
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "DeleteProductSendWayBox", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductSendWayBox(ProductSendWayBox ProductSendWayBox)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            try
            {
                int id = ProductSendWayBox.ProductSendWayId;
                uow.ProductSendWayBoxRepository.Delete(ProductSendWayBox);
                uow.Save();
                ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "DeleteProductSendWayBox", true, 200, "حذف باکس ارسال انتخابیِ" + ProductSendWayBox.Id, DateTime.Now, User.Identity.GetUserId());

                return RedirectToAction("ProductSendWayBoxes", new { id = id });

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "DeleteProductSendWayBox", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " باکس انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(ProductSendWayBox);
            }
        }

        #endregion

        #region PriceDetail

        public ActionResult Detail(int id, string sortOrder, string proviencId, string provienceFilter, string cityId, string cityFilter, int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", (!String.IsNullOrEmpty(proviencId) ? proviencId : ""));
                    ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name", (!String.IsNullOrEmpty(cityId) ? cityId : ""));

                    var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                    ViewBag.setting = setting;

                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.AddPermission = p.First();

                    if (string.IsNullOrEmpty(proviencId))
                        proviencId = provienceFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(cityId))
                        cityId = cityFilter;
                    else
                        page = 1;

                    ViewBag.cityFilter = cityId;
                    ViewBag.provienceFilter = proviencId;
                    ViewBag.id = id;


                    var ProductSendWays = uow.ProductSendWayDetailRepository.GetByReturnQueryable(x => x, x => x.ProductSendWayBoxId == id, null, "City,ProductSendWayBox");

                    if (!String.IsNullOrEmpty(cityId))
                        ProductSendWays = ProductSendWays.Where(s => s.CityId == int.Parse(cityId));
                    if (!String.IsNullOrEmpty(proviencId))
                    {
                        int prvid = int.Parse(proviencId);
                        List<int> cities = uow.CityRepository.Get(x => x, x => x.ProvinceId == prvid).Select(x => x.Id).ToList();
                        ProductSendWays = ProductSendWays.Where(s => cities.Contains(s.CityId));
                    }

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    #region Sort
                    switch (sortOrder)
                    {
                        case "proviencId":
                            ProductSendWays = ProductSendWays.OrderBy(s => s.City.Province.Name);
                            ViewBag.CurrentSort = "proviencId";
                            break;
                        case "proviencId_desc":
                            ProductSendWays = ProductSendWays.OrderByDescending(s => s.City.Province.Name);
                            ViewBag.CurrentSort = "proviencId_desc";
                            break;
                        case "cityId":
                            ProductSendWays = ProductSendWays.OrderBy(s => s.City.Name);
                            ViewBag.CurrentSort = "cityId";
                            break;
                        case "cityId_desc":
                            ProductSendWays = ProductSendWays.OrderByDescending(s => s.City.Name);
                            ViewBag.CurrentSort = "cityId_desc";
                            break;
                        //case "From":
                        //    ProductSendWays = ProductSendWays.OrderBy(s => s.From);
                        //    ViewBag.CurrentSort = "From";
                        //    break;
                        //case "From_desc":
                        //    ProductSendWays = ProductSendWays.OrderByDescending(s => s.From);
                        //    ViewBag.CurrentSort = "From_desc";
                        //    break;
                        //case "To":
                        //    ProductSendWays = ProductSendWays.OrderBy(s => s.To);
                        //    ViewBag.CurrentSort = "To";
                        //    break;
                        //case "To_desc":
                        //    ProductSendWays = ProductSendWays.OrderByDescending(s => s.To);
                        //    ViewBag.CurrentSort = "To_desc";
                        //    break;
                        case "Price":
                            ProductSendWays = ProductSendWays.OrderBy(s => s.Value);
                            ViewBag.CurrentSort = "Price";
                            break;
                        case "Price_desc":
                            ProductSendWays = ProductSendWays.OrderByDescending(s => s.Value);
                            ViewBag.CurrentSort = "Price_desc";
                            break;
                        default:  // Name ascending 
                            ProductSendWays = ProductSendWays.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion



                    ViewBag.ProductSendWayBox = uow.ProductSendWayBoxRepository.Get(x => x, x => x.Id == id, null, "ProductSendWay,SendwayBox").FirstOrDefault();

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "جزئیات روشهای ارسال", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "Detail", true, 200, " نمایش صفحه جزئیات روشهای ارسال", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductSendWays.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult CreatePrice(int id)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {


                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name");
                    var ProductSendWayBox = uow.ProductSendWayBoxRepository.Get(x => x, x => x.Id == id, null, "ProductSendWay,SendwayBox").FirstOrDefault();
                    ViewBag.ProductSendWayBox = ProductSendWayBox;
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 24 && x.Name == "ایجاد", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "CreatePrice", true, 200, "نمایش صفحه ایجاد جزئیات باکس" + ProductSendWayBox.SendwayBox.Title + " برای روش ارسال " + ProductSendWayBox.ProductSendWay.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreatePrice", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreatePrice(Domain.ProductSendWayDetail ProductSendWayDetail, int? ProvinceId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name");
                var ProductSendWayBox = uow.ProductSendWayBoxRepository.Get(x => x, x => x.Id == ProductSendWayDetail.ProductSendWayBoxId, null, "ProductSendWay,SendwayBox").FirstOrDefault();
                ViewBag.ProductSendWayBox = ProductSendWayBox;

                if (!ProvinceId.HasValue)
                {
                    ViewBag.Error = "استان و شهر انتخاب نشده است !";
                    return View(ProductSendWayDetail);
                }
                //if (ProductSendWayDetail.From >= ProductSendWayDetail.To || ProductSendWayDetail.To <= ProductSendWayDetail.From)
                //{
                //    ViewBag.Error = "مقادیر را به صورت صحیح وارد نمایید !";
                //    return View(ProductSendWayDetail);
                //}

                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModelState.IsValid)
                {
                    if (ProductSendWayDetail.CityId == 0)
                    {
                        foreach (var item in uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId))
                        {
                            bool CompeleteDuplicate = false;
                            if (uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBoxId == ProductSendWayDetail.ProductSendWayBoxId && x.CityId == item.Id).Any())
                                CompeleteDuplicate = true;
                            if (CompeleteDuplicate == false && !uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBoxId == ProductSendWayDetail.ProductSendWayBoxId && x.CityId == item.Id).Any())
                            {

                                ProductSendWayDetail.CityId = item.Id;
                                uow.ProductSendWayDetailRepository.Insert(ProductSendWayDetail);
                                uow.Save();
                            }
                        }

                    }
                    else
                    {
                        bool CompeleteDuplicate = false;
                        if (uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBoxId == ProductSendWayDetail.ProductSendWayBoxId && x.CityId == ProductSendWayDetail.CityId).Any())
                            CompeleteDuplicate = true;
                        if (CompeleteDuplicate == false && !uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBoxId == ProductSendWayDetail.ProductSendWayBoxId && x.CityId == ProductSendWayDetail.CityId).Any())
                        {
                            uow.ProductSendWayDetailRepository.Insert(ProductSendWayDetail);
                            uow.Save();
                        }
                        else
                        {
                            ViewBag.Error = "رکورد انتخابی قبلا وارد شده است !";
                            return View(ProductSendWayDetail);
                        }
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "SendWays", "CreatePrice", false, 200, " ایجاد جزئیات برای " + ProductSendWayDetail.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Detail", "SendWays", new { id = ProductSendWayDetail.ProductSendWayBoxId });

                }

                return View(uow.ProductSendWayDetailRepository.Get(x => x, x => x.Id == ProductSendWayDetail.Id, null, "City,ProductSendWayBox"));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "CreatePrice", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public JsonResult AddPrice(int SendwayId, int ProvinceId, int? CityId,bool IsActive)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (!CityId.HasValue)
                {
                    foreach (var item in uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId))
                    {
                        foreach (var ProductSendWayBoxId in uow.ProductSendWayBoxRepository.Get(x => x.Id, x => x.ProductSendWayId == SendwayId))
                        {

                            bool CompeleteDuplicate = false;
                            if (uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBox.ProductSendWayId == SendwayId && x.ProductSendWayBoxId == ProductSendWayBoxId && x.CityId == item.Id).Any())
                                CompeleteDuplicate = true;
                            if (CompeleteDuplicate == false && !uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBox.ProductSendWayId == SendwayId && x.ProductSendWayBoxId == ProductSendWayBoxId && x.CityId == item.Id).Any())
                            {
                                ProductSendWayDetail productSendWayDetail = new ProductSendWayDetail();
                                productSendWayDetail.CityId = item.Id;
                                productSendWayDetail.ProductSendWayBoxId = ProductSendWayBoxId;
                                productSendWayDetail.IsActive = IsActive;
                                uow.ProductSendWayDetailRepository.Insert(productSendWayDetail);
                                uow.Save();
                            }
                        }
                    }

                }
                else
                {
                    foreach (var ProductSendWayBoxId in uow.ProductSendWayBoxRepository.Get(x => x.Id, x => x.ProductSendWayId == SendwayId))
                    {
                        bool CompeleteDuplicate = false;
                        if (uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBox.ProductSendWayId == SendwayId && x.ProductSendWayBoxId == ProductSendWayBoxId && x.CityId == CityId.Value).Any())
                            CompeleteDuplicate = true;
                        if (CompeleteDuplicate == false && !uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBox.ProductSendWayId == SendwayId && x.ProductSendWayBoxId == ProductSendWayBoxId && x.CityId == CityId.Value).Any())
                        {
                            ProductSendWayDetail productSendWayDetail = new ProductSendWayDetail();
                            productSendWayDetail.CityId = CityId.Value;
                            productSendWayDetail.ProductSendWayBoxId = ProductSendWayBoxId;
                            productSendWayDetail.IsActive = IsActive;
                            uow.ProductSendWayDetailRepository.Insert(productSendWayDetail);
                            uow.Save();
                        }
                    }
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "SendWays", "AddPrice", false, 200, " ایجاد هزینه برای برای " + SendwayId, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return Json(new
                {
                    statuscode = 200
                }, JsonRequestBehavior.AllowGet);



            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "AddPrice", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion


                return Json(new
                {
                    statuscode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult RemovePrice(int SendwayId, int ProvinceId, int? CityId)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (!CityId.HasValue)
                {
                    foreach (var item in uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId))
                    {
                        foreach (var ProductSendWayBoxId in uow.ProductSendWayBoxRepository.Get(x => x.Id, x => x.ProductSendWayId == SendwayId))
                        {
                            ProductSendWayDetail productSendWayDetail = uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBox.ProductSendWayId == SendwayId && x.ProductSendWayBoxId == ProductSendWayBoxId && x.CityId == item.Id).First();
                            uow.ProductSendWayDetailRepository.Delete(productSendWayDetail);
                            uow.Save();
                        }
                    }

                }
                else
                {
                    foreach (var ProductSendWayBoxId in uow.ProductSendWayBoxRepository.Get(x => x.Id, x => x.ProductSendWayId == SendwayId))
                    {
                        ProductSendWayDetail productSendWayDetail = uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBox.ProductSendWayId == SendwayId && x.ProductSendWayBoxId == ProductSendWayBoxId && x.CityId == CityId.Value).Single();
                        uow.ProductSendWayDetailRepository.Delete(productSendWayDetail);
                        uow.Save();

                    }
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "SendWays", "RemovePrice", false, 200, " حذف هزینه برای برای " + SendwayId, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return Json(new
                {
                    statuscode = 200
                }, JsonRequestBehavior.AllowGet);



            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "RemovePrice", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion


                return Json(new
                {
                    statuscode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdatePrice(List<SenwayPrice> senwayPrices)
        {
            uow = new UnitOfWorkClass();
            try
            {
                int cityId = senwayPrices.First().cityId;
                int sendwayId = senwayPrices.First().sendwayId;
                var ProductSendWayDetails = uow.ProductSendWayDetailRepository.Get(x => x, x => x.CityId == cityId && x.ProductSendWayBox.ProductSendWayId == sendwayId, null, "ProductSendWayBox");
                foreach (var item in senwayPrices)
                {
                    var ProductSendWayDetail = ProductSendWayDetails.Where(x => x.CityId == item.cityId && x.ProductSendWayBox.ProductSendWayId == item.sendwayId && x.ProductSendWayBoxId == item.SendWayBoxID).Single();
                    ProductSendWayDetail.Limitation = item.limitation;
                    ProductSendWayDetail.Value = item.value;
                    ProductSendWayDetail.IsActive = item.isActive;
                    uow.ProductSendWayDetailRepository.Update(ProductSendWayDetail);
                    uow.Save();
                }


                return Json(new
                {
                    statuscode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new
                {
                    statuscode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditPrice(int id)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var ProductSendWayDetail = uow.ProductSendWayDetailRepository.Get(x => x, x => x.Id == id, null).First();
                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProductSendWayDetail.City.ProvinceId);
                    ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProductSendWayDetail.City.ProvinceId), "Id", "Name", ProductSendWayDetail.CityId);
                    var ProductSendWayBox = uow.ProductSendWayBoxRepository.Get(x => x, x => x.Id == ProductSendWayDetail.ProductSendWayBoxId, null, "ProductSendWay,SendwayBox").FirstOrDefault();
                    ViewBag.ProductSendWayBox = ProductSendWayBox;
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 24 && x.Name == "ویرایش", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "EditPrice", true, 200, "نمایش صفحه ویرایش جزئیات باکس" + ProductSendWayBox.ProductSendWay.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductSendWayDetail);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditPrice", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditPrice(Domain.ProductSendWayDetail ProductSendWayDetail, int? ProvinceId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId.Value);
                ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId.Value), "Id", "Name", ProductSendWayDetail.CityId);
                var ProductSendWayBox = uow.ProductSendWayBoxRepository.Get(x => x, x => x.Id == ProductSendWayDetail.ProductSendWayBoxId, null, "ProductSendWay,SendwayBox").FirstOrDefault();
                ViewBag.ProductSendWayBox = ProductSendWayBox;

                if (!ProvinceId.HasValue)
                {
                    ViewBag.Error = "استان و شهر انتخاب نشده است !";
                    return View(ProductSendWayDetail);
                }
                //if (ProductSendWayDetail.From >= ProductSendWayDetail.To || ProductSendWayDetail.To <= ProductSendWayDetail.From)
                //{
                //    ViewBag.Error = "مقادیر را به صورت صحیح وارد نمایید !";
                //    return View(ProductSendWayDetail);
                //}

                var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                ViewBag.setting = setting;
                if (ModelState.IsValid)
                {

                    bool CompeleteDuplicate = false;
                    if (uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBoxId == ProductSendWayDetail.ProductSendWayBoxId && x.CityId == ProductSendWayDetail.CityId).Any())
                        CompeleteDuplicate = true;

                    if (CompeleteDuplicate == false && !uow.ProductSendWayDetailRepository.Get(x => x, x => x.ProductSendWayBoxId == ProductSendWayDetail.ProductSendWayBoxId && x.CityId == ProductSendWayDetail.CityId && x.Id != ProductSendWayDetail.Id).Any())
                    {
                        uow.ProductSendWayDetailRepository.Update(ProductSendWayDetail);
                        uow.Save();
                    }
                    else
                    {
                        ViewBag.Error = "رکورد انتخابی قبلا وارد شده است !";
                        return View(ProductSendWayDetail);
                    }


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "SendWays", "EditPrice", false, 200, " ایجاد جزئیات برای " + ProductSendWayDetail.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Detail", "SendWays", new { id = ProductSendWayDetail.ProductSendWayBoxId });

                }

                return View(uow.ProductSendWayDetailRepository.Get(x => x, x => x.Id == ProductSendWayDetail.Id, null, "City,ProductSendWayBox"));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "EditPrice", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/SendWays/Delete/5
        public ActionResult DeletePrice(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Domain.ProductSendWayDetail ProductSendWayDetail = uow.ProductSendWayDetailRepository.GetByID(id);
                    var ProductSendWayBox = uow.ProductSendWayBoxRepository.Get(x => x, x => x.Id == ProductSendWayDetail.ProductSendWayBoxId, null, "ProductSendWay,SendwayBox").FirstOrDefault();
                    ViewBag.ProductSendWayBox = ProductSendWayBox;


                    if (ProductSendWayDetail == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "SendWays", "DeletePrice", true, 200, " نمایش صفحه حذفِ جزئیات روش ارسال" + ProductSendWayDetail.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductSendWayDetail);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "روش های ارسال" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "DeletePrice", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/SendWays/DeletePrice/5
        [HttpPost, ActionName("DeletePrice")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePrice(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            Domain.ProductSendWayDetail ProductSendWayDetail = uow.ProductSendWayDetailRepository.GetByID(id);
            var ProductSendWayBox = uow.ProductSendWayBoxRepository.Get(x => x, x => x.Id == ProductSendWayDetail.ProductSendWayBoxId, null, "ProductSendWay,SendwayBox").FirstOrDefault();
            ViewBag.ProductSendWayBox = ProductSendWayBox;


            try
            {
                int ProductSendWayId = ProductSendWayDetail.ProductSendWayBox.ProductSendWayId;
                uow.ProductSendWayDetailRepository.Delete(ProductSendWayDetail);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "SendWays", "DeletePrice", false, 200, " حذفِ جزئیات روش ارسال " + ProductSendWayDetail.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Detail", "SendWays", new { id = ProductSendWayId });

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "SendWays", "DeletePrice", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " باکس انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(ProductSendWayDetail);
            }
        }
        #endregion


    }

    public class SenwayPrice
    {
        public int sendwayId { get; set; }
        public int cityId { get; set; }
        public int SendWayBoxID { get; set; }
        public int limitation { get; set; }
        public int value { get; set; }
        public bool isActive { get; set; }
    }
}

