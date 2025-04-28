using CoreLib.Infrastructure.DateTime;
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
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Quartz.Impl;
using Quartz;
using CoreLib;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class OffersController : Controller
    {
        private ApplicationUserManager _userManager;

        private UnitOfWorkClass uow;

        public OffersController()
        {
            uow = new UnitOfWorkClass();
        }
        public OffersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #region Offers

        // GET: Admin/Offers
        public ActionResult Index(int? PageSize, int? pgsize, string offerId, string OfferIdFilter, int? SearchType, string sortOrder, string NameString2, string NameFilter2, string NameString, string NameFilter, string IsActive, string IsActiveFilter, string Expire, string ExpireFilter, string state, string stateFilter, string CodeType, string CodeTypeFilter, string LanguagenameString, string LanguagenameFilter, string InsertDateStart, string InsertDateStartFilter, string InsertDateEnd, string InsertDateEndFilter, string StartDateStart, string StartDateStartFilter, string StartDateEnd, string StartDateEndFilter, string ExpireDateStart, string ExpireDateStartFilter, string ExpireDateEnd, string ExpireDateEndFilter, int? page, int? pg)
        {

            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 24);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    if (SearchType.HasValue)
                    {
                        ViewBag.SearchType = SearchType.Value;
                    }
                    else
                        ViewBag.SearchType = 1;

                    #region ProductOffers
                    #region Load DropDown List

                    ViewBag.ProductStateList = uow.ProductStateRepository.Get(x => x);


                    #endregion

                    #region search

                    if (string.IsNullOrEmpty(offerId))
                        offerId = OfferIdFilter;
                    else
                        pg = 1;
                    if (string.IsNullOrEmpty(NameString2))
                        NameString2 = NameFilter2;
                    else
                        pg = 1;

                    ViewBag.OfferIdFilter = offerId;
                    ViewBag.NameFilter2 = NameString2;
                    ViewBag.offerList = new SelectList(uow.OfferRepository.Get(x => new { x.Id, x.Title }, x => x.CodeTypeValueCode == 1 || x.CodeTypeValueCode == 2), "Id", "Title", offerId);

                    int pageSize2 = 10;
                    if (pgsize.HasValue)
                    {
                        if (pgsize.Value > 100)
                            pageSize2 = 100;
                        else if (pgsize < 10)
                            pageSize2 = 10;
                        else
                            pageSize2 = pgsize.Value;
                    }
                    ViewBag.pgsize = pageSize2;
                    int pageNumber2 = (pg ?? 1);


                    var ProductPrices = uow.ProductPriceRepository.GetQueryList().AsNoTracking().Include("ProductImages").Include("ProductImages.Image").Include("Product.User").Include("Product").Include("Product.ProductImages.Image").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectSize").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectweight").Include("Product.ProductCategories").Include("ProductState").Include("ProductOffers").Where(x => (x.ProductStateId == 1 || x.ProductStateId == 2) && x.ProductOffers.Any(s => s.Offer.IsDeleted == false));



                    if (!String.IsNullOrEmpty(offerId))
                    {
                        int offid = Convert.ToInt32(offerId);
                        ProductPrices = ProductPrices.Where(s => s.ProductOffers.Any(a => a.OfferId == offid));
                    }
                    if (!String.IsNullOrEmpty(NameString2))
                    {
                        var color = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.Color.Contains(NameString2)).FirstOrDefault();
                        string colorId = "0";
                        if (color != null)
                            colorId = color.Id.ToString();
                        var Garanty = uow.ProductAttributeItemRepository.Get(x => x, x => x.Value.Contains(NameString2)).FirstOrDefault();
                        string GarantyId = "0";
                        if (Garanty != null)
                            GarantyId = Garanty.Id.ToString();
                        ProductPrices = from s in ProductPrices
                                        where
                                        (s.Product.LatinName != null && s.Product.LatinName.ToLower().Contains(NameString2)) ||
                                        (s.Product.Name != null && s.Product.Name.ToLower().Contains(NameString2)) ||
                                        (s.Product.Code != null && s.Product.Code.ToLower().Contains(NameString2)) ||
                                        (s.code != null && s.code.ToLower().Contains(NameString2)) ||
                                        (s.ProductAttributeSelectModelId != null && s.ProductAttributeSelectModel.Value.ToLower().Contains(NameString2)) ||
                                        (s.ProductAttributeSelectSizeId != null && s.ProductAttributeSelectSize.Value.ToLower().Contains(NameString2)) ||
                                        (s.ProductAttributeSelectWeightId != null && s.ProductAttributeSelectweight.Value.ToLower().Contains(NameString2)) ||
                                        (s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor.Value.ToLower() == colorId) ||
                                        (s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId)
                                        select s;
                    }


                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "IsDefault":
                            ProductPrices = ProductPrices.OrderBy(s => s.IsDefault);
                            ViewBag.CurrentSort = "IsDefault";
                            break;
                        case "IsDefault_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.IsDefault);
                            ViewBag.CurrentSort = "IsDefault_desc";
                            break;
                        case "Quantity":
                            ProductPrices = ProductPrices.OrderBy(s => s.Quantity);
                            ViewBag.CurrentSort = "Quantity";
                            break;
                        case "Quantity_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.Quantity);
                            ViewBag.CurrentSort = "Quantity_desc";
                            break;
                        case "DeliveryTimeout":
                            ProductPrices = ProductPrices.OrderBy(s => s.DeliveryTimeout);
                            ViewBag.CurrentSort = "DeliveryTimeout";
                            break;
                        case "DeliveryTimeout_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.DeliveryTimeout);
                            ViewBag.CurrentSort = "DeliveryTimeout_desc";
                            break;
                        case "MaxBasketCount":
                            ProductPrices = ProductPrices.OrderBy(s => s.MaxBasketCount);
                            ViewBag.CurrentSort = "MaxBasketCount";
                            break;
                        case "MaxBasketCount_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.MaxBasketCount);
                            ViewBag.CurrentSort = "MaxBasketCount_desc";
                            break;
                        case "price":
                            ProductPrices = ProductPrices.OrderBy(s => s.Price);
                            ViewBag.CurrentSort = "price";
                            break;
                        case "price_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.Price);
                            ViewBag.CurrentSort = "price_desc";
                            break;
                        case "code":
                            ProductPrices = ProductPrices.OrderBy(s => s.code);
                            ViewBag.CurrentSort = "code";
                            break;
                        case "code_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.code);
                            ViewBag.CurrentSort = "code_desc";
                            break;
                        case "ProductState":
                            ProductPrices = ProductPrices.OrderBy(s => s.ProductState.Title);
                            ViewBag.CurrentSort = "ProductState";
                            break;
                        case "ProductState_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.ProductState.Title);
                            ViewBag.CurrentSort = "ProductState_desc";
                            break;
                        case "state":
                            ProductPrices = ProductPrices.OrderBy(s => s.Product.state);
                            ViewBag.CurrentSort = "state";
                            break;
                        case "state_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.Product.state);
                            ViewBag.CurrentSort = "state_desc";
                            break;
                        case "IsActive":
                            ProductPrices = ProductPrices.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;
                        case "Language":
                            ProductPrices = ProductPrices.OrderBy(s => s.Product.LanguageId);
                            ViewBag.CurrentSort = "Language";
                            break;
                        case "Language_desc":
                            ProductPrices = ProductPrices.OrderByDescending(s => s.Product.LanguageId);
                            ViewBag.CurrentSort = "Language_desc";
                            break;
                        default:  // Name ascending 
                            ProductPrices = ProductPrices.OrderByDescending(s => s.Updatedate).ThenByDescending(x => x.Id);
                            break;
                    }

                    #endregion
                    ViewBag.ProductOffers = ProductPrices.ToPagedList(pageNumber2, pageSize2);
                    #endregion


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
                    List<SelectListItem> stateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "عدم تایید", Value = "False" }, new SelectListItem() { Text = "تایید", Value = "True" } };
                    ViewBag.StateId = stateSelectListItem;
                    List<SelectListItem> ExpireSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "شروع شده", Value = "1" }, new SelectListItem() { Text = "پایان یافته", Value = "2" }, new SelectListItem() { Text = "منتظر شروع", Value = "3" } };
                    ViewBag.ExpireId = ExpireSelectListItem;
                    List<SelectListItem> TypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "شگفت انگیز", Value = "1" }, new SelectListItem() { Text = "تخفیف و حراج", Value = "2" }, new SelectListItem() { Text = "ارسال رایگان", Value = "0" }, new SelectListItem() { Text = "کد تخفیف", Value = "4" }, new SelectListItem() { Text = "کد تخفیف عمومی", Value = "5" } };
                    ViewBag.TypeListId = TypeSelectListItem;


                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    #endregion

                    #region search
                    if (string.IsNullOrEmpty(state))
                        state = stateFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(Expire))
                        Expire = ExpireFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(IsActive))
                        IsActive = IsActiveFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(CodeType))
                        CodeType = CodeTypeFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StartDateStart))
                        StartDateStart = StartDateStartFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StartDateEnd))
                        StartDateEnd = StartDateEndFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ExpireDateStart))
                        ExpireDateStart = ExpireDateStartFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ExpireDateEnd))
                        ExpireDateEnd = ExpireDateEndFilter;
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

                    ViewBag.NameFilter = NameString;
                    ViewBag.stateFilter = state;
                    ViewBag.ExpireFilter = Expire;
                    ViewBag.IsActiveFilter = IsActive;
                    ViewBag.CodeTypeFilter = CodeType;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.InsertDateEndFilter = InsertDateEnd;
                    ViewBag.InsertDateStartFilter = InsertDateStart;
                    ViewBag.StartDateStartFilter = StartDateStart;
                    ViewBag.StartDateEndFilter = StartDateEnd;
                    ViewBag.ExpireDateStartFilter = ExpireDateStart;
                    ViewBag.ExpireDateEndFilter = ExpireDateEnd;

                    var Offers = uow.OfferRepository.GetByReturnQueryable(x => x, x => x.IsDeleted == false, x => x.OrderByDescending(s => s.Id), "attachment,offerProductCategories.productCategory,ProductOffers.OrderRows");

                    if (!String.IsNullOrEmpty(NameString))
                        Offers = Offers.Where(s => s.Title.Contains(NameString));
                    if (!String.IsNullOrEmpty(LanguagenameString))
                        Offers = Offers.Where(s => s.LanguageId == langid);

                    if (!String.IsNullOrEmpty(IsActive))
                    {
                        bool isActive = Convert.ToBoolean(IsActive);
                        Offers = Offers.Where(s => s.IsActive == isActive);
                    }
                    if (!String.IsNullOrEmpty(state))
                    {
                        bool stat = Convert.ToBoolean(state);
                        Offers = Offers.Where(s => s.state == stat);
                    }
                    if (!String.IsNullOrEmpty(CodeType))
                        Offers = Offers.Where(s => s.CodeTypeValueCode == int.Parse(CodeType));
                    if (!String.IsNullOrEmpty(Expire))
                    {
                        var date = DateTime.Now;
                        int exp = Convert.ToInt32(Expire);
                        switch (exp)
                        {
                            case 1:
                                Offers = Offers.Where(x => x.IsActive && x.state && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)); break;
                            case 2:
                                Offers = Offers.Where(x => x.IsActive == false || (x.ExpireDate != null && x.ExpireDate < date)); break;
                            case 3:
                                Offers = Offers.Where(x => x.IsActive && x.state && (x.StartDate != null && x.StartDate > date)); break;
                            default:
                                break;
                        }
                    }

                    DateTime dtInsertDateStart = DateTime.Now.Date, dtInsertDateEnd = DateTime.Now.Date, dtStartDateStart = DateTime.Now.Date, dtStartDateEnd = DateTime.Now.Date, dtExpireDateStart = DateTime.Now.Date, dtExpireDateEnd = DateTime.Now.Date;
                    if (!String.IsNullOrEmpty(InsertDateStart))
                        dtInsertDateStart = DateTimeConverter.ChangeShamsiToMiladi(InsertDateStart);
                    if (!String.IsNullOrEmpty(InsertDateEnd))
                        dtInsertDateEnd = DateTimeConverter.ChangeShamsiToMiladi(InsertDateEnd);
                    if (!String.IsNullOrEmpty(StartDateStart))
                        dtStartDateStart = DateTimeConverter.ChangeShamsiToMiladi(StartDateStart);
                    if (!String.IsNullOrEmpty(StartDateEnd))
                        dtStartDateEnd = DateTimeConverter.ChangeShamsiToMiladi(StartDateEnd);
                    if (!String.IsNullOrEmpty(ExpireDateStart))
                        dtExpireDateStart = DateTimeConverter.ChangeShamsiToMiladi(ExpireDateStart);
                    if (!String.IsNullOrEmpty(ExpireDateEnd))
                        dtExpireDateEnd = DateTimeConverter.ChangeShamsiToMiladi(ExpireDateEnd);

                    if (!String.IsNullOrEmpty(InsertDateStart) && !String.IsNullOrEmpty(InsertDateEnd))
                        Offers = Offers.Where(s => s.InsertDate >= dtInsertDateStart && s.InsertDate <= dtInsertDateEnd);
                    else if (!String.IsNullOrEmpty(InsertDateStart))
                        Offers = Offers.Where(s => s.InsertDate >= dtInsertDateStart);
                    else if (!String.IsNullOrEmpty(InsertDateEnd))
                        Offers = Offers.Where(s => s.InsertDate <= dtInsertDateEnd);


                    if (!String.IsNullOrEmpty(StartDateStart) && !String.IsNullOrEmpty(StartDateEnd))
                        Offers = Offers.Where(s => s.StartDate >= dtStartDateStart && s.StartDate <= dtStartDateEnd);
                    else if (!String.IsNullOrEmpty(StartDateStart))
                        Offers = Offers.Where(s => s.StartDate >= dtStartDateStart);
                    else if (!String.IsNullOrEmpty(StartDateEnd))
                        Offers = Offers.Where(s => s.StartDate <= dtStartDateEnd);


                    if (!String.IsNullOrEmpty(ExpireDateStart) && !String.IsNullOrEmpty(ExpireDateEnd))
                        Offers = Offers.Where(s => s.ExpireDate >= dtExpireDateStart && s.InsertDate <= dtExpireDateEnd);
                    else if (!String.IsNullOrEmpty(ExpireDateStart))
                        Offers = Offers.Where(s => s.ExpireDate >= dtExpireDateStart);
                    else if (!String.IsNullOrEmpty(ExpireDateEnd))
                        Offers = Offers.Where(s => s.ExpireDate <= dtExpireDateEnd);

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        //case "Value":
                        //    Offers = Offers.OrderBy(s => s.CodeValue);
                        //    ViewBag.CurrentSort = "Value";
                        //    break;
                        //case "Value_desc":
                        //    Offers = Offers.OrderByDescending(s => s.CodeValue);
                        //    ViewBag.CurrentSort = "Value_desc";
                        //    break;
                        case "Name":
                            Offers = Offers.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Name";
                            break;
                        case "Name_desc":
                            Offers = Offers.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Name_desc";
                            break;
                        case "CodeType":
                            Offers = Offers.OrderBy(s => s.CodeTypeValueCode);
                            ViewBag.CurrentSort = "CodeType";
                            break;
                        case "CodeType_desc":
                            Offers = Offers.OrderByDescending(s => s.CodeTypeValueCode);
                            ViewBag.CurrentSort = "CodeType_desc";
                            break;
                        case "state":
                            Offers = Offers.OrderBy(s => s.state);
                            ViewBag.CurrentSort = "state";
                            break;
                        case "state_desc":
                            Offers = Offers.OrderByDescending(s => s.state);
                            ViewBag.CurrentSort = "state_desc";
                            break;
                        case "InsertDate":
                            Offers = Offers.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            Offers = Offers.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        case "StartDate":
                            Offers = Offers.OrderBy(s => s.StartDate);
                            ViewBag.CurrentSort = "StartDate";
                            break;
                        case "StartDate_desc":
                            Offers = Offers.OrderByDescending(s => s.StartDate);
                            ViewBag.CurrentSort = "StartDate_desc";
                            break;
                        case "ExpireDate":
                            Offers = Offers.OrderBy(s => s.ExpireDate);
                            ViewBag.CurrentSort = "ExpireDate";
                            break;
                        case "ExpireDate_desc":
                            Offers = Offers.OrderByDescending(s => s.ExpireDate);
                            ViewBag.CurrentSort = "ExpireDate_desc";
                            break;
                        case "IsActive":
                            Offers = Offers.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive";
                            break;
                        case "IsActive_desc":
                            Offers = Offers.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "IsActive_desc";
                            break;
                        case "Language":
                            Offers = Offers.OrderBy(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language";
                            break;
                        case "Language_desc":
                            Offers = Offers.OrderByDescending(s => s.LanguageId);
                            ViewBag.CurrentSort = "Language_desc";
                            break;
                        default:
                            Offers = Offers.OrderByDescending(s => s.Id);
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

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت تخفیف ها", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "Index", true, 200, " نمایش صفحه مدیریت تخفیف ها", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Offers.ToList().ToPagedList(pageNumber, pageSize));

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Offers/Create
        public virtual ActionResult Create()
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 1))
                {
                    List<SelectListItem> TypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "شگفت انگیز", Value = "1" }, new SelectListItem() { Text = "تخفیف و حراج", Value = "2" }, new SelectListItem() { Text = "ارسال رایگان", Value = "0" }, new SelectListItem() { Text = "کد تخفیف", Value = "4" }, new SelectListItem() { Text = "کد تخفیف عمومی", Value = "5" } };
                    ViewBag.TypeListId = TypeSelectListItem;


                    ViewBag.productCategories = new MultiSelectList(uow.ProductCategoryRepository.Get(x => new { x.Id, x.Name }, x => x.IsActive == true && x.ParrentId == null), "Id", "Name");
                    ViewBag.UserGroups = new MultiSelectList(uow.UserGroupRepository.Get(x => new { x.Id, x.Name }), "Id", "Name");

                    List<SelectListItem> defaultCodeTypeListItem = new List<SelectListItem>() { new SelectListItem() { Text = "ثابت", Value = "1" }, new SelectListItem() { Text = "درصدی", Value = "2" } };
                    ViewBag.defaultCodeTypeListItem = defaultCodeTypeListItem;

                    List<SelectListItem> CodeUseTypeList = new List<SelectListItem>();
                    CodeUseTypeList.Add(new SelectListItem() { Text = CodeUseType.ثبت_در_پنل_مدیریت.EnumDisplayNameFor(), Value = Convert.ToInt16(CodeUseType.ثبت_در_پنل_مدیریت).ToString() });
                    CodeUseTypeList.Add(new SelectListItem() { Text = CodeUseType.ثبت_پس_از_ثبت_نظر_محصول.EnumDisplayNameFor(), Value = Convert.ToInt16(CodeUseType.ثبت_پس_از_ثبت_نظر_محصول).ToString() });
                    CodeUseTypeList.Add(new SelectListItem() { Text = CodeUseType.ثبت_پس_از_ثبت_نظر_سفارش.EnumDisplayNameFor(), Value = Convert.ToInt16(CodeUseType.ثبت_پس_از_ثبت_نظر_سفارش).ToString() });
                    ViewBag.CodeUseTypeList = CodeUseTypeList;


                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");

                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 37 && x.Name == "تعریف تخفیف", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "Create", true, 200, "نمایش صفحه ایجاد تخفیف", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POST: Admin/Offers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create(Offer Offer, string StartDate, string ExpireDate, int?[] CatId, int?[] UserGroupId)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    Offer.InsertDate = DateTime.Now;
                    Offer.UserId = User.Identity.GetUserId();
                    if (!string.IsNullOrEmpty(StartDate))
                        Offer.StartDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladiDateTime(StartDate);
                    if (!string.IsNullOrEmpty(ExpireDate))
                        Offer.ExpireDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladiDateTime(ExpireDate);

                    Offer.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Offer.Title);
                    if (Offer.CodeTypeValueCode == 4)
                        Offer.state = true;
                    uow.OfferRepository.Insert(Offer);
                    uow.Save();
                    if (CatId != null)
                    {
                        Offer.offerProductCategories = new List<OfferProductCategory>();
                        foreach (var item in CatId)
                        {
                            Offer.offerProductCategories.Add(new OfferProductCategory()
                            {
                                CatId = item.Value,
                                OfferId = Offer.Id
                            });
                        }
                    }
                    if (UserGroupId != null)
                    {
                        Offer.offerUserGroups = new List<OfferUserGroup>();
                        foreach (var item in UserGroupId)
                        {
                            Offer.offerUserGroups.Add(new OfferUserGroup()
                            {
                                UserGroupId = item.Value,
                                OfferId = Offer.Id
                            });

                        }
                    }
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "Create", false, 200, "ایجاد تخفیف " + Offer.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                List<SelectListItem> TypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "شگفت انگیز", Value = "1" }, new SelectListItem() { Text = "تخفیف و حراج", Value = "2" }, new SelectListItem() { Text = "ارسال رایگان", Value = "0" }, new SelectListItem() { Text = "کد تخفیف", Value = "4" }, new SelectListItem() { Text = "کد تخفیف عمومی", Value = "5" } };
                ViewBag.TypeListId = TypeSelectListItem;
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 37 && x.Name == "تعریف تخفیف", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "Create", false, 500, "خطا در ایجاد تخفیف" + Offer.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);

                return View(Offer);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Offers/Edit/5
        public virtual ActionResult Edit(int? id)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 2))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var offer = uow.OfferRepository.Get(x => x, x => x.Id == id, null, "attachment,offerProductCategories,offerUserGroups").Single();
                    if (offer == null)
                    {
                        return HttpNotFound();
                    }
                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    if (offer.ExpireDate.HasValue)
                        ViewBag.ExpireDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(offer.ExpireDate.Value) + " " + offer.ExpireDate.Value.ToString("HH:mm");
                    else
                        ViewBag.ExpireDate = "";

                    if (offer.StartDate.HasValue)
                        ViewBag.StartDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(offer.StartDate.Value) + " " + offer.StartDate.Value.ToString("HH:mm");
                    else
                        ViewBag.StartDate = "";

                    ViewBag.productCategories = new MultiSelectList(uow.ProductCategoryRepository.Get(x => new { x.Id, x.Name }, x => x.IsActive == true && x.ParrentId == null), "Id", "Name", offer.offerProductCategories.Select(x => x.CatId));
                    ViewBag.UserGroups = new MultiSelectList(uow.UserGroupRepository.Get(x => new { x.Id, x.Name }), "Id", "Name", offer.offerUserGroups.Select(x => x.UserGroupId));

                    List<SelectListItem> defaultCodeTypeListItem = new List<SelectListItem>() { new SelectListItem() { Text = "ثابت", Value = "1", Selected = offer.DefaultCodeType == 1 ? true : false }, new SelectListItem() { Text = "درصدی", Value = "2", Selected = offer.DefaultCodeType == 2 ? true : false } };
                    ViewBag.defaultCodeTypeListItem = defaultCodeTypeListItem;

                    List<SelectListItem> CodeUseTypeList = new List<SelectListItem>();
                    CodeUseTypeList.Add(new SelectListItem() { Text = CodeUseType.ثبت_در_پنل_مدیریت.EnumDisplayNameFor(), Value = Convert.ToInt16(CodeUseType.ثبت_در_پنل_مدیریت).ToString(), Selected = offer.codeUseType == CodeUseType.ثبت_در_پنل_مدیریت ? true : false });
                    CodeUseTypeList.Add(new SelectListItem() { Text = CodeUseType.ثبت_پس_از_ثبت_نظر_محصول.EnumDisplayNameFor(), Value = Convert.ToInt16(CodeUseType.ثبت_پس_از_ثبت_نظر_محصول).ToString(), Selected = offer.codeUseType == CodeUseType.ثبت_پس_از_ثبت_نظر_محصول ? true : false });
                    CodeUseTypeList.Add(new SelectListItem() { Text = CodeUseType.ثبت_پس_از_ثبت_نظر_سفارش.EnumDisplayNameFor(), Value = Convert.ToInt16(CodeUseType.ثبت_پس_از_ثبت_نظر_سفارش).ToString(), Selected = offer.codeUseType == CodeUseType.ثبت_پس_از_ثبت_نظر_سفارش ? true : false });
                    ViewBag.CodeUseTypeList = CodeUseTypeList;

                    List<SelectListItem> TypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "شگفت انگیز", Value = "1", Selected = offer.CodeTypeValueCode == 1 ? true : false }, new SelectListItem() { Text = "تخفیف و حراج", Value = "2", Selected = offer.CodeTypeValueCode == 2 ? true : false }, new SelectListItem() { Text = "ارسال رایگان", Value = "0", Selected = offer.CodeTypeValueCode == 0 ? true : false }, new SelectListItem() { Text = "کد تخفیف", Value = "4", Selected = offer.CodeTypeValueCode == 4 ? true : false }, new SelectListItem() { Text = "کد تخفیف عمومی", Value = "5", Selected = offer.CodeTypeValueCode == 5 ? true : false } };
                    ViewBag.TypeListId = TypeSelectListItem;
                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 37 && x.Name == "تعریف تخفیف", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "Edit", true, 200, "نمایش صفحه ویرایش تخفیف   " + offer.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(offer);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "offer", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POST: Admin/Offers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Edit(Offer Offer, string StartDate, string ExpireDate, int?[] CatId, int?[] UserGroupId)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    Offer.InsertDate = DateTime.Now;
                    Offer.UserId = User.Identity.GetUserId();
                    if (!string.IsNullOrEmpty(StartDate))
                        Offer.StartDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladiDateTime(StartDate);
                    if (!string.IsNullOrEmpty(ExpireDate))
                        Offer.ExpireDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladiDateTime(ExpireDate);

                    Offer.Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(Offer.Title);
                    uow.OfferRepository.Update(Offer);
                    uow.Save();

                    if (CatId != null)
                    {
                        foreach (var item in CatId)
                            if (!uow.OfferProductCategoryRepository.Any(x => x.Id, x => x.OfferId == Offer.Id && x.CatId == item))
                                uow.OfferProductCategoryRepository.Insert(new OfferProductCategory { CatId = item.Value, OfferId = Offer.Id });
                        uow.Save();

                        var deletedOfferCategories = uow.OfferProductCategoryRepository.Get(x => x, x => x.OfferId == Offer.Id && !CatId.Contains(x.CatId));
                        uow.OfferProductCategoryRepository.Delete(deletedOfferCategories.ToList());
                        uow.Save();
                    }
                    else
                    {

                        var deletedOfferCategories = uow.OfferProductCategoryRepository.Get(x => x, x => x.OfferId == Offer.Id);
                        uow.OfferProductCategoryRepository.Delete(deletedOfferCategories.ToList());
                        uow.Save();
                    }

                    if (UserGroupId != null)
                    {
                        foreach (var item in UserGroupId)
                            if (!uow.OfferUserGroupRepository.Any(x => x.Id, x => x.OfferId == Offer.Id && x.UserGroupId == item))
                                uow.OfferUserGroupRepository.Insert(new OfferUserGroup { UserGroupId = item.Value, OfferId = Offer.Id });
                        uow.Save();

                        var deletedOfferUserGroups = uow.OfferUserGroupRepository.Get(x => x, x => x.OfferId == Offer.Id && !UserGroupId.Contains(x.UserGroupId));
                        uow.OfferUserGroupRepository.Delete(deletedOfferUserGroups.ToList());
                        uow.Save();
                    }
                    else
                    {

                        var deletedOfferUserGroups = uow.OfferUserGroupRepository.Get(x => x, x => x.OfferId == Offer.Id);
                        uow.OfferUserGroupRepository.Delete(deletedOfferUserGroups.ToList());
                        uow.Save();
                    }

                    CheckState(Offer.Id);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "Offer", "Edit", false, 200, "ویرایش تخفیف " + Offer.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }

                List<SelectListItem> TypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "شگفت انگیز", Value = "1", Selected = Offer.CodeTypeValueCode == 1 ? true : false }, new SelectListItem() { Text = "تخفیف و حراج", Value = "2", Selected = Offer.CodeTypeValueCode == 2 ? true : false }, new SelectListItem() { Text = "ارسال رایگان", Value = "0", Selected = Offer.CodeTypeValueCode == 0 ? true : false }, new SelectListItem() { Text = "کد تخفیف", Value = "4", Selected = Offer.CodeTypeValueCode == 4 ? true : false }, new SelectListItem() { Text = "کد تخفیف عمومی", Value = "5", Selected = Offer.CodeTypeValueCode == 5 ? true : false } };
                ViewBag.TypeListId = TypeSelectListItem;
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 37 && x.Name == "تعریف تخفیف", null, "HelpModuleSectionFields").FirstOrDefault();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "Edit", false, 500, "خطا در ویرایش تخفیف" + Offer.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName", Offer.LanguageId);

                return View(Offer);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }



        // GET: Admin/Gallery/Delete/5
        public virtual ActionResult Delete(int? id)
        {

            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var Offer = uow.OfferRepository.GetByID(id);
                    if (Offer == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "Delete", true, 200, "نمایش صفحه حذف تخفیف " + Offer.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Offer);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Galleries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {

            try
            {

                var Offer = uow.OfferRepository.GetByID(id);
                Offer.IsDeleted = true;
                Offer.IsActive = false;
                Offer.ExpireDate = DateTime.Now.AddDays(-1);
                uow.OfferRepository.Update(Offer);
                uow.Save();

                //var ProductOfferTypes = uow.ProductOfferRepository.Get(x => x, x => x.OfferId == id);
                //uow.ProductOfferRepository.Delete(ProductOfferTypes.ToList());
                //uow.Save();
                //var UserCodeGiftLogs = uow.UserCodeGiftLogRepository.Get(x => x, x => x.UserCodeGift.OfferId == id);
                //uow.UserCodeGiftLogRepository.Delete(UserCodeGiftLogs.ToList());
                //uow.Save();
                //var UserCodeGifts = uow.UserCodeGiftRepository.Get(x => x, x => x.OfferId == id);
                //uow.UserCodeGiftRepository.Delete(UserCodeGifts.ToList());
                //uow.Save();
                //var FreeSendOfferStates = uow.FreeSendOfferStateRepository.Get(x => x, x => x.FreeSendOffer.OfferId == id);
                //uow.FreeSendOfferStateRepository.Delete(FreeSendOfferStates.ToList());
                //uow.Save();
                //var FreeSendOffers = uow.FreeSendOfferRepository.Get(x => x, x => x.OfferId == id);
                //uow.FreeSendOfferRepository.Delete(FreeSendOffers.ToList());
                //uow.Save();

                //var Offer = uow.OfferRepository.GetByID(id);
                //uow.OfferRepository.Delete(Offer);
                //uow.Save();
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Offers", "DeleteConfirmed", false, 200, "حذف تخفیف " + Offer.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                var gallery = uow.GalleryCategoryRepository.GetByID(id);
                ViewBag.Erorr = "خطایی رخ داد" + x.Message;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(gallery);
            }
        }


        private void CheckState(int offerId)
        {
            var offer = uow.OfferRepository.Get(x => x, x => x.Id == offerId, null, "ProductOffers,UserCodeGifts,FreeSendOffers").FirstOrDefault();

            if (offer.CodeTypeValueCode == 1 || offer.CodeTypeValueCode == 2)
            {
                if (offer.ProductOffers.Any() || offer.UserCodeGifts.Any())
                    offer.state = true;
                else
                    offer.state = false;
            }
            else if (offer.CodeTypeValueCode == 3)
            {
                if (offer.FreeSendOffers.Any())
                    offer.state = true;
                else
                    offer.state = false;
            }
            else
            {
                offer.state = true;
            }
            uow.OfferRepository.Update(offer);
            uow.Save();
        }

        #endregion

        #region ProductOffer
        // GET: Admin/Offers/Detail/5
        public virtual ActionResult Detail(int? id, int? PageSize, int? PageSize2, int? SearchType, string sortOrder, string ProductInsertStateId, string ProductInsertStateFilter, string Quantity, string QuantityFilter, string IsActive, string IsActiveFilter, string ProductStateId, string ProductStateFilter, string LanguagenameString, string LanguagenameFilter, string NameString, string NameFilter, string CityId, string CityIdFilter, string proviencId, string proviencIdFilter, string ProductCatId, string ProductCatIdFilter, string BrandId, string BrandFilter, string ProductTypeId, string ProductTypeFilter, string ProductOfferTypeId, string ProductOfferTypeIdFilter, int? page, int? page2)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 24);
                if (p.Where(x => x == true).Any())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var offer = uow.OfferRepository.Get(x => x, x => x.Id == id, null, "FreeSendOffers,ProductOffers,UserCodeGifts,offerProductCategories").FirstOrDefault();
                    if (offer == null)
                    {
                        return HttpNotFound();
                    }

                    #region Load DropDown List
                    short langid = setting.LanguageId.Value;
                    List<SelectListItem> ProductOfferTypeSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "شگفت انگیز", Value = "True" }, new SelectListItem() { Text = "تخفیف و حراج", Value = "False" } };
                    ViewBag.ProductOfferType = ProductOfferTypeSelectListItem;
                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.BrandId = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);

                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");
                    ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name");
                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name");

                    #endregion

                    #region  PageSize
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

                    int pageSize2 = 10;
                    if (PageSize2.HasValue)
                    {
                        if (PageSize2.Value > 100)
                            pageSize2 = 100;
                        else if (pageSize2 < 10)
                            pageSize2 = 10;
                        else
                            pageSize2 = PageSize2.Value;
                    }
                    ViewBag.PageSize2 = pageSize2;
                    int pageNumber2 = (page2 ?? 1);

                    if (SearchType.HasValue)
                    {
                        ViewBag.SearchType = SearchType.Value;
                    }
                    else
                        ViewBag.SearchType = Convert.ToInt32(offer.CodeTypeValueCode == 0 ? 2 : 1);
                    #endregion


                    #region ProductOffer
                    if (offer.CodeTypeValueCode == 1 || offer.CodeTypeValueCode == 2)
                    {

                        #region Load DropDown List
                        langid = 1;
                        if (string.IsNullOrEmpty(LanguagenameString))
                            LanguagenameString = LanguagenameFilter;
                        else
                            page = 1;
                        if (LanguagenameString != null)
                            langid = Int16.Parse(LanguagenameString);

                        List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                        ViewBag.IsActive = IsActiveSelectListItem;
                        List<SelectListItem> stateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1" }, new SelectListItem() { Text = "بررسی مجدد", Value = "2" }, new SelectListItem() { Text = "عدم تایید", Value = "3" }, new SelectListItem() { Text = "تایید", Value = "4" } };
                        ViewBag.ProductInsertStateId = stateSelectListItem;

                        List<int> offerProductCategoryId = offer.offerProductCategories.Select(x => x.CatId).ToList();
                        ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => offerProductCategoryId.Contains(c.Id) && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                        ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                        ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title");
                        ViewBag.ProductStateList = uow.ProductStateRepository.Get(x => x);
                        if (offerProductCategoryId.Any())
                        {
                            List<int> SubCatIds = new List<int>();
                            foreach (var item in offerProductCategoryId)
                            {
                                SubCatIds.AddRange(uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", item)).ToList());
                            }
                            ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x, x => x.Products.Any(s => s.ProductCategories.Any(a => SubCatIds.Contains(a.Id)))), "Id", "PersianName");
                        }
                        else
                            ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x), "Id", "PersianName");

                        ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                        #endregion

                        #region search
                        if (string.IsNullOrEmpty(Quantity))
                            Quantity = QuantityFilter;
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
                        if (string.IsNullOrEmpty(IsActive))
                            IsActive = IsActiveFilter;
                        else
                            page = 1;

                        if (string.IsNullOrEmpty(ProductTypeId))
                            ProductTypeId = ProductTypeFilter;
                        else
                            page = 1;

                        if (string.IsNullOrEmpty(ProductStateId))
                            ProductStateId = ProductStateFilter;
                        else
                            page = 1;

                        if (string.IsNullOrEmpty(LanguagenameString))
                            LanguagenameString = LanguagenameFilter;
                        else
                            page = 1;


                        ViewBag.QuantityFilter = Quantity;
                        ViewBag.ProductCatIdFilter = ProductCatId;
                        ViewBag.NameFilter = NameString;
                        ViewBag.IsActiveFilter = IsActive;
                        ViewBag.ProductTypeFilter = ProductTypeId;
                        ViewBag.ProductStateFilter = ProductStateId;
                        ViewBag.LanguagenameFilter = LanguagenameString;

                        pageSize = 10;
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
                        pageNumber = (page ?? 1);


                        var ProductPrices = uow.ProductPriceRepository.GetQueryList().AsNoTracking().Include("ProductImages").Include("ProductImages.Image").Include("Product.User").Include("Product").Include("Product.ProductImages.Image").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectSize").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectweight").Include("Product.ProductCategories").Include("ProductState").Include("ProductOffers").Where(x => x.ProductStateId == 1 || x.ProductStateId == 2);
                        if (offerProductCategoryId.Any())
                        {
                            List<int> AllofferProductCategoryId = new List<int>();
                            foreach (var item in offerProductCategoryId)
                            {

                                List<int> SubCatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", item)).ToList();
                                AllofferProductCategoryId.AddRange(SubCatIds);
                            }
                            ProductPrices = ProductPrices.Where(x => x.Product.ProductCategories.Any(s => AllofferProductCategoryId.Contains(s.Id)));
                        }
                        var ProductPrices2 = ProductPrices.Where(x => !x.ProductOffers.Any(s => s.OfferId==id.Value && s.Offer.IsDeleted == false));
                        ProductPrices = ProductPrices.Where(x => x.ProductOffers.Any(s => s.OfferId == id.Value && s.Offer.IsDeleted == false));

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

                            ProductPrices = from s in ProductPrices
                                            where
                                            (s.Product.LatinName != null && s.Product.LatinName.ToLower().Contains(NameString)) ||
                                            (s.Product.Name != null && s.Product.Name.ToLower().Contains(NameString)) ||
                                            (s.Product.Code != null && s.Product.Code.ToLower().Contains(NameString)) ||
                                            (s.code != null && s.code.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectModelId != null && s.ProductAttributeSelectModel.Value.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectSizeId != null && s.ProductAttributeSelectSize.Value.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectWeightId != null && s.ProductAttributeSelectweight.Value.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor.Value.ToLower() == colorId) ||
                                            (s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId)
                                            select s;

                            ProductPrices2 = from s in ProductPrices2
                                             where
                                            (s.Product.LatinName != null && s.Product.LatinName.ToLower().Contains(NameString)) ||
                                            (s.Product.Name != null && s.Product.Name.ToLower().Contains(NameString)) ||
                                            (s.Product.Code != null && s.Product.Code.ToLower().Contains(NameString)) ||
                                            (s.code != null && s.code.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectModelId != null && s.ProductAttributeSelectModel.Value.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectSizeId != null && s.ProductAttributeSelectSize.Value.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectWeightId != null && s.ProductAttributeSelectweight.Value.ToLower().Contains(NameString)) ||
                                            (s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor.Value.ToLower() == colorId) ||
                                            (s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId)
                                             select s;
                        }

                        if (!String.IsNullOrEmpty(LanguagenameString))
                        {
                            ProductPrices = ProductPrices.Where(s => s.Product.LanguageId == langid);
                            ProductPrices2 = ProductPrices2.Where(s => s.Product.LanguageId == langid);
                        }
                        if (!String.IsNullOrEmpty(ProductCatId))
                        {
                            int ctId = Convert.ToInt32(ProductCatId);
                            if (ctId > 0)
                            {

                                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ctId)).ToList();
                                ProductPrices = ProductPrices.Where(s => s.Product.ProductCategories.Any(x => CatIds.Contains(x.Id)));
                                ProductPrices2 = ProductPrices2.Where(s => s.Product.ProductCategories.Any(x => CatIds.Contains(x.Id)));
                            }
                        }
                        if (!String.IsNullOrEmpty(IsActive))
                        {
                            bool isActive = Convert.ToBoolean(IsActive);
                            ProductPrices = ProductPrices.Where(s => s.IsActive == isActive);
                            ProductPrices2 = ProductPrices2.Where(s => s.IsActive == isActive);
                        }
                        if (!String.IsNullOrEmpty(ProductTypeId))
                        {
                            int value = int.Parse(ProductTypeId);
                            ProductPrices = ProductPrices.Where(s => s.Product.ProductTypeId == value);
                            ProductPrices2 = ProductPrices2.Where(s => s.Product.ProductTypeId == value);
                        }
                        if (!String.IsNullOrEmpty(ProductStateId))
                        {
                            int value = int.Parse(ProductStateId);
                            ProductPrices = ProductPrices.Where(s => s.ProductStateId == value);
                            ProductPrices2 = ProductPrices2.Where(s => s.ProductStateId == value);
                        }
                        if (!String.IsNullOrEmpty(ProductInsertStateId))
                        {
                            int value = int.Parse(ProductInsertStateId);
                            ProductPrices = ProductPrices.Where(s => s.Product.state == value);
                            ProductPrices2 = ProductPrices2.Where(s => s.Product.state == value);
                        }

                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {
                            case "IsDefault":
                                ProductPrices = ProductPrices.OrderBy(s => s.IsDefault);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.IsDefault);
                                ViewBag.CurrentSort = "IsDefault";
                                break;
                            case "IsDefault_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.IsDefault);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.IsDefault);
                                ViewBag.CurrentSort = "IsDefault_desc";
                                break;
                            case "Quantity":
                                ProductPrices = ProductPrices.OrderBy(s => s.Quantity);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.Quantity);
                                ViewBag.CurrentSort = "Quantity";
                                break;
                            case "Quantity_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.Quantity);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.Quantity);
                                ViewBag.CurrentSort = "Quantity_desc";
                                break;
                            case "DeliveryTimeout":
                                ProductPrices = ProductPrices.OrderBy(s => s.DeliveryTimeout);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.DeliveryTimeout);
                                ViewBag.CurrentSort = "DeliveryTimeout";
                                break;
                            case "DeliveryTimeout_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.DeliveryTimeout);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.DeliveryTimeout);
                                ViewBag.CurrentSort = "DeliveryTimeout_desc";
                                break;
                            case "MaxBasketCount":
                                ProductPrices = ProductPrices.OrderBy(s => s.MaxBasketCount);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.MaxBasketCount);
                                ViewBag.CurrentSort = "MaxBasketCount";
                                break;
                            case "MaxBasketCount_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.MaxBasketCount);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.MaxBasketCount);
                                ViewBag.CurrentSort = "MaxBasketCount_desc";
                                break;
                            case "price":
                                ProductPrices = ProductPrices.OrderBy(s => s.Price);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.Price);
                                ViewBag.CurrentSort = "price";
                                break;
                            case "price_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.Price);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.Price);
                                ViewBag.CurrentSort = "price_desc";
                                break;
                            case "code":
                                ProductPrices = ProductPrices.OrderBy(s => s.code);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.code);
                                ViewBag.CurrentSort = "code";
                                break;
                            case "code_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.code);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.code);
                                ViewBag.CurrentSort = "code_desc";
                                break;
                            case "ProductState":
                                ProductPrices = ProductPrices.OrderBy(s => s.ProductState.Title);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.ProductState.Title);
                                ViewBag.CurrentSort = "ProductState";
                                break;
                            case "ProductState_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.ProductState.Title);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.ProductState.Title);
                                ViewBag.CurrentSort = "ProductState_desc";
                                break;
                            case "state":
                                ProductPrices = ProductPrices.OrderBy(s => s.Product.state);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.Product.state);
                                ViewBag.CurrentSort = "state";
                                break;
                            case "state_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.Product.state);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.Product.state);
                                ViewBag.CurrentSort = "state_desc";
                                break;
                            case "IsActive":
                                ProductPrices = ProductPrices.OrderBy(s => s.IsActive);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive";
                                break;
                            case "IsActive_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.IsActive);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.IsActive);
                                ViewBag.CurrentSort = "IsActive_desc";
                                break;
                            case "Language":
                                ProductPrices = ProductPrices.OrderBy(s => s.Product.LanguageId);
                                ProductPrices2 = ProductPrices2.OrderBy(s => s.Product.LanguageId);
                                ViewBag.CurrentSort = "Language";
                                break;
                            case "Language_desc":
                                ProductPrices = ProductPrices.OrderByDescending(s => s.Product.LanguageId);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.Product.LanguageId);
                                ViewBag.CurrentSort = "Language_desc";
                                break;
                            default:  // Name ascending 
                                ProductPrices = ProductPrices.OrderByDescending(s => s.Updatedate).ThenByDescending(x => x.Id);
                                ProductPrices2 = ProductPrices2.OrderByDescending(s => s.Updatedate);
                                break;
                        }

                        #endregion
                        ViewBag.ProductOffers = ProductPrices.ToPagedList(pageNumber2, pageSize2);
                        ViewBag.ProductOfferOffs = ProductPrices2.ToPagedList(pageNumber, pageSize);
                    }
                    #endregion

                    #region FreeSend
                    if (SearchType == 2 || (offer.CodeTypeValueCode == 0))
                    {

                        #region search  
                        if (string.IsNullOrEmpty(proviencId))
                            proviencId = proviencIdFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(CityId))
                            CityId = CityIdFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(ProductCatId))
                            ProductCatId = ProductCatIdFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(NameString))
                            NameString = NameFilter;
                        else
                            page = 1;
                        if (string.IsNullOrEmpty(BrandId))
                            BrandId = BrandFilter;
                        else
                            page = 1;



                        ViewBag.proviencIdFilter = proviencId;
                        ViewBag.CityIdFilter = CityId;
                        ViewBag.ProductCatIdFilter = ProductCatId;
                        ViewBag.NameFilter = NameString;
                        ViewBag.BrandFilter = BrandId;

                        var FreeSendOffer = uow.FreeSendOfferRepository.Get(x => x, x => x.OfferId == id.Value, x => x.OrderByDescending(s => s.Id), "FreeSendOfferStates,ProductCategory,Brand,Product");

                        if (!String.IsNullOrEmpty(NameString))
                        {
                            FreeSendOffer = from s in FreeSendOffer
                                            where
                                            (s.Product.Title != null && s.Product.Title.Contains(NameString)) ||
                                            (s.Product.Name != null && s.Product.Name.Contains(NameString)) ||
                                            (s.Product.LatinName != null && s.Product.LatinName.Contains(NameString))
                                            select s;
                        }

                        if (!String.IsNullOrEmpty(ProductCatId))
                        {
                            int ctId = Convert.ToInt32(ProductCatId);
                            if (ctId > 0)
                            {

                                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ctId)).ToList();
                                FreeSendOffer = from s in FreeSendOffer
                                                where
                                                (s.CatId != null && CatIds.Contains(s.CatId.Value))
                                                select s;

                            }
                        }
                        if (!String.IsNullOrEmpty(CityId))
                        {
                            int ctid = int.Parse(CityId);
                            if (ctid > 0)
                                FreeSendOffer = FreeSendOffer.Where(s => s.FreeSendOfferStates.Any(a => a.CityId == ctid));
                            else
                            {
                                int ProviencId = int.Parse(proviencId);
                                FreeSendOffer = FreeSendOffer.Where(s => s.FreeSendOfferStates.Any(a => a.City.ProvinceId == ProviencId));
                            }
                        }
                        if (!String.IsNullOrEmpty(BrandId))
                            FreeSendOffer = FreeSendOffer.Where(s => s.BrandId == int.Parse(BrandId));




                        #endregion

                        #region Sort
                        switch (sortOrder)
                        {

                            case "Brand":
                                FreeSendOffer = FreeSendOffer.OrderBy(s => s.Brand.Name);
                                ViewBag.CurrentSort = "Brand";
                                break;
                            case "Brand_desc":
                                FreeSendOffer = FreeSendOffer.OrderByDescending(s => s.Brand.Name);
                                ViewBag.CurrentSort = "Brand_desc";
                                break;
                            case "ProductCat":
                                FreeSendOffer = FreeSendOffer.OrderBy(s => s.ProductCategory.Title);
                                ViewBag.CurrentSort = "ProductCat";
                                break;
                            case "ProductCat_desc":
                                FreeSendOffer = FreeSendOffer.OrderByDescending(s => s.ProductCategory.Title);
                                ViewBag.CurrentSort = "ProductCat_desc";
                                break;
                            case "Product":
                                FreeSendOffer = FreeSendOffer.OrderBy(s => s.Product.Name);
                                ViewBag.CurrentSort = "Product";
                                break;
                            case "Product_desc":
                                FreeSendOffer = FreeSendOffer.OrderByDescending(s => s.Product.Name);
                                ViewBag.CurrentSort = "Product_desc";
                                break;
                            default:  // Name ascending 
                                FreeSendOffer = FreeSendOffer.OrderByDescending(s => s.Id);
                                break;
                        }

                        #endregion
                        ViewBag.FreeSendOffers = FreeSendOffer.ToPagedList(pageNumber, pageSize);
                    }
                    #endregion

                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();


                    ViewBag.LanguageId = new SelectList(uow.SettingRepository.Get(x => x), "LanguageId", "SettingName");
                    if (offer.ExpireDate.HasValue)
                        ViewBag.ExpireDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(offer.ExpireDate.Value) + " " + offer.ExpireDate.Value.ToString("HH:mm");
                    else
                        ViewBag.ExpireDate = "";

                    if (offer.StartDate.HasValue)
                        ViewBag.StartDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(offer.StartDate.Value) + " " + offer.StartDate.Value.ToString("HH:mm");
                    else
                        ViewBag.StartDate = "";


                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 37 && x.Name == "جزئیات تخفیف", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "Detail", true, 200, "نمایش صفحه جزئیات تخفیف   " + offer.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(offer);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "Detail", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        #endregion

        #region Code Gift
        public virtual ActionResult CodeDetail(int? id, int? PageSize, int? SearchType, string sortOrder, string UsernameFilter, string UsernameString, string CreationStartDateInput, string currentStartDateInputFiltering, string CreationEndDateInput, string currentEndDateInputFiltering, string LastActivityStartDateInput, string currentLastActivityStartDateInputFiltering, string LastActivityEndDateInput, string currentLastActivityEndDateInputFiltering, string PhoneNumberString, string PhoneNumberFilter, string UserGroupId, string UserGroupIdFiltering, string EmailConfirmId, string CurrentEmailConfirmId, string PhoneNumberConfirmId, string CurrentPhoneNumberConfirmId, string Disable, string CurrentDisable, string FullName, string FullNameFilter, string Gender, string GenderFilter, int? page)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 24);
                if (p.Where(x => x == true).Any())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var offer = uow.OfferRepository.Get(x => x, x => x.Id == id, null, "FreeSendOffers,ProductOffers,UserCodeGifts,offerProductCategories,offerUserGroups.UserGroup").FirstOrDefault();
                    if (offer == null)
                    {
                        return HttpNotFound();
                    }


                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    ViewBag.AddPermission = ModulePermission.check(User.Identity.GetUserId(), 8, null);

                    #region LoadDropdown


                    List<SelectListItem> EmailConfirmSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید شده", Value = "True" }, new SelectListItem() { Text = "تایید نشده", Value = "False" } };
                    ViewBag.EmailConfirmSelectListItem = EmailConfirmSelectListItem;
                    List<SelectListItem> PhoneNumberSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید شده", Value = "True" }, new SelectListItem() { Text = "تایید نشده", Value = "False" } };
                    ViewBag.PhoneNumberSelectListItem = PhoneNumberSelectListItem;
                    List<SelectListItem> DisableSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "False" }, new SelectListItem() { Text = "غیر فعال", Value = "True" } };
                    ViewBag.DisableSelectListItem = DisableSelectListItem;
                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;

                    if (offer.offerUserGroups.Any())
                        ViewBag.UserGroups = new SelectList(offer.offerUserGroups.Select(x => x.UserGroup).OrderBy(x => x.Name), "Id", "Name");
                    else

                        ViewBag.UserGroups = new SelectList(uow.UserGroupRepository.Get(x => x, null, x => x.OrderBy(s => s.Name)), "Id", "Name");

                    #endregion

                    #region Search
                    if (string.IsNullOrEmpty(UsernameString))
                        UsernameString = UsernameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(FullName))
                        FullName = FullNameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(Gender))
                        Gender = GenderFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(PhoneNumberString))
                        PhoneNumberString = PhoneNumberFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(UserGroupId))
                        UserGroupId = UserGroupIdFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(EmailConfirmId))
                        EmailConfirmId = CurrentEmailConfirmId;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(PhoneNumberConfirmId))
                        PhoneNumberConfirmId = CurrentPhoneNumberConfirmId;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(Disable))
                        Disable = CurrentDisable;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(CreationStartDateInput))
                        CreationStartDateInput = currentStartDateInputFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(CreationEndDateInput))
                        CreationEndDateInput = currentEndDateInputFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LastActivityStartDateInput))
                        LastActivityStartDateInput = currentLastActivityStartDateInputFiltering;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(LastActivityEndDateInput))
                        LastActivityEndDateInput = currentLastActivityEndDateInputFiltering;
                    else
                        page = 1;


                    ViewBag.UsernameFilter = UsernameString;
                    ViewBag.FullNameFilter = FullName;
                    ViewBag.GenderFilter = Gender;
                    ViewBag.PhoneNumberFilter = PhoneNumberString;
                    ViewBag.UserGroupIdFiltering = UserGroupId;
                    ViewBag.CurrentEmailConfirmId = EmailConfirmId;
                    ViewBag.CurrentPhoneNumberConfirmId = PhoneNumberConfirmId;
                    ViewBag.CurrentDisable = Disable;
                    ViewBag.currentStartDateInputFiltering = CreationStartDateInput;
                    ViewBag.currentEndDateInputFiltering = CreationEndDateInput;
                    ViewBag.currentLastActivityStartDateInputFiltering = LastActivityStartDateInput;
                    ViewBag.currentLastActivityEndDateInputFiltering = LastActivityEndDateInput;


                    var user = uow.UserRepository.GetByReturnQueryable(x => x, x => x.Email != "admin@admin.admin", null, "Avatarattachment,UserCodeGifts,UserGroupSelects.UserGroup");
                    if (offer.offerUserGroups.Any() && offer.codeUseType==CodeUseType.ثبت_در_پنل_مدیریت)
                    {
                        var ids = offer.offerUserGroups.Select(x => x.UserGroup.Id);
                        user = user.Where(x => x.UserGroupSelects.Any(s => ids.Contains(s.userGroup.Id)));
                    }
                    else if(offer.codeUseType==CodeUseType.ثبت_پس_از_ثبت_نظر_محصول || offer.codeUseType==CodeUseType.ثبت_پس_از_ثبت_نظر_سفارش)
                    {
                        var ids = offer.UserCodeGifts.Select(x => x.UserId);
                        user = user.Where(x => ids.Contains(x.Id));
                    }

                    //Get All user except Admin and superadmin role , if current Role's user is Security
                    if (UserManager.IsInRole(User.Identity.GetUserId(), "Security"))
                    {
                        var Roleusers = uow.UserRepository.Get(x => x, r => r.Roles.Any(x => x.RoleId != "1" || x.RoleId != "5"));
                        string[] roleIds = Roleusers.Select(r => r.Id).ToArray();
                        user = user.Where(x => roleIds.Contains(x.Id));
                    }

                    if (!String.IsNullOrEmpty(UsernameString))
                        user = user.Where(s => s.UserName.Contains(UsernameString));
                    if (!String.IsNullOrEmpty(Gender))
                    {
                        bool gndr = Convert.ToBoolean(Gender);
                        user = user.Where(s => s.Gender == gndr);
                    }
                    if (!String.IsNullOrEmpty(FullName))
                        user = user.Where(s => s.FirstName.Contains(FullName) || s.LastName.Contains(FullName));
                    if (!String.IsNullOrEmpty(PhoneNumberString))
                        user = user.Where(s => s.PhoneNumber.Contains(PhoneNumberString));
                    if (!String.IsNullOrEmpty(UserGroupId))
                    {
                        int gid = Convert.ToInt32(UserGroupId);
                        user = user.Where(x => x.UserGroupSelects.Any(s => s.userGroupId == gid));

                    }
                    if (!String.IsNullOrEmpty(EmailConfirmId))
                    {
                        bool ec = Convert.ToBoolean(EmailConfirmId);
                        user = user.Where(s => s.EmailConfirmed == ec);
                    }
                    if (!String.IsNullOrEmpty(PhoneNumberConfirmId))
                    {
                        bool pc = Convert.ToBoolean(PhoneNumberConfirmId);
                        user = user.Where(s => s.PhoneNumberConfirmed == pc);
                    }
                    if (!String.IsNullOrEmpty(Disable))
                    {
                        bool ds = Convert.ToBoolean(Disable);
                        user = user.Where(s => s.Disable == ds);
                    }
                    #region Creation Date
                    DateTime startCreationDate = DateTime.Now, endCreationDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(CreationStartDateInput))
                        startCreationDate = DateTimeConverter.ChangeShamsiToMiladi(CreationStartDateInput);
                    if (!String.IsNullOrEmpty(CreationEndDateInput))
                        endCreationDate = DateTimeConverter.ChangeShamsiToMiladi(CreationEndDateInput);
                    if (!String.IsNullOrEmpty(CreationStartDateInput) && !String.IsNullOrEmpty(CreationEndDateInput))
                        user = user.Where(s => s.CreationDate >= startCreationDate && s.CreationDate <= endCreationDate);
                    else if (!String.IsNullOrEmpty(CreationStartDateInput))
                        user = user.Where(s => s.CreationDate >= startCreationDate);
                    else if (!String.IsNullOrEmpty(CreationEndDateInput))
                        user = user.Where(s => s.CreationDate <= endCreationDate);
                    #endregion
                    #region Last Activity Date
                    DateTime startLastActivityDate = DateTime.Now, endLastActivityDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(LastActivityStartDateInput))
                        startLastActivityDate = DateTimeConverter.ChangeShamsiToMiladi(LastActivityStartDateInput);
                    if (!String.IsNullOrEmpty(LastActivityEndDateInput))
                        endLastActivityDate = DateTimeConverter.ChangeShamsiToMiladi(LastActivityEndDateInput);
                    if (!String.IsNullOrEmpty(LastActivityStartDateInput) && !String.IsNullOrEmpty(LastActivityEndDateInput))
                        user = user.Where(s => s.LastActivityDate >= startLastActivityDate && s.LastActivityDate <= endLastActivityDate);
                    else if (!String.IsNullOrEmpty(LastActivityStartDateInput))
                        user = user.Where(s => s.LastActivityDate >= startLastActivityDate);
                    else if (!String.IsNullOrEmpty(LastActivityEndDateInput))
                        user = user.Where(s => s.LastActivityDate <= endLastActivityDate);
                    #endregion

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "UserName":
                            user = user.OrderBy(s => s.UserName);
                            ViewBag.CurrentSort = "UserName";
                            break;
                        case "UserName_desc":
                            user = user.OrderByDescending(s => s.UserName);
                            ViewBag.CurrentSort = "UserName_desc";
                            break;
                        case "FullName":
                            user = user.OrderBy(s => s.FirstName + " " + s.LastName);
                            ViewBag.CurrentSort = "FullName";
                            break;
                        case "FullName_desc":
                            user = user.OrderByDescending(s => s.FirstName + " " + s.LastName);
                            ViewBag.CurrentSort = "FullName_desc";
                            break;
                        case "Gender":
                            user = user.OrderBy(s => s.Gender);
                            ViewBag.CurrentSort = "Gender";
                            break;
                        case "Gender_desc":
                            user = user.OrderByDescending(s => s.Gender);
                            ViewBag.CurrentSort = "Gender_desc";
                            break;
                        default:  // Name ascending 
                            user = user.OrderByDescending(s => s.CreationDate);
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

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت کاربران", x => x.OrderBy(o => o.Id), "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserManagement", "Index", true, 200, " نمایش صفحه مدیریت کاربران", DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    if (SearchType.HasValue)
                    {
                        ViewBag.SearchType = SearchType.Value;
                    }
                    else
                        ViewBag.SearchType = Convert.ToInt32(offer.CodeTypeValueCode == 0 ? 2 : 1);

                    ViewBag.users = user.ToPagedList(pageNumber, pageSize);


                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 37 && x.Name == "جزئیات تخفیف", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "CodeDetail", true, 200, "نمایش صفحه جزئیات تخفیف   " + offer.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(offer);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CodeDetail", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public virtual JsonResult UpdateCodeGift(int OfferId, int UserCodeGiftId, int Value, string Code, int MaxValue, int CountUse, bool IsActive, short CodeType, string UserId)
        {
            try
            {
                if (Value > 0 && CodeType > 0 && !String.IsNullOrEmpty(Code))
                {
                    var UserCodeGift = uow.UserCodeGiftRepository.GetByID(UserCodeGiftId);
                    if (UserCodeGift != null)
                    {
                        UserCodeGift.Value = Value;
                        UserCodeGift.Code = Code;
                        UserCodeGift.CodeType = CodeType;
                        UserCodeGift.CountUse = CountUse;
                        UserCodeGift.MaxValue = MaxValue;
                        UserCodeGift.IsActive = IsActive;
                        uow.UserCodeGiftRepository.Update(UserCodeGift);
                    }
                    else
                    {
                        UserCodeGift userCodeGift = new UserCodeGift()
                        {
                            Value = Value,
                            Code = Code,
                            CodeType = CodeType,
                            CountUse = CountUse,
                            MaxValue = MaxValue,
                            IsActive = IsActive,
                            OfferId = OfferId,
                            UserId = UserId
                        };
                        uow.UserCodeGiftRepository.Insert(userCodeGift);
                    }
                    uow.Save();



                    return Json(new
                    {
                        message = "ثبت شد.",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (OfferId > 0 && UserCodeGiftId > 0 && (Value <= 0 || CodeType == 0 || String.IsNullOrEmpty(Code)))
                {
                    try
                    {
                        var UserCodeGift = uow.UserCodeGiftRepository.GetByID(UserCodeGiftId);
                        if (UserCodeGift != null)
                        {
                            UserCodeGift.Value = 0;
                            UserCodeGift.Code = Code;
                            UserCodeGift.CodeType = CodeType;
                            UserCodeGift.CountUse = 0;
                            UserCodeGift.MaxValue = 0;
                            UserCodeGift.IsActive = false;
                            uow.UserCodeGiftRepository.Update(UserCodeGift);
                        }

                        uow.Save();
                        return Json(new
                        {
                            message = "تخفیف حذف شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        return Json(new
                        {
                            message = "تخفیف حذف نشد. این تخفیف در سفارش استفاده شده است.",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        message = "تخفیف ثبت نشد. همه مقادیر را وارد نمایید",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }

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



        public virtual JsonResult AddUserCodeGiftGroup(int UserGroupId, int OfferId, short codeType, string codeValueCat, int ValueCat, int MaxValueCat, int CountUseCat)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var date = DateTime.Now;
                var userGroup = uow.UserGroupSelectRepository.Get(x => x.UserId, x => x.userGroupId == UserGroupId);
                foreach (string item in userGroup)
                {
                    if (!uow.UserCodeGiftRepository.Any(x => x.Id, x => x.Offer.IsActive && x.Offer.state && ((x.Offer.ExpireDate != null && x.Offer.ExpireDate >= date) || x.Offer.ExpireDate == null) && ((x.Offer.StartDate != null && x.Offer.StartDate <= date) || x.Offer.StartDate == null) && x.UserId == item))
                    {
                        string code = codeValueCat;
                        if (String.IsNullOrEmpty(code))
                            code = CoreLib.Infrastructure.CommonFunctions.RandomString(4);

                        UserCodeGift userCodeGift = new UserCodeGift()
                        {
                            Value = ValueCat,
                            Code = code,
                            CodeType = codeType,
                            CountUse = CountUseCat,
                            MaxValue = MaxValueCat,
                            IsActive = true,
                            OfferId = OfferId,
                            UserId = item
                        };
                        uow.UserCodeGiftRepository.Insert(userCodeGift);
                    }
                }

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
        #endregion

        #region General Code Gift 
        public virtual ActionResult GeneralCodeDetail(int? id)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 24);
                if (p.Where(x => x == true).Any())
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var offer = uow.OfferRepository.Get(x => x, x => x.Id == id, null, "generalCodeGifts").FirstOrDefault();
                    if (offer == null)
                    {
                        return HttpNotFound();
                    }

                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت کاربران", x => x.OrderBy(o => o.Id), "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "UserManagement", "Index", true, 200, " نمایش صفحه مدیریت کاربران", DateTime.Now, User.Identity.GetUserId());
                    #endregion



                    ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 37 && x.Name == "جزئیات تخفیف", null, "HelpModuleSectionFields").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "GeneralCodeDetail", true, 200, "نمایش صفحه جزئیات تخفیف   " + offer.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(offer);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "GeneralCodeDetail", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public virtual ActionResult CreateGeneralCode(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 24, 1))
                {
                    var offer = uow.OfferRepository.Get(x => x, x => x.Id == id.Value).SingleOrDefault();
                    if (offer == null)
                        return Redirect("~/Admin");

                    ViewBag.offer = offer;

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "CreateFreeSend", true, 200, "نمایش صفحه ایجاد ارسال رایگان", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CreateFreeSend", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult CreateGeneralCode(GeneralCodeGift GeneralCodeGift)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            short langid = setting.LanguageId.Value;
            try
            {
                if (ModelState.IsValid)
                {

                    uow.GeneralCodeGiftRepository.Insert(GeneralCodeGift);
                    uow.Save();

                    CheckState(GeneralCodeGift.OfferId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "CreateCodeDetail", false, 200, "ایجاد کد تخفیف عمومی", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("GeneralCodeDetail", new { id = GeneralCodeGift.OfferId });
                }
                ViewBag.Error = " خطایی رخ داد ";
                ViewBag.offer = uow.OfferRepository.Get(x => x, x => x.Id == GeneralCodeGift.OfferId).SingleOrDefault();
                return View(GeneralCodeGift);
            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CreateCodeDetail", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public virtual ActionResult EditGeneralCode(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 24, 1))
                {
                    var GeneralCodeGift = uow.GeneralCodeGiftRepository.Get(x => x, x => x.Id == id.Value).SingleOrDefault();
                    if (GeneralCodeGift == null)
                        return Redirect("~/Admin");


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "EditGeneralCode", true, 200, "نمایش صفحه ایجاد ارسال رایگان", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(GeneralCodeGift);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "EditGeneralCode", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult EditGeneralCode(GeneralCodeGift GeneralCodeGift)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            short langid = setting.LanguageId.Value;
            try
            {
                if (ModelState.IsValid)
                {

                    uow.GeneralCodeGiftRepository.Update(GeneralCodeGift);
                    uow.Save();


                    CheckState(GeneralCodeGift.OfferId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "EditGeneralCode", false, 200, "ویرایش کد تخفیف عمومی", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("GeneralCodeDetail", new { id = GeneralCodeGift.OfferId });
                }
                ViewBag.Error = " خطایی رخ داد ";
                return View(GeneralCodeGift);
            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "EditGeneralCode", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }



        // GET: Admin/Gallery/Delete/5
        public virtual ActionResult DeleteGeneralCode(int? id)
        {
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 24, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var GeneralCodeGift = uow.GeneralCodeGiftRepository.GetByID(id);
                    if (GeneralCodeGift == null)
                    {
                        return HttpNotFound();
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "DeleteGeneralCode", true, 200, "نمایش صفحه حذف تخفیف عمومی ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(GeneralCodeGift);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "DeleteGeneralCode", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Galleries/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteGeneralCode(int id)
        {
            try
            {
                var GeneralCodeGift = uow.GeneralCodeGiftRepository.GetByID(id);
                int offerId = GeneralCodeGift.OfferId;
                uow.GeneralCodeGiftRepository.Delete(GeneralCodeGift);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Offers", "DeleteGeneralCode", false, 200, " حذف تخفیف عمومی ", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("GeneralCodeDetail", new { id = offerId });
            }
            catch (Exception x)
            {
                var gallery = uow.GalleryCategoryRepository.GetByID(id);
                ViewBag.Erorr = "خطایی رخ داد" + x.Message;
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "DeleteGeneralCode", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View(gallery);
            }
        }

        #endregion

        #region FreeSendDetail
        // GET: Admin/Offers/CreateFreeSend
        public virtual ActionResult CreateFreeSend(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 1))
                {

                    ViewBag.OfferId = id.Value;
                    short langid = setting.LanguageId.Value;

                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.BrandId = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "CreateFreeSend", true, 200, "نمایش صفحه ایجاد ارسال رایگان", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CreateFreeSend", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POst: Admin/Offers/CreateFreeSend
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult CreateFreeSend(FreeSendOffer FreeSendOffer, int? ProductCatId)
        {
            ViewBag.OfferId = FreeSendOffer.OfferId;
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            short langid = setting.LanguageId.Value;

            ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
            ViewBag.BrandId = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name");
            ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);


            try
            {
                if (ModelState.IsValid)
                {

                    if (ProductCatId.HasValue)
                    {
                        if (ProductCatId.Value == 0)
                            ProductCatId = null;
                        else
                            FreeSendOffer.BrandId = null;
                    }
                    else if (FreeSendOffer.BrandId.HasValue)
                    {
                        ProductCatId = null;
                    }

                    if (!FreeSendOffer.BrandId.HasValue && !ProductCatId.HasValue)
                    {

                        ViewBag.Error = "از بین گروه،زیرگروه یا محصول و برند باید یکی انتخاب گردد !";
                        return View(FreeSendOffer);
                    }

                    if (uow.FreeSendOfferRepository.Any(x => x, x => x.OfferId == FreeSendOffer.OfferId && x.BrandId == FreeSendOffer.BrandId && x.CatId == ProductCatId && x.ProductId == FreeSendOffer.ProductId))
                    {
                        ViewBag.Error = "این محصولات برای این تخفیف قبلا انتخاب شده اند !";
                        return View(FreeSendOffer);
                    }

                    if (!FreeSendOffer.OrderSum.HasValue)
                    {
                        ViewBag.Error = " مبلغ سفارش را انتخاب نمایید ";
                        return View(FreeSendOffer);
                    }
                    if (ProductCatId.HasValue)
                        FreeSendOffer.CatId = ProductCatId.Value;
                    uow.FreeSendOfferRepository.Insert(FreeSendOffer);
                    uow.Save(); CheckState(FreeSendOffer.OfferId);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "CreateFreeSend", false, 200, "ایجاد تخفیف ارسال رایگان با آی دیِ " + FreeSendOffer.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Detail", new { id = FreeSendOffer.OfferId });
                }
                ViewBag.Error = " خطایی رخ داد ";
                return View(FreeSendOffer);
            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CreateFreeSend", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Offers/EditFreeSend
        public virtual ActionResult EditFreeSend(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 1))
                {
                    var FreeSendOffer = uow.FreeSendOfferRepository.Get(x => x, x => x.Id == id.Value).FirstOrDefault();
                    if (FreeSendOffer == null)
                        return RedirectToAction("Index");

                    ViewBag.OfferId = FreeSendOffer.OfferId;

                    short langid = setting.LanguageId.Value;

                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name", FreeSendOffer.BrandId);
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                    if (FreeSendOffer.ProductId.HasValue)
                        ViewBag.ProductList = new SelectList(uow.ProductRepository.Get(x => x, x => x.LanguageId == langid && x.ProductCategories.Any(s => s.Id == FreeSendOffer.CatId.Value), x => x.OrderBy(s => s.Name)), "Id", "Name", FreeSendOffer.ProductId);
                    else
                        ViewBag.ProductList = new SelectListItem();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "EditFreeSend", true, 200, "نمایش صفحه ویرایش تخفیف ارسال رایگان", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(FreeSendOffer);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "EditFreeSend", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }


        // POst: Admin/Offers/EditFreeSend
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult EditFreeSend(FreeSendOffer FreeSendOffer, int? ProductCatId)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            short langid = setting.LanguageId.Value;
            ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
            ViewBag.BrandList = new SelectList(uow.BrandRepository.Get(x => x, x => x.LanguageId == langid, x => x.OrderBy(s => s.Name)), "Id", "Name", FreeSendOffer.BrandId);
            ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
            if (FreeSendOffer.ProductId.HasValue)
                ViewBag.ProductList = new SelectList(uow.ProductRepository.Get(x => x, x => x.LanguageId == langid && x.ProductCategories.Any(s => s.Id == FreeSendOffer.CatId.Value), x => x.OrderBy(s => s.Name)), "Id", "Name", FreeSendOffer.ProductId);
            else
                ViewBag.ProductList = new SelectListItem();

            try
            {
                if (ModelState.IsValid)
                {

                    if (ProductCatId.HasValue)
                    {
                        if (ProductCatId.Value == 0)
                            ProductCatId = null;
                        else
                            FreeSendOffer.BrandId = null;
                    }
                    else if (FreeSendOffer.BrandId.HasValue)
                    {
                        ProductCatId = null;
                    }

                    if (!FreeSendOffer.BrandId.HasValue && !ProductCatId.HasValue)
                    {

                        ViewBag.Error = "از بین گروه،زیرگروه یا محصول و برند باید یکی انتخاب گردد !";
                        return View(FreeSendOffer);
                    }

                    if (uow.FreeSendOfferRepository.Any(x => x, x => x.OfferId == FreeSendOffer.OfferId && x.BrandId == FreeSendOffer.BrandId && x.CatId == ProductCatId && x.ProductId == FreeSendOffer.ProductId && x.Id != FreeSendOffer.Id))
                    {
                        ViewBag.Error = "این محصولات برای این تخفیف قبلا انتخاب شده اند !";
                        return View(FreeSendOffer);
                    }
                    if (!FreeSendOffer.OrderSum.HasValue)
                    {
                        ViewBag.Error = " مبلغ سفارش یکی را انتخاب نمایید ";
                        return View(FreeSendOffer);
                    }

                    if (ProductCatId.HasValue)
                        FreeSendOffer.CatId = ProductCatId.Value;
                    uow.FreeSendOfferRepository.Update(FreeSendOffer);
                    uow.Save(); CheckState(FreeSendOffer.OfferId);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "EditFreeSend", false, 200, "ویرایش تخفیف ارسال رایگان با آی دیِ " + FreeSendOffer.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Detail", new { id = FreeSendOffer.OfferId });
                }
                ViewBag.Error = " خطایی رخ داد ";
                return View(FreeSendOffer);
            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "EditFreeSend", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }



        // GET: Admin/Offers/DeleteFreeSend/5
        public virtual ActionResult DeleteFreeSend(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    FreeSendOffer FreeSendOffer = uow.FreeSendOfferRepository.GetByID(id);


                    if (FreeSendOffer == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "DeleteFreeSend", true, 200, " نمایش صفحه حذفِ تخفیف ارسال رایگانِ " + FreeSendOffer.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(FreeSendOffer);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "DeleteFreeSend", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Offers/DeleteFreeSend/5
        [HttpPost, ActionName("DeleteFreeSend")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteFreeSend(int id)
        {
            Domain.FreeSendOffer FreeSendOffer = uow.FreeSendOfferRepository.GetByID(id);


            try
            {
                int OfferId = FreeSendOffer.OfferId;
                uow.FreeSendOfferRepository.Delete(FreeSendOffer);
                uow.Save(); CheckState(FreeSendOffer.OfferId);

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Offers", "DeleteFreeSend", false, 200, " حذفِ تخفیف ارسال رایگانِ " + FreeSendOffer.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Detail", "Offers", new { id = OfferId });

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "DeleteFreeSend", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " تخفیف محصول انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(FreeSendOffer);
            }
        }


        // GET: Admin/Offers/FreeSendCities
        public virtual ActionResult FreeSendCities(int? id, string sortOrder, string CityId, string CityIdFilter, string proviencId, string proviencIdFilter, int? page)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 24);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name");
                    ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name");
                    #region search  
                    if (string.IsNullOrEmpty(proviencId))
                        proviencId = proviencIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(CityId))
                        CityId = CityIdFilter;
                    else
                        page = 1;




                    ViewBag.proviencIdFilter = proviencId;
                    ViewBag.CityIdFilter = CityId;

                    var FreeSendOfferStates = uow.FreeSendOfferStateRepository.GetByReturnQueryable(x => x, x => x.FreeSendOfferId == id, x => x.OrderByDescending(s => s.Id));


                    if (!String.IsNullOrEmpty(CityId))
                    {
                        int ctid = int.Parse(CityId);
                        if (ctid > 0)
                            FreeSendOfferStates = FreeSendOfferStates.Where(s => s.CityId == ctid);
                        else
                        {
                            int ProviencId = int.Parse(proviencId);
                            FreeSendOfferStates = FreeSendOfferStates.Where(s => s.City.ProvinceId == ProviencId);
                        }
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {

                        case "CityId":
                            FreeSendOfferStates = FreeSendOfferStates.OrderBy(s => s.City.Name);
                            ViewBag.CurrentSort = "CityId";
                            break;
                        case "CityId_desc":
                            FreeSendOfferStates = FreeSendOfferStates.OrderByDescending(s => s.City.Name);
                            ViewBag.CurrentSort = "CityId_desc";
                            break;
                        case "ProvinceId":
                            FreeSendOfferStates = FreeSendOfferStates.OrderBy(s => s.City.Province.Name);
                            ViewBag.CurrentSort = "ProvinceId";
                            break;
                        case "ProvinceId_desc":
                            FreeSendOfferStates = FreeSendOfferStates.OrderByDescending(s => s.City.Province.Name);
                            ViewBag.CurrentSort = "ProvinceId_desc";
                            break;

                        default:  // Name ascending 
                            FreeSendOfferStates = FreeSendOfferStates.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion

                    int pageSize = 20;
                    int pageNumber = (page ?? 1);

                    ViewBag.FreeSendOffer = uow.FreeSendOfferRepository.Get(x => x, x => x.Id == id.Value).FirstOrDefault();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "FreeSendCities", true, 200, "نمایش صفحه مناطق ارسال رایگانِ " + id.Value, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(FreeSendOfferStates.ToPagedList(pageNumber, pageSize));

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "FreeSendCities", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/Offers/CreateFreeSend
        public virtual ActionResult CreateCity(int? id)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 1))
                {
                    var FreeSendDetail = uow.FreeSendOfferRepository.Get(x => x, x => x.Id == id.Value).FirstOrDefault();
                    if (FreeSendDetail == null)
                        return RedirectToAction("FreeSendCities", new { id = id.Value });

                    ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name");
                    ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name");
                    ViewBag.FreeSendDetail = FreeSendDetail;


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "CreateCity", true, 200, "نمایش صفحه ایجاد شهر برای ارسال رایگانِ " + id.Value, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CreateCity", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POst: Admin/Offers/CreateFreeSend
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult CreateCity(FreeSendOfferState FreeSendOfferState, int? ProvinceId)
        {
            var FreeSendDetail = uow.FreeSendOfferRepository.Get(x => x, x => x.Id == FreeSendOfferState.Id).FirstOrDefault();
            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name");
            ViewBag.CityList = new SelectList(uow.CityRepository.Get(x => x), "Id", "Name");
            ViewBag.FreeSendDetail = FreeSendDetail;

            try
            {
                if (ModelState.IsValid)
                {
                    if (!ProvinceId.HasValue)
                    {
                        ViewBag.Error = "استان و شهر انتخاب نشده است !";
                        return View(FreeSendOfferState);
                    }

                    if (FreeSendOfferState.CityId == 0)
                    {
                        foreach (var item in uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId))
                        {
                            if (!uow.FreeSendOfferStateRepository.Get(x => x, x => x.FreeSendOfferId == FreeSendOfferState.FreeSendOfferId && x.CityId == item.Id).Any())
                            {

                                FreeSendOfferState.CityId = item.Id;
                                uow.FreeSendOfferStateRepository.Insert(FreeSendOfferState);
                                uow.Save();
                            }
                        }

                    }
                    else
                    {
                        if (!uow.FreeSendOfferStateRepository.Get(x => x, x => x.FreeSendOfferId == FreeSendOfferState.FreeSendOfferId && x.CityId == FreeSendOfferState.CityId).Any())
                        {
                            uow.FreeSendOfferStateRepository.Insert(FreeSendOfferState);
                            uow.Save();
                        }
                        else
                        {
                            ViewBag.Error = "رکورد انتخابی قبلا وارد شده است !";
                            return View(FreeSendOfferState);
                        }
                    }



                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "CreateCity", false, 200, "ایجاد شهر برای تخفیفِ ارسال رایگان با آی دیِ " + FreeSendOfferState.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("FreeSendCities", new { id = FreeSendOfferState.FreeSendOfferId });
                }
                ViewBag.Error = " خطایی رخ داد ";
                return View(FreeSendOfferState);
            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CreateCity", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Offers/DeleteCity/5
        public virtual ActionResult DeleteCity(int? id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Domain.FreeSendOfferState FreeSendOfferState = uow.FreeSendOfferStateRepository.GetByID(id);


                    if (FreeSendOfferState == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "DeleteCity", true, 200, " نمایش صفحه حذفِ شهر ارسال رایگانِ" + FreeSendOfferState.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(FreeSendOfferState);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "تخفیف ها" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "DeleteCity", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // POST: Admin/Offers/DeleteCity/5
        [HttpPost, ActionName("DeleteCity")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteCity(int id)
        {
            uow = new UnitOfWork.UnitOfWorkClass();


            var psos = uow.FreeSendOfferStateRepository.GetByID(id);
            try
            {
                uow.FreeSendOfferStateRepository.Delete(psos);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "Offer", "DeleteCity", false, 200, " حذفِ شهر ارسال رایگانِ " + id, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("FreeSendCities", new { id = psos.FreeSendOfferId });

            }
            catch (Exception s)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "DeleteCity", false, 500, s.Message, DateTime.Now, User.Identity.GetUserId());

                #endregion
                ViewBag.Erorr = " شهر انتخابی در حال استفاده می باشد و نمیتوانید آن را حذف نمایید.  " + s.Message;
                return View(psos);
            }
        }

        #endregion

        #region UserOfferMessage
        public ActionResult UserOfferMessages(int id)
        {
            var offer = uow.OfferRepository.Get(x => x, x => x.Id == id, null, "userOfferMessages").SingleOrDefault();
            if (offer == null)
                return Redirect("~/Admin/Offers");
            var p = ModulePermission.check(User.Identity.GetUserId(), 24);
            if (p.Where(x => x == true).Any())
            {

                ViewBag.AddPermission = p.First();
                ViewBag.EditPermission = p.Skip(1).First();
                ViewBag.DeletePermission = p.Skip(2).First();
                return View(offer);
            }
            return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));
        }


        public virtual ActionResult CreateUserOfferMessages(int id)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var offer = uow.OfferRepository.Get(x => x, x => x.Id == id, null, "userOfferMessages").SingleOrDefault();
                if (offer == null)
                    return Redirect("~/Admin/Offers");

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 1))
                {
                    ViewBag.offer = offer;
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "CreateUserOfferMessages", true, 200, "نمایش صفحه ایجاد پیام تخفیف", DateTime.Now, User.Identity.GetUserId());

                    return View();
                }
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));



            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "CreateUserOfferMessages", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult CreateUserOfferMessages(UserOfferMessage UserOfferMessage)
        {

            //CoreLib.Infrastructure.SMS.HeroSMSManager.send("09385060192", UserOfferMessage.Text);

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    UserOfferMessage.InsertDate = DateTime.Now;
                    uow.UserOfferMessageRepository.Insert(UserOfferMessage);
                    uow.Save();

                    var codeUseType = uow.OfferRepository.Get(x => x.codeUseType, x => x.Id == UserOfferMessage.OfferId).First();

                    //add to messages
                    var userGroupIds = uow.OfferUserGroupRepository.Get(x => x.UserGroupId, x => x.OfferId == UserOfferMessage.OfferId);
                    if (userGroupIds.Any())
                    {
                        var users = uow.UserRepository.Get(x => new UserOfferMessageMember() { InsertDate = DateTime.Now, UserId = x.Id, UserOfferMessageId = UserOfferMessage.Id, state = OfferMessageSendMessageType.Waiting }, x => x.UserGroupSelects.Any(a => userGroupIds.Contains(a.userGroupId)));
                        uow.UserOfferMessageMemberRepository.InsertList(users.ToList());
                        uow.Save();
                    }
                    else if(codeUseType==CodeUseType.ثبت_در_پنل_مدیریت)
                    {
                        uow.UserOfferMessageMemberRepository.InsertList(uow.UserRepository.Get(x => new UserOfferMessageMember() { InsertDate = DateTime.Now, UserId = x.Id, UserOfferMessageId = UserOfferMessage.Id, state = OfferMessageSendMessageType.Waiting }).ToList());
                        uow.Save();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "CreateUserOfferMessages", false, 200, "ایجاد پیام تخفیف  " + UserOfferMessage.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("UserOfferMessages", new { id = UserOfferMessage.OfferId });
                }

                ViewBag.offer = uow.OfferRepository.Get(x => x, x => x.Id == UserOfferMessage.OfferId).First();
                return View(UserOfferMessage);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "CreateUserOfferMessages", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        public virtual ActionResult EditUserOfferMessage(int id)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var UserOfferMessage = uow.UserOfferMessageRepository.Get(x => x, x => x.Id == id).SingleOrDefault();
                if (UserOfferMessage == null)
                    return Redirect("~/Admin/Offers");

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 2))
                {
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "EditUserOfferMessages", true, 200, "نمایش صفحه ویرایش پیام تخفیف", DateTime.Now, User.Identity.GetUserId());

                    return View(UserOfferMessage);
                }
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));



            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "EditUserOfferMessages", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult EditUserOfferMessage(UserOfferMessage UserOfferMessage)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                if (ModelState.IsValid)
                {
                    uow.UserOfferMessageRepository.Update(UserOfferMessage);
                    uow.Save();



                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "EditUserOfferMessages", false, 200, "ویرایش پیام تخفیف  " + UserOfferMessage.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("UserOfferMessages", new { id = UserOfferMessage.OfferId });
                }

                return View(UserOfferMessage);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "EditUserOfferMessages", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public virtual ActionResult DeleteUserOfferMessage(int id)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var UserOfferMessage = uow.UserOfferMessageRepository.Get(x => x, x => x.Id == id).SingleOrDefault();
                if (UserOfferMessage == null)
                    return Redirect("~/Admin/Offers");

                if (ModulePermission.check(User.Identity.GetUserId(), 24, 3))
                {
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Offers", "DeleteUserOfferMessage", true, 200, "نمایش صفحه حذف پیام تخفیف", DateTime.Now, User.Identity.GetUserId());

                    return View(UserOfferMessage);
                }
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));



            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "DeleteUserOfferMessage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult DeleteUserOfferMessage(UserOfferMessage UserOfferMessage)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var UserOfferMessageMembers = uow.UserOfferMessageMemberRepository.Get(x => x, x => x.UserOfferMessageId == UserOfferMessage.Id);
                uow.UserOfferMessageMemberRepository.Delete(UserOfferMessageMembers.ToList());
                uow.Save();

                int id = UserOfferMessage.OfferId;
                uow.UserOfferMessageRepository.Delete(UserOfferMessage);
                uow.Save();

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(2, "Offer", "DeleteUserOfferMessage", false, 200, "حذف پیام تخفیف  " + UserOfferMessage.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("UserOfferMessages", new { id = id });

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offer", "DeleteUserOfferMessage", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public JsonResult SetState(int stateId)
        {
            try
            {
                var UserOfferMessage = uow.UserOfferMessageRepository.GetByID(stateId);
                UserOfferMessage.state = false;
                uow.UserOfferMessageRepository.Update(UserOfferMessage);
                uow.Save();

                var UserOfferMessageMembers = uow.UserOfferMessageMemberRepository.Get(x => x, x => x.UserOfferMessageId == stateId);
                uow.UserOfferMessageMemberRepository.SqlQuery("EXEC [SetUserSendOfferMessageState] @id=@Id,@userMessageId=@userMessageId", new System.Data.SqlClient.SqlParameter("Id", String.Join(",", UserOfferMessageMembers.Select(x => x.Id).ToArray())), new System.Data.SqlClient.SqlParameter("userMessageId", stateId));
                uow.Save();


                return Json(new
                {
                    Message = "انجام شد.",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد" + ex.Message,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult UserOfferMessageMembers(int id, string sortOrder, int? PageSize, string NameString, string NameFilter, string state, string stateFilter, int? page)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            try
            {
                var p = ModulePermission.check(User.Identity.GetUserId(), 24);
                if (p.Where(x => x == true).Any())
                {
                    var useroffermessage = uow.UserOfferMessageRepository.GetByID(id);
                    if (useroffermessage == null)
                        return Redirect("~/Admin/Offers");

                    ViewBag.useroffermessage = useroffermessage;

                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "ارسال شده", Value = "1" }, new SelectListItem() { Text = "ارسال نشده", Value = "0" } };
                    ViewBag.IsActiveList = IsActiveSelectListItem;

                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(state))
                        state = stateFilter;
                    else
                        page = 1;

                    ViewBag.NameFilter = NameString;
                    ViewBag.stateFilter = state;

                    int pageSize = 10;
                    if (PageSize.HasValue)
                    {
                        if (PageSize.Value > 100)
                            pageSize = 100;
                        else if (PageSize < 10)
                            pageSize = 10;
                        else
                            pageSize = PageSize.Value;
                    }

                    ViewBag.PageSize = pageSize;
                    int pageNumber = (page ?? 1);

                    var UserOfferMessageMembers = uow.UserOfferMessageMemberRepository.GetQueryList().AsNoTracking().Include("User").Where(x => x.UserOfferMessageId == id);

                    int countAll = UserOfferMessageMembers.Count();
                    int CountAllSent = UserOfferMessageMembers.Where(x => x.state == OfferMessageSendMessageType.Sent).Count();
                    ViewBag.countAll = countAll;
                    ViewBag.CountAllSent = CountAllSent;
                    ViewBag.SentPercent = (CountAllSent * 1.0 / countAll * 1.0) * 100;

                    if (!String.IsNullOrEmpty(state))
                    {
                        var state2 = Convert.ToInt16(state);
                        UserOfferMessageMembers = UserOfferMessageMembers.Where(s => s.state == (OfferMessageSendMessageType)state2);
                    }
                    if (!String.IsNullOrEmpty(NameString))
                        UserOfferMessageMembers = UserOfferMessageMembers.Where(s => s.User.UserName.Contains(NameString) || s.User.FirstName.Contains(NameString) || s.User.LastName.Contains(NameString));


                    #region Sort
                    switch (sortOrder)
                    {
                        case "state":
                            UserOfferMessageMembers = UserOfferMessageMembers.OrderBy(s => s.state);
                            ViewBag.CurrentSort = "state";
                            break;
                        case "state_desc":
                            UserOfferMessageMembers = UserOfferMessageMembers.OrderByDescending(s => s.state);
                            ViewBag.CurrentSort = "state_desc";
                            break;
                        case "mobile":
                            UserOfferMessageMembers = UserOfferMessageMembers.OrderBy(s => s.User.PhoneNumber);
                            ViewBag.CurrentSort = "mobile";
                            break;
                        case "mobile_desc":
                            UserOfferMessageMembers = UserOfferMessageMembers.OrderByDescending(s => s.User.PhoneNumber);
                            ViewBag.CurrentSort = "mobile_desc";
                            break;
                        case "fullname":
                            UserOfferMessageMembers = UserOfferMessageMembers.OrderBy(s => s.User.FirstName).ThenBy(s => s.User.FirstName);
                            ViewBag.CurrentSort = "fullname";
                            break;
                        case "fullname_desc":
                            UserOfferMessageMembers = UserOfferMessageMembers.OrderByDescending(s => s.User.FirstName).OrderByDescending(s => s.User.FirstName);
                            ViewBag.CurrentSort = "fullname_desc";
                            break;
                        default:  // Name ascending 
                            UserOfferMessageMembers = UserOfferMessageMembers.OrderByDescending(s => s.Id);
                            break;
                    }

                    #endregion



                    return View(UserOfferMessageMembers.ToPagedList(pageNumber, pageSize));

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> SendSMS(int id)
        {
            try
            {
                SmsService sms = new SmsService();

                var member = uow.UserOfferMessageMemberRepository.Get(x => x, x => x.Id == id, null, "UserOfferMessage,UserOfferMessage.Offer,User").First();
                var UserOfferMessage = member.UserOfferMessage;

                if (UserOfferMessage.Text.Contains("%Name"))
                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Name", uow.UserRepository.GetByID(member.UserId).FirstName);
                if (UserOfferMessage.Text.Contains("%FullName"))
                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%FullName", uow.UserRepository.GetByID(member.UserId).LastName);
                if (UserOfferMessage.Text.Contains("%Mobile"))
                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Mobile", uow.UserRepository.GetByID(member.UserId).PhoneNumber);
                if (UserOfferMessage.Text.Contains("%Title"))
                    UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Title", member.UserOfferMessage.Offer.Title);
                if (UserOfferMessage.Text.Contains("%StartDate"))
                {
                    if (UserOfferMessage.Offer.StartDate.HasValue)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%StartDate", CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(member.UserOfferMessage.Offer.StartDate.Value));
                    else
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%StartDate", "");
                }
                if (UserOfferMessage.Text.Contains("%EndDate"))
                {
                    if (UserOfferMessage.Offer.ExpireDate.HasValue)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%EndDate", CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(member.UserOfferMessage.Offer.ExpireDate.Value));
                    else
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%EndDate", "");
                }
                if (UserOfferMessage.Text.Contains("%Groups"))
                {
                    if (UserOfferMessage.Offer.offerProductCategories.Any())
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Groups", String.Join(",", member.UserOfferMessage.Offer.offerProductCategories.Select(x => x.productCategory.Name).ToArray()));
                    else
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Groups", "همه گروهها");
                }
                if (UserOfferMessage.Text.Contains("%Value"))
                {
                    var CodeTypeValueCode = member.UserOfferMessage.Offer.CodeTypeValueCode;
                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Value", "");
                    if (CodeTypeValueCode == 4)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Value", uow.UserCodeGiftRepository.Get(x => x.Value, x => x.UserId == member.UserId && x.OfferId == member.UserOfferMessage.OfferId).First().ToString());
                    else if (CodeTypeValueCode == 5)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%Value", uow.GeneralCodeGiftRepository.Get(x => x.Value, x => x.OfferId == member.UserOfferMessage.OfferId).First().ToString());
                }
                if (UserOfferMessage.Text.Contains("%CodeGift"))
                {
                    var CodeTypeValueCode = member.UserOfferMessage.Offer.CodeTypeValueCode;
                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CodeGift", "");
                    if (CodeTypeValueCode == 4 )
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CodeGift", uow.UserCodeGiftRepository.Get(x => x.Code, x => x.UserId == member.UserId && x.OfferId == member.UserOfferMessage.OfferId).First().ToString());
                    else if (CodeTypeValueCode == 5)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CodeGift", uow.GeneralCodeGiftRepository.Get(x => x.Code, x => x.OfferId == member.UserOfferMessage.OfferId).First().ToString());

                }
                if (UserOfferMessage.Text.Contains("%CountUse"))
                {
                    var CodeTypeValueCode = member.UserOfferMessage.Offer.CodeTypeValueCode;
                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CountUse", "");
                    else if (CodeTypeValueCode == 4 )
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CountUse", uow.UserCodeGiftRepository.Get(x => x.CountUse, x => x.UserId == member.UserId && x.OfferId == member.UserOfferMessage.OfferId).First().ToString());
                    else if (CodeTypeValueCode == 5)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%CountUse", uow.GeneralCodeGiftRepository.Get(x => x.CountUse, x => x.OfferId == member.UserOfferMessage.OfferId).First().ToString());

                }
                if (UserOfferMessage.Text.Contains("%MaxUse"))
                {
                    var CodeTypeValueCode = member.UserOfferMessage.Offer.CodeTypeValueCode;
                    if (CodeTypeValueCode == 1 || CodeTypeValueCode == 2)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%MaxUse", "");
                    else if (CodeTypeValueCode == 4 )
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%MaxUse", uow.UserCodeGiftRepository.Get(x => x.MaxValue, x => x.UserId == member.UserId && x.OfferId == member.UserOfferMessage.OfferId).First().ToString());
                    else if (CodeTypeValueCode == 5)
                        UserOfferMessage.Text = UserOfferMessage.Text.Replace("%MaxUse", uow.GeneralCodeGiftRepository.Get(x => x.MaxValue, x => x.OfferId == member.UserOfferMessage.OfferId).First().ToString());

                }

                await sms.SendSMSAsync(new IdentityMessage() { Body = UserOfferMessage.Text, Destination = member.User.PhoneNumber }, null, null, null, null, null, null, true);


                member.state = OfferMessageSendMessageType.Sent;
                uow.UserOfferMessageMemberRepository.Update(member);
                uow.Save();


                return Json(new
                {
                    Message = "ارسال شد.",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد" + ex.Message,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SendSmsSetting()
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {
                var p = ModulePermission.check(User.Identity.GetUserId(), 24);
                if (p.Where(x => x == true).Any())
                {

                    StdSchedulerFactory factory = new StdSchedulerFactory();
                    IScheduler scheduler = factory.GetScheduler().Result;
                    ViewBag.state = scheduler.IsStarted;

                    DateTime date = DateTime.Now;
                    return View(uow.UserOfferMessageRepository.Get(x => x, x => x.Offer.IsActive && x.Offer.state && ((x.Offer.ExpireDate != null && x.Offer.ExpireDate >= date) || x.Offer.ExpireDate == null) && ((x.Offer.StartDate != null && x.Offer.StartDate <= date) || x.Offer.StartDate == null), null, "Offer,UserOfferMessageMembers"));

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت تخفیف ها" }));
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Offers", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }


        [HttpPost]
        public JsonResult SetStateBot()
        {
            try
            {

                StdSchedulerFactory factory = new StdSchedulerFactory();
                IScheduler scheduler = factory.GetScheduler().Result;
                if (!scheduler.IsStarted)
                {
                    scheduler.Start();
                }
                else
                    scheduler.Shutdown();
                return Json(new
                {
                    Message = "انجام شد.",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد" + ex.Message,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


    }


}

