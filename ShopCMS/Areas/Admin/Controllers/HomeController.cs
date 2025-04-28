using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UnitOfWork;
using Microsoft.AspNet.Identity;
using ahmadi.Areas.Admin.ViewModel.AdminPanel;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Domain;
using System.Data.Entity;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Security,Support,SuperUser")]
    public class HomeController : Controller
    {
        UnitOfWorkClass uow = null;
        public HomeController()
        {

            uow = new UnitOfWorkClass();
        }

        private ApplicationUserManager _userManager;
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

        // GET: Admin/Home
        [Infrastructure.Filter.AutoExecueFilter]
        public ActionResult Index(int? dateOrder)
        {




            ////اطلاعات فروشگاه
            //string userID = User.Identity.GetUserId();
            //var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            //Dashboard dashboard = new Dashboard();
            //dashboard.OrderItemAvgs = uow.OrderRateRepository.GetQueryList().AsNoTracking().GroupBy(x => x.orderRateItem).Select(x => new Domain.ViewModels.OrderItemAvg { ItemName = x.Key.Title, ItemTotalCount = x.Count(), ItemCount0 = x.Count(s => s.state == OrderRateStatus.کاملا_راضی), ItemCount1 = x.Count(s => s.state == OrderRateStatus.راضی), ItemCount2 = x.Count(s => s.state == OrderRateStatus.نظری_ندارم), ItemCount3 = x.Count(s => s.state == OrderRateStatus.ناراضی), ItemCount4 = x.Count(s => s.state == OrderRateStatus.کاملا_ناراضی) });
            //dashboard.Logo = setting.attachment.FileName;
            //dashboard.WebSiteName = setting.WebSiteName;
            //dashboard.ProductComments = uow.ProductCommentRepository.Count(x => x.Visited == false);
            //dashboard.ProductQuestions = uow.ProductQuestionRepository.Count(x => x.Visited == false);
            //dashboard.UserMessages = uow.UserMessageRepository.Count(x => x.state == false && x.UserIdTo == userID);

            ////مدیریت محصولات
            //dashboard.ProductPriceState = new List<ChartState>();
            //dashboard.ProductPriceState.Add(new ChartState() { Name = "کل تنوع های فعال", Count = uow.ProductPriceRepository.Count(x => x.IsActive), Link = "/Admin/ProductPrice/List?IsActive=True" });
            //dashboard.ProductPriceState.Add(new ChartState() { Name = "کل تنوع های  غیرفعال", Count = uow.ProductPriceRepository.Count(x => x.IsActive == false), Link = "/Admin/ProductPrice/List?IsActive=False" });
            //dashboard.ProductPriceState.Add(new ChartState() { Name = "تنوع های فعال بدون موجودی", Count = uow.ProductPriceRepository.Count(x => x.IsActive && x.Quantity == 0), Link = "/Admin/ProductPrice/List?IsActive=True&Quantity=0" });
            //dashboard.ProductPriceState.Add(new ChartState() { Name = "تنوع های در حال اتمام موجودی", Count = uow.ProductPriceRepository.Count(x => x.IsActive && x.Quantity == 1), Link = "/Admin/ProductPrice/List?Quantity=1" });

            ////مدیریت سفارشات
            //dashboard.OrderState = new List<ChartState>();
            //var orders = from x in uow.OrderStateRepository.GetQueryList().AsNoTracking().Include("Order.OrderWallets.Wallet")
            //             where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
            //             group x by x.OrderId into g
            //             select new { g.Key, state = g.Max(x => x.state) };
            //dashboard.OrderState.Add(new ChartState() { Name = "کل سفارشات", Count = orders.Count(x => x.state == 0), Link = "/Admin/Orders?StateLastString=0" });
            //dashboard.OrderState.Add(new ChartState() { Name = " در انتظار تایید", Count = orders.Count(x => x.state == 0), Link = "/Admin/Orders?StateLastString=1" });
            //dashboard.OrderState.Add(new ChartState() { Name = "تایید سفارش", Count = orders.Count(x => x.state == OrderStatus.تایید_سفارش), Link = "/Admin/Orders?StateLastString=2" });
            //dashboard.OrderState.Add(new ChartState() { Name = "تایید پرداخت", Count = orders.Count(x => x.state == OrderStatus.تایید_پرداخت), Link = "/Admin/Orders?StateLastString=3" });
            //dashboard.OrderState.Add(new ChartState() { Name = "پردازش انبار", Count = orders.Count(x => x.state == OrderStatus.پردازش_انبار), Link = "/Admin/Orders?StateLastString=4" });
            //dashboard.OrderState.Add(new ChartState() { Name = "آماده ارسال", Count = orders.Count(x => x.state == OrderStatus.آماده_ارسال), Link = "/Admin/Orders?StateLastString=5" });
            //dashboard.OrderState.Add(new ChartState() { Name = "ارسال شده", Count = orders.Count(x => x.state == OrderStatus.ارسال_شده), Link = "/Admin/Orders?StateLastString=6" });
            //dashboard.OrderState.Add(new ChartState() { Name = "تحویل داده شده", Count = orders.Count(x => x.state == OrderStatus.تحویل_داده_شده), Link = "/Admin/Orders?StateLastString=7" });
            //dashboard.OrderState.Add(new ChartState() { Name = "لغو شده", Count = orders.Count(x => x.state == OrderStatus.لغو_شده), Link = "/Admin/Orders?StateLastString=8" });
            //dashboard.OrderState.Add(new ChartState() { Name = "مرجوعی", Count = orders.Count(x => x.state == OrderStatus.مرجوعی), Link = "/Admin/Orders?StateLastString=9" });

            ////مدیریت وبلاگ
            //dashboard.BlogState = new List<ChartState>();
            //DateTime WeekDate = DateTime.Now.AddDays(-7).Date;
            //DateTime MonthDate = DateTime.Now.AddDays(-31).Date;
            //DateTime ThreeMonthDate = DateTime.Now.AddDays(-93).Date;
            //DateTime SixMonthDate = DateTime.Now.AddDays(-186).Date;
            //DateTime YearDate = DateTime.Now.AddDays(-365).Date;
            //dashboard.BlogState.Add(new ChartState() { Name = "کل پست های یک هفته اخیر", Count = uow.ContentRepository.Count(x => x.ContentTypeId == 3 && x.IsActive && x.InsertDate >= WeekDate), Link = "#" });
            //dashboard.BlogState.Add(new ChartState() { Name = "پست های یک ماه اخیر", Count = uow.ContentRepository.Count(x => x.ContentTypeId == 3 && x.IsActive && x.InsertDate >= MonthDate), Link = "#" });
            //dashboard.BlogState.Add(new ChartState() { Name = "پست های سه ماه اخیر", Count = uow.ContentRepository.Count(x => x.ContentTypeId == 3 && x.IsActive && x.InsertDate >= ThreeMonthDate), Link = "#" });
            //dashboard.BlogState.Add(new ChartState() { Name = "پست های شش ماه اخیر", Count = uow.ContentRepository.Count(x => x.ContentTypeId == 3 && x.IsActive && x.InsertDate >= SixMonthDate), Link = "#" });
            //dashboard.BlogState.Add(new ChartState() { Name = "پست های یک سال اخیر", Count = uow.ContentRepository.Count(x => x.ContentTypeId == 3 && x.IsActive && x.InsertDate >= YearDate), Link = "#" });

            ////امتیاز عملکرد شما
            //double delay = uow.OrderRepository.GetByReturnQueryable(x => x.Id, x => x.OrderDeliveries.Any(s => s.DeliveryState == DeliveryState.تحویل_با_تاخیر)).Count();
            //dashboard.DelayOrders = delay > 0 ? Math.Round((delay * 1.0 * 100 / (orders.Count())), 2) : 0;
            //double cancel = orders.Count(x => x.state == OrderStatus.مرجوعی);
            //dashboard.CancelOrders = cancel > 0 ? Math.Round((cancel * 1.0 * 100 / (orders.Count())), 2) : 0;
            //double returned = orders.Count(x => x.state == OrderStatus.جبران_مرجوعی);
            //dashboard.ReturnedOrders = returned > 0 ? Math.Round((returned * 1.0 * 100 / (orders.Count())), 2) : returned;

            ////رضایت خرید
            //int sat = uow.ProductCommentRepository.Count(x => x.IsActive && x.IsBuy && x.Satisfaction.Value < (ProductCommentSatisfaction)2);
            //dashboard.UsersSatisfactionCount = sat;
            //dashboard.UsersSatisfaction = sat > 0 ? Math.Round((sat * 1.0 * 100 / (uow.ProductCommentRepository.Count(x => x.IsActive && x.IsBuy))), 2) : 0;

            ////آخرین مطالب وبلاگ
            //dashboard.Blogs = uow.ContentRepository.Get(x => new Domain.Content { Id = x.Id, Title = x.Title, PageAddress = x.PageAddress, attachment = x.attachment, InsertDate = x.InsertDate }, x => x.IsActive, x => x.OrderByDescending(s => s.Id), "attachment", 0, 6, true);

            ////مدیریت محصولات
            //dashboard.Products = uow.ProductRepository.Count();
            //dashboard.Products30s = uow.ProductRepository.Count(x => x.InsertDate >= MonthDate);
            //dashboard.ProductPrices = uow.ProductPriceRepository.Count();
            //dashboard.ProductPricesExist = uow.ProductPriceRepository.Count(x => x.ProductStateId == 1);
            //dashboard.ProductPricesNotExist = uow.ProductPriceRepository.Count(x => x.ProductStateId == 5);
            //dashboard.ProductPricesEstelam = uow.ProductPriceRepository.Count(x => x.ProductStateId == 2);
            //dashboard.ProductPricesStopProduce = uow.ProductPriceRepository.Count(x => x.ProductStateId == 4);
            //dashboard.ProductPricesCommingSoon = uow.ProductPriceRepository.Count(x => x.ProductStateId == 3);
            //dashboard.ProductFavorites = uow.ProductFavorateRepository.GetByReturnQueryable(x => x).GroupBy(x => x.ProductId).Count();
            //dashboard.ProductLetMeKhows = uow.ProductLetmeknowRepository.GetByReturnQueryable(x => x).GroupBy(x => x.ProductId).Count();
            //dashboard.ProductCommentList = uow.ProductCommentRepository.Get(x => new ProductComment { Id = x.Id, Title = x.Title, User = x.User, Text = x.Text, IsBuy = x.IsBuy, Satisfaction = x.Satisfaction, InsertDate = x.InsertDate }, null, x => x.OrderByDescending(s => s.Id), "User", 0, 3, true);
            //dashboard.CommentList = uow.CommentRepository.Get(x => new Comment { Id = x.Id, Message = x.Message, InsertDate = x.InsertDate, FullName = x.FullName }, null, x => x.OrderByDescending(s => s.Id), "", 0, 4, true);

            ////روند سفارشات
            //if (dateOrder.HasValue)
            //{
            //    switch (dateOrder.Value)
            //    {
            //        case 1: dateOrder = -7; break;
            //        case 2: dateOrder = -31; break;
            //        case 3: dateOrder = -93; break;
            //        case 4: dateOrder = -186; break;
            //        case 5: dateOrder = -279; break;
            //        case 6: dateOrder = -365; break;
            //        default:
            //            break;
            //    }
            //    DateTime orderdate = DateTime.Now.AddDays(dateOrder.Value).Date;

            //    // dashboard.OrdersChart = uow.OrderRepository.Get(x => x, x => x.IsActive && x.OrderWallets.Any(s => s.Wallet.State == true) && x.InsertDate >= orderdate).GroupBy(x => x).Select(x => new ChartState { Name = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(x.Key), Count = x.Count(), Link = "-" });
            //}
            //else
            //{
            //    dashboard.OrdersChart = from x in uow.OrderRepository.GetByReturnQueryable(x => x, null, null, "OrderWallets.Wallet")
            //                            where x.IsActive && x.OrderWallets.Any(s => s.Wallet.State == true)
            //                            group x by x.InsertDate into g
            //                            select new ChartState { Name = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(g.Key), Count = g.Sum(x => x.OrderWallets.First().Wallet.Price), Link = "#" };

            //    // dashboard.OrdersChart = uow.OrderRepository.Get(x => x.OrderWallets, x => x.IsActive && x.OrderWallets.Any(s => s.Wallet.State == true) && x.InsertDate >= MonthDate).GroupBy(x => x.First().Order.InsertDate).Select(x => new ChartState { Name = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(x.Key), Count = x.Sum(x.Key.), Link = "-" });

            //}

            #region ClearLog
            Infrastructure.EventLog.ClearLog.Clear();
            #endregion
            #region EventLogger
            User.Identity.GetUserId();
            Infrastructure.EventLog.Logger.Add(1, "Home", "Index", true, 200, " نمایش صفحه اصلی مدیریت", DateTime.Now, User.Identity.GetUserId());
            #endregion
            return View();

        }


        public virtual PartialViewResult GetHeader(bool whichPlace)
        {

            HeaderLayout hl = new HeaderLayout();
            hl.au = uow.UserRepository.Get(x => x, s => s.UserName == User.Identity.Name, null, "Avatarattachment").Single();
            hl.RoleName = (User.IsInRole("Admin") ? "Admin" : (User.IsInRole("Security") ? "Security" : (User.IsInRole("Support") ? "Support" : "User")));
            hl.Comments = uow.CommentRepository.Get(x => x, s => (s.Visited == false) || (s.IsAuthorSeen == false && s.Content.User.UserName == User.Identity.Name), x => x.OrderByDescending(s => s.Id));
            hl.ContactUs = uow.ContactUsRepository.Get(x => x, s => s.IsVisit == false, x => x.OrderByDescending(s => s.Id));
            hl.FormRequests = uow.FormRequestRepository.Get(x => x, s => s.IsVisit == false, x => x.OrderByDescending(s => s.Id));
            //hl.ProductQuestions = uow.ProductQuestionRepository.Get(x => x, s => !s.Visited, x => x.OrderByDescending(s => s.Id), "User");
            // hl.ProductComments = uow.ProductCommentRepository.Get(x => x, s => !s.Visited, x => x.OrderByDescending(s => s.Id), "User");
            hl.Tickets= uow.TicketRepository.Get(x => x, s => !s.IsVisit && s.Answer==false, x => x.OrderByDescending(s => s.Id), "User");
            //Orders = db.Orders.Where(x=>x.Visited==false);
            ViewData["whichPlace"] = whichPlace;
            #region Check License


            ViewBag.ProductLicense = true;
            #endregion
            return PartialView("_Header", hl);
        }


        public virtual PartialViewResult GetSitebar()
        {
            CoreLib.Infrastructure.licenseModules oModulesList = new CoreLib.Infrastructure.licenseModules();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            #region Check License


            #endregion
            ViewBag.setting = setting;
            return PartialView("_Sidebar", oModulesList.GetlicenseModules());
        }

        [HttpPost]
        public virtual JsonResult UpdateSitemap()
        {
            try
            {
                Infrastructure.SiteMap.Generator sitemap = new Infrastructure.SiteMap.Generator();
                sitemap.WebUrl(HttpContext.Request.Url.Host.Replace("www.", ""), true, true);

                #region EventLogger
                User.Identity.GetUserId();
                Infrastructure.EventLog.Logger.Add(1, "Home", "UpdateSitemap", true, 200, " بروزرسانی سایت مپ", DateTime.Now, User.Identity.GetUserId());
                #endregion

                return Json(new
                {
                    message = "انجام شد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {

                #region EventLogger
                User.Identity.GetUserId();
                Infrastructure.EventLog.Logger.Add(1, "Home", "UpdateSitemap", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }


        [HttpPost]
        public virtual JsonResult ClearCache()
        {
            try
            {
                HttpContext.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                HttpContext.Response.AddHeader("Pragma", "no-cache");
                HttpContext.Response.AddHeader("Expires", "0");
                Session["settingPersian"] = null;
                Session["Languages"] = null;
                Session["HomePage"] = null;
                Session["submenu"] = null;
                #region EventLogger
                User.Identity.GetUserId();
                Infrastructure.EventLog.Logger.Add(1, "Home", "ClearCache", true, 200, "پاک کردن کش", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    message = "انجام شد",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                #region EventLogger
                User.Identity.GetUserId();
                Infrastructure.EventLog.Logger.Add(1, "Home", "ClearCache", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }


        }


        public virtual ActionResult ChangePassword(string id)
        {
            try
            {
                ViewBag.id = id;

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(1, "Home", "ChangePassword", true, 200, " نمایش صفحه تغییر رمز عبور", DateTime.Now, User.Identity.GetUserId());
                #endregion
                return View();
            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "Home", "ChangePassword", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult ChangePassword(string id, string OldPassword, string NewPassword, string ConfirmPassword)
        {
            string msg = "";
            if (String.IsNullOrEmpty(NewPassword))
                msg += string.Format("<div>{0}</div>", "رمز عبور جدید را وارد نمایید");
            if (String.IsNullOrEmpty(ConfirmPassword))
                msg += string.Format("<div>{0}</div>", "تایید رمز عبور جدید را وارد نمایید");
            if (msg == "")
            {
                if (!NewPassword.Equals(ConfirmPassword))
                {
                    msg += string.Format("<div>{0}</div>", "رمز عبور جدید و تایید آن یکی نیست");
                    ViewBag.Error = msg;
                    return View();
                }
                else
                {
                    var result = UserManager.ChangePassword(User.Identity.GetUserId(), OldPassword, NewPassword);
                    if (result.Succeeded)
                    {
                        var au = UserManager.FindById(User.Identity.GetUserId());
                        #region EventLogger
                        Infrastructure.EventLog.Logger.Add(3, "Home", "ChangePassword", false, 200, " تغییر رمز عبورِ کاربرِ  " + au.FirstName + " " + au.LastName, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        ViewBag.Message = " رمز عبور با موفقیت تغییر یافت. ";
                    }
                    else
                    {
                        ViewBag.Error = "خطایی رخ داد بعدا تلاش نمایید.";
                    }

                }
            }
            else
            {
                ViewBag.Error = msg;
                return View();
            }

            return View();

        }



        public virtual PartialViewResult GetIconList()
        {
            return PartialView("_IconList");

        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
