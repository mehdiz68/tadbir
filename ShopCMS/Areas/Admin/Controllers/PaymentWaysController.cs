using Domain;
using ahmadi.Infrastructure.Security;
using CoreLib.ViewModel.Xml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class PaymentWaysController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;
        //
        // GET: /Admin/PaymentWays/
        public virtual ActionResult Index()
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

                    var paymentWay = uow.BankAccountRepository.Get(x => x, null, s => s.OrderBy(x => x.DisplayOrder), "ProductSendWay,BankAccountOnlineInfo").ToList();

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "روش های پرداخت", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "PaymentWays", "Index", true, 200, " نمایش صفحه مدیریت روشهای پرداخت ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(paymentWay);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای پرداخت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PaymentWays", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/PaymentWays/Edit/5
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var bankAccount = uow.BankAccountRepository.Get(x => x, x => x.Id == id, null, "BankAccountOnlineInfo").FirstOrDefault();
                    if (bankAccount == null)
                    {
                        return HttpNotFound();
                    }

                    var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                    ViewBag.setting = setting;
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 25 && x.Name == "ایجاد حساب بانکی", null, "HelpModuleSectionFields").FirstOrDefault();
                    ViewBag.ProductSendWays = new SelectList(uow.ProductSendWayRepository.Get(x => x), "Id", "Title", bankAccount.ProductSendWayId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "PaymentWays", "Edit", true, 200, " نمایش صفحه ویرایش شیوه پرداخت " + bankAccount.CardNumber, DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    return View(bankAccount);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت شیوه های پرداخت" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PaymentWays", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/PaymentWays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Edit(BankAccount ba, string NewBankId, string OnlineType, string TerminalId, string UserName, string Password, string callbackBank)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    if (NewBankId != ba.BankId.ToString())
                    {
                        Int16 newId = Convert.ToInt16(NewBankId);
                        var bankAccount = uow.BankAccountRepository.Get(x => x, x => x.BankId == newId && x.AccountNumber == ba.AccountNumber && x.CardNumber == ba.CardNumber && x.Id != ba.Id).SingleOrDefault();
                        if (bankAccount != null)
                        {
                            ViewBag.error = "این بانک شامل روش پرداخت میباشد، لطفا بانک دیگری را انتخاب نمایید";
                            return View(ba);
                        }
                        else
                        {
                            ba.BankId = newId;
                        }
                    }
                    if (OnlineType != null)
                    {
                        if (!string.IsNullOrEmpty(TerminalId) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                        {
                            var bc = uow.BankAccountOnlineInfoRepository.GetByID(ba.Id);
                            if (bc != null)
                            {
                                bc.TerminalId = TerminalId;
                                bc.UserName = UserName;
                                bc.Password = Password;
                                bc.CallbackUrl = callbackBank;
                                uow.BankAccountOnlineInfoRepository.Update(bc);
                                uow.Save();
                            }
                            else
                            {
                                BankAccountOnlineInfo newbc = new BankAccountOnlineInfo()
                                {
                                    TerminalId = TerminalId,
                                    UserName = UserName,
                                    Password = Password,
                                    OnlineInfoId = ba.Id,
                                    CallbackUrl = callbackBank
                                };
                                uow.BankAccountOnlineInfoRepository.Insert(newbc);
                                uow.Save();
                            }
                        }
                    }
                    else
                    {
                        var bc = uow.BankAccountOnlineInfoRepository.GetByID(ba.Id);
                        if (bc != null)
                        {
                            uow.BankAccountOnlineInfoRepository.Delete(bc);
                            uow.Save();
                        }
                    }

                    uow.BankAccountRepository.Update(ba);
                    uow.Save();

                    XMLReader readXML = new XMLReader(uow.SettingRepository.Get(x => x).First().StaticContentDomain);
                    XPaymentWayEdit XPaymentWayEdit = readXML.ListOfXPaymentWayEdit().FirstOrDefault();
                    if (XPaymentWayEdit != null)
                    {
                        XPaymentWayEdit.Edit = 1;
                        readXML.EditXPaymentWayEdit(XPaymentWayEdit);
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "BankAccounts", "Edit", false, 200, "ویرایش روش پرداخت " + ba.CardNumber, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index", new { id = ba.BankId });
                }

                ViewBag.ProductSendWays = new SelectList(uow.ProductSendWayRepository.Get(x => x), "Id", "Title", ba.ProductSendWayId);
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 25 && x.Name == "ایجاد حساب بانکی", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "BankAccounts", "Edit", false, 500, "خطا در ویرایش روش پرداخت" + ba.CardNumber, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(ba);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "BankAccounts", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/PaymentWays/Create
        public virtual ActionResult Create()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 16, 1))
                {
                    var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                    ViewBag.setting = setting;
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 25 && x.Name == "ایجاد حساب بانکی", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "PaymentWays", "Create", true, 200, "نمایش صفحه ایجاد روش پرداخت", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    ViewBag.ProductSendWays = new SelectList(uow.ProductSendWayRepository.Get(x => x), "Id", "Title");
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روش های پرداخت" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PaymentWays", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/PaymentWays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create(BankAccount ba, string OnlineType, string TerminalId, string UserName, string Password, string callbackBank)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {

                    var bankAccount = uow.BankAccountRepository.Get(x => x, x => x.BankId == ba.BankId && x.AccountNumber == ba.AccountNumber && x.CardNumber == ba.CardNumber).SingleOrDefault();
                    if (bankAccount != null)
                    {
                        ViewBag.error = "این بانک شامل روش پرداخت میباشد، لطفا بانک دیگری را انتخاب نمایید";
                        return View(ba);
                    }

                    if (OnlineType != null)
                    {
                        if (!string.IsNullOrEmpty(TerminalId) && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                        {
                            BankAccountOnlineInfo newbc = new BankAccountOnlineInfo()
                            {
                                TerminalId = TerminalId,
                                UserName = UserName,
                                Password = Password,
                                OnlineInfoId = ba.Id,
                                CallbackUrl = callbackBank
                            };
                            uow.BankAccountOnlineInfoRepository.Insert(newbc);
                        }
                    }

                    ba.DisplayOrder = uow.BankAccountRepository.Get(x => x).Any() ? uow.BankAccountRepository.Max(x => x.DisplayOrder) + 1 : 1;
                    uow.BankAccountRepository.Insert(ba);
                    uow.Save();

                    XMLReader readXML = new XMLReader(uow.SettingRepository.Get(x => x).First().StaticContentDomain);
                    XPaymentWayEdit XPaymentWayEdit = readXML.ListOfXPaymentWayEdit().FirstOrDefault();
                    if (XPaymentWayEdit != null)
                    {
                        XPaymentWayEdit.Edit = 1;
                        readXML.EditXPaymentWayEdit(XPaymentWayEdit);
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "PaymentWays", "Create", false, 200, "ایجاد روش پرداخت " + ba.CardNumber, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                ViewBag.ProductSendWays = new SelectList(uow.ProductSendWayRepository.Get(x => x), "Id", "Title", ba.ProductSendWayId);
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PaymentWays", "Create", false, 500, "خطا در ایجاد روش پرداخت" + ba.CardNumber, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(ba);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PaymentWays", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/PaymentWays/Sort
        public virtual ActionResult Sort(int? page)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
               
                    var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                    if (p.Where(x => x == true).Any())
                    {
                        ViewBag.EditPermission = p.Skip(1).First();

                        var setting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                        ViewBag.setting = setting;

                        var paymentWay = uow.BankAccountRepository.Get(x => x, null, s => s.OrderBy(x => x.DisplayOrder));

                        int pageSize = 100;
                        int pageNumber = (page ?? 1);

                        ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 25 && x.Name == "مرتب سازی", null, "HelpModuleSectionFields").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "PaymentWays", "Sort", true, 200, " نمایش صفحه مرتب سازی روشهای پرداخت ", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(paymentWay.ToPagedList(pageNumber, pageSize));
                    }
                    else
                        return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت روشهای پرداخت" }));
                
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PaymentWays", "Sort", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/PaymentWays/Sort/5
        [HttpPost]
        public virtual JsonResult Sort(string ids)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                string[] idss = ids.Split(',');
                for (int i = 0; i < idss.Length; i++)
                {
                    int id = Convert.ToInt32(idss[i]);
                    var paymentWay = uow.BankAccountRepository.GetByID(id);
                    paymentWay.DisplayOrder = i;
                    uow.Save();
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(3, "PaymentWays", "Sort", false, 200, "مرتب سازی روشهای پرداخت", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "PaymentWays", "Sort", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
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