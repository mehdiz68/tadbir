using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using Domain;
using System.Data.Entity;
using CoreLib.Infrastructure.CustomAttribute;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using CoreLib.Infrastructure;
using CoreLib.Infrastructure.ModelBinder;
using ahmadi.Infrastructure.Security;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class ProductCommentController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;

        public ProductCommentController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        //
        // GET: /Admin/ProductComment/
        [CorrectArabianLetter(new string[] { "UserString", "UserFilter", "ProductString", "ProductFilter", "TitleString", "TitleFilter" })]
        public virtual ActionResult Index(string sortOrder, string UserFilter, string UserString, string ProductFilter, string ProductString, string TitleFilter, string TitleString, string ActiveFilter, string ActiveString, int? page)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    List<SelectListItem> ActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "تایید", Value = "True" }, new SelectListItem() { Text = "عدم تایید", Value = "False" } };
                    ViewBag.ActiveSelectListItem = ActiveSelectListItem;

                    #region search
                    if (string.IsNullOrEmpty(UserString))
                        UserString = UserFilter;
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

                    ViewBag.UserFilter = UserString;
                    ViewBag.ProductFilter = ProductString;
                    ViewBag.TitleFilter = TitleString;
                    ViewBag.ActiveFilter = ActiveString;

                    var productComment = uow.ProductCommentRepository.GetQueryList().AsNoTracking().Include("User").Include("Product");
                    if (!String.IsNullOrEmpty(UserString))
                    {
                        if (UserString.IndexOf(" ") != -1)
                        {
                            string strName = UserString.Substring(0, UserString.IndexOf(" "));
                            string strFamily = UserString.Remove(0, UserString.IndexOf(" ") + 1);
                            if (!string.IsNullOrEmpty(strName))
                                productComment = productComment.Where(s => s.User.FirstName == strName);
                            if (!string.IsNullOrEmpty(strFamily))
                                productComment = productComment.Where(s => s.User.LastName == strFamily);
                        }
                        else
                            productComment = productComment.Where(s => s.User.FirstName == UserString);
                    }
                    if (!String.IsNullOrEmpty(ProductString))
                    {
                        productComment = productComment.Where(s => s.Product.Name.Contains(ProductString) || s.Product.Title.Contains(ProductString) || s.Product.LatinName.Contains(ProductString) || s.Product.Code.Contains(ProductString));
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
                        case "IsBuy":
                            productComment = productComment.OrderBy(s => s.IsBuy);
                            ViewBag.CurrentSort = "IsBuy";
                            break;
                        case "IsBuy_desc":
                            productComment = productComment.OrderByDescending(s => s.IsBuy);
                            ViewBag.CurrentSort = "IsBuy_desc";
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
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductComment", "Index", true, 200, " نمایش صفحه مدیریت نظرات محصول ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(productComment.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت نظرات محصول" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ProductComment/Details/5
        public virtual ActionResult Details(int? id)
        {
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 16);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var productComment = uow.ProductCommentRepository.Get(x => x, x => x.Id == id, null, "ProductCommentAdvantages,ProductCommentDisAdvantages").SingleOrDefault();
                    if (productComment == null)
                    {
                        return HttpNotFound();
                    }
                    productComment.Visited = true;
                    uow.Save();

                    ViewBag.Buyer = uow.ProductRepository.CheckBuyer(productComment.ProductId, productComment.UserId);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductComment", "Details", true, 200, " نمایش جزئیاتِ نظر در مورد محصول  " + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(productComment);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت نظرات محصول" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public virtual JsonResult Edit([Bind(Include = "Id,Title,Text")] ProductComment productComment, IEnumerable<ProductCommentAdvantage> advantage, IEnumerable<ProductCommentDisAdvantage> disAdvantage)
        {
            try
            {
                var pc = uow.ProductCommentRepository.GetByID(productComment.Id);
                if (pc != null)
                {
                    pc.Title = CommonFunctions.CorrectArabianLetter(pc.Title);
                    pc.Title = productComment.Title;
                    pc.Text = productComment.Text;

                    if (advantage != null)
                    {
                        foreach (var item in advantage)
                        {
                            item.ProductCommentId = productComment.Id;
                            uow.ProductCommentRepository.Update(productComment);
                        }
                    }
                    if (disAdvantage != null)
                    {
                        foreach (var item in disAdvantage)
                        {
                            item.ProductCommentId = productComment.Id;
                            uow.ProductCommentRepository.Update(productComment);
                        }
                    }
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductComment", "Edit", false, 200, "ویرایش نظر محصول " + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        data = true,
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Edit", false, 500, "خطا در ویرایش نظر محصول" + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    return Json(new
                    {
                        data = false,
                        statusCode = 400
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return Json(new
                {
                    data = x.Message,
                    statusCode = 400
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task<bool> SendMail(ProductComment pc)
        {
            try
            {
                #region SendMail
                EmailService es = new EmailService();

                IdentityMessage imessage = new IdentityMessage();
                string EmailBodyHtml = "";

                var oSetting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                string productName = pc.Product.Name;
                string productUrl = "http://" + HttpContext.Request.Url.Host + "/product/" + pc.ProductId + "/" + CommonFunctions.NormalizeAddress(productName);
                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید نظر شما", "کاربر گرامی ، " + pc.User.FirstName + " " + pc.User.LastName + "، نظر شما تایید شد.لینک صفحه : <br/> <a href='" + productUrl + "'>" + productName + "</a>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                #endregion

                imessage.Body = EmailBodyHtml;
                imessage.Destination = pc.User.Email;
                imessage.Subject = " تایید نظر شما ";
                await es.SendAsync(imessage);
                return true;
            }
            catch (Exception)
            {
                return true;
            }

        }

        [HttpParamAction]
        [HttpPost]
        public virtual async Task<ActionResult> Confirm(int id)
        {
            try
            {
                var productComment = uow.ProductCommentRepository.Get(x => x, x => x.Id == id, null, "Product,User").Single();
                if (productComment != null)
                {
                    productComment.IsActive = !productComment.IsActive;
                    uow.Save();
                    try
                    {
                        // تخصیص کد تخفیف نظر سفارش
                        if (productComment.IsActive)
                        {
                            if (uow.ProductRepository.CheckBuyer(productComment.ProductId, productComment.UserId))
                            {
                                DateTime date = DateTime.Now;
                                var offer = uow.OfferRepository.Get(x => x, x => x.codeUseType == CodeUseType.ثبت_پس_از_ثبت_نظر_سفارش && x.CodeTypeValueCode == 4 && x.IsActive && x.LanguageId == 1 && x.state && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "userOfferMessages").FirstOrDefault();
                                if (offer != null)
                                {
                                    var order = uow.OrderRepository.Get(x =>x, x => x.OrderRows.Any(s => s.ProductId == productComment.ProductId) && x.UserId == productComment.UserId && x.SetGiftCode == false && x.compeleteRate == true, null, "OrderRows").FirstOrDefault();
                                    if (order != null)
                                    {
                                        List<int> pids = order.OrderRows.Select(a => a.ProductId).ToList();
                                        int TotalOrderCommentsCount = uow.ProductCommentRepository.Get(s => s, s => pids.Contains(s.ProductId) && s.IsActive == true).Count();
                                        if (pids.Count == TotalOrderCommentsCount)
                                        {

                                            Random r = new Random();
                                            int x = r.Next(100, 999);
                                            UserCodeGift userCodeGift = new UserCodeGift()
                                            {
                                                Value = offer.DeflautValue,
                                                Code = offer.DefalutCode + x,
                                                CodeType = offer.DefaultCodeType,
                                                CountUse = offer.DefalutCountUse,
                                                MaxValue = offer.DefalutMaxValue,
                                                IsActive = true,
                                                OfferId = offer.Id,
                                                UserId = productComment.UserId
                                            };
                                            if (offer.DefaultDayExpire.HasValue)
                                                if (offer.DefaultDayExpire.Value > 0)
                                                    userCodeGift.ExpireDate = DateTime.Now.AddDays(offer.DefaultDayExpire.Value);
                                            uow.UserCodeGiftRepository.Insert(userCodeGift);
                                            if (offer.userOfferMessages.Any())
                                            {
                                                var message = offer.userOfferMessages.First();
                                                message.state = false;
                                                uow.UserOfferMessageRepository.Update(message);
                                                UserOfferMessageMember userOfferMessageMember = new UserOfferMessageMember()
                                                {
                                                    InsertDate = DateTime.Now,
                                                    state = OfferMessageSendMessageType.Waiting,
                                                    UserId = productComment.UserId,
                                                    UserOfferMessageId = offer.userOfferMessages.First().Id
                                                };
                                                uow.UserOfferMessageMemberRepository.Insert(userOfferMessageMember);
                                            }
                                            uow.Save();

                                            order.SetGiftCode = true;
                                            uow.OrderRepository.Update(order);
                                            uow.Save();
                                        }
                                    }
                                }
                            }
                        }
                        await SendMail(productComment);
                    }
                    catch (Exception)
                    {

                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductComment", "Confirm", false, 200, "تایید و عدم تایید نظر محصول " + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Confirm", false, 500, "خطا در تایید و عدم تایید نظر محصول" + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(productComment);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Confirm", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        [HttpParamAction]
        [HttpPost]
        public ActionResult SetGiftCode(int id)
        {
            try
            {
                var productComment = uow.ProductCommentRepository.Get(x => x, x => x.Id == id, null, "Product,User").Single();
                if (productComment != null)
                {
                    if (!uow.ProductRepository.CheckBuyer(productComment.ProductId, productComment.UserId))
                    {
                        productComment.SetGiftCode = true;
                        productComment.IsActive = true;
                        uow.Save();
                        try
                        {

                            #region اعطای کد تخفیف
                            DateTime date = DateTime.Now;
                            var offer = uow.OfferRepository.Get(x => x, x => x.codeUseType == CodeUseType.ثبت_پس_از_ثبت_نظر_محصول && x.CodeTypeValueCode == 4 && x.IsActive && x.LanguageId == 1 && x.state && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "userOfferMessages").FirstOrDefault();
                            if (offer != null)
                            {
                                Random r = new Random();
                                int x = r.Next(100, 999);
                                UserCodeGift userCodeGift = new UserCodeGift()
                                {
                                    Value = offer.DeflautValue,
                                    Code = offer.DefalutCode + x,
                                    CodeType = offer.DefaultCodeType,
                                    CountUse = offer.DefalutCountUse,
                                    MaxValue = offer.DefalutMaxValue,
                                    IsActive = true,
                                    OfferId = offer.Id,
                                    UserId = productComment.UserId
                                };
                                if (offer.DefaultDayExpire.HasValue)
                                    if (offer.DefaultDayExpire.Value > 0)
                                        userCodeGift.ExpireDate = DateTime.Now.AddDays(offer.DefaultDayExpire.Value);
                                uow.UserCodeGiftRepository.Insert(userCodeGift);
                                if (offer.userOfferMessages.Any())
                                {
                                    var message = offer.userOfferMessages.First();
                                    message.state = false;
                                    uow.UserOfferMessageRepository.Update(message);
                                    UserOfferMessageMember userOfferMessageMember = new UserOfferMessageMember()
                                    {
                                        InsertDate = DateTime.Now,
                                        state = OfferMessageSendMessageType.Waiting,
                                        UserId = productComment.UserId,
                                        UserOfferMessageId = offer.userOfferMessages.First().Id
                                    };
                                    uow.UserOfferMessageMemberRepository.Insert(userOfferMessageMember);
                                }
                                uow.Save();
                            }
                            #endregion


                        }
                        catch (Exception)
                        {

                        }
                    }
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductComment", "SetGiftCode", false, 200, "تخصیص کد تخفیف نظر محصول " + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "SetGiftCode", false, 500, "خطا در تخصیص کد تخفیف نظر محصول" + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(productComment);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "SetGiftCode", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpParamAction]
        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            try
            {
                var productComment = uow.ProductCommentRepository.GetByID(id);
                if (productComment != null)
                {
                    uow.ProductCommentRepository.Delete(productComment);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductComment", "Delete", false, 200, "حذف نظر محصول " + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Delete", false, 500, "خطا در حذف نظر محصول" + productComment.Title, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(productComment);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductComment", "Delete", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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