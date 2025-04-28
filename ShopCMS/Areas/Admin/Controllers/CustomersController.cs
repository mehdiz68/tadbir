using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Domain;
using CoreLib.Infrastructure.DateTime;
using PagedList;
using CoreLib.ViewModel.Xml;
using CoreLib.Infrastructure.ModelBinder;
using ahmadi.Infrastructure.Security;
using Microsoft.AspNet.Identity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public partial class CustomersController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;

        public CustomersController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        // GET: Admin/Customers
        [CorrectArabianLetter(new string[] { "UsernameString", "UsernameFilter", "FullName", "FullNameFilter" })]
        public virtual ActionResult Index(string sortOrder, string UsernameFilter, string UsernameString, string CreationStartDateInput, string currentStartDateInputFiltering, string CreationEndDateInput, string currentEndDateInputFiltering, string LastActivityStartDateInput, string currentLastActivityStartDateInputFiltering, string LastActivityEndDateInput, string currentLastActivityEndDateInputFiltering, string PhoneNumberString, string PhoneNumberFilter, string EmailConfirmId, string CurrentEmailConfirmId, string PhoneNumberConfirmId, string CurrentPhoneNumberConfirmId, string Disable, string CurrentDisable, string FullName, string FullNameFilter, string Gender, string GenderFilter, int? page)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    ViewBag.AddPermission = ModulePermission.check(User.Identity.GetUserId(), 10, null);

                    #region LoadDropdown

                    List<SelectListItem> EmailConfirmSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید شده", Value = "True" }, new SelectListItem() { Text = "تایید نشده", Value = "False" } };
                    ViewBag.EmailConfirmSelectListItem = EmailConfirmSelectListItem;
                    List<SelectListItem> PhoneNumberSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید شده", Value = "True" }, new SelectListItem() { Text = "تایید نشده", Value = "False" } };
                    ViewBag.PhoneNumberSelectListItem = PhoneNumberSelectListItem;
                    List<SelectListItem> DisableSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "False" }, new SelectListItem() { Text = "غیر فعال", Value = "True" } };
                    ViewBag.DisableSelectListItem = DisableSelectListItem;
                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;

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
                    ViewBag.CurrentEmailConfirmId = EmailConfirmId;
                    ViewBag.CurrentPhoneNumberConfirmId = PhoneNumberConfirmId;
                    ViewBag.CurrentDisable = Disable;
                    ViewBag.currentStartDateInputFiltering = CreationStartDateInput;
                    ViewBag.currentEndDateInputFiltering = CreationEndDateInput;
                    ViewBag.currentLastActivityStartDateInputFiltering = LastActivityStartDateInput;
                    ViewBag.currentLastActivityEndDateInputFiltering = LastActivityEndDateInput;


                    var user = from s in uow.UserRepository.GetByReturnQueryable(x => x, null, null, "Avatarattachment")
                               select s;


                    var Roleusers = uow.UserRepository.GetByReturnQueryable(x => x);
                    string[] roleIds = Roleusers.Select(r => r.Id).ToArray();
                    user = user.Where(x => roleIds.Contains(x.Id));


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

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مشتریان", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Customers", "Index", true, 200, " نمایش صفحه مدیریت مشتریان", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(user.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Customers", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/Customers/Details/5
        public virtual ActionResult Details(string id)
        {

            if (ModulePermission.check(User.Identity.GetUserId(), 9, null))
            {
                ApplicationUser au = uow.UserRepository.Get(x => x, x => x.Id == id).SingleOrDefault();
                if (au != null)
                {
                    ViewBag.setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                    return View(au);
                }
                else
                {

                    return HttpNotFound();
                }
            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت کاربران" }));

        }

        // GET: /Admin/Customers/CustomerOrderHistory
        public virtual ActionResult CustomerOrderHistory(string LanguageId, string sortOrder, string OrderIdString, string OrderIdFilter, string LanguagenameFilter, string LanguagenameString, string UserId, string UserIdFilter, string StartDateFilter, string StartDateString, string EndDateFilter, string EndDateString, string UpdateDateStartString, string UpdateDateStartFilter, string UpdateDateEndString, string UpdateDateEndFilter, string PriceSearchString, string PriceSearchFilter, string TotalPriceString, string TotalPriceFilter, string StateString, string StateFilter, string StateLastString, string StateLastFilter, string VisitedString, string VisitedFilter, int? page)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 25);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();

                    ViewBag.setting = setting;
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameSelectListItem = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    List<SelectListItem> PriceSearchSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "برابر", Value = "1" }, new SelectListItem() { Text = "بالاتر از", Value = "2" }
                        , new SelectListItem() { Text = "کمتر از", Value = "3" }};
                    ViewBag.PriceSearchSelectListItem = PriceSearchSelectListItem;

                    ViewBag.StateSelectListItem = new SelectList(readXML.ListOfXState(), "Id", "Name");

                    List<SelectListItem> VisitedSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "مشاهده شده", Value = "true" }, new SelectListItem() { Text = "مشاهده نشده", Value = "false" } };
                    ViewBag.VisitedSelectListItem = VisitedSelectListItem;

                    List<SelectListItem> StateLastSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }
                            , new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "تحویل شده", Value = "5" }, new SelectListItem() { Text = "انصراف", Value = "6" }};
                    ViewBag.StateLastSelectListItem = StateLastSelectListItem;

                    #region search
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(UserId))
                        UserId = UserIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StartDateString))
                        StartDateString = StartDateFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(EndDateString))
                        EndDateString = EndDateFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(UpdateDateStartString))
                        UpdateDateStartString = UpdateDateStartFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(UpdateDateEndString))
                        UpdateDateEndString = UpdateDateEndFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(PriceSearchString))
                        PriceSearchString = PriceSearchFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(TotalPriceString))
                        TotalPriceString = TotalPriceFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(OrderIdString))
                        OrderIdString = OrderIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StateString))
                        StateString = StateFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StateLastString))
                        StateLastString = StateLastFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(VisitedString))
                        VisitedString = VisitedFilter;
                    else
                        page = 1;

                    var user = uow.UserRepository.GetByID(UserId);
                    ViewBag.user = user;

                    ViewBag.UserId = UserId;
                    ViewBag.LanguagenameFilter = LanguagenameString;
                    ViewBag.StartDateFilter = StartDateString;
                    ViewBag.EndDateFilter = EndDateString;
                    ViewBag.UpdateDateStartFilter = UpdateDateStartString;
                    ViewBag.UpdateDateEndFilter = UpdateDateEndString;
                    ViewBag.PriceSearchFilter = PriceSearchString;
                    ViewBag.TotalPriceFilter = TotalPriceString;
                    ViewBag.OrderIdFilter = OrderIdString;
                    ViewBag.StateFilter = StateString;
                    ViewBag.StateLastFilter = StateLastString;
                    ViewBag.VisitedFilter = VisitedString;

                    var Orders = uow.OrderRepository.GetQueryList().AsNoTracking().Include("User").Include("User.CityEntity.Province").Include("OrderWallets.Wallet").Include("OrderDeliveries.ProductSendWay").Include("OrderStates").Include("OrderRows.Product.ProductPrices");
                    if (!String.IsNullOrEmpty(LanguagenameString))
                    {
                        int langId = Convert.ToInt32(LanguagenameString);
                        Orders = Orders.Where(s => s.LanguageId == langId);
                    }
                    if (!String.IsNullOrEmpty(UserId))
                    {
                        Orders = Orders.Where(s => s.UserId == UserId);
                    }
                    if (!String.IsNullOrEmpty(StartDateString) && !String.IsNullOrEmpty(EndDateString))
                    {
                        DateTime StartDate = DateTimeConverter.ChangeShamsiToMiladi(StartDateString);
                        DateTime EndDate = DateTimeConverter.ChangeShamsiToMiladi(EndDateString);
                        Orders = Orders.Where(s => s.InsertDate >= StartDate
                             && s.InsertDate <= EndDate);
                    }
                    if (!String.IsNullOrEmpty(UpdateDateStartString) && !String.IsNullOrEmpty(UpdateDateEndString))
                    {
                        DateTime StartDate = DateTimeConverter.ChangeShamsiToMiladi(UpdateDateStartString);
                        DateTime EndDate = DateTimeConverter.ChangeShamsiToMiladi(UpdateDateEndString);
                        Orders = Orders.Where(s => s.UpdateDate >= StartDate
                             && s.UpdateDate <= EndDate);
                    }
                    if (!String.IsNullOrEmpty(PriceSearchString) && !String.IsNullOrEmpty(TotalPriceString))
                    {
                        Int32 price = Convert.ToInt32(TotalPriceString);
                        if (PriceSearchString == "1")
                            Orders = Orders.Where(s => s.OrderRows.Sum(x => x.Price) == price);
                        else if (PriceSearchString == "2")
                            Orders = Orders.Where(s => s.OrderRows.Sum(x => x.Price) > price);
                        else if (PriceSearchString == "3")
                            Orders = Orders.Where(s => s.OrderRows.Sum(x => x.Price) < price);
                    }
                    if (!String.IsNullOrEmpty(OrderIdString))
                    {
                        Orders = Orders.Where(s => s.Id.ToString() == OrderIdString);
                    }
                    if (!String.IsNullOrEmpty(StateString))
                    {
                        Int16 StateId = Convert.ToInt16(StateString);
                        Orders = Orders.Where(s => s.User.State == StateId);
                    }
                    if (!String.IsNullOrEmpty(VisitedString))
                    {
                        bool visited = Convert.ToBoolean(VisitedString);
                        Orders = Orders.Where(s => s.Visited == visited);
                    }
                    if (!String.IsNullOrEmpty(StateLastString))
                    {
                        OrderStatus SL = (OrderStatus)Convert.ToInt16(StateLastString);
                        Orders = Orders.Where(s => s.OrderStates.OrderByDescending(x => x.LogDate)
                            .Take(1).Any(x => x.state == SL));
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "User":
                            Orders = Orders.OrderBy(s => s.User.FirstName).ThenBy(s => s.User.LastName);
                            ViewBag.CurrentSort = "User";
                            break;
                        case "User_desc":
                            Orders = Orders.OrderByDescending(s => s.User.FirstName).ThenByDescending(s => s.User.LastName);
                            ViewBag.CurrentSort = "User_desc";
                            break;
                        case "InsertDate":
                            Orders = Orders.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate";
                            break;
                        case "InsertDate_desc":
                            Orders = Orders.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "InsertDate_desc";
                            break;
                        case "Price":
                            Orders = Orders.OrderBy(s => s.OrderRows.Sum(x => x.Price));
                            ViewBag.CurrentSort = "Price";
                            break;
                        case "Price_desc":
                            Orders = Orders.OrderByDescending(s => s.OrderRows.Sum(x => x.Price));
                            ViewBag.CurrentSort = "Price_desc";
                            break;
                        case "State":
                            Orders = Orders.OrderBy(s => readXML.DetailOfXState(s.User.State).Name);
                            ViewBag.CurrentSort = "State";
                            break;
                        case "State_desc":
                            Orders = Orders.OrderByDescending(s => readXML.DetailOfXState(s.User.State).Name);
                            ViewBag.CurrentSort = "State_desc";
                            break;
                        case "StateLast":
                            //Orders = from o in Orders
                            //         join os in db.OrderStates on o.Id equals os.OrderId
                            //         group os by o into g
                            //         orderby g.Key.OrderStates.LastOrDefault().state
                            //         select g.Key;
                            Orders = Orders.OrderBy(s => s.OrderStates.OrderByDescending(x => x.LogDate).Take(1).FirstOrDefault().state);
                            ViewBag.CurrentSort = "StateLast";
                            break;
                        case "StateLast_desc":
                            //Orders = from o in Orders
                            //         join os in db.OrderStates on o.Id equals os.OrderId
                            //         group os by o into g
                            //         orderby g.Key.OrderStates.FirstOrDefault().LogDate descending
                            //         select g.Key;
                            Orders = Orders.OrderByDescending(s => s.OrderStates.OrderByDescending(x => x.LogDate).Take(1).FirstOrDefault().state);
                            ViewBag.CurrentSort = "StateLast_desc";
                            break;
                        case "Visited":
                            Orders = Orders.OrderBy(s => s.Visited);
                            ViewBag.CurrentSort = "Visited";
                            break;
                        default:  // Name ascending 
                            Orders = Orders.OrderByDescending(s => s.InsertDate);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, " Customers", "CustomerOrderHistory", true, 200, " نمایش صفحه سوابق خریدِ " + user.FirstName + " " + user.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Orders.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت سفارشات" }));

            }

            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, " Customers", "CustomerOrderHistory", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: /Admin/Customers/CustomerComments
        [CorrectArabianLetter(new string[] { "ProductString", "ProductFilter", "TitleString", "TitleFilter" })]
        public virtual ActionResult CustomerComments(string sortOrder, string UserIdFilter, string UserId, string ProductFilter, string ProductString, string TitleFilter, string TitleString, string ActiveFilter, string ActiveString, int? page)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    List<SelectListItem> ActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید", Value = "True" }, new SelectListItem() { Text = "عدم تایید", Value = "False" } };
                    ViewBag.ActiveSelectListItem = ActiveSelectListItem;

                    #region search
                    if (string.IsNullOrEmpty(UserId))
                        UserId = UserIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductString))
                        ProductString = ProductFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(TitleString))
                        TitleString = TitleFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ActiveString))
                        ActiveString = ActiveFilter;
                    else
                        page = 1;

                    ViewBag.UserIdFilter = UserId;
                    ViewBag.ProductFilter = ProductString;
                    ViewBag.TitleFilter = TitleString;
                    ViewBag.ActiveFilter = ActiveString;


                    var user = uow.UserRepository.GetByID(UserId);
                    ViewBag.user = user;

                    var productComment = uow.ProductCommentRepository.GetByReturnQueryable(x => x, x => x.UserId == UserId, null, "User");

                    if (!String.IsNullOrEmpty(ProductString))
                    {
                        productComment = productComment.Where(s => s.Product.Name.Contains(ProductString));
                    }
                    if (!String.IsNullOrEmpty(TitleString))
                    {
                        productComment = productComment.Where(s => s.Title.Contains(TitleString));
                    }
                    if (!String.IsNullOrEmpty(ActiveString))
                    {
                        bool active = Convert.ToBoolean(ActiveString);
                        productComment = productComment.Where(s => s.IsActive == active);
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "User":
                            productComment = productComment.OrderBy(s => s.User.FirstName).ThenBy(s => s.User.LastName);
                            ViewBag.CurrentSort = "User";
                            break;
                        case "User_desc":
                            productComment = productComment.OrderByDescending(s => s.User.FirstName).ThenByDescending(s => s.User.LastName);
                            ViewBag.CurrentSort = "User_desc";
                            break;
                        case "Product":
                            productComment = productComment.OrderBy(s => s.Product.Name);
                            ViewBag.CurrentSort = "Product";
                            break;
                        case "Product_desc":
                            productComment = productComment.OrderByDescending(s => s.Product.Name);
                            ViewBag.CurrentSort = "Product_desc";
                            break;
                        case "Title":
                            productComment = productComment.OrderBy(s => s.Title);
                            ViewBag.CurrentSort = "Title";
                            break;
                        case "Title_desc":
                            productComment = productComment.OrderByDescending(s => s.Title);
                            ViewBag.CurrentSort = "Title_desc";
                            break;
                        case "Confirm":
                            productComment = productComment.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "Confirm";
                            break;
                        case "Confirm_desc":
                            productComment = productComment.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "Confirm_desc";
                            break;
                        case "Date":
                            productComment = productComment.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "Date";
                            break;
                        case "Date_desc":
                            productComment = productComment.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "Date_desc";
                            break;
                        default:
                            productComment = productComment.OrderByDescending(s => s.InsertDate);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);
                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "نظرات محصولات", null, "HelpModuleSections").FirstOrDefault();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "CustomerComments", "Customers", true, 200, " نمایش صفحه نظرات محصولِ " + user.FirstName + " " + user.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(productComment.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت نظرات محصول" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "CustomerComments", "Customers", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: /Admin/Customers/CustomerQuestions
        [CorrectArabianLetter(new string[] { "ProductString", "ProductFilter" })]
        public virtual ActionResult CustomerQuestions(string sortOrder, string UserIdFilter, string UserId, string ProductFilter, string ProductString, string ActiveFilter, string ActiveString, string StartDateFilter, string StartDateString, string EndDateFilter, string EndDateString, int? page)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    List<SelectListItem> ActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید", Value = "True" }, new SelectListItem() { Text = "عدم تایید", Value = "False" } };
                    ViewBag.ActiveSelectListItem = ActiveSelectListItem;

                    #region search
                    if (string.IsNullOrEmpty(UserId))
                        UserId = UserIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductString))
                        ProductString = ProductFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ActiveString))
                        ActiveString = ActiveFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(StartDateString))
                        StartDateString = StartDateFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(EndDateString))
                        EndDateString = EndDateFilter;
                    else
                        page = 1;


                    ViewBag.UserIdFilter = UserId;
                    ViewBag.ProductFilter = ProductString;
                    ViewBag.ActiveFilter = ActiveString;
                    ViewBag.StartDateFilter = StartDateString;
                    ViewBag.EndDateFilter = EndDateString;

                    var productQuestion = uow.ProductQuestionRepository.GetByReturnQueryable(x => x, x => x.UserId == UserId, null, "User");


                    var user = uow.UserRepository.GetByID(UserId);
                    ViewBag.user = user;


                    if (!String.IsNullOrEmpty(ProductString))
                    {
                        productQuestion = productQuestion.Where(s => s.Product.Name.Contains(ProductString));
                    }
                    if (!String.IsNullOrEmpty(ActiveString))
                    {
                        bool active = Convert.ToBoolean(ActiveString);
                        productQuestion = productQuestion.Where(s => s.IsActive == active);
                    }
                    if (!String.IsNullOrEmpty(StartDateString) && !String.IsNullOrEmpty(EndDateString))
                    {
                        DateTime StartDate = DateTimeConverter.ChangeShamsiToMiladi(StartDateString);
                        DateTime EndDate = DateTimeConverter.ChangeShamsiToMiladi(EndDateString);
                        productQuestion = productQuestion.Where(s => s.InsertDate >= StartDate
                             && s.InsertDate <= EndDate);
                    }

                    #endregion

                    #region Sort
                    switch (sortOrder)
                    {
                        case "User":
                            productQuestion = productQuestion.OrderBy(s => s.User.FirstName).ThenBy(s => s.User.LastName);
                            ViewBag.CurrentSort = "User";
                            break;
                        case "User_desc":
                            productQuestion = productQuestion.OrderByDescending(s => s.User.FirstName).ThenByDescending(s => s.User.LastName);
                            ViewBag.CurrentSort = "User_desc";
                            break;
                        case "Product":
                            productQuestion = productQuestion.OrderBy(s => s.Product.Name);
                            ViewBag.CurrentSort = "Product";
                            break;
                        case "Product_desc":
                            productQuestion = productQuestion.OrderByDescending(s => s.Product.Name);
                            ViewBag.CurrentSort = "Product_desc";
                            break;
                        case "Confirm":
                            productQuestion = productQuestion.OrderBy(s => s.IsActive);
                            ViewBag.CurrentSort = "Confirm";
                            break;
                        case "Confirm_desc":
                            productQuestion = productQuestion.OrderByDescending(s => s.IsActive);
                            ViewBag.CurrentSort = "Confirm_desc";
                            break;
                        case "Date":
                            productQuestion = productQuestion.OrderBy(s => s.InsertDate);
                            ViewBag.CurrentSort = "Date";
                            break;
                        case "Date_desc":
                            productQuestion = productQuestion.OrderByDescending(s => s.InsertDate);
                            ViewBag.CurrentSort = "Date_desc";
                            break;
                        default:
                            productQuestion = productQuestion.OrderByDescending(s => s.InsertDate);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "پرسش و پاسخ محصولات", null, "HelpModuleSections").FirstOrDefault();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "Customers", "CustomerQuestions", true, 200, " نمایش صفحه پرسش های محصولِ " + user.FirstName + " " + user.LastName, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(productQuestion.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت پرسش های محصول" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Customers", "CustomerQuestions", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: /Admin/Customers/CustomerFavorates
        public virtual ActionResult CustomerFavorates(int? page, string UserIdFilter, string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
                UserId = UserIdFilter;
            else
                page = 1;

            var user = uow.UserRepository.GetByID(UserId);
            ViewBag.user = user;

            ViewBag.UserIdFilter = UserId;

            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var Favorates = uow.ProductFavorateRepository.Get(x => x, x => x.UserId == UserId).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);

            List<int> ProductIds = Favorates.Select(x => x.ProductId).Distinct().ToList();
            ViewBag.Products = uow.ProductRepository.ProductItemList(x => ProductIds.Contains(x.Id), x => x.OrderByDescending(o => o.Id), 0, ProductIds.Count).ToList();
            ViewBag.folders = uow.ProductFavorateRepository.Get(x => x, x => x.UserId == UserId).GroupBy(x => x.FolderName).Select(x => x.Key);

            #region EventLogger
            ahmadi.Infrastructure.EventLog.Logger.Add(1, "Customers", "CustomerFavorates", true, 200, " نمایش صفحه محصولات مورد علاقه ی" + user.FirstName + " " + user.LastName, DateTime.Now, User.Identity.GetUserId());
            #endregion
            return View(Favorates);
        }

        // GET: /Admin/Customers/CustomerNotifications
        public virtual ActionResult CustomerNotifications(int? page, string UserIdFilter, string UserId)
        {
            if (string.IsNullOrEmpty(UserId))
                UserId = UserIdFilter;
            else
                page = 1;

            var user = uow.UserRepository.GetByID(UserId);
            ViewBag.user = user;

            ViewBag.UserIdFilter = UserId;



            int pageSize = 8;
            int pageNumber = (page ?? 1);
            var ProductLetmeknows = uow.ProductLetmeknowRepository.Get(x => x, x => x.UserId == UserId).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);
            List<int> ProductIds = ProductLetmeknows.Select(x => x.ProductId).Distinct().ToList();
            ViewBag.Products = uow.ProductRepository.ProductItemList(x => ProductIds.Contains(x.Id), x => x.OrderByDescending(o => o.Id), 0, ProductIds.Count).ToList();

            #region EventLogger
            ahmadi.Infrastructure.EventLog.Logger.Add(1, "Customers", "CustomerNotifications", true, 200, " نمایش صفحه اطلاع رسانی های" + user.FirstName + " " + user.LastName, DateTime.Now, User.Identity.GetUserId());
            #endregion
            return View(ProductLetmeknows);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
