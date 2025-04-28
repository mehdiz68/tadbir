using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoreLib.Infrastructure.DateTime;
using PagedList;
using System.Net;
using CoreLib.Infrastructure.CustomAttribute;
using Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using CoreLib.Infrastructure;
using System.Data.Entity;
using System.Threading.Tasks;
using CoreLib.Infrastructure.ModelBinder;
using ahmadi.Infrastructure.Security;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class ProductQuestionsController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;

        public ProductQuestionsController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }
        //
        // GET: /Admin/ProductQuestions/
        [CorrectArabianLetter(new string[] { "UserFilter", "UserString", "ProductFilter", "ProductString" })]
        public virtual ActionResult Index(string sortOrder, string UserFilter, string UserString, string ProductFilter, string ProductString, string ActiveFilter, string ActiveString, string StartDateFilter, string StartDateString, string EndDateFilter, string EndDateString, int? page)
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


                    ViewBag.UserFilter = UserString;
                    ViewBag.ProductFilter = ProductString;
                    ViewBag.ActiveFilter = ActiveString;
                    ViewBag.StartDateFilter = StartDateString;
                    ViewBag.EndDateFilter = EndDateString;

                    var productQuestion = uow.ProductQuestionRepository.GetQueryList().AsNoTracking().Include("User").Include("Product").Include("Product.ProductImages.Image").Include("Product.ProductPrices").Include("ChildComment").Where(x => x.ParrentId == null);

                    if (!String.IsNullOrEmpty(UserString))
                    {
                        if (UserString.IndexOf(" ") != -1)
                        {
                            string strName = UserString.Substring(0, UserString.IndexOf(" "));
                            string strFamily = UserString.Remove(0, UserString.IndexOf(" ") + 1);
                            if (!string.IsNullOrEmpty(strName))
                                productQuestion = productQuestion.Where(s => s.User.FirstName == strName);
                            if (!string.IsNullOrEmpty(strFamily))
                                productQuestion = productQuestion.Where(s => s.User.LastName == strFamily);
                        }
                        else
                            productQuestion = productQuestion.Where(s => s.User.FirstName == UserString);
                    }
                    if (!String.IsNullOrEmpty(ProductString))
                    {
                        productQuestion = productQuestion.Where(s => s.Product.Name.Contains(ProductString) || s.Product.Title.Contains(ProductString) || s.Product.LatinName.Contains(ProductString) || s.Product.Code.Contains(ProductString));
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
                        case "count":
                            productQuestion = productQuestion.OrderBy(s => s.ChildComment.Count);
                            ViewBag.CurrentSort = "count";
                            break;
                        case "count_desc":
                            productQuestion = productQuestion.OrderByDescending(s => s.ChildComment.Count);
                            ViewBag.CurrentSort = "count_desc";
                            break;
                        case "Product":
                            productQuestion = productQuestion.OrderBy(s => s.Product.Title);
                            ViewBag.CurrentSort = "Product";
                            break;
                        case "Product_desc":
                            productQuestion = productQuestion.OrderByDescending(s => s.Product.Title);
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
                            ViewBag.CurrentSort = "Date_desc";
                            productQuestion = productQuestion.OrderByDescending(s => s.InsertDate);
                            break;
                    }

                    #endregion

                    int pageSize = 8;
                    int pageNumber = (page ?? 1);

                    ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "پرسش و پاسخ محصولات", null, "HelpModuleSections").FirstOrDefault();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductQuestions", "Index", true, 200, " نمایش صفحه مدیریت پرسش های محصول ", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(productQuestion.ToPagedList(pageNumber, pageSize));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت پرسش های محصول" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ProductQuestions/Details/5
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
                    var productQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == id, null, "User,ParentComment.User,Product,ChildComment.User").FirstOrDefault();
                    if (productQuestion == null)
                    {
                        return HttpNotFound();
                    }
                    productQuestion.Visited = true;
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductQuestions", "Details", true, 200, " نمایش جزئیاتِ پرسش در مورد محصول  " + productQuestion.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(productQuestion);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت پرسشهای کاربران در مورد محصول" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "Details", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> replay(string ReplayMessage, int Id, string[] janebi)
        {
            uow = new UnitOfWork.UnitOfWorkClass();
            try
            {
                ProductQuestion productQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == Id, null, "User,ParentComment.User,Product,ChildComment.User").Single();
                if (productQuestion == null)
                {
                    return Json(new
                    {
                        message = "پرسش یافت نشد",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }

                #region Add Comment

                ProductQuestion prq = new ProductQuestion()
                {
                    InsertDate = DateTime.Now,
                    IsActive = true,
                    Like = 0,
                    Message = ReplayMessage,
                    ProductId = productQuestion.ProductId,
                    ParrentId = Id,
                    UnLike = 0,
                    UserId = User.Identity.GetUserId(),
                    Visited = true,
                    AdminAnswer = true
                };

                if (janebi != null)
                {
                    if (janebi.Any())
                    {
                        prq.attachments = new List<attachment> ();
                        foreach (var item in janebi)
                        {
                            Guid id = new Guid(item);
                            prq.attachments.Add(uow.AttachmentRepository.GetByID(id));
                        }
                    }
                }


                uow.ProductQuestionRepository.Insert(prq);
                uow.Save();
                #endregion

                #region Edit Parent Comment
                if (productQuestion.IsActive == false)
                {
                    productQuestion.IsActive = true;
                    uow.Save();
                }
                #endregion

                uow.context.Entry(productQuestion).Collection(x => x.ChildComment).Load();

                #region Create Html Body
                string EmailBodyHtml = "";

                var oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment").SingleOrDefault();
                string contentUrl = "http://" + HttpContext.Request.Url.Host + "/TFP/" + productQuestion.ProductId + "/" + CommonFunctions.NormalizeAddress(productQuestion.Product.PageAddress);
                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید و پاسخ به پرسش شما", "کاربر گرامی ، " + productQuestion.User.firstTime + " " + productQuestion.User.LastName + "، توسط مدیر سایت برای پرسش شما پاسخی ثبت شد :<br/><div>" + ReplayMessage + "</div><br/> لینک صفحه :  <a href='" + contentUrl + "'>" + productQuestion.Product.Title + "</a> ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                #endregion

                #region SendMail

                try
                {

                    EmailService es = new EmailService();
                    IdentityMessage imessage = new IdentityMessage();
                    imessage.Body = EmailBodyHtml;
                    imessage.Destination = productQuestion.User.Email;
                    imessage.Subject = " تایید و پاسخ به نظر شما ";
                    await es.SendAsync(imessage);
                }
                catch (Exception)
                {

                }

                #endregion

                return Json(new
                {
                    message = "پاسخ شما ثبت شد و در سایت نمایش پیدا کرد.",
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

        //[HttpPost]
        //public virtual async Task<ActionResult> Replay(string ReplayMessage, int Id)
        //{
        //    var p = ModulePermission.check(User.Identity.GetUserId(), 16);
        //    if (p.Where(x => x == true).Any())
        //    {
        //        ViewBag.EditPermission = p.Skip(1).First();
        //        ViewBag.DeletePermission = p.Skip(2).First();
        //    }
        //    uow = new UnitOfWork.UnitOfWorkClass();
        //    ProductQuestion productQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == Id, null, "User,ParentComment.User,Product,ChildComment.User").Single();
        //    if (productQuestion == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    try
        //    {
        //        #region Add Comment

        //        ProductQuestion prq = new ProductQuestion()
        //        {
        //            InsertDate = DateTime.Now,
        //            IsActive = true,
        //            Like = 0,
        //            Message = ReplayMessage,
        //            ProductId = productQuestion.ProductId,
        //            ParrentId = Id,
        //            UnLike = 0,
        //            UserId = User.Identity.GetUserId(),
        //            Visited = true,
        //            AdminAnswer=true
        //        };
        //        uow.ProductQuestionRepository.Insert(prq);
        //        uow.Save();
        //        #endregion

        //        #region Edit Parent Comment
        //        ViewBag.message = "پاسخ شما ثبت شد و در سایت نمایش پیدا کرد.";
        //        if (productQuestion.IsActive == false)
        //        {
        //            productQuestion.IsActive = true;
        //            uow.Save();
        //        }
        //        #endregion

        //        uow.context.Entry(productQuestion).Collection(x => x.ChildComment).Load();

        //        #region Create Html Body
        //        string EmailBodyHtml = "";

        //        var oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment").SingleOrDefault();
        //        string contentUrl = "http://" + HttpContext.Request.Url.Host + "/TFP/" + productQuestion.ProductId + "/" + CommonFunctions.NormalizeAddress(productQuestion.Product.PageAddress);
        //        CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید و پاسخ به پرسش شما", "کاربر گرامی ، " + productQuestion.User.firstTime + " " + productQuestion.User.LastName + "، توسط مدیر سایت برای پرسش شما پاسخی ثبت شد :<br/><div>" + ReplayMessage + "</div><br/> لینک صفحه :  <a href='" + contentUrl + "'>" + productQuestion.Product.Title + "</a> ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
        //        EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
        //        #endregion

        //        #region SendMail
        //        EmailService es = new EmailService();
        //        IdentityMessage imessage = new IdentityMessage();
        //        imessage.Body = EmailBodyHtml;
        //        imessage.Destination = productQuestion.User.Email;
        //        imessage.Subject = " تایید و پاسخ به نظر شما ";
        //        await es.SendAsync(imessage);

        //        #endregion


        //        #region EventLogger
        //        ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductQuestions", "Replay", false, 200, " پاسخ دهی به پرسش " + productQuestion.User.LastName, DateTime.Now, User.Identity.GetUserId());
        //        #endregion
        //        return View("Details", productQuestion);
        //    }
        //    catch (Exception x)
        //    {

        //        #region EventLogger
        //        ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "Replay", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
        //        #endregion
        //        return View("Details", productQuestion);
        //    }
        //}

        [HttpPost]
        [HttpParamAction]
        [ValidateInput(false)]
        public virtual async Task<ActionResult> Edit([Bind(Include = "Id,Message")] ProductQuestion productQuestion, string Answer)
        {
            try
            {
                var pq = uow.ProductQuestionRepository.GetByID(productQuestion.Id);
                if (pq != null)
                {
                    if (pq.ParrentId != null)
                        pq.ParentComment.Message = productQuestion.Message;
                    else
                        pq.Message = productQuestion.Message;

                    if (!string.IsNullOrEmpty(Answer))
                    {
                        IdentityManager im = new IdentityManager();
                        if (pq.ParrentId == null)
                        {
                            ProductQuestion newPQ = new ProductQuestion()
                            {
                                Message = Answer,
                                UserId = User.Identity.GetUserId(),
                                InsertDate = DateTime.Now,
                                Visited = true,
                                IsActive = true,
                                ProductId = pq.ProductId,
                                ParrentId = pq.Id
                            };
                            uow.ProductQuestionRepository.Insert(newPQ);
                        }
                        else
                        {
                            pq.Message = Answer;
                        }
                        pq.IsActive = true;

                        await SendMail(pq);
                    }


                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductQuestions", "Edit", false, 200, "ویرایش پرسش در مورد محصول " + pq.Message, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                else
                {
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "Edit", false, 500, "خطا در ویرایش پرسش در مورد محصول" + pq.Message, DateTime.Now, User.Identity.GetUserId());
                    #endregion

                    return View(pq);
                }
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        private async Task<bool> SendMail(ProductQuestion pq)
        {
            try
            {
                #region SendMail to Current Question User
                EmailService es = new EmailService();

                IdentityMessage imessage = new IdentityMessage();
                string EmailBodyHtml = "";

                var oSetting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
                string productName = pq.Product.Name;
                string productUrl = "http://" + HttpContext.Request.Url.Host + "/product/" + pq.ProductId + "/" + CommonFunctions.NormalizeAddress(productName);
                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید سوال شما", "کاربر گرامی ، " + pq.User.FirstName + " " + pq.User.LastName + "، سوال شما تایید شد.لینک صفحه : <br/> <a href='" + productUrl + "'>" + productName + "</a>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                #endregion

                imessage.Body = EmailBodyHtml;
                imessage.Destination = pq.User.Email;
                if (pq.ParrentId.HasValue)
                    imessage.Subject = " تایید پاسخ شما ";
                else
                    imessage.Subject = " تایید سوال شما ";
                await es.SendAsync(imessage);

                if (pq.ParrentId.HasValue)
                {
                    var ParrentQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == pq.ParrentId.Value, null, "ChildComment").SingleOrDefault();

                    if (ParrentQuestion.ChildComment.Count() == 1)
                    {
                        //Send Email
                        try
                        {
                            #region SendMail
                            #region Create Html Body

                            string contentUrl = "http://" + HttpContext.Request.Url.Host + "/Product/" + pq.ProductId;
                            emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "پاسخ به پرسش شما", "کاربر گرامی ، پاسخی به پرسش شما در سایت ثبت شد . شما میتوانید با مراجعه به سایت آن را ببینید. لینک صفحه مربوط به پرسش : <br/> <a href='" + contentUrl + "'>لینک</a><br/> پاسخ : <br/> <p>" + pq.Message + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                            EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                            #endregion
                            imessage.Body = EmailBodyHtml;
                            imessage.Destination = ParrentQuestion.User.Email;
                            imessage.Subject = " پاسخ به پرسش شما ";
                            await es.SendAsync(imessage);

                            #endregion
                        }
                        catch (Exception)
                        {

                        }

                    }

                }


                return true;
            }
            catch (Exception)
            {

                return true;
            }

        }

        [HttpParamAction]
        [HttpPost]
        public virtual async Task<ActionResult> ConfirmQuestion(int id)
        {
            try
            {
                var productQuestion = uow.ProductQuestionRepository.Get(x => x, x => x.Id == id, null, "Product,User").Single();
                if (productQuestion != null)
                {
                    if (productQuestion.ParrentId != null)
                    {
                        productQuestion.ParentComment.IsActive = !productQuestion.ParentComment.IsActive;
                        if (productQuestion.ParentComment.IsActive)
                            await SendMail(productQuestion.ParentComment);
                    }
                    else
                    {
                        productQuestion.IsActive = !productQuestion.IsActive;
                        if (productQuestion.IsActive)
                            await SendMail(productQuestion);
                    }
                    uow.Save();

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductQuestions", "ConfirmQuestion", false, 200, "تایید و عدم تایید پرسش در مورد محصول " + productQuestion.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "ConfirmQuestion", false, 500, "خطا در تایید و عدم تایید پرسش محصول" + productQuestion.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(productQuestion);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "ConfirmQuestion", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpParamAction]
        [HttpPost]
        public virtual async Task<ActionResult> ConfirmAnswer(int id)
        {
            try
            {
                var productAnswer = uow.ProductQuestionRepository.Get(x => x, x => x.Id == id, null, "Product,User").Single();
                if (productAnswer != null)
                {
                    productAnswer.IsActive = !productAnswer.IsActive;
                    uow.Save();
                    if (productAnswer.IsActive)
                        await SendMail(productAnswer);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductQuestions", "ConfirmAnswer", false, 200, "تایید و عدم تایید پاسخ در مورد محصول " + productAnswer.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "ConfirmAnswer", false, 500, "خطا در تایید و عدم تایید پاسخ محصول" + productAnswer.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(productAnswer);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "ConfirmAnswer", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpParamAction]
        [HttpPost]
        public virtual ActionResult DeleteQuestion(int id)
        {
            try
            {
                var productQuestion = uow.ProductQuestionRepository.GetByID(id);
                if (productQuestion != null)
                {
                    if (productQuestion.ParrentId != null)
                    {
                        foreach (var row in productQuestion.ParentComment.ChildComment)
                        {
                            uow.ProductQuestionRepository.Delete(row);
                        }
                        uow.ProductQuestionRepository.Delete(productQuestion.ParentComment);
                    }
                    else
                        uow.ProductQuestionRepository.Delete(productQuestion);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductQuestions", "DeleteQuestion", false, 200, "جذف پرسش در مورد محصول " + productQuestion.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "DeleteQuestion", false, 500, "خطا در جذف پرسش در مورد محصول" + productQuestion.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(productQuestion);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "DeleteQuestion", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpParamAction]
        [HttpPost]
        public virtual ActionResult DeleteAnswer(int id)
        {
            try
            {
                var productAnswer = uow.ProductQuestionRepository.GetByID(id);
                if (productAnswer != null)
                {
                    uow.ProductQuestionRepository.Delete(productAnswer);
                    uow.Save();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(3, "ProductQuestions", "DeleteAnswer", false, 200, "جذف پاسخ در مورد محصول " + productAnswer.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Index");
                }
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "DeleteAnswer", false, 500, "خطا در جذف پاسخ در مورد محصول" + productAnswer.Id, DateTime.Now, User.Identity.GetUserId());
                #endregion

                return View(productAnswer);
            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductQuestions", "DeleteAnswer", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
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