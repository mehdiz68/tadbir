using ahmadi.Infrastructure.Helper;
using CoreLib.Infrastructure;
using CoreLib.Infrastructure.Captcha;
using Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UnitOfWork;

namespace ahmadi.Controllers
{
    [Authorize]
    public class FreeConsultationController : BaseController
    {
        private UnitOfWorkClass uow = null;
        public FreeConsultationController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }
        // GET: FreeConsultation
        public ActionResult Index()
        {
            #region Get Setting 

            var setting = GetSetting();

            #endregion
            #region Meta
            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "سوالات حقوقی خود را در عدلجو بپرسید و به سرعت پاسخ آنها را دریافت نمایید.";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "ثبت پرسش جدید در عدل جو";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + "/FreeConsultation");
            oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + setting.attachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            ViewBag.cats = uow.TicketCategoryRepository.Get(x => x, x => x.IsActive);
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Index(Ticket Ticket, int CaptchaValue)
        {

            #region Get Setting 

            var setting = GetSetting();

            #endregion
            #region Meta
            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "سوالات حقوقی خود را در عدلجو بپرسید و به سرعت پاسخ آنها را دریافت نمایید.";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "ثبت پرسش جدید در عدل جو";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + "/FreeConsultation");
            oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + setting.attachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {
                if (CaptchaValue == Convert.ToInt32(Session["captcha"]))
                {
                    Ticket MainTicket = new Ticket();
                    zCaptcha c = new zCaptcha();
                    Session["captcha"] = c.GetCurrentValue();
                    Ticket.InsertDate = DateTime.Now;
                    Ticket.UserId = User.Identity.GetUserId();
                    uow.TicketRepository.Insert(Ticket);
                    uow.Save();

                    Ticket.Code = CoreLib.Infrastructure.CommonFunctions.GetOrderCode(Ticket.Id);
                    uow.TicketRepository.Update(Ticket);
                    uow.Save();

                    var user = uow.UserRepository.GetByID(Ticket.UserId);


                    //Send Mail To Related Admin And User
                    try
                    {

                        #region Create Html Body
                        string EmailBodyHtml = "";
                        var oSetting = uow.SettingRepository.Get(x => x, null, null, "attachment").FirstOrDefault();
                        string contentUrl = "https://" + HttpContext.Request.Url.Host;
                        CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "ثبت پرسش جدید در سایت", "مدیر گرامی پرسش جدیدی در سایت ثبت شد . شما میتوانید با مراجعه به پنل مدیریت آن را ببینید.لینک صفحه مربوط به پرسش : <br/> <a href='" + contentUrl + "'>ثبت پرسش جدید در عدل جو</a><br/>پیام : <br/><p>" + Ticket.Message + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                        EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                        #endregion

                        #region SendMail
                        var emails = uow.AdministratorPermissionRepository.Get(x => x, x => x.ModuleId == 4 && x.NotificationEmail == true, null, "user").Select(x => x.User.Email).Distinct();
                        if (emails.Any())
                        {
                            if (emails.Any(s => s != null))
                            {
                                EmailService es = new EmailService();
                                await es.SendMultiDestinationAsync(" ثبت پیام تماس با مای جدید در سایت ", EmailBodyHtml, emails.ToList());
                            }
                        }

                        #endregion


                    }
                    catch (Exception)
                    {

                    }

                    string ticketCode = Ticket.Code;


                    try
                    {
                        #region SendSMS
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = User.Identity.Name;
                        iPhonemessage.Body = setting.WebSiteName + "\n" + user.FirstName + " " + "عزیز\n" + " درخواست شما با شماره پیگیری  " + Ticket.Code + " ثبت شد . پس از پاسخ دهی، از طریق پیامک مطلع خواهید شد . ";
                        await sms.SendSMSAsync(iPhonemessage, "NewQuestionAdd", Ticket.Code, null, null, user.FirstName);


                        #endregion
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = setting.Mobile;
                        iPhonemessage.Body = setting.WebSiteName + " پرسش جدیدی با شماره " + Ticket.Code + " در سایت ثبت شد . ";
                        await sms.SendSMSAsync(iPhonemessage, "NewAdminQuestionAdd", Ticket.Code, null, null, null);
                    }
                    catch (Exception)
                    {

                    }

                    return Json(new
                    {
                        data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                        Message = "درخواست شما با شماره پیگیری " + Ticket.Code + " ثبت شد. پس از پاسخ دهی، از طریق پیامک مطلع خواهید شد.",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    zCaptcha c = new zCaptcha();
                    Session["captcha"] = c.GetCurrentValue();
                    return Json(new
                    {
                        data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                        Message = "پاسخ اشتباه",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception x)
            {
                zCaptcha c = new zCaptcha();
                Session["captcha"] = c.GetCurrentValue();
                return Json(new
                {
                    data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                    Message = "درخواست شما با شماره پیگیری " + Ticket.Code + " ثبت شد. پس از پاسخ دهی، از طریق پیامک مطلع خواهید شد." + x.Message,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Replay(Ticket Ticket, bool ReplayAnswer)
        {

            #region Get Setting 

            var setting = GetSetting();

            #endregion
            Ticket MainTicket = new Ticket();

            string ticketCode = "";
            try
            {
                zCaptcha c = new zCaptcha();
                Session["captcha"] = c.GetCurrentValue();
                Ticket.InsertDate = DateTime.Now;
                Ticket.UserId = User.Identity.GetUserId();

                List<int> CatIds = uow.ContentRepository.SqlQuery("exec [GetParentTickets] @CatId", new SqlParameter("@CatId", Ticket.ParrentId)).ToList();
                int MainTicketId = CatIds.FirstOrDefault();
                MainTicket = uow.TicketRepository.Get(x=>x,x=>x.Id==MainTicketId,null,"User").SingleOrDefault();
                MainTicket.UpdateDate = DateTime.Now;
                Ticket.Answer = ReplayAnswer;
                Ticket.IsVisit = ReplayAnswer ? true : false;
                Ticket.UserIsVisit = ReplayAnswer ? false : true;

                uow.TicketRepository.Insert(Ticket);
                uow.Save();

                Ticket.Code = CoreLib.Infrastructure.CommonFunctions.GetOrderCode(Ticket.Id);
                uow.TicketRepository.Update(Ticket);
                uow.Save();

                var user = uow.UserRepository.GetByID(Ticket.UserId);

                if (Ticket.Answer == false)
                {
                    //Send Mail To Related Admin And User
                    try
                    {

                        #region Create Html Body
                        string EmailBodyHtml = "";
                        var oSetting = uow.SettingRepository.Get(x => x, null, null, "attachment").FirstOrDefault();
                        string contentUrl = "https://" + HttpContext.Request.Url.Host;
                        CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "ثبت پرسش جدید در سایت", "مدیر گرامی پرسش جدیدی در سایت ثبت شد . شما میتوانید با مراجعه به پنل مدیریت آن را ببینید.لینک صفحه مربوط به پرسش : <br/> <a href='" + contentUrl + "'>ثبت پرسش جدید در عدل جو</a><br/>پیام : <br/><p>" + Ticket.Message + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                        EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                        #endregion

                        #region SendMail
                        var emails = uow.AdministratorPermissionRepository.Get(x => x, x => x.ModuleId == 4 && x.NotificationEmail == true, null, "user").Select(x => x.User.Email).Distinct();
                        if (emails.Any())
                        {
                            if (emails.Any(s => s != null))
                            {
                                EmailService es = new EmailService();
                                await es.SendMultiDestinationAsync(" ثبت پیام تماس با مای جدید در سایت ", EmailBodyHtml, emails.ToList());
                            }
                        }

                        #endregion


                    }
                    catch (Exception)
                    {

                    }
                }

                ticketCode = MainTicket.Code;


                try
                {
                    #region SendSMS
                    SmsService sms = new SmsService();
                    IdentityMessage iPhonemessage = new IdentityMessage();
                    iPhonemessage.Destination = User.Identity.Name;
                    if (Ticket.Answer == false)
                    {
                        iPhonemessage.Body = setting.WebSiteName + "\n" + user.FirstName + " " + "عزیز\n" + " درخواست شما با شماره پیگیری  " + ticketCode + " ثبت شد . پس از پاسخ دهی، از طریق پیامک مطلع خواهید شد . ";
                        await sms.SendSMSAsync(iPhonemessage, "NewQuestionAdd", ticketCode, null, null, user.FirstName);
                    }
                    else
                    {
                        iPhonemessage.Destination = MainTicket.User.PhoneNumber;
                        iPhonemessage.Body = setting.WebSiteName + "\n" + user.FirstName + " " + "عزیز\n" + " درخواست شما با شماره پیگیری  " + ticketCode + " پاسخ داده شد .  برای پیگیری به لینک زیر مراجعه فرمایید:\n " + "https://www.adljou.com/Profile/Ticket/" + ticketCode;
                        await sms.SendSMSAsync(iPhonemessage, "NewAnswerAdd", ticketCode, null, null, user.FirstName, "https://www.adljou.com/Profile/Ticket/" + ticketCode);
                    }


                    #endregion
                }
                catch (Exception)
                {

                }
                if (Ticket.Answer == false)
                {
                    try
                    {
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = setting.Mobile;
                        iPhonemessage.Body = setting.WebSiteName + " پرسش جدیدی با شماره " + Ticket.Code + " در سایت ثبت شد . ";
                        await sms.SendSMSAsync(iPhonemessage, "NewAdminQuestionAdd", Ticket.Code, null, null, null);
                    }
                    catch (Exception)
                    {

                    }
                }
                if (ReplayAnswer)
                    return Redirect("~/Admin/Tickets/Details/" + MainTicket.Id);
                else
                    return Redirect("~/Profile/Ticket/" + MainTicket.Code);

            }
            catch (Exception x)
            {
                if (ReplayAnswer)
                    return Redirect("~/Admin/Tickets/Details/" + MainTicket.Id);
                else
                    return Redirect("~/Profile/Ticket/" + MainTicket.Code);
            }
        }
    }
}