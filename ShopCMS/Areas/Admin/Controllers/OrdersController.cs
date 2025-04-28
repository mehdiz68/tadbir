using ahmadi.Infrastructure;
using CoreLib.ViewModel.Xml;
using Domain;
using Microsoft.AspNet.Identity;
using PagedList;
using ahmadi.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UnitOfWork;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CoreLib.Infrastructure.DateTime;
using System.Data.Odbc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using ahmadi.Infrastructure.Report;
using System.IO;
using ahmadi.Areas.Admin.ViewModels.Report;
using AutoMapper;
using System.Data;
using CoreLib;
using Domain.ViewModels;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class OrdersController : Controller
    {
        private UnitOfWorkClass uow;
        public OrdersController()
        {
            uow = new UnitOfWorkClass();
        }

        // GET: Admin/Orders
        [Infrastructure.Filter.AutoExecueFilter]
        public ActionResult Index(int? PageSize, string sortOrder, string OrderCodeString, string OrderCodeStringFilter, string UserNameFamilyString, string UserNameFamilyStringFilter, string StateString, string StateStringFilter, string VisitedString, string VisitedStringFilter, string StateLastString, string StateLastStringFilter, string IsActive, string IsActiveFilter, string ProductCatId, string ProductCatIdFilter, string NameString, string NameFilter, string BrandId, string BrandFilter, string ProductTypeId, string ProductTypeFilter, string ProductCodeString, string ProductCodeFilter, string LanguagenameString, string LanguagenameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, string SellerId, string SellerFilter, int? page)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 25);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    #region Load DropDown List
                    int langid = 1;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;
                    if (LanguagenameString != null)
                        langid = int.Parse(LanguagenameString);

                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                    ViewBag.IsActive = IsActiveSelectListItem;

                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.BrandId = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                    ViewBag.SellerId = new SelectList(uow.SellerRepository.Get(x => x), "Id", "User.LastName");


                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    ViewBag.StateSelectListItem = new SelectList(readXML.ListOfXState(), "Id", "Name");

                    List<SelectListItem> VisitedSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "مشاهده شده", Value = "true" }, new SelectListItem() { Text = "مشاهده نشده", Value = "false" } };
                    ViewBag.VisitedSelectListItem = VisitedSelectListItem;

                    List<SelectListItem> StateLastSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" }, new SelectListItem() { Text = "جبران مرجوعی", Value = "9" }, new SelectListItem() { Text = "درخواست لغو", Value = "10" }, new SelectListItem() { Text = "درخواست مرجوعی", Value = "11" }, new SelectListItem() { Text = "عدم تایید درخواست لغو", Value = "12" }, new SelectListItem() { Text = "عدم تایید درخواست مرجوعی", Value = "13" } };
                    ViewBag.StateLastSelectListItem = StateLastSelectListItem;

                    #endregion

                    #region search
                    if (string.IsNullOrEmpty(OrderCodeString))
                        OrderCodeString = OrderCodeStringFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(UserNameFamilyString))
                        UserNameFamilyString = UserNameFamilyStringFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StateString))
                        StateString = StateStringFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(VisitedString))
                        VisitedString = VisitedStringFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StateLastString))
                        StateLastString = StateLastStringFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(IsActive))
                        IsActive = IsActiveFilter;
                    else
                        page = 1;


                    if (string.IsNullOrEmpty(ProductCatId))
                        ProductCatId = ProductCatIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                    {
                        page = 1;

                        NameString = NameString.ToLower();
                    }


                    if (string.IsNullOrEmpty(BrandId))
                        BrandId = BrandFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductTypeId))
                        ProductTypeId = ProductTypeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductCodeString))
                        ProductCodeString = ProductCodeFilter;
                    else
                        page = 1;

                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
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
                    if (string.IsNullOrEmpty(SellerId))
                        SellerId = SellerFilter;
                    else
                        page = 1;

                    ViewBag.OrderCodeStringFilter = OrderCodeString;
                    ViewBag.UserNameFamilyStringFilter = UserNameFamilyString;
                    ViewBag.StateStringFilter = StateString;
                    ViewBag.VisitedStringFilter = VisitedString;
                    ViewBag.StateLastStringFilter = StateLastString;
                    ViewBag.IsActiveFilter = IsActive;
                    ViewBag.ProductCatIdFilter = ProductCatId;
                    ViewBag.NameFilter = NameString;
                    ViewBag.BrandFilter = BrandId;
                    ViewBag.ProductTypeFilter = ProductTypeId;
                    ViewBag.ProductCodeFilter = ProductCodeString;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.InsertDateEndFilter = InsertDateEnd;
                    ViewBag.InsertDateStartFilter = InsertDateStart;
                    ViewBag.SellerFilter = SellerId;

                    var orders = uow.OrderRepository.GetQueryList().AsNoTracking().Include("User").Include("User.CityEntity.Province").Include("OrderWallets.Wallet").Include("OrderDeliveries.ProductSendWay").Include("OrderStates").Include("OrderRows.Product.ProductPrices");

                    if (!String.IsNullOrEmpty(OrderCodeString))
                    {
                        int id = Convert.ToInt32(OrderCodeString);
                        orders = orders.Where(s => s.CustomerOrderId == OrderCodeString || s.BankOrderId == id);
                    }
                    if (!String.IsNullOrEmpty(UserNameFamilyString))
                    {
                        orders = orders.Where(s => s.User.FirstName.Contains(UserNameFamilyString) || s.User.LastName.Contains(UserNameFamilyString));

                    }
                    if (!String.IsNullOrEmpty(StateString))
                    {
                        int id = Convert.ToInt32(StateString);
                        orders = orders.Where(s => s.User.State == id);
                    }
                    if (!String.IsNullOrEmpty(StateLastString))
                    {
                        OrderStatus id = (OrderStatus)Convert.ToInt32(StateLastString);
                        orders = orders.Where(s => s.OrderStates.OrderByDescending(x => x.Id).FirstOrDefault().state == id);
                    }
                    if (!String.IsNullOrEmpty(VisitedString))
                    {
                        bool id = Convert.ToBoolean(VisitedString);
                        orders = orders.Where(s => s.Visited == id);
                    }
                    if (!String.IsNullOrEmpty(IsActive))
                    {
                        bool id = Convert.ToBoolean(IsActive);
                        orders = orders.Where(s => s.IsActive == id);
                    }
                    if (!String.IsNullOrEmpty(NameString))
                    {
                        var color = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.Color.Contains(NameString)).FirstOrDefault();
                        string colorId = "0";
                        if (color != null)
                            colorId = color.Id.ToString();
                        var Garanty = uow.ProductAttributeItemRepository.Get(x => x, x => x.Value.Contains(NameString)).FirstOrDefault();
                        string GarantyId = "0";
                        if (Garanty != null)
                            GarantyId = Garanty.Id.ToString();
                        orders = from s in orders
                                 where
                                        (s.OrderRows.Any(x => x.Product.LatinName != null) && s.OrderRows.Any(x => x.Product.LatinName.ToLower().Contains(NameString))) ||
                                        (s.OrderRows.Any(x => x.Product.Name != null) && s.OrderRows.Any(x => x.Product.Name.ToLower().Contains(NameString))) ||
                                         (s.OrderRows.Any(x => x.ProductPrice.code != null) && s.OrderRows.Any(x => x.ProductPrice.code.ToLower().Contains(NameString))) ||
                                        (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModelId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModel.Value.ToLower().Contains(NameString))) ||
                                        (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSizeId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSize.Value.ToLower().Contains(NameString))) ||
                                        (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectWeightId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectweight.Value.ToLower().Contains(NameString))) ||
                                        (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColorId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColor.Value.ToLower() == colorId)) ||
                                        (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGarantyId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId))
                                 select s;

                    }
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int id = Convert.ToInt32(LanguagenameString);

                        orders = orders.Where(s => s.LanguageId == id);
                    }
                    if (!String.IsNullOrEmpty(ProductCatId))
                    {
                        int ctId = Convert.ToInt32(ProductCatId);
                        if (ctId > 0)
                        {

                            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ctId)).ToList();
                            orders = orders.Where(s => s.OrderRows.Any(x => x.Product.ProductCategories.Any(w => CatIds.Contains(w.Id))));
                        }
                    }
                    if (!String.IsNullOrEmpty(BrandId))
                    {
                        int id = Convert.ToInt32(BrandId);
                        orders = orders.Where(s => s.OrderRows.Any(w => w.Product.BrandId == id));
                    }
                    if (!String.IsNullOrEmpty(ProductTypeId))
                    {
                        int id = Convert.ToInt32(ProductTypeId);
                        orders = orders.Where(s => s.OrderRows.Any(w => w.Product.ProductTypeId == id));
                    }
                    if (!String.IsNullOrEmpty(ProductCodeString))
                    {
                        orders = orders.Where(s => s.OrderRows.Any(w => w.Product.Code == ProductCodeString || s.OrderRows.Any(a => a.Product.ProductPrices.Any(x => x.code == ProductCodeString))));
                    }
                    if (!String.IsNullOrEmpty(SellerId))
                    {
                        int id = Convert.ToInt32(SellerId);

                        orders = orders.Where(s => s.OrderRows.Any(w => w.ProductPrice.SellerId == id));
                    }
                    DateTime dtInsertDateStart = DateTime.Now.Date, dtInsertDateEnd = DateTime.Now.Date, dtUpdateDateStart = DateTime.Now.Date, dtUpdateDateEnd = DateTime.Now.Date, dtDeleteDateStart = DateTime.Now.Date, dtDeleteDateEnd = DateTime.Now.Date;
                    if (!String.IsNullOrEmpty(InsertDateStart))
                        dtInsertDateStart = DateTimeConverter.ChangeShamsiToMiladi(InsertDateStart);
                    if (!String.IsNullOrEmpty(InsertDateEnd))
                        dtInsertDateEnd = DateTimeConverter.ChangeShamsiToMiladi(InsertDateEnd);

                    if (!String.IsNullOrEmpty(InsertDateStart) && !String.IsNullOrEmpty(InsertDateEnd))
                        orders = orders.Where(s => s.InsertDate >= dtInsertDateStart && s.InsertDate <= dtInsertDateEnd);
                    else if (!String.IsNullOrEmpty(InsertDateStart))
                        orders = orders.Where(s => s.InsertDate >= dtInsertDateStart);
                    else if (!String.IsNullOrEmpty(InsertDateEnd))
                        orders = orders.Where(s => s.InsertDate <= dtInsertDateEnd);

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        //case "sendway":
                        //    orders = orders.OrderBy(s => s.ProductSendWay.Title);
                        //    ViewBag.CurrentSort = "sendway";
                        //    break;
                        //case "sendway_desc":
                        //    orders = orders.OrderByDescending(s => s.ProductSendWay.Title);
                        //    ViewBag.CurrentSort = "sendway_desc";
                        //    break;
                        case "state":
                            orders = orders.OrderBy(s => s.User.State);
                            ViewBag.CurrentSort = "state";
                            break;
                        case "state_desc":
                            orders = orders.OrderByDescending(s => s.User.State);
                            ViewBag.CurrentSort = "state_desc";
                            break;
                        case "insertDate":
                            orders = orders.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "inserDate";
                            break;
                        case "insertDate_desc":
                            orders = orders.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "insertDate_desc";
                            break;
                        case "isactive":
                            orders = orders.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "isactive";
                            break;
                        case "isactive_desc":
                            orders = orders.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "isactive_desc";
                            break;

                        default:
                            orders = orders.OrderByDescending(s => s.InsertDate);
                            break;
                    }

                    #endregion

                    int pageSize = 10;
                    if (PageSize.HasValue)
                    {
                        if (PageSize.Value > 100)
                            pageSize = 100;
                        else if (pageSize < 10)
                            pageSize = 10;
                        else
                            pageSize = PageSize.Value;
                    }
                    ViewBag.PageSize = pageSize;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "سفارشات", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "orders", "Index", true, 200, " نمایش صفحه مدیریت سفارشات", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(orders.ToPagedList(pageNumber, pageSize));

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت سفارشات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "orders", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Orders/Edit/5
        [Infrastructure.Filter.AutoExecueFilter]
        public virtual ActionResult Edit(string id, Int16? result)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 25);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var order = uow.OrderRepository.Get(x => x, x => x.Id.ToString() == id, null, "OrderRows.Product,OrderStates,OrderWallets,User.CityEntity.Province,OrderDeliveries.ProductSendWayWorkTime,OrderAttributeSelects.OrderAttribute,OrderDeliveries.ProductSendWay,OrderWallets.Wallet,OrderDeliveries.UserAddress.CityEntity.Province,OrderWallets.wallet.BankAccount,OrderWallets.wallet.WalletAttributeWallets.WalletAttribute,OrderRows,UserBons,UserBonLogs,UserCodeGiftLogs.UserCodeGift.Offer,GeneralCodeGiftLogs.GeneralCodeGift.Offer,OrderRows.ProductPrice.ProductImages.Image,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,OrderRows.ProductPrice.Product.ProductCategories,OrderRows.ProductPrice.ProductAttributeSelectModel,OrderRows.ProductPrice.ProductAttributeSelectSize,OrderRows.ProductPrice.ProductAttributeSelectColor,OrderRows.ProductPrice.ProductAttributeSelectGaranty,OrderRows.ProductPrice.ProductAttributeSelectweight,OrderRows.ProductPrice.Product.ProductCategories,OrderRows.OrderDelivery.ProductSendWayWorkTime,OrderRows.ProductOffer.Offer").SingleOrDefault();
                    if (order == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.LabelIcon = uow.LabelIconRepository.Get(x => x, null, null, "attachment");

                    ViewBag.ProductStateList = uow.ProductStateRepository.Get(x => x);
                    XMLReader XR = new XMLReader(setting.StaticContentDomain);
                    ViewBag.SendWays = new SelectList(uow.ProductSendWayRepository.Get(x => x), "Id", "Title");


                    if (result != null)
                    {
                        if (result == 1)
                            ViewBag.result = "<p class='alert alert-success'>آدرس با موفقیت ویرایش شد</p>";
                        else if (result == 2)
                            ViewBag.result = "<p class='alert alert-danger'>ویرایش آدرس با خطا روبه رو شد</p>";
                        else if (result == 3)
                            ViewBag.result = "<p class='alert alert-success'>به روز رسانی وضعیت با موفقیت انجام شد</p>";
                        else if (result == 4)
                            ViewBag.result = "<p  class='alert alert-danger'>به روز رسانی وضعیت با خطا رو به رو شد</p>";
                        else if (result == 5)
                            ViewBag.result = "<p  class='alert alert-danger'>به روز رسانی انجام شد اما مشتری مورد نظر در حال حاضر بن تخفیف به مقدار لازم برای این سفارش ندارد </p>";
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Orders", "Edit", true, 200, " نمایش صفحه ویرایش سفارش " + order.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    order.Visited = true;
                    uow.Save();

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 23 && x.Name == "جزئیات سفارش", null, "HelpModuleSectionFields").FirstOrDefault();
                    return View(order);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت سفارش" }));


            }
            catch (Exception x)
            {

                throw;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Orders", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                //return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> StateUpdate(string Id, string OrderStateId, string trackingCode, string barbariNumber, string barbariNumber2, string barbariNumber3, string description, bool ConfirmCashPeik, bool ConfirmPosPeik, string PosCodePeigiry, string PosDateTime, bool ConfirmEstelam, int OrderDeliveryId, string[] IconId,int? SendWay)
        {
            try
            {
                string sendwayName = "";
                if (SendWay.HasValue)
                    sendwayName = uow.ProductSendWayRepository.Get(x => x.Title, x => x.Id == SendWay.Value).Single();
                var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                int result = 3;
                var order = uow.OrderRepository.GetQueryList().Include("OrderStates").Include("OrderWallets").Include("User.CityEntity.Province").Include("OrderDeliveries.labelIcons").Include("OrderDeliveries.ProductSendWayWorkTime").Include("OrderAttributeSelects.OrderAttribute").Include("OrderDeliveries.ProductSendWay").Include("OrderWallets.Wallet.BankAccount").Include("OrderDeliveries.UserAddress.CityEntity.Province").Include("OrderWallets.wallet.WalletAttributeWallets.WalletAttribute").Include("OrderRows").Include("UserBons").Include("UserBonLogs").Include("UserCodeGiftLogs").Include("OrderRows.OrderDelivery.ProductSendWayWorkTime").Where(x => x.Id.ToString() == Id).SingleOrDefault();

                if (order != null)
                {

                    // آیکن های پرینت لیبل
                    #region LabelIcons
                    #region Delete Old LabelIcons
                    List<string> lstIcon = new List<string>();
                    if (IconId != null)
                    {
                        lstIcon = IconId.ToList();
                        var OrderDliveryIcons = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == OrderDeliveryId, null, "labelIcons").FirstOrDefault().labelIcons;
                        List<labelIcon> DeletelabelIcon = new List<labelIcon>();
                        foreach (labelIcon item in OrderDliveryIcons)
                        {
                            var current = lstIcon.Where(x => x.Equals(item.Id.ToString())).FirstOrDefault();
                            if (current != null)
                                lstIcon.Remove(current);
                            else
                                DeletelabelIcon.Add(item);
                        }
                        foreach (labelIcon item in DeletelabelIcon)
                        {

                            OrderDelivery CurrentOrderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == OrderDeliveryId).FirstOrDefault();
                            uow.OrderDeliveryRepository.Load(CurrentOrderDelivery, "labelIcons");
                            CurrentOrderDelivery.labelIcons.Remove(item);
                            uow.Save();
                        }

                    }
                    else
                    {
                        var OrderDliveryIcons = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == OrderDeliveryId, null, "labelIcons").FirstOrDefault().labelIcons;
                        List<labelIcon> DeleteOtherImages = new List<labelIcon>();
                        foreach (labelIcon item in OrderDliveryIcons)
                        {
                            DeleteOtherImages.Add(item);
                        }
                        foreach (labelIcon item in DeleteOtherImages)
                        {
                            OrderDelivery CurrentOrderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == OrderDeliveryId).FirstOrDefault();
                            uow.OrderDeliveryRepository.Load(CurrentOrderDelivery, "labelIcons");
                            CurrentOrderDelivery.labelIcons.Remove(item);
                            uow.Save();
                        }

                    }
                    #endregion

                    List<string> CurrentTags = new List<string>();
                    var orderdelivery = order.OrderDeliveries.Where(x => x.Id == OrderDeliveryId).FirstOrDefault();
                    if (IconId != null)
                    {
                        foreach (string item in lstIcon)
                        {
                            int id = int.Parse(item);
                            orderdelivery.labelIcons.Add(uow.LabelIconRepository.GetByID(id));
                        }
                        uow.OrderDeliveryRepository.Update(orderdelivery);
                    }

                    #endregion

                    //بروزرسانی وضعیت
                    OrderStatus stateId = (OrderStatus)Convert.ToInt16(OrderStateId);
                    var orderState = order.OrderStates.OrderByDescending(x => x.LogDate).Take(1).Where(x => x.state == stateId && x.OrderDeliveryId == OrderDeliveryId).FirstOrDefault();
                    DateTime LogDate = DateTime.Now;
                    if (orderState == null)
                    {
                        OrderState os = new OrderState()
                        {
                            OrderDeliveryId = OrderDeliveryId,
                            OrderId = new Guid(Id),
                            state = stateId,
                            LogDate = LogDate,
                            Description = !String.IsNullOrEmpty(description) ? description : "به روز رسانی سفارش در پنل مدیریت"
                        };

                        uow.OrderStateRepository.Insert(os);
                    }

                    //تایید سفارش
                    if (stateId == OrderStatus.تایید_سفارش)
                    {
                        var wallet = order.OrderWallets.First().Wallet;

                        var WalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 23 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                        if (WalletAttribute != null)
                            WalletAttribute.Value = ConfirmEstelam.ToString();
                        else
                        {
                            WalletAttributeWallet newAttributeConfirmEstelam = new WalletAttributeWallet()
                            {
                                OrderDeliveryId = OrderDeliveryId,
                                WalletId = wallet.Id,
                                WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 23).FirstOrDefault().Id,
                                Value = ConfirmEstelam.ToString()
                            };

                            uow.WalletAttributeWalletRepository.Insert(newAttributeConfirmEstelam);
                        }


                        uow.Save();

                        //سفارش استعلامی در ساعت غیراداری ثبت شده است؟
                        var dateTime = order.OrderStates.Last().LogDate.TimeOfDay;
                        short weekday = (short)order.OrderStates.Last().LogDate.PersionDayOfWeek();
                        if (!uow.ShoppingWorkTimeRepository.Get(x => x, x => x.SettingId == setting.Id && x.IsActive && x.WeekDay == weekday && x.StartTime < dateTime && x.EndTime > dateTime).Any())
                        {
                            order.OrderStates.Last().LogDate = DateTime.Now;

                            int hours = 0;
                            switch (order.OrderWallets.First().Wallet.PaymentType)
                            {
                                case 1: case 2: hours = order.OrderWallets.First().Wallet.BankAccount.OnliePaymentHours; break;
                                case 3: hours = order.OrderWallets.First().Wallet.BankAccount.CardNumberHours; break;
                                case 4: hours = order.OrderWallets.First().Wallet.BankAccount.HasCourierDeliveryPosHours; break;
                                case 5: hours = order.OrderWallets.First().Wallet.BankAccount.HasCourierDeliveryCashHours; break;
                                case 6: hours = order.OrderWallets.First().Wallet.BankAccount.HasFishHours; break;
                                default:
                                    break;
                            }
                            order.ExpireDate = DateTime.Now.AddHours(hours);
                            uow.Save();
                        }

                    }

                    else if (stateId == OrderStatus.عدم_تایید_سفارش)
                    {
                        var wallet = order.OrderWallets.First().Wallet;

                        var WalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 23 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                        if (WalletAttribute != null)
                            WalletAttribute.Value = ConfirmEstelam.ToString();
                        else
                        {
                            WalletAttributeWallet newAttributeConfirmEstelam = new WalletAttributeWallet()
                            {
                                OrderDeliveryId = OrderDeliveryId,
                                WalletId = wallet.Id,
                                WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 23).FirstOrDefault().Id,
                                Value = ConfirmEstelam.ToString()
                            };

                            uow.WalletAttributeWalletRepository.Insert(newAttributeConfirmEstelam);
                        }


                        uow.Save();
                    }

                    //ارسال شد
                    else if (stateId == OrderStatus.ارسال_شده)
                    {
                        if (!string.IsNullOrEmpty(trackingCode))
                        {
                            var tracking = uow.OrderAttributeOrderRepository.Get(x => x, x => x.OrderAttribute.DataType == 17 && x.OrderDeliveryId == OrderDeliveryId).FirstOrDefault();
                            if (tracking != null)
                            {
                                tracking.Value = trackingCode;
                            }
                            else
                            {
                                OrderAttributeOrder newAttributeTracking = new OrderAttributeOrder()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    OrderId = new Guid(Id),
                                    AttributeId = uow.OrderAttributeRepository.Get(x => x, x => x.DataType == 17).FirstOrDefault().Id,
                                    Value = trackingCode
                                };

                                uow.OrderAttributeOrderRepository.Insert(newAttributeTracking);
                            }
                        }

                        if (!string.IsNullOrEmpty(barbariNumber))
                        {

                            var barbari = uow.OrderAttributeOrderRepository.Get(x => x, x => x.OrderAttribute.DataType == 21 && x.OrderDeliveryId == OrderDeliveryId).FirstOrDefault();
                            if (barbari != null)
                            {
                                barbari.Value = barbariNumber;
                            }
                            else
                            {
                                OrderAttributeOrder newAttributeBarbari = new OrderAttributeOrder()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    OrderId = new Guid(Id),
                                    AttributeId = uow.OrderAttributeRepository.Get(x => x, x => x.DataType == 21).FirstOrDefault().Id,
                                    Value = barbariNumber
                                };

                                uow.OrderAttributeOrderRepository.Insert(newAttributeBarbari);
                            }

                        }
                        if (!string.IsNullOrEmpty(barbariNumber2))
                        {

                            var barbari = uow.OrderAttributeOrderRepository.Get(x => x, x => x.OrderAttribute.DataType == 27 && x.OrderDeliveryId == OrderDeliveryId).FirstOrDefault();
                            if (barbari != null)
                            {
                                barbari.Value = barbariNumber2;
                            }
                            else
                            {
                                OrderAttributeOrder newAttributeBarbari = new OrderAttributeOrder()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    OrderId = new Guid(Id),
                                    AttributeId = uow.OrderAttributeRepository.Get(x => x, x => x.DataType == 27).FirstOrDefault().Id,
                                    Value = barbariNumber2
                                };

                                uow.OrderAttributeOrderRepository.Insert(newAttributeBarbari);
                            }

                        }
                        if (!string.IsNullOrEmpty(barbariNumber3))
                        {

                            var barbari = uow.OrderAttributeOrderRepository.Get(x => x, x => x.OrderAttribute.DataType == 28 && x.OrderDeliveryId == OrderDeliveryId).FirstOrDefault();
                            if (barbari != null)
                            {
                                barbari.Value = barbariNumber3;
                            }
                            else
                            {
                                OrderAttributeOrder newAttributeBarbari = new OrderAttributeOrder()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    OrderId = new Guid(Id),
                                    AttributeId = uow.OrderAttributeRepository.Get(x => x, x => x.DataType == 28).FirstOrDefault().Id,
                                    Value = barbariNumber3
                                };

                                uow.OrderAttributeOrderRepository.Insert(newAttributeBarbari);
                            }

                        }
                    }

                    //تحویل داده شد
                    else if (stateId == OrderStatus.تحویل_داده_شده)
                    {
                        if (orderdelivery.RequestDate.HasValue)
                        {
                            if (LogDate.Date > orderdelivery.RequestDate.Value.Date)
                                orderdelivery.DeliveryState = DeliveryState.تحویل_با_تاخیر;
                            else
                                orderdelivery.DeliveryState = DeliveryState.تحویل_به_موقع;
                            uow.OrderDeliveryRepository.Update(orderdelivery);
                        }

                        if (order.OrderWallets.First().Wallet.PaymentType == 4 || order.OrderWallets.First().Wallet.PaymentType == 5)
                        {
                            OrderState os = new OrderState()
                            {
                                OrderDeliveryId = OrderDeliveryId,
                                OrderId = new Guid(Id),
                                state = OrderStatus.تایید_پرداخت,
                                LogDate = LogDate,
                                Description = !String.IsNullOrEmpty(description) ? description : "به روز رسانی سفارش در پنل مدیریت"
                            };
                            uow.OrderStateRepository.Insert(os);


                            var wallet = order.OrderWallets.First().Wallet;
                            if (wallet.PaymentType == 5)//پرداخت نقدی
                            {
                                var WalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 19 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                                if (WalletAttribute != null)
                                    WalletAttribute.Value = true.ToString();
                                else
                                {
                                    WalletAttributeWallet newAttributeConfirmCashPeik = new WalletAttributeWallet()
                                    {
                                        OrderDeliveryId = OrderDeliveryId,
                                        WalletId = wallet.Id,
                                        WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 19).FirstOrDefault().Id,
                                        Value = true.ToString()
                                    };

                                    uow.WalletAttributeWalletRepository.Insert(newAttributeConfirmCashPeik);
                                }
                            }
                            else// پرداخت با Pos
                            {
                                //ConfirmPos
                                var ConfirmPosWalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 20 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                                if (ConfirmPosWalletAttribute != null)
                                    ConfirmPosWalletAttribute.Value = true.ToString();
                                else
                                {
                                    WalletAttributeWallet newAttributeConfirmPosPeik = new WalletAttributeWallet()
                                    {
                                        OrderDeliveryId = OrderDeliveryId,
                                        WalletId = wallet.Id,
                                        WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 20).FirstOrDefault().Id,
                                        Value = true.ToString()
                                    };

                                    uow.WalletAttributeWalletRepository.Insert(newAttributeConfirmPosPeik);
                                }

                                //PosCodePeigiry
                                var PosCodePeigiryWalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 21 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                                if (PosCodePeigiryWalletAttribute != null)
                                    PosCodePeigiryWalletAttribute.Value = 11111111.ToString();
                                else
                                {
                                    WalletAttributeWallet newAttributePosCodePeigiry = new WalletAttributeWallet()
                                    {
                                        OrderDeliveryId = OrderDeliveryId,
                                        WalletId = wallet.Id,
                                        WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 21).FirstOrDefault().Id,
                                        Value = 11111111.ToString()
                                    };

                                    uow.WalletAttributeWalletRepository.Insert(newAttributePosCodePeigiry);
                                }
                                //PosDateTime
                                var PosDateTimeWalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 22 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                                if (PosDateTimeWalletAttribute != null)
                                    PosDateTimeWalletAttribute.Value = DateTime.Now.ToString();
                                else
                                {
                                    WalletAttributeWallet newAttributePosDateTime = new WalletAttributeWallet()
                                    {
                                        OrderDeliveryId = OrderDeliveryId,
                                        WalletId = wallet.Id,
                                        WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 22).FirstOrDefault().Id,
                                        Value = DateTime.Now.ToString()
                                    };

                                    uow.WalletAttributeWalletRepository.Insert(newAttributePosDateTime);
                                }

                            }

                            order.IsActive = true;
                            uow.OrderRepository.Update(order);

                            order.OrderWallets.First().Wallet.State = true;


                            uow.Save();

                        }
                    }

                    //تایید پرداخت
                    else if (stateId == OrderStatus.تایید_پرداخت && order.OrderWallets.First().Wallet.PaymentType > 3) // تایید پرداخت برای پرداخت به پیک
                    {
                        var wallet = order.OrderWallets.First().Wallet;

                        if (wallet.PaymentType == 5)//پرداخت نقدی
                        {
                            var WalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 19 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                            if (WalletAttribute != null)
                                WalletAttribute.Value = ConfirmCashPeik.ToString();
                            else
                            {
                                WalletAttributeWallet newAttributeConfirmCashPeik = new WalletAttributeWallet()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    WalletId = wallet.Id,
                                    WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 19).FirstOrDefault().Id,
                                    Value = ConfirmCashPeik.ToString()
                                };

                                uow.WalletAttributeWalletRepository.Insert(newAttributeConfirmCashPeik);
                            }
                        }
                        else// پرداخت با Pos
                        {
                            //ConfirmPos
                            var ConfirmPosWalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 20 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                            if (ConfirmPosWalletAttribute != null)
                                ConfirmPosWalletAttribute.Value = ConfirmPosPeik.ToString();
                            else
                            {
                                WalletAttributeWallet newAttributeConfirmPosPeik = new WalletAttributeWallet()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    WalletId = wallet.Id,
                                    WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 20).FirstOrDefault().Id,
                                    Value = ConfirmPosPeik.ToString()
                                };

                                uow.WalletAttributeWalletRepository.Insert(newAttributeConfirmPosPeik);
                            }

                            //PosCodePeigiry
                            var PosCodePeigiryWalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 21 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                            if (PosCodePeigiryWalletAttribute != null)
                                PosCodePeigiryWalletAttribute.Value = PosCodePeigiry.ToString();
                            else
                            {
                                WalletAttributeWallet newAttributePosCodePeigiry = new WalletAttributeWallet()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    WalletId = wallet.Id,
                                    WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 21).FirstOrDefault().Id,
                                    Value = PosCodePeigiry.ToString()
                                };

                                uow.WalletAttributeWalletRepository.Insert(newAttributePosCodePeigiry);
                            }
                            //PosDateTime
                            var PosDateTimeWalletAttribute = wallet.WalletAttributeWallets.Where(x => x.WalletAttribute.DataType == 22 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                            if (PosDateTimeWalletAttribute != null)
                                PosDateTimeWalletAttribute.Value = PosDateTime.ToString();
                            else
                            {
                                WalletAttributeWallet newAttributePosDateTime = new WalletAttributeWallet()
                                {
                                    OrderDeliveryId = OrderDeliveryId,
                                    WalletId = wallet.Id,
                                    WalletAttributeId = uow.WalletAttributeRepository.Get(x => x, x => x.DataType == 22).FirstOrDefault().Id,
                                    Value = PosDateTime.ToString()
                                };

                                uow.WalletAttributeWalletRepository.Insert(newAttributePosDateTime);
                            }

                        }

                        order.IsActive = true;
                        uow.OrderRepository.Update(order);

                        uow.Save();

                        result = ConfirmPeikPayment(wallet.Id, order.Id, wallet.PaymentType.Value);

                        UpdateBonCodeLog(order.Id);
                    }

                    //لغو سفارش
                    else if (Convert.ToInt32(stateId) > 6)
                    {
                        //if (order.IsActive)
                        //update quantity
                        uow.OrderRepository.CheckQuantity(order);
                        order.IsActive = false;
                        order.New = false;
                        uow.Save();
                    }
                    //توضیحات
                    if (!string.IsNullOrEmpty(description))
                    {
                        var orderAttribute = order.OrderAttributeSelects.Where(x => x.OrderAttribute.DataType == 19 && x.OrderDeliveryId == OrderDeliveryId).SingleOrDefault();
                        if (orderAttribute != null)
                            orderAttribute.Value = description;
                        else
                        {
                            OrderAttributeOrder newAttributeDescription = new OrderAttributeOrder()
                            {
                                OrderDeliveryId = OrderDeliveryId,
                                OrderId = new Guid(Id),
                                AttributeId = uow.OrderAttributeRepository.Get(x => x, x => x.DataType == 19).FirstOrDefault().Id,
                                Value = description
                            };

                            uow.OrderAttributeOrderRepository.Insert(newAttributeDescription);
                        }
                    }

                    uow.Save();
                    order.New = false;
                    uow.Save();

                    DateTime shoppingdate = order.OrderStates.Last().LogDate.AddMinutes(setting.ShoppingPayEstelamMinutes);
                    string shoppingtime = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(shoppingdate) + shoppingdate.ToString("HH:mm:ss");

                    await SendSms(order.CustomerOrderId, order.BankOrderId, order.User.FirstName, OrderStateId, trackingCode, barbariNumber, barbariNumber2, barbariNumber3, order.User.FirstName, order.User.LastName, order.User.PhoneNumber, setting.ShoppingPayEstelamMinutes.ToString(), orderdelivery.ProductSendWay.DeliverSelectable, sendwayName);
                    // await SendMail(OrderStateId, trackingCode, barbariNumber, barbariNumber2, barbariNumber3, order.User.FirstName, order.User.LastName, order.User.Email, shoppingtime);


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Orders", "Edit", false, 200, "ویرایش سفارش " + order.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    return RedirectToAction("Edit", new { id = order.Id, result = result });
                }

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Orders", "Edit", false, 500, "خطا در ویرایش سفارش" + order.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return RedirectToAction("Edit", new { id = order.Id, result = 4 });
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Orders", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public virtual async Task<JsonResult> ConfirmVariz(int WalletId, Guid OrderId)
        {
            try
            {
                var order = uow.OrderRepository.Get(x => x, x => x.Id == OrderId, null, "OrderAttributeSelects,OrderWallets,OrderDeliveries,OrderStates,OrderRows,User").FirstOrDefault();
                var wallet = uow.WalletRepository.GetByID(WalletId);


                wallet.State = true;


                //Add Wallet
                Wallet oWallet = new Wallet();
                oWallet.ForWhat = uow.ForWhatRepository.Get(x => x, x => x.ForWhatType == ForWhatType.پرداخت_سفارش).First();
                oWallet.UserId = User.Identity.GetUserId();
                oWallet.InsertDate = System.DateTime.Now;
                oWallet.State = true;
                oWallet.Price = wallet.Price;
                oWallet.DepositOrWithdrawal = false;
                oWallet.PaymentType = 1;
                oWallet.BankAccountId = wallet.BankAccountId;
                uow.WalletRepository.Insert(oWallet);
                uow.Save();
                order.OrderWallets.Add(new OrderWallet() { Wallet = oWallet });
                uow.Save();
                //Update Order State
                order.OrderStates.Add(new OrderState() { LogDate = System.DateTime.Now, state = OrderStatus.تایید_پرداخت });

                order.IsActive = true;
                uow.OrderRepository.Update(order);

                uow.Save();



                UpdateBonCodeLog(order.Id);

                #region Send Mail & SMS
                var oSetting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();

                SmsService sms = new SmsService();

                #region Send To User

                try
                {
                    string EmailBodyHtml = "";
                    CoreLib.ViewModel.Email.Template UseremailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید پرداخت سفارش", " کاربر گرامی ، سفارش شما با شماره پیگیری " + order.Id + " پرداخت شد.  می توانید از پروفایل کاربری خود ، وضعیت سفارش را پیگیری نمایید. ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                    EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", UseremailBody, this.ControllerContext);

                    EmailService esUser = new EmailService();
                    IdentityMessage imessage = new IdentityMessage();
                    imessage.Body = EmailBodyHtml;
                    imessage.Destination = order.User.Email;
                    imessage.Subject = " تایید پرداخت سفارش ";
                    await esUser.SendAsync(imessage);


                    var currentuser = order.User;
                    IdentityMessage iPhonemessage = new IdentityMessage();
                    iPhonemessage.Body = "سفارش شما با موفقیت پرداخت شد.";
                    iPhonemessage.Destination = currentuser.PhoneNumber;

                    await sms.SendSMSAsync(iPhonemessage);

                }
                catch (Exception)
                {

                }


                #endregion

                #endregion

                return Json(new
                {
                    Message = "تایید شد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public virtual async Task<JsonResult> ConfirmCardToCard(int WalletId, Guid OrderId)
        {
            try
            {
                var order = uow.OrderRepository.Get(x => x, x => x.Id == OrderId, null, "OrderAttributeSelects,OrderWallets,OrderDeliveries,OrderStates,OrderRows,User").FirstOrDefault();
                var wallet = uow.WalletRepository.GetByID(WalletId);
                wallet.State = true;

                //Add Wallet
                Wallet oWallet = new Wallet();
                oWallet.ForWhat = uow.ForWhatRepository.Get(x => x, x => x.ForWhatType == ForWhatType.پرداخت_سفارش).First();
                oWallet.UserId = User.Identity.GetUserId();
                oWallet.InsertDate = System.DateTime.Now;
                oWallet.State = true;
                oWallet.Price = wallet.Price;
                oWallet.DepositOrWithdrawal = false;
                oWallet.PaymentType = 2;
                oWallet.BankAccountId = wallet.BankAccountId;
                uow.WalletRepository.Insert(oWallet);
                uow.Save();
                order.OrderWallets.Add(new OrderWallet() { Wallet = oWallet });
                uow.Save();
                //Update Order State
                foreach (var item in order.OrderDeliveries)
                {
                    order.OrderStates.Add(new OrderState() { OrderDeliveryId = item.Id, LogDate = System.DateTime.Now, state = OrderStatus.تایید_پرداخت });
                }


                order.IsActive = true;
                uow.OrderRepository.Update(order);

                uow.Save();


                UpdateBonCodeLog(order.Id);

                #region Send Mail & SMS

                var oSetting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                SmsService sms = new SmsService();

                #region Send To User

                try
                {
                    try
                    {
                        string EmailBodyHtml = "";
                        CoreLib.ViewModel.Email.Template UseremailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید پرداخت سفارش", " کاربر گرامی ، سفارش شما با شماره پیگیری " + order.Id + " پرداخت شد.  می توانید از پروفایل کاربری خود ، وضعیت سفارش را پیگیری نمایید. ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                        EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", UseremailBody, this.ControllerContext);

                        EmailService esUser = new EmailService();
                        IdentityMessage imessage = new IdentityMessage();
                        imessage.Body = EmailBodyHtml;
                        imessage.Destination = order.User.Email;
                        imessage.Subject = " تایید پرداخت سفارش ";
                        await esUser.SendAsync(imessage);
                    }
                    catch (Exception)
                    {
                    }




                }
                catch (Exception)
                {

                }

                try
                {
                    var currentuser = order.User;
                    IdentityMessage iPhonemessage = new IdentityMessage();
                    iPhonemessage.Body = "سفارش شما با موفقیت پرداخت شد.";
                    iPhonemessage.Destination = currentuser.PhoneNumber;

                    await sms.SendAsync(iPhonemessage);
                }
                catch (Exception)
                {

                }

                #endregion

                #endregion

                return Json(new
                {
                    Message = "تایید شد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        protected int ConfirmPeikPayment(int WalletId, Guid OrderId, int PaymentType)
        {
            try
            {
                var order = uow.OrderRepository.Get(x => x, x => x.Id == OrderId, null, "OrderAttributeSelects,OrderWallets,OrderDeliveries,OrderStates,OrderRows,User").FirstOrDefault();
                var wallet = uow.WalletRepository.GetByID(WalletId);
                wallet.State = true;


                //Add Wallet
                Wallet oWallet = new Wallet();
                oWallet.ForWhat = uow.ForWhatRepository.Get(x => x, x => x.ForWhatType == ForWhatType.پرداخت_سفارش).First();
                oWallet.UserId = User.Identity.GetUserId();
                oWallet.InsertDate = System.DateTime.Now;
                oWallet.State = true;
                oWallet.Price = wallet.Price;
                oWallet.DepositOrWithdrawal = false;
                oWallet.PaymentType = PaymentType;
                oWallet.BankAccountId = wallet.BankAccountId;
                uow.WalletRepository.Insert(oWallet);
                uow.Save();
                order.OrderWallets.Add(new OrderWallet() { Wallet = oWallet });
                uow.Save();
                //Update Order State
                order.OrderStates.Add(new OrderState() { LogDate = System.DateTime.Now, state = OrderStatus.تایید_پرداخت });
                uow.Save();

                #region Send Mail & SMS

                var oSetting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                SmsService sms = new SmsService();

                #region Send To User

                try
                {
                    string EmailBodyHtml = "";
                    CoreLib.ViewModel.Email.Template UseremailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید پرداخت سفارش", " کاربر گرامی ، سفارش شما با شماره پیگیری " + order.Id + " پرداخت شد.  می توانید از پروفایل کاربری خود ، اطلاعات کامل سفارش خود را ببینید. ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                    EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", UseremailBody, this.ControllerContext);

                    EmailService esUser = new EmailService();
                    IdentityMessage imessage = new IdentityMessage();
                    imessage.Body = EmailBodyHtml;
                    imessage.Destination = order.User.Email;
                    imessage.Subject = " تایید پرداخت سفارش ";
                    esUser.SendAsync(imessage);


                    var currentuser = order.User;
                    IdentityMessage iPhonemessage = new IdentityMessage();
                    iPhonemessage.Body = "سفارش شما با موفقیت پرداخت شد.";
                    iPhonemessage.Destination = currentuser.PhoneNumber;

                    sms.SendSMSAsync(iPhonemessage);

                }
                catch (Exception)
                {

                }


                #endregion

                #endregion


            }
            catch (Exception)
            {
                return 4;
            }
            return 3;
        }

        private async Task SendMail(string state, string trackingCode, string barbariNumber, string barbariNumber2, string barbariNumber3, string userName, string userLastName, string email, string ShoppingTime)
        {
            try
            {

                #region SendMail
                EmailService es = new EmailService();

                IdentityMessage imessage = new IdentityMessage();
                string EmailBodyHtml = "";

                var oSetting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                string siteUrl = "http://" + HttpContext.Request.Url.Host;
                string title = "";
                string body = "سفارش شما تحویل داده شد.";
                if (state == "1")
                {
                    title = "تایید سفارش شما";
                    body = "سفارش شما تایید شد. مهلت پرداخت تا  " + ShoppingTime;
                }
                else if (state == "2")
                {
                    title = "پرداخت سفارش شما";
                    body = "پرداخت سفارش شما انجام شد";
                }
                else if (state == "3")
                {
                    title = "پردازش سفارش شما";
                    body = "سفارش شما در انبار مورد پردازش قرار گرفت";
                }
                else if (state == "4")
                {
                    title = "ارسال سفارش شما";
                    body = "سفارش شما آماده ارسال است";
                }
                else if (state == "5")
                {
                    title = "ارسال سفارش شما";
                    body = " سفارش شما ارسال شد . ";
                    if (!string.IsNullOrEmpty(trackingCode))
                        body += " نحوه پیگیری سفارش:  " + trackingCode;
                    if (!string.IsNullOrEmpty(barbariNumber))
                        body += " تلفن پیگیری سفارش:  " + barbariNumber;
                    if (!string.IsNullOrEmpty(barbariNumber3))
                        body += " شماره بارنامه:  " + barbariNumber3;
                    if (!string.IsNullOrEmpty(barbariNumber2))
                        body += " تاریخ ارسال سفارش:  " + barbariNumber2;
                }
                else if (state == "6")
                {
                    title = "تحویل سفارش شما";
                    body = "سفارش به دست شما رسید ";
                }
                else if (state == "7")
                {
                    title = "لغو سفارش شما";
                    body = "سفارش شما لغو شد";
                }
                else if (state == "8")
                {
                    title = "مرجوع شدن سفارش شما";
                    body = "سفارش شما مرجوع شد";
                }

                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, title, "کاربر گرامی ، " + userName + " " + userLastName + "، " + body + " لینک سایت : <br/> <a href='" + siteUrl + "'>" + siteUrl + "</a>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                #endregion

                imessage.Body = EmailBodyHtml;
                imessage.Destination = email;
                imessage.Subject = title;
                await es.SendAsync(imessage);
            }
            catch (Exception)
            {

            }
        }

        private async Task SendSms(string CustomerOrderId, long bankOrderId, string name, string state, string trackingCode, string barbariNumber, string barbariNumber2, string barbariNumber3, string userName, string userLastName, string phoneNumber, string ShoppingTime, bool DeliverSelectable, string sendwayName)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "FactorAttachment,attachment,Faviconattachment,Province,City").SingleOrDefault();

            SmsService ss = new SmsService();
            IdentityMessage iMessage = new IdentityMessage();
            iMessage.Destination = phoneNumber;
            try
            {
                if (state == "1")
                {
                    iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "درخواست استعلام موجودی کالاهای انتخاب شده شما ، تایید شد.\n" + " شما میتوانید از طریق نشانی زیر اقدام به پرداخت و تکمیل سفارش خود نمایید :\n" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId + "\n" + "مهلت پرداخت تا :\n" + ShoppingTime + " " + " دقیقه";
                    await ss.SendSMSAsync(iMessage, "NewEstelamConfirm", ShoppingTime, null, null, name, ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId);
                }
                else if (state == "14")
                {
                    iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "با عرض پوزش ؛ به دلیل عدم موجودی کافی ، درخواست استعلام سفارش شما با شماره پیگیری " + CustomerOrderId + "مورد تایید نیست .";
                    await ss.SendSMSAsync(iMessage, "NewEstelamDecline", CustomerOrderId, null, null, name, null);
                }
                else if (state == "2")
                {
                    iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "سفارش شما هم اکنون در وضعیت : " + OrderStatus.تایید_پرداخت.EnumDisplayNameFor() + " " + "قرار گرفت.\n" + "شما می توانید سفارش خود را از طریق لینک زیر پیگیری بفرمایید :\n" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId;
                    await ss.SendSMSAsync(iMessage, "NewOrderStateChange", ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId, null, null, name, OrderStatus.تایید_پرداخت.EnumDisplayNameFor());
                }
                else if (state == "3")
                {
                    iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "سفارش شما هم اکنون در وضعیت : " + OrderStatus.پردازش_انبار.EnumDisplayNameFor() + " " + "قرار گرفت.\n" + "شما می توانید سفارش خود را از طریق لینک زیر پیگیری بفرمایید :\n" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId;
                    await ss.SendSMSAsync(iMessage, "NewOrderStateChange", ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId, null, null, name, OrderStatus.پردازش_انبار.EnumDisplayNameFor());
                }
                else if (state == "4")
                {
                    iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "سفارش شما هم اکنون در وضعیت : " + OrderStatus.آماده_ارسال.EnumDisplayNameFor() + " " + "قرار گرفت.\n" + "شما می توانید سفارش خود را از طریق لینک زیر پیگیری بفرمایید :\n" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId;
                    await ss.SendSMSAsync(iMessage, "NewOrderStateChange", ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId, null, null, name, OrderStatus.آماده_ارسال.EnumDisplayNameFor());
                }
                else if (state == "5")
                {
                    if (DeliverSelectable)
                    {
                        iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "با تشکر از انتخاب تان ، \n" + "سفارش شما از طریق: " + sendwayName + " ارسال گردید .\n" + "ارسال گردید .این سفارش تا ساعاتی دیگر به دست تان می رسد.\n" + "شماره سفارش : " + CustomerOrderId;
                        await ss.SendSMSAsync(iMessage, "NewSendOrderPeik", CustomerOrderId, null, null, name, sendwayName);
                    }
                    else
                    {
                        iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "با تشکر از انتخاب تان ، \n" + "سفارش شما از طریق: " + sendwayName + " ارسال گردید .\n" + "ارسال گردید .این سفارش تا ساعاتی دیگر به دست تان می رسد.\n" + "شماره پیگیری : " + barbariNumber3 + "\n" + "شماره سفارش : " + CustomerOrderId + "\n" + "شما می توانید سفارش خود را از طریق لینک زیر پیگیری بفرمایید : \n" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId;
                        await ss.SendSMSAsync(iMessage, "NewSendOrderPost", ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Detail/" + CustomerOrderId, CustomerOrderId, barbariNumber3, name, sendwayName);
                    }

                }
                else if (state == "6")
                {
                    iMessage.Body = setting.WebSiteName + "\n" + name + " " + "عزیز\n" + "با تشکر از خرید شما ؛ \n" + "لطفا از طریق نشانی زیر نظرها و تجربه خود را درباره استفاده از کالاهای خریداری شده بیان کنید.\n" + "با تشکر\n" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Rate/" + CustomerOrderId;
                    await ss.SendSMSAsync(iMessage, "NewRequestProductComment", ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/Profile/Rate/" + CustomerOrderId, null, null, name);

                }
                else if (state == "7")
                {
                }
                else if (state == "8")
                {
                }

            }
            catch (Exception)
            {

            }
        }


        public virtual JsonResult UpdateOrderRow(int OrderRowId, long Price, int Quantity)
        {
            try
            {

                var orderrow = uow.OrderRowRepository.Get(x => x, x => x.Id == OrderRowId, null, "Order,Order.OrderStates,Order.OrderAttributeSelects,order.OrderRows,order.OrderWallets").FirstOrDefault();

                var order = orderrow.Order;

                if (order.OrderStates.Any(x => x.state == OrderStatus.تایید_پرداخت))
                {

                    return Json(new
                    {
                        Message = "قابلیت تغییر سفارشی که تایید پراخت شده است، وجود ندارد",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }

                orderrow.Price = Price;
                orderrow.Quantity = Quantity;
                uow.OrderRowRepository.Update(orderrow);
                uow.Save();

                //Add Order Attribute
                order.OrderAttributeSelects = uow.OrderAttributeOrderRepository.Get(x => x, x => x.OrderId == order.Id).ToList();

                var SendPriceValue = ahmadi.Infrastructure.Cart.Delivery.CalculateDeliveryPrice().ToString();
                var SendPrice = order.OrderAttributeSelects.Where(x => x.OrderAttribute.DataType == 14).FirstOrDefault();
                SendPrice.Value = SendPriceValue;
                uow.OrderAttributeOrderRepository.Update(SendPrice);

                var ValueAddedValue = ahmadi.Infrastructure.Cart.Price.CalculateValueAdded().ToString();
                var ValueAdded = order.OrderAttributeSelects.Where(x => x.OrderAttribute.DataType == 15).FirstOrDefault();
                ValueAdded.Value = ValueAddedValue;
                uow.OrderAttributeOrderRepository.Update(ValueAdded);

                long SumFinalPrice = Convert.ToInt64(SendPrice.Value) + Convert.ToInt64(ValueAdded.Value);
                foreach (var item in order.OrderRows)
                {
                    SumFinalPrice += item.Quantity * item.Price;
                }

                order.OrderWallets.First().Wallet.Price = SumFinalPrice;

                uow.OrderRepository.Update(order);
                uow.Save();


                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Report(string id, string DeliveryId, string report)
        {
            if (String.IsNullOrEmpty(id) || String.IsNullOrEmpty(report))
                return Redirect("~/Orders");
            TempData["orderId"] = id;
            TempData["DeliveryId"] = DeliveryId;
            return View(report);
        }

        //public ActionResult GetFactorReport()
        //{
        //    var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment,Province,City").SingleOrDefault();
        //    XMLReader readXml = new XMLReader(setting.StaticContentDomain);

        //    StiReport report = new StiReport();

        //    report.Load(Server.MapPath("~/Content/Reports/FactorReport.mrt"));

        //    int orderDeliverId = Convert.ToInt32(TempData["orderDeliverId"].ToString());
        //    var order = uow.OrderDeliveryRepository.Get(x => new { x.Order.BankOrderId, x.Order.InsertDate, x.UserAddress, x.UserAddressId, x.Order.User, x.OrderRows, x.Order.OrderWallets, ProductSendWay = x.ProductSendWay.Title, x.Order.OrderAttributeSelects }, x => x.Id == orderDeliverId,
        //        null, "UserAddress,order.User.CityEntity.Province,order.OrderWallets.Wallet,order.OrderAttributeSelects.OrderAttribute,order.OrderRows"
        //        ).First();
        //    Admin.ViewModels.Report.CustomerOrderInfo CustomerOrderInfo = new ViewModels.Report.CustomerOrderInfo()
        //    {
        //        InsertDate = DateTimeConverter.ChangeMiladiToShamsi(DateTime.Now),
        //        Logo = setting.attachment.FileName,
        //        SerialNumber = 147 + order.BankOrderId,
        //        ShoppingName = setting.WebSiteName,
        //        ShoppingProvience = setting.Province.Name,
        //        ShoppingCity = setting.City.Name,
        //        ShoppingAddress = setting.Address,
        //        ShoppingPostalCode = setting.PostalCode,
        //        ShoppingTele = setting.Tele,
        //        ShoppingTaxNumber = setting.TaxNumber,
        //        CustomerName = order.UserAddressId.HasValue ? order.UserAddress.FullName : order.User.FirstName + " " + order.User.LastName,
        //        CustomerProvience = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Province.Name : order.User.CityEntity.Province.Name,
        //        CustomerCity = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Name : order.User.CityEntity.Name,
        //        CustomerAddress = order.UserAddressId.HasValue ? order.UserAddress.Address : order.User.Address,
        //        CustomerPostalCode = order.UserAddressId.HasValue ? order.UserAddress.PostalCode : order.User.PostalCode,
        //        CustomerTele = order.UserAddressId.HasValue ? order.UserAddress.PhoneNumber : order.User.PhoneNumber,
        //        CustomerTaxNumber = "",
        //        ShoppingOrderId = "TF-" + order.BankOrderId,
        //        ShoppingPayWay = order.OrderWallets.First().Wallet.PaymentType == 1 ? "پرداخت نقدی" : order.OrderWallets.First().Wallet.PaymentType == 2 ? "کارت به کارت" : order.OrderWallets.First().Wallet.PaymentType == 3 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 4 ? "پرداخت آنلاین غیرمستقیم" : order.OrderWallets.First().Wallet.PaymentType == 5 ? "پرداخت به پیک نقدی" : order.OrderWallets.First().Wallet.PaymentType == 6 ? "پرداخت به پیک کارتخوان" : "---",
        //        ShoppingSenWay = order.ProductSendWay,
        //        ShoppingUserDescr = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 19) ? order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 19).First().Value : "---",
        //        ShoppingSenWayPrice = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 14) ? Convert.ToInt64(order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 14).First().Value) * 10 : 0,
        //        ShoppingTotalPrice = order.OrderWallets.First().Wallet.Price * 10
        //    };
        //    var CustomerOrderRows = new List<ViewModels.Report.CustomerOrderRow>();
        //    int i = 1;
        //    foreach (var item in order.OrderRows)
        //    {
        //        CustomerOrderRows.Add(new CustomerOrderRow()
        //        {
        //            Id = i,
        //            Code = item.ProductPrice.code,
        //            Name = item.Product.Title,
        //            RawPrice = item.RawPrice * 10,
        //            SumPrice = (item.RawPrice * item.Quantity) * 10,
        //            Quantity = item.Quantity,
        //            TaxPrice = (item.RawPrice - item.ProductPrice.Price) * item.Quantity * 10,
        //            OffPrice = Math.Abs(item.RawPrice - item.Price) * item.Quantity * 10,
        //            Price = (item.Price - (item.RawPrice - item.ProductPrice.Price)) * item.Quantity * 10,
        //            FinalPrice = item.Price * item.Quantity * 10

        //        });
        //        i++;
        //    }


        //    var img = new System.Drawing.Bitmap(Server.MapPath("~/Content/UploadFiles/" + CustomerOrderInfo.Logo));
        //    byte[] array1 = imageToByteArray(img);
        //    MemoryStream ms = new MemoryStream(array1);
        //    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

        //    report.Dictionary.Variables.Add("Logo", image);

        //    report.RegBusinessObject("Order", CustomerOrderInfo);
        //    report.RegBusinessObject("CustomerOrderRow", CustomerOrderRows);
        //    return StiMvcViewer.GetReportResult(report);

        //}
        public ActionResult GetFactorReport()
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "FactorAttachment,attachment,Faviconattachment,Province,City").SingleOrDefault();
            XMLReader readXml = new XMLReader(setting.StaticContentDomain);

            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Content/Reports/FactorReport.mrt"));

            Guid orderId = new Guid(TempData["orderId"].ToString());
            var order = uow.OrderRepository.Get(x => new { x.CustomerOrderId, x.BankOrderId, x.InsertDate, x.OrderDeliveries.First().UserAddress, x.OrderDeliveries.First().UserAddressId, x.User, x.OrderRows, x.OrderWallets, ProductSendWay = x.OrderDeliveries.First().ProductSendWay.Title, x.OrderAttributeSelects }, x => x.Id == orderId,
                null, "OrderDeliveries.UserAddress.CityEntity.Province,OrderDeliveries.ProductSendWay,User.CityEntity.Province,OrderWallets.Wallet,OrderAttributeSelects.OrderAttribute,OrderRows,OrderRows.ProductPrice.Product,OrderRows.ProductPrice.ProductAttributeSelectColor,OrderRows.ProductPrice.ProductAttributeSelectSize,OrderRows.ProductPrice.ProductAttributeSelectModel,OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").First();
            Admin.ViewModels.Report.CustomerOrderInfo CustomerOrderInfo = new ViewModels.Report.CustomerOrderInfo()
            {
                InsertDate = DateTimeConverter.ChangeMiladiToShamsi(DateTime.Now),
                Logo = setting.FactorAttachment.FileName,
                SerialNumber = order.CustomerOrderId,
                ShoppingName = setting.WebSiteName,
                ShoppingProvience = setting.Province.Name,
                ShoppingCity = setting.City.Name,
                ShoppingAddress = setting.Address,
                ShoppingPostalCode = setting.PostalCode,
                ShoppingTele = setting.Tele,
                ShoppingTaxNumber = setting.TaxNumber,
                CustomerName = order.UserAddressId.HasValue ? order.UserAddress.FullName : order.User.FirstName + " " + order.User.LastName,
                CustomerProvience = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Province.Name : order.User.CityEntity.Province.Name,
                CustomerCity = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Name : order.User.CityEntity.Name,
                CustomerAddress = order.UserAddressId.HasValue ? string.Format("{0}{1}{2}", order.UserAddress.Address, (!String.IsNullOrEmpty(order.UserAddress.AddressNumber) ? " ، پلاک" + order.UserAddress.AddressNumber : ""), (!String.IsNullOrEmpty(order.UserAddress.AddressUnit) ? " ، واحد" + order.UserAddress.AddressUnit : "")) : string.Format("{0}{1}{2}", order.User.Address, (!String.IsNullOrEmpty(order.User.AddressNumber) ? " ، پلاک" + order.User.AddressNumber : ""), (!String.IsNullOrEmpty(order.User.AddressUnit) ? " ، واحد" + order.User.AddressUnit : "")),
                CustomerPostalCode = order.UserAddressId.HasValue ? order.UserAddress.PostalCode : order.User.PostalCode,
                CustomerTele = order.UserAddressId.HasValue ? order.UserAddress.PhoneNumber : order.User.PhoneNumber,
                CustomerTaxNumber = "",
                ShoppingOrderId = "TF-" + order.BankOrderId,
                ShoppingPayWay = order.OrderWallets.First().Wallet.PaymentType == 1 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 2 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 3 ? "کارت به کارت" : order.OrderWallets.First().Wallet.PaymentType == 4 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 5 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 6 ? "فیش نقدی" : "---",
                ShoppingSenWay = order.ProductSendWay,
                ShoppingUserDescr = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 18) ? order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 18).First().Value : "---",
                ShoppingSenWayPrice = order.OrderAttributeSelects.Any(s => s.OrderAttribute.DataType == 14) ? Convert.ToInt64(order.OrderAttributeSelects.Where(s => s.OrderAttribute.DataType == 14).First().Value) * 10 : 0,
                ShoppingTotalPrice = order.OrderWallets.First().Wallet.Price * 10
            };
            var CustomerOrderRows = new List<ViewModels.Report.CustomerOrderRow>();
            int i = 1;
            foreach (var item in order.OrderRows)
            {
                string name = item.Product.Name;
                if (item.ProductPrice.ProductAttributeSelectModelId.HasValue)
                {
                    name += " مدل " + item.ProductPrice.ProductAttributeSelectModel.Value;
                }
                if (item.ProductPrice.ProductAttributeSelectSizeId.HasValue)
                    name += " سایز " + item.ProductPrice.ProductAttributeSelectSize.Value + item.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                if (item.ProductPrice.ProductAttributeSelectColorId.HasValue)
                    name += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(item.ProductPrice.ProductAttributeSelectColorId).Value)).Color;

                CustomerOrderRows.Add(new CustomerOrderRow()
                {
                    Id = i,
                    Code = item.ProductPrice.code,
                    Name = name,
                    RawPrice = item.RawPrice * 10,
                    SumPrice = (item.RawPrice * item.Quantity) * 10,
                    Quantity = item.Quantity,
                    TaxPrice = item.taxValue * 10,
                    OffPrice = Math.Abs(item.RawPrice - item.Price) * item.Quantity * 10,
                    Price = (item.RawPrice - (item.RawPrice - item.Price)) * item.Quantity * 10,
                    FinalPrice = ((item.Price * item.Quantity) + item.taxValue) * 10

                });
                i++;
            }


            var img = new System.Drawing.Bitmap(Server.MapPath("~/Content/UploadFiles/" + CustomerOrderInfo.Logo));
            byte[] array1 = imageToByteArray(img);
            MemoryStream ms = new MemoryStream(array1);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms);

            report.Dictionary.Variables.Add("Logo", image);

            report.RegBusinessObject("Order", CustomerOrderInfo);
            report.RegBusinessObject("CustomerOrderRow", CustomerOrderRows);
            return StiMvcViewer.GetReportResult(report);

        }


        public ActionResult GetLabelReport()
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "FactorAttachment,attachment,Faviconattachment,Province,City").SingleOrDefault();
            XMLReader readXml = new XMLReader(setting.StaticContentDomain);

            Guid orderId = new Guid(TempData["orderId"].ToString());
            int DeliveryId = Convert.ToInt32(TempData["DeliveryId"].ToString());
            var order = uow.OrderDeliveryRepository.Get(x => new { x.Insurance, x.RequestDate, x.ProductSendWayWorkTimeId, x.ProductSendWayWorkTime, x.labelIcons, x.Order.BankOrderId, x.Order.InsertDate, x.UserAddress, x.UserAddressId, x.Order.User, x.OrderRows, x.Order.OrderWallets, ProductSendWay = x.ProductSendWay.Title, x.Order.OrderAttributeSelects }, x => x.Id == DeliveryId,
                null, "UserAddress.CityEntity.Province,order.User.CityEntity.Province,order.OrderWallets.Wallet,order.OrderAttributeSelects.OrderAttribute,order.OrderRows,ProductSendWayWorkTime,labelIcons,ProductSendWay"
                ).First();

            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Content/Reports/labelReport" + order.labelIcons.Count + ".mrt"));



            Admin.ViewModels.Report.LabelReport LabelReport = new ViewModels.Report.LabelReport()
            {
                Logo = setting.FactorAttachment.FileName,
                WebsiteName = setting.WebSiteName,
                WebsiteAddress = setting.Address,
                WebsitePhoneNumber = setting.Tele,
                CustomerFullName = order.UserAddressId.HasValue ? order.UserAddress.FullName : order.User.FirstName + " " + order.User.LastName,
                CustomerProvience = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Province.Name : order.User.CityEntity.Province.Name,
                CustomerCity = order.UserAddressId.HasValue ? order.UserAddress.CityEntity.Name : order.User.CityEntity.Name,
                CustomerAddress = order.UserAddressId.HasValue ? string.Format("{0}{1}{2}", order.UserAddress.Address, (!String.IsNullOrEmpty(order.UserAddress.AddressNumber) ? " ، پلاک" + order.UserAddress.AddressNumber : ""), (!String.IsNullOrEmpty(order.UserAddress.AddressUnit) ? " ، واحد" + order.UserAddress.AddressUnit : "")) : string.Format("{0}{1}{2}", order.User.Address, (!String.IsNullOrEmpty(order.User.AddressNumber) ? " ، پلاک" + order.User.AddressNumber : ""), (!String.IsNullOrEmpty(order.User.AddressUnit) ? " ، واحد" + order.User.AddressUnit : "")),
                CustomerPostalCode = order.UserAddressId.HasValue ? order.UserAddress.PostalCode : order.User.PostalCode,
                CustomerTele = order.UserAddressId.HasValue ? order.UserAddress.PostalCode : order.User.LandlinePhone,
                CustomerMobile = order.UserAddressId.HasValue ? order.UserAddress.PhoneNumber : order.User.PhoneNumber,
                OrderPayWay = order.OrderWallets.First().Wallet.PaymentType == 1 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 2 ? "پرداخت آنلاین" : order.OrderWallets.First().Wallet.PaymentType == 3 ? "کارت به کارت" : order.OrderWallets.First().Wallet.PaymentType == 4 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 5 ? "پرداخت به پیک" : order.OrderWallets.First().Wallet.PaymentType == 6 ? "فیش نقدی" : "---",
                OrderSendWay = order.ProductSendWay + (order.Insurance ? " بیمه دارد " : ""),
                OrderDate = order.RequestDate.HasValue ? CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(order.RequestDate.Value) : "",
                OrderTime = order.ProductSendWayWorkTimeId.HasValue ? String.Format("{0}{1}{2}", order.ProductSendWayWorkTime.StartTime.ToString(@"hh\:mm"), " ---- ", order.ProductSendWayWorkTime.EndTime.ToString(@"hh\:mm")) : null
            };

            {
                var img = new System.Drawing.Bitmap(Server.MapPath("~/Content/UploadFiles/" + LabelReport.Logo));
                byte[] array1 = imageToByteArray(img);
                MemoryStream ms = new MemoryStream(array1);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                report.Dictionary.Variables.Add("Logo", image);

            }
            var label1 = order.labelIcons.FirstOrDefault();
            if (label1 != null)
            {

                var img = new System.Drawing.Bitmap(Server.MapPath("~/Content/UploadFiles/" + label1.attachment.FileName));
                byte[] array1 = imageToByteArray(img);
                MemoryStream ms = new MemoryStream(array1);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                report.Dictionary.Variables.Add("label1", image);
            }

            var label2 = order.labelIcons.Skip(1).FirstOrDefault();
            if (label2 != null)
            {

                var img = new System.Drawing.Bitmap(Server.MapPath("~/Content/UploadFiles/" + label2.attachment.FileName));
                byte[] array1 = imageToByteArray(img);
                MemoryStream ms = new MemoryStream(array1);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                report.Dictionary.Variables.Add("label2", image);
            }


            report.RegBusinessObject("labelbill", LabelReport);
            return StiMvcViewer.GetReportResult(report);

        }

        private void UpdateBonCodeLog(Guid OrderId)
        {
            //update codegift log
            var UserCodeGiftLog = uow.UserCodeGiftLogRepository.Get(x => x, x => x.OrderId == OrderId, null, "Order");
            foreach (var item in UserCodeGiftLog)
            {
                item.state = true;
                uow.UserCodeGiftLogRepository.Update(item);
            }
            uow.Save();
            //update Generalcodegift log
            var GeneralCodeGiftLog = uow.GeneralCodeGiftLogRepository.Get(x => x, x => x.OrderId == OrderId, null, "Order");
            foreach (var item in GeneralCodeGiftLog)
            {
                item.state = true;
                uow.GeneralCodeGiftLogRepository.Update(item);
            }
            uow.Save();

            //update bon log
            var UserBonLog = uow.UserBonLogRepository.Get(x => x, x => x.OrderId == OrderId);
            foreach (var item in UserBonLog)
            {
                item.state = true;
                uow.UserBonLogRepository.Update(item);
            }
            uow.Save();

            //update user bon
            string userid = User.Identity.GetUserId();
            var UserBon = uow.UserBonRepository.Get(x => x, x => x.OrderId == OrderId && x.UserId == userid).FirstOrDefault();
            if (UserBon != null)
            {
                UserBon.state = true;
                uow.UserBonRepository.Update(UserBon);
                uow.Save();
            }
        }


        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }

    }
}

