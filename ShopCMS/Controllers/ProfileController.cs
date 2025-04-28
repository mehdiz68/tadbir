using Domain;
using ahmadi.Infrastructure.Security;
using ahmadi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UnitOfWork;
using ahmadi.ViewModels;
using PagedList;
using System.IO;
using ahmadi.Infrastructure.Helper;
using Stimulsoft.Report.Dashboard;
using ahmadi.Infrastructure.Filter;
using CoreLib.ViewModel.Xml;
using Stimulsoft.Report;
using CoreLib.Infrastructure.DateTime;
using ahmadi.Areas.Admin.ViewModels.Report;
using Stimulsoft.Report.Mvc;
using Domain.ViewModel;
using CoreLib.Infrastructure.CustomAttribute;
using System.Data.SqlClient;

namespace ahmadi.Controllers
{
    [Authorize]
    public partial class ProfileController : BaseController
    {
        private UnitOfWork.UnitOfWorkClass uow = null;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ProfileController()
        {
            uow = new UnitOfWorkClass();
        }

        public ProfileController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        #region Index
        // GET: Profile

        public virtual async Task<ActionResult> Index(ManageMessageId? message, string PhoneNumberConfirmation, int? Error, string msg)
        {


            #region Check License


            #endregion
            ViewBag.Error = Error;
            ViewBag.Message = msg;
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "رمز عبور شما تغییر یافت."
                : message == ManageMessageId.SetPasswordSuccess ? "رمز عبور شما تنظیم شد."
                : message == ManageMessageId.SetTwoFactorSuccess ? "احراز هویت دو مرحله ای شما تنظیم شد."
                : message == ManageMessageId.Error ? "خطایی اتفاق افتاد."
                : message == ManageMessageId.AddPhoneSuccess ? "شماره تلفن شما اضافه شد."
                : message == ManageMessageId.RemovePhoneSuccess ? "شماره تماس شما حذف شد."
                : message == ManageMessageId.SendConfirmEmail ? "ایمیلِ تاییدِ ایمیل برای شما ارسال شد."
                : message == ManageMessageId.SendConfirmPhone ? "کد فعال سازی به تلفن همراه شما ارسال شد."
                : "";

            if (!String.IsNullOrEmpty(PhoneNumberConfirmation))
            {
                ViewBag.EducationMessage = "برای استفاده از امکانات وب سایت باید تلفن همراه خود را تایید نمایید. لطفا با کلیک روی تایید تلفن همراه، شماره موبایل خود را تایید نمایید.";
            }
            DateTime dt = DateTime.Now;
            var userId = User.Identity.GetUserId();
            var user = uow.UserRepository.Get(x => x, x => x.Id == userId, null, "Tickets,UserMessage2s,Avatarattachment,ProductLetmeknows,ProductFavorates,Wallets").SingleOrDefault();
            var model = new IndexViewModel
            {
                IsInNewsLetter = uow.NewsLetterEmailRepository.Any(x => x, x => x.Email == user.UserName && x.IsVerified),
                HasPassword = HasPassword(),
                PhoneNumber = user.PhoneNumber,
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                EmailConfirmed = await UserManager.IsEmailConfirmedAsync(userId),
                PhoneNumberConfirmed = await UserManager.IsPhoneNumberConfirmedAsync(userId),
                FirstName = user.FirstName,
                LastName = user.LastName,
                About = user.About,
                Gender = user.Gender,
                Avatar = user.Avatar,
                BirthDate = user.BirthDate,
                CardNumber = user.CardNumber,
                City = user.City,
                State = user.State,
                LandlinePhone = user.LandlinePhone,
                NationalCode = user.NationalCode,
                Address = user.Address,
                Email = user.Email,
                Avatarattachment = user.Avatarattachment,
                AppUser = user,
                MessageCout = user.UserMessage2s.Where(x => x.state == false).Count(),
                QuestionCount = user.Tickets.Where(x => x.Answer == false).Count(),
                AnswerCount = user.Tickets.Where(x => x.Answer).Count(),
                AnswerNotVisitedCount = user.Tickets.Where(x => x.Answer && x.UserIsVisit== false).Count(),
                //BonCount = uow.UserBonRepository.Count(x => x.state == false && x.ExpireDate > dt),
                //CodeCount = uow.UserCodeGiftRepository.Count(x => ((x.ExpireDate != null && x.ExpireDate > dt) || x.ExpireDate == null) && ((x.Offer.ExpireDate != null && x.Offer.ExpireDate > dt) || x.Offer.ExpireDate == null) && ((x.Offer.StartDate != null && x.Offer.StartDate <= dt) || x.Offer.StartDate == null) && x.IsActive && x.Offer.IsActive && x.Offer.IsDeleted == false && x.Offer.state == true),
                //NoticeCount = user.ProductLetmeknows.Where(x => x.Notofied == false || x.NotofiedEmail == false || x.NotofiedSms == false).Count(),
                //FavCount = user.ProductFavorates.Count(),
                //wallet = user.Wallets.Where(x =>x.State && x.DepositOrWithdrawal == true).Sum(x => x.Price) - user.Wallets.Where(x =>x.State && x.DepositOrWithdrawal == false).Sum(x => x.Price)
            };

            ViewBag.ProuctLisence = true;
            ViewBag.OrderLisence = true;

            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " پروفایل کاربری";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "پروفایل کاربری";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("Profile", "Index");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ConfirmEmail()
        {



            ManageMessageId? message;
            var user = UserManager.FindByName(User.Identity.Name);
            if (!String.IsNullOrEmpty(user.Email))
            {
                try
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("VerifyEmail", "Profile", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    #region Create Html Body
                    string EmailBodyHtml = "";


                    var setting = GetSetting();
                    CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(setting.attachmentFileName, "تایید ایمیلِ حساب کاربری", "لطفا با کلیک روی لینک مقابل ایمیل خود را تایید نمایید : <a href='" + callbackUrl + "'> لینک تایید ایمیل</a>", setting.WebSiteName, HttpContext.Request.Url.Host, setting.WebSiteTitle);
                    EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);

                    #endregion

                    EmailService es = new EmailService();
                    IdentityMessage imessage = new IdentityMessage();
                    imessage.Body = EmailBodyHtml;
                    imessage.Destination = user.Email;
                    imessage.Subject = " تایید ایمیل حساب کاربری شما در " + setting.WebSiteName;
                    await es.SendAsync(imessage);

                    message = ManageMessageId.SendConfirmEmail;
                    return RedirectToAction("Index", "Profile", new { msg = message });

                }
                catch (Exception)
                {

                    return RedirectToAction("Index", "Profile", new { msg = "خطایی رخ داد." });
                }
            }
            else
            {
                return RedirectToAction("Index", "Profile", new { msg = "ایمیل خود را وارد نکرده اید." });
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> PhoneConfirm()
        {


            // ManageMessageId? message;
            var user = UserManager.FindByName(User.Identity.Name);

            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            Session["Code"] = r;
            try
            {

                SmsService sms = new SmsService();
                IdentityMessage iPhonemessage = new IdentityMessage();
                iPhonemessage.Body = "کد فعال سازی حساب کاربری شما:" + r + ".";
                iPhonemessage.Destination = user.UserName;

                await sms.SendSMSAsync(iPhonemessage);


                //message = ManageMessageId.SendConfirmPhone;
                return RedirectToAction("VerifyPhone", "Profile");
            }
            catch (Exception x)
            {

                return RedirectToAction("Index", "Profile", new { Error = 1 });
            }
        }

        [AllowAnonymous]
        public virtual async Task<ActionResult> VerifyEmail(string userId, string code)
        {



            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {

                NewsLetterEmail oNewsLetterEmail = new NewsLetterEmail();
                IdentityManager im = new IdentityManager();
                oNewsLetterEmail.Email = im.GetUser(userId).Email;
                oNewsLetterEmail.InsertDate = DateTime.Now;
                oNewsLetterEmail.LanguageId = 1;
                oNewsLetterEmail.IsVerified = true;
                uow.NewsLetterEmailRepository.Insert(oNewsLetterEmail);
                uow.Save();
            }

            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "تایید ایمیل ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = " تایید ایمیل ";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/VerifyEmail");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion
            return View(result.Succeeded ? "VerifyEmail" : "Error");


        }

        [HttpGet]
        public virtual ActionResult VerifyPhone()
        {


            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "تایید موبایل ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = " تایید موبایل ";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Action("VerifyEmail", "Profile");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult VerifyPhone(string userName, string code)
        {


            try
            {
                if (Session["Code"] == null)
                {
                    ViewBag.message = " مدت زمانِ کدِ فعال سازی گذشته است. دوباره اقدام به دریافت کد فعال سازی نمایید.";
                    return View();
                }
                if (code == null)
                {
                    ViewBag.message = "کد فعال سازی را وارد نمایید.";
                    return View();
                }
                if (Session["Code"].ToString() != code)
                {
                    ViewBag.message = "کد وارد شده صحیح نیست.";
                    return View();
                }

                var user = uow.UserRepository.Get(x => x, x => x.UserName == userName).SingleOrDefault();
                user.PhoneNumberConfirmed = true;
                user.Disable = false;
                uow.Save();
                Session["Code"] = null;

                return RedirectToAction("Index");
            }
            catch (Exception x)
            {
                ViewBag.message = x.Message;
                return View();
            }
        }

        //
        // GET: /Profile/ChangePassword
        public virtual ActionResult ChangePassword()
        {



            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " اقدام برای تغییر رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "تغییر رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/ChangePassword");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion
            return View();


        }

        //
        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {


            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        // GET: /Profile/Edit/5
        public virtual async Task<ActionResult> Edit(string r, int? Update, string returnurl)
        {


            #region Check License


            #endregion

            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " ویرایش اطلاعات کاربری ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "ویرایش اطلاعات کاربری";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Edit");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            if (Update.HasValue)
            {
                if (Update.Value == 0)
                {
                    ViewBag.Error = " لطفا پروفایل کاربری خود را تکمیل نمایید ( وارد کردن نام و نام خانوادگی اجباری است). ";
                }
            }

            var userId = User.Identity.GetUserId();
            var user = uow.UserRepository.Get(x => x, x => x.Id == userId, null, "CityEntity,Avatarattachment").First();
            var model = new EditProfileViewModel
            {
                Id = userId,
                CityEntity = user.CityEntity,
                CityId = user.CityId,
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                FirstName = user.FirstName,
                LastName = user.LastName,
                About = user.About,
                Gender = user.Gender,
                Email = user.Email,
                Avatar = user.Avatar,
                Avatarattachment = user.Avatarattachment,
                LandlinePhone = user.LandlinePhone,
                PostalCode = user.PostalCode,
                State = user.State,
                City = user.City,
                Address = user.Address,
                NationalCode = user.NationalCode,
                BirthDate = user.BirthDate,
                CardNumber = user.CardNumber,
                AddressNumber = user.AddressNumber,
                AddressUnit = user.AddressUnit
            };
            ViewBag.returnurl = returnurl;
            int ProvinceId = 8;
            if (model.CityId.HasValue)
                ProvinceId = model.CityEntity.ProvinceId;
            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId);
            ViewBag.cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304);

            ViewBag.ProuctLisence = true;
            CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
            ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name", model.State);

            Session["CurrentEmail"] = model.Email;



            ViewBag.r = r;
            return View(model);


        }

        /// <summary>
        /// نمایش ساید بار برای گروفاییل کاربر
        /// </summary>
        /// <param name="avatar">avatar</param>
        /// <returns></returns>
        public ActionResult ProfileSideBar()
        {

            string userId = User.Identity.GetUserId();
            var user = uow.UserRepository.GetQueryList().Include(x => x.UserMessage2s).Include(x => x.Avatarattachment).AsNoTracking().FirstOrDefault(u => u.Id == userId);
            var showSideBarProfileViewModel = new ShowSideBarProfileViewModel()
            {
                Avatar = user.Avatar,
                Avatarattachment = user.Avatarattachment,
                AppUser = user,
                MessageCout = user.UserMessage2s.Where(x => x.state == false).Count()
            };
            return PartialView("_ProfileSideBar", showSideBarProfileViewModel);
        }

        //
        // POST: /Profile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Edit(EditProfileViewModel model, HttpPostedFileBase file, string ShamsiBirthDate, string r, string returnurl)
        {
            ViewBag.returnurl = returnurl;
            ViewBag.r = r;
            int ProvinceId = 8;
            if (model.CityId.HasValue)
                ProvinceId = uow.CityRepository.Get(x => x.ProvinceId, x => x.Id == model.CityId).First();
            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId);
            ViewBag.cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", model.CityId.HasValue ? model.CityId : 304);

            model.CityEntity = uow.CityRepository.GetByID(model.CityId);

            #region Get Setting & meta

            var setting = GetSetting();

            ViewBag.ProuctLisence = true;
            CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
            ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name", model.State);


            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " ویرایش اطلاعات کاربری ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "ویرایش اطلاعات کاربری";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + "/Profile/Edit";
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            string msg = "";
            bool IsValid = true;
            string attachementId = "";

            if (!model.CityId.HasValue)
            {
                msg = "شهر انتخاب نشده است ! ";
                IsValid = false;
            }
            if (model.Gender == null)
            {
                msg += "جنسیت انتخاب نشده است !";
                IsValid = false;
            }

            if (file != null)
            {
                if (IsImage(file))
                {
                    if ((file.ContentLength / 1024) <= 150)
                    {

                        string extention = file.FileName.Substring(file.FileName.LastIndexOf("."));
                        var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                        if (oFiletype != null)//Check Extension is Valid?
                        {
                            try
                            {
                                attachment NewAttachement = new attachment();
                                NewAttachement.Capacity = file.ContentLength / 1024;
                                NewAttachement.DisplaySort = 0;
                                NewAttachement.FileName = User.Identity.Name + "/" + file.FileName;
                                NewAttachement.FileTypeId = oFiletype.Id;
                                NewAttachement.HasMultiSize = false;
                                NewAttachement.HasWatermark = false;
                                NewAttachement.InsertDate = DateTime.Now;
                                NewAttachement.IsActive = true;
                                NewAttachement.LanguageId = setting.LanguageId;
                                NewAttachement.Title = " آواتارِ کاربرِ " + User.Identity.Name;
                                NewAttachement.UpdateDate = DateTime.Now;
                                NewAttachement.UseCount = 1;
                                IdentityManager im = new IdentityManager();
                                NewAttachement.UserId = User.Identity.GetUserId();

                                uow.AttachmentRepository.Insert(NewAttachement);
                                uow.Save();

                                attachementId = NewAttachement.Id.ToString();
                                string fileName = "LG_" + NewAttachement.Id.ToString() + NewAttachement.FileType.FileTypeName;
                                attachment upAttachement = uow.AttachmentRepository.GetByID(NewAttachement.Id);
                                upAttachement.FileName = User.Identity.Name + "/" + fileName;
                                uow.Save();

                                if (!System.IO.Directory.Exists(Server.MapPath("~/Content/UploadFiles/" + User.Identity.Name)))
                                    System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/UploadFiles/" + User.Identity.Name));

                                string targetFolder = Server.MapPath("~/Content/UploadFiles/" + User.Identity.Name);
                                string targetPath = System.IO.Path.Combine(targetFolder, fileName);
                                file.SaveAs(targetPath);
                            }
                            catch (Exception x)
                            {
                                msg = x.Message;
                                IsValid = false;
                            }
                        }
                        else
                        {
                            msg = " پسوند فایل مجاز نیست. ";
                            IsValid = false;
                        }
                    }
                    else
                    {
                        msg = "حجم فایل باید کمتر از 150 کیلوبایت باشد.";
                        IsValid = false;
                    }
                }
                else
                {
                    msg = " فایل انتخاب شده باید .jpg،.png،.jpeg یا .gif باشد . ";
                    IsValid = false;
                }
            }
            if (IsValid)
            {

                if (uow.UserRepository.Get(x => x, x => x.PhoneNumber == model.PhoneNumber && x.Id != model.Id).Any())
                {
                    ViewBag.Error = " تلفن همراه وارد شده تکراری است.";


                    return View(model);
                }
                var userId = model.Id;
                var user = UserManager.FindById(userId);
                user.PhoneNumber = model.PhoneNumber;
                user.Email = model.Email;
                user.Gender = model.Gender;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.About = model.About;
                user.LandlinePhone = model.LandlinePhone;
                user.PostalCode = model.PostalCode;
                user.State = model.State;
                user.City = model.City;
                user.Address = model.Address;
                user.AddressNumber = model.AddressNumber;
                user.AddressUnit = model.AddressUnit;
                user.NationalCode = model.NationalCode;
                user.CardNumber = model.CardNumber;
                user.CardNumber = model.CardNumber;
                user.CityId = model.CityId;
                if (ShamsiBirthDate != "")
                {
                    user.BirthDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeShamsiToMiladi(ShamsiBirthDate);
                }
                if (file != null)
                {
                    //delete old file
                    if (user.Avatar != null)
                    {
                        attachment OldAttachement = uow.AttachmentRepository.GetByID(user.Avatar);
                        if (OldAttachement != null)
                        {
                            //Delete File
                            string OldFileName = Server.MapPath("~/Content/UploadFiles/" + OldAttachement.FileName);
                            //Delete Row
                            uow.AttachmentRepository.Delete(OldAttachement);
                            uow.Save();

                            if (System.IO.File.Exists(OldFileName))
                                System.IO.File.Delete(OldFileName);
                        }
                    }
                    //update user avatart Id
                    Guid g = new Guid(attachementId);
                    user.Avatar = g;

                }
                if (Session["CurrentEmail"] != null)
                {
                    if (model.PhoneNumber != Session["CurrentEmail"].ToString())
                        user.EmailConfirmed = false;
                }
                Session["CurrentEmail"] = null;
                IdentityResult ir = await UserManager.UpdateAsync(user);
                if (ir.Succeeded)
                {
                    if (!String.IsNullOrEmpty(returnurl))
                        return Redirect("~" + returnurl);
                    else if (r != "")
                        return Redirect(r);
                    else
                        return RedirectToAction("Index");
                }
                else
                {

                    ViewBag.ReturnUrl = r;
                    return View(model);
                }
            }
            else
            {
                ViewBag.ReturnUrl = r;
                ViewBag.Error = msg;
                return View(model);
            }
        }

        #endregion

        #region Favorates

        public ActionResult favorites(int? page)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "محصولات موردعلاقه";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "محصولات موردعلاقه";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/favorites");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {
                #region Check License


                #endregion


                string userid = User.Identity.GetUserId();
                var Products = uow.ProductRepository.ProductItemList(x => x.ProductFavorates.Any(s => s.UserId == userid)).ToList();

                int pageSize = 10;

                int pageNumber = (page ?? 1);


                return View(Products.ToPagedList(pageNumber, pageSize));


            }
            catch (Exception x)
            {


                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveFavorate(int? id)
        {
            if (id.HasValue)
            {
                string userid = User.Identity.GetUserId();
                int? pf = uow.ProductFavorateRepository.checkUserProductFavorate(id.Value, userid);
                if (pf.HasValue)
                {
                    await uow.ProductFavorateRepository.removeFavorate(pf.Value);
                    await uow.ProductRepository.removeProductFavorate(id.Value);
                    return Json(new
                    {
                        statusCode = 200,
                        Message = "حذف شد "
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "محصول شما پیدا نشد !"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new
                {
                    statusCode = 500,
                    Message = "محصولی انتخاب نشده است ! "
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Notices

        public ActionResult Notices(int? page)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "به من اطلاع بده ها";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "به من اطلاع بده ها";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Notices");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var ProductLetmeknows = uow.ProductLetmeknowRepository.GetList(userid);

                int pageSize = 10;

                int pageNumber = (page ?? 1);


                return View(ProductLetmeknows.ToPagedList(pageNumber, pageSize));


            }
            catch (Exception x)
            {

                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveNotice(int? id)
        {
            if (id.HasValue)
            {
                string userid = User.Identity.GetUserId();
                int? pf = uow.ProductLetmeknowRepository.checkUserProductletmeKnow(id.Value, userid);
                if (pf.HasValue)
                {
                    await uow.ProductLetmeknowRepository.removeLetmeKnow(pf.Value);
                    return Json(new
                    {
                        statusCode = 200,
                        Message = "حذف شد "
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "محصول شما پیدا نشد !"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new
                {
                    statusCode = 500,
                    Message = "محصولی انتخاب نشده است ! "
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region ProductComments

        public ActionResult Comments(int? page, int? tab)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "لیست نظرات";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لیست نظرات";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Comments");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var products = uow.ProductCommentRepository.GetNoUserCommenProducts(userid);

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                ViewBag.tab = tab;

                ViewBag.Productcomments = uow.ProductCommentRepository.GetUserComments(userid).ToPagedList(pageNumber, pageSize);
                return View(products.ToPagedList(pageNumber, pageSize));


            }
            catch (Exception x)
            {

                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveComment(int? id)
        {
            if (id.HasValue)
            {
                string userid = User.Identity.GetUserId();
                int? pf = uow.ProductCommentRepository.checkUserProductComment(id.Value, userid);
                if (pf.HasValue)
                {
                    await uow.ProductCommentRepository.removeProductComment(pf.Value);
                    return Json(new
                    {
                        statusCode = 200,
                        Message = "حذف شد "
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "نظر شما پیدا نشد !"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new
                {
                    statusCode = 500,
                    Message = "نظر انتخاب نشده است ! "
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Addresses
        public ActionResult addresses(int? page)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "آدرس ها ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لیست آدرسها";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/addresses");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var UserAddress = uow.UserAddressRepository.Get(x => x, x => x.UserId == userid, null, "CityEntity.Province");

                int pageSize = 10;

                int pageNumber = (page ?? 1);


                return View(UserAddress.ToPagedList(pageNumber, pageSize));


            }
            catch (Exception x)
            {

                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<JsonResult> RemoveAddress(int? id)
        {
            if (id.HasValue)
            {
                string userid = User.Identity.GetUserId();
                int? pf = uow.UserAddressRepository.checkUserUserAddress(id.Value, userid);
                if (pf.HasValue)
                {
                    await uow.UserAddressRepository.removeUserAddress(pf.Value);
                    return Json(new
                    {
                        statusCode = 200,
                        Message = "حذف شد "
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "آدرس شما پیدا نشد !"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new
                {
                    statusCode = 500,
                    Message = "آدرس انتخاب نشده است ! "
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AddAddress(string returnurl)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "افزودن آدرس جدید";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "افزودن آدرس جدید";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/AddAddress");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion
            ViewBag.returnurl = returnurl;
            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddAddress(UserAddress useraddress, string returnurl)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "افزودن آدرس جدید";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "افزودن آدرس جدید";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/AddAddress");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");

            try
            {
                if (!useraddress.CityId.HasValue)
                {

                    ViewBag.error = " شهر انتخاب نشده است ! ";
                    return View(useraddress);
                }
                useraddress.UserId = User.Identity.GetUserId();
                await uow.UserAddressRepository.addUserAddress(useraddress);
                if (!String.IsNullOrEmpty(returnurl))
                    return Redirect("~" + returnurl + "?aid=" + useraddress.Id);
                else
                    return Redirect("~/Profile/addresses");
            }
            catch (Exception x)
            {
                ViewBag.error = " خطایی رخ داد ";
                return View(useraddress);
            }

        }

        public ActionResult EditAddress(int? id, string returnurl)
        {
            if (!id.HasValue)
                return Redirect("~/Profile/addresses");

            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "ویرایش آدرس ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "ویرایش آدرس ";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/EditAddress");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            var useraddress = uow.UserAddressRepository.Get(x => x, x => x.Id == id.Value, null, "CityEntity").FirstOrDefault();
            if (useraddress == null)
                return Redirect("~/Profile/addresses");

            int ProvinceId = 8;
            if (useraddress.CityId.HasValue)
                ProvinceId = useraddress.CityEntity.ProvinceId;
            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.Get(x => x), "Id", "Name", ProvinceId);
            ViewBag.cityList = new SelectList(uow.CityRepository.Get(x => x, x => x.ProvinceId == ProvinceId), "Id", "Name", useraddress.CityId.HasValue ? useraddress.CityId : 304);


            ViewBag.returnurl = returnurl;
            return View(useraddress);
        }

        [HttpPost]
        public async Task<ActionResult> EditAddress(UserAddress useraddress, string returnurl)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "ویرایش آدرس ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "ویرایش آدرس ";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/EditAddress");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            ViewBag.ProvinceList = new SelectList(uow.ProvinceRepository.GetByReturnQueryable(x => x), "Id", "Name");

            try
            {
                if (!useraddress.CityId.HasValue)
                {

                    ViewBag.error = " شهر انتخاب نشده است ! ";
                    return View(useraddress);
                }
                useraddress.UserId = User.Identity.GetUserId();
                await uow.UserAddressRepository.EditUserAddress(useraddress);
                if (!String.IsNullOrEmpty(returnurl))
                    return Redirect("~" + returnurl);
                else
                    return Redirect("~/Profile/addresses");
            }
            catch (Exception x)
            {
                ViewBag.error = " خطایی رخ داد ";
                return View(useraddress);
            }

        }


        public virtual JsonResult GetCities(int ProvinceId)
        {

            var jsonResult = Json(new
            {
                data = uow.CityRepository.GetByReturnQueryable(x => x, x => x.ProvinceId == ProvinceId, x => x.OrderBy(s => s.Name), "", 0, 0).Select(x => new { x.Id, x.Name }).ToList(),
            }, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        #endregion

        #region Bon

        public ActionResult GiftBon(int? page, int? tab)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "بن های تخفیف";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لیست بن های تخفیف";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/GiftBon");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var UserBons = uow.UserBonRepository.GetByReturnQueryable(x => x, x => x.UserId == userid, null, "UserBonLogs");

                int pageSize = 10;
                int pageNumber = (page ?? 1);


                return View(UserBons.ToPagedList(pageNumber, pageSize));


            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error");
            }
        }
        #endregion

        #region GiftCode

        public ActionResult GiftCode(int? page, int? tab)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "کدهای تخفیف";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لیست کدهای تخفیف";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/GiftCode");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var UserCodeGifts = uow.UserCodeGiftRepository.GetByReturnQueryable(x => x, x => x.UserId == userid, null, "UserCodeGiftLogs,Offer");

                int pageSize = 10;
                int pageNumber = (page ?? 1);


                return View(UserCodeGifts.ToPagedList(pageNumber, pageSize));

            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error");
            }
        }
        #endregion

        #region Messages

        public ActionResult Messages(int? page, int? tab)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "پیام ها";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لیست پیام ها";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Messages");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var UserMessages = uow.UserMessageRepository.GetByReturnQueryable(x => x, x => x.UserIdTo == userid || x.UserIdTo == null || x.UserId == "");

                int pageSize = 10;
                int pageNumber = (page ?? 1);


                return View(UserMessages.ToPagedList(pageNumber, pageSize));

            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error");
            }
        }


        [HttpPost]
        public JsonResult ReadMessage(int? id)
        {
            if (id.HasValue)
            {
                string userid = User.Identity.GetUserId();
                var um = uow.UserMessageRepository.Get(x => x, x => x.Id == id.Value && x.UserId == userid).FirstOrDefault();
                if (um != null)
                {
                    um.state = true;
                    uow.UserMessageRepository.Update(um);
                    uow.Save();
                    return Json(new
                    {
                        statusCode = 200,
                        Message = " بروز شد "
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        statusCode = 500,
                        Message = "پیام شما پیدا نشد !"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new
                {
                    statusCode = 500,
                    Message = "پیام انتخاب نشده است ! "
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Orders

        [Infrastructure.Filter.AutoExecueFilter]
        public ActionResult Orders(int? page, int? tab)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "تاریخچه سفارشات";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لیست سفارشات";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Orders");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                ViewBag.tab = tab;

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                string userid = User.Identity.GetUserId();
                ahmadi.ViewModels.Basket.ProfileOrder profileOrder = new ahmadi.ViewModels.Basket.ProfileOrder()
                {
                    AllOrders = uow.OrderRepository.GetAllOrder(userid).ToPagedList(pageNumber, pageSize),
                    CurrentOrders = uow.OrderRepository.GetCurrentOrder(userid).ToPagedList(pageNumber, pageSize),
                    ProccessOrders = uow.OrderRepository.GetProccessOrder(userid).ToPagedList(pageNumber, pageSize),
                    SentOrders = uow.OrderRepository.GetSentOrder(userid).ToPagedList(pageNumber, pageSize),
                    CancelOrders = uow.OrderRepository.GetCancelOrders(userid).ToPagedList(pageNumber, pageSize),
                    CancelWaitOrders = uow.OrderRepository.GetCancelWaitOrders(userid).ToPagedList(pageNumber, pageSize),
                    DeliveredOrders = uow.OrderRepository.GetDeliveredOrders(userid).ToPagedList(pageNumber, pageSize),
                    ReturnedOrders = uow.OrderRepository.GetReturnedOrders(userid).ToPagedList(pageNumber, pageSize),
                    EstelamOrders = uow.OrderRepository.GetEstelamOrders(userid).ToPagedList(pageNumber, pageSize)
                };



                return View(profileOrder);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Infrastructure.Filter.AutoExecueFilter]
        public ActionResult Detail(int id)
        {

            #region Get Setting & meta

            #region Get Setting 
            var setting = GetSetting();

            #endregion

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "جزئیات سفارش";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "جزئیات سفارش";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Orders");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {
                string userid = User.Identity.GetUserId();
                var order = uow.OrderRepository.Get(x => x, x => x.CustomerOrderId == id.ToString() && x.UserId == userid, null, "OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,OrderAttributeSelects.OrderAttribute,OrderWallets.wallet.BankAccount,OrderWallets.Wallet.ForWhat,OrderDeliveries.ProductSendWayWorkTime,OrderDeliveries.OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,User.CityEntity.Province,OrderStates,OrderDeliveries.OrderStates,OrderDeliveries.OrderAttributeOrders.OrderAttribute,OrderDeliveries.ProductSendWay,OrderDeliveries.UserAddress.CityEntity.Province").SingleOrDefault();
                if (order == null)
                    return Redirect("~/Profile/Orders");

                List<SelectListItem> OrderStateSelectListItem;
                if (order.OrderWallets.FirstOrDefault().Wallet.PaymentType < 5)//پرداخت به پیک ها نیست

                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };
                else
                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };

                ViewBag.OrderStateSelectListItem = OrderStateSelectListItem;

                bool IsEstelam = false;
                if (order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.Value.ToLower() == "false" && a.WalletAttribute.DataType == 23)) && (order.OrderStates.Max(s => s.state) == OrderStatus.در_انتظار_تایید || order.OrderStates.Max(s => s.state) == OrderStatus.تایید_سفارش))
                {
                    IsEstelam = true;
                }
                ViewBag.IsEstelam = IsEstelam;
                ViewBag.setting = GetSetting();
                return View(order);

            }
            catch (Exception ex)
            {

                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult invoice(string id)
        {
            if (String.IsNullOrEmpty(id))
                return Redirect("~/Profile/Orders");
            Guid gid = new Guid(id);
            if (!uow.OrderStateRepository.Any(s => s.OrderId == gid && s.state >= OrderStatus.تایید_پرداخت))
                return Redirect("~/Profile/Orders");

            TempData["orderId"] = id;
            return View("Factor");
        }

        public ActionResult GetFactorReport()
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment,Province,City,FactorAttachment").SingleOrDefault();
            XMLReader readXml = new XMLReader(setting.StaticContentDomain);

            StiReport report = new StiReport();

            report.Load(Server.MapPath("~/Content/Reports/FactorReport.mrt"));

            Guid orderId = new Guid(TempData["orderId"].ToString());
            var order = uow.OrderRepository.Get(x => new { x.CustomerOrderId, x.BankOrderId, x.InsertDate, x.OrderDeliveries.First().UserAddress, x.OrderDeliveries.First().UserAddressId, x.User, x.OrderRows, x.OrderWallets, ProductSendWay = x.OrderDeliveries.First().ProductSendWay.Title, x.OrderAttributeSelects }, x => x.Id == orderId,
                    null, "OrderDeliveries.UserAddress.CityEntity.Province,OrderDeliveries.ProductSendWay,User.CityEntity.Province,OrderWallets.Wallet,OrderAttributeSelects.OrderAttribute,OrderRows,OrderRows.ProductPrice.Product,OrderRows.ProductPrice.ProductAttributeSelectColor,OrderRows.ProductPrice.ProductAttributeSelectSize,OrderRows.ProductPrice.ProductAttributeSelectModel,OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").First();
            Areas.Admin.ViewModels.Report.CustomerOrderInfo CustomerOrderInfo = new Areas.Admin.ViewModels.Report.CustomerOrderInfo()
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
            var CustomerOrderRows = new List<Areas.Admin.ViewModels.Report.CustomerOrderRow>();
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

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, imageIn.RawFormat);
            return ms.ToArray();
        }
        public ActionResult ViewerEvent()
        {
            return StiMvcViewer.ViewerEventResult();
        }

        [Infrastructure.Filter.AutoExecueFilter]
        public ActionResult CancelOrder(int id)
        {

            #region Get Setting & meta

            #region Get Setting 
            var setting = GetSetting();

            #endregion

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "لغو سفارش";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لغو سفارش";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/CancelOrder");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {

                string userid = User.Identity.GetUserId();
                var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == id && x.Order.UserId == userid, null, "Order,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.ForWhat,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
                if (orderDelivery == null)
                    return Redirect("~/Profile/Orders");
                if (orderDelivery.OrderStates.Last().state >= OrderStatus.پردازش_انبار)
                    return Redirect("~/Profile/Orders");

                List<SelectListItem> OrderStateSelectListItem;
                if (orderDelivery.Order.OrderWallets.FirstOrDefault().Wallet.PaymentType < 5)//پرداخت به پیک ها نیست

                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };
                else
                    OrderStateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در انتظار تایید سفارش", Value = "0" }, new SelectListItem() { Text = "تایید سفارش", Value = "1" }, new SelectListItem() { Text = "پردازش انبار", Value = "3" }, new SelectListItem() { Text = "آماده ارسال", Value = "4" }, new SelectListItem() { Text = "ارسال شده", Value = "5" }, new SelectListItem() { Text = "تحویل داده شده", Value = "6" }, new SelectListItem() { Text = "تایید پرداخت", Value = "2" }, new SelectListItem() { Text = "لغو شده", Value = "7" }, new SelectListItem() { Text = "مرجوعی", Value = "8" } };

                ViewBag.OrderStateSelectListItem = OrderStateSelectListItem;
                ViewBag.userCard = orderDelivery.Order.User.CardNumber;


                return View(orderDelivery);

            }
            catch (Exception ex)
            {

                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult> CancelOrder(CancelOrderReson cancelOrderReson, int DeliveryId, Guid OrderId)
        {
            #region Get Setting & meta

            #region Get Setting 
            var setting = GetSetting();

            #endregion

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "لغو سفارش";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لغو سفارش";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/CancelOrder");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            string userid = User.Identity.GetUserId();
            var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == DeliveryId && x.Order.UserId == userid, null, "Order,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.ForWhat,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
            try
            {
                if (orderDelivery == null)
                    return Redirect("~/Profile/Orders");
                if (orderDelivery.OrderStates.Last().state >= OrderStatus.پردازش_انبار)
                    return Redirect("~/Profile/Orders");

                ViewBag.userCard = orderDelivery.Order.User.CardNumber;
                orderDelivery.OrderStates.Add(new OrderState()
                {
                    cancelOrderReson = cancelOrderReson,
                    LogDate = DateTime.Now,
                    state = OrderStatus.درخواست_لغو,
                    OrderDeliveryId = DeliveryId,
                    OrderId = OrderId
                });
                uow.OrderDeliveryRepository.Update(orderDelivery);
                uow.Save();


                SmsService ss = new SmsService();
                IdentityMessage iMessage = new IdentityMessage();
                iMessage.Destination = setting.Mobile;
                iMessage.Body = setting.WebSiteName + "\n" + "مدیر گرامی\n" + "یک درخواست لغو سفارش ثبت شد.\n" + "شماره سفارش : " + orderDelivery.Order.CustomerOrderId;
                await ss.SendSMSAsync(iMessage, "NewCancelOrder", orderDelivery.Order.CustomerOrderId, null, null, null);

                ViewBag.status = "1";
                ViewBag.msg = "درخواست اولیه شما ثبت شد. پس از بررسی درخواست شما، اطلاع رسانی خواهد شد.";
            }
            catch (Exception ex)
            {
                ViewBag.status = "0";
                ViewBag.msg = " خطایی رخ داد. " + ex.Message;
            }

            return View(orderDelivery);
        }

        public ActionResult CancelUserOrder(int id)
        {
            string userid = User.Identity.GetUserId();
            var orderDelivery = uow.OrderDeliveryRepository.Get(x => x, x => x.Id == id && x.Order.UserId == userid, null, "Order,Order.OrderAttributeSelects.OrderAttribute,Order.OrderWallets.Wallet.ForWhat,OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,Order.User.CityEntity.Province,Order.OrderStates,OrderStates").SingleOrDefault();
            if (orderDelivery == null)
                return Redirect("~/Profile/Orders");
            if (orderDelivery.OrderStates.Last().state >= OrderStatus.پردازش_انبار || orderDelivery.OrderStates.Any(x => x.state == OrderStatus.لغو_شده))
                return Redirect("~/Profile/Orders");


            #region Get Setting & meta

            #region Get Setting 
            var setting = GetSetting();

            #endregion

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "لغو سفارش";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "لغو سفارش";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/CancelOrder");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            try
            {
                ViewBag.userCard = orderDelivery.Order.User.CardNumber;
                orderDelivery.OrderStates.Add(new OrderState()
                {
                    cancelOrderReson = CancelOrderReson.InvalidCancel,
                    LogDate = DateTime.Now,
                    state = OrderStatus.درخواست_لغو,
                    OrderDeliveryId = orderDelivery.Id,
                    OrderId = orderDelivery.OrderId.Value
                });
                orderDelivery.OrderStates.Add(new OrderState()
                {
                    cancelOrderReson = CancelOrderReson.InvalidCancel,
                    LogDate = DateTime.Now,
                    state = OrderStatus.لغو_شده,
                    OrderDeliveryId = orderDelivery.Id,
                    OrderId = orderDelivery.OrderId.Value
                });
                uow.OrderDeliveryRepository.Update(orderDelivery);
                uow.Save();


                orderDelivery.Order.IsActive = false;
                orderDelivery.Order.New = false;
                orderDelivery.Order.IsExpire = true;
                uow.Save();
                uow.OrderRepository.CheckQuantity(orderDelivery.Order);


                ViewBag.status = "1";
                ViewBag.msg = "سفارش شما با شماره " + orderDelivery.Order.CustomerOrderId + " لغو شد.";
            }
            catch (Exception ex)
            {
                ViewBag.status = "0";
                ViewBag.msg = " خطایی رخ داد. " + ex.Message;
            }

            return View(orderDelivery);
        }

        public ActionResult Rate(int id, int? step, int? status)
        {
            #region Get Setting & meta

            #region Get Setting 
            var setting = GetSetting();

            #endregion

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "امتیازدهی سفارش";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "امتیاز دهی سفارش";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Orders");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            #region Message
            if (status.HasValue)
            {
                switch (status.Value)
                {
                    case -1: ViewBag.message = "خطایی در ثبت نظر شما اتفاق افتاد."; break;
                    default: break;
                }
            }
            #endregion

            try
            {
                if (!step.HasValue)
                    step = 0;
                string userid = User.Identity.GetUserId();
                var order = uow.OrderRepository.Get(x => x, x => x.OrderStates.Any(s => s.state == OrderStatus.تایید_پرداخت) && x.OrderStates.Any(s => s.state == OrderStatus.تحویل_داده_شده) && !x.OrderStates.Any(s => s.state == OrderStatus.لغو_شده) && !x.OrderStates.Any(s => s.state == OrderStatus.مرجوعی) && x.CustomerOrderId == id.ToString() && x.UserId == userid, null, "OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,OrderAttributeSelects.OrderAttribute,OrderWallets.wallet.BankAccount,OrderWallets.Wallet.ForWhat,OrderDeliveries.ProductSendWayWorkTime,OrderDeliveries.OrderRows.Product,OrderRows.ProductPrice.ProductImages,OrderRows.ProductPrice.Product.ProductImages.Image,User.CityEntity.Province,OrderStates,OrderDeliveries.OrderStates,OrderDeliveries.OrderAttributeOrders.OrderAttribute,OrderDeliveries.ProductSendWay,OrderDeliveries.UserAddress.CityEntity.Province").SingleOrDefault();
                if (order == null)
                    return Redirect("~/Profile/Orders");



                var savedOrderRates = uow.OrderRateRepository.Get(x => x.orderRateId, x => x.Order.CustomerOrderId == id.ToString());
                int count = 0;
                OrderRateVM orderRateVM = new OrderRateVM()
                {
                    order = order,
                    orderRateItems = uow.OrderRateItemRepository.Get(x => x, x => !savedOrderRates.Contains(x.Id), x => x.OrderBy(s => s.Title), "", 0, 1),
                    products = uow.ProductCommentRepository.GetNoUserCommenProductsRanks(order.Id, userid, out count),
                    productCount = order.OrderRows.Count,
                    productRemainCount = count
                };

                if (step != 0)
                {
                    if (orderRateVM.orderRateItems.Any() && (step != null && step != 1))
                        return RedirectToAction("Rate", new { id = id, step = 1 });
                    else if (!orderRateVM.orderRateItems.Any() && orderRateVM.products.Any() && (step != null && step != 2))
                        return RedirectToAction("Rate", new { id = id, step = 2 });
                    else if (!orderRateVM.orderRateItems.Any() && !orderRateVM.products.Any() && (step != null && step != 3))
                        return RedirectToAction("Rate", new { id = id, step = 3 });
                }
                //step 3
                if (!orderRateVM.orderRateItems.Any() && !orderRateVM.products.Any())
                {
                    ViewBag.finished = true;

                    order.compeleteRate = true;
                    uow.OrderRepository.Update(order);
                    uow.Save();
                }

                ViewBag.step = step;
                return View(orderRateVM);
            }
            catch (Exception ex)
            {

                throw;
                // return RedirectToAction("Index", "Error");
            }
        }


        [HttpParamAction]
        [HttpPost]
        public ActionResult RateOrder(string id, Int16 RateOrder, int RateItem)
        {
            try
            {
                OrderRate orderRate = new OrderRate()
                {
                    LogDate = DateTime.Now,
                    OrderId = uow.OrderRepository.Get(x => x.Id, x => x.CustomerOrderId == id).First(),
                    orderRateId = RateItem,
                    state = (OrderRateStatus)RateOrder
                };
                uow.OrderRateRepository.Insert(orderRate);
                uow.Save();

                var savedOrderRates = uow.OrderRateRepository.Get(x => x.orderRateId, x => x.Order.CustomerOrderId == id.ToString());

                return RedirectToAction("Rate", new { id = id, step = uow.OrderRateItemRepository.Any(x => x, x => !savedOrderRates.Contains(x.Id)) ? 1 : 2, status = 1 });

            }
            catch (Exception x)
            {

                return RedirectToAction("Rate", new { id = id, status = -1 });
            }
        }

        #endregion

        #region Tickets
        public virtual ActionResult Tickets(int? page)
        {
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            var userid = User.Identity.GetUserId();

            var contactUs = uow.TicketRepository.GetQueryList().Include("User").Include("TicketCategory").Include("ChildTickets").Where(x => x.UserId == userid && x.ParrentId == null).AsNoTracking();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(contactUs.OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Ticket(string id)
        {

            #region Get Setting & meta

            #region Get Setting 
            var setting = GetSetting();

            #endregion

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "جزئیات سفارش";
            oMeta.WebSiteMetakeyword = "";
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteTitle = "جزئیات سفارش";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + Url.Content("/Profile/Orders");
            oMeta.PageCover = Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            string userid = User.Identity.GetUserId();
            var contactUs = uow.TicketRepository.GetQueryList().Include("User").Include("TicketCategory").Include("ChildTickets").Where(x => x.UserId == userid && x.Code==id).AsNoTracking().FirstOrDefault();
            if (contactUs == null)
                return Redirect("~/Profile/Tickets");

            contactUs.UserIsVisit = true;
            uow.Save();
            
            List<int> CatIds = uow.ContentRepository.SqlQuery("exec [GetTicketSubCats] @CatId", new SqlParameter("@CatId", contactUs.Id)).ToList();
            ViewBag.lastId = CatIds.FirstOrDefault();

            return View(contactUs);

        }
            
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {

                _userManager.Dispose();
                _userManager = null;
            }


            base.Dispose(disposing);
        }

        #region Helpers


        public virtual JsonResult DeleteProductCommentAttachement(string AttachementId, int ProductId)
        {
            try
            {
                string UserId = User.Identity.GetUserId();
                Guid attid = new Guid(AttachementId);
                var productcomment = uow.ProductCommentRepository.Get(x => x, x => x.ProductId == ProductId && x.UserId == UserId, null, "attachments").FirstOrDefault();
                productcomment.attachments.Remove(productcomment.attachments.Where(x => x.Id == attid).First());
                uow.ProductCommentRepository.Update(productcomment);
                uow.Save();

                var attachement = uow.AttachmentRepository.Get(x => x, x => x.Id == attid && x.UserId == UserId).FirstOrDefault();
                if (attachement != null)
                {
                    if (System.IO.File.Exists(Server.MapPath("~/Content/UploadFiles/" + attachement.FileName)))
                        System.IO.File.Delete(Server.MapPath("~/Content/UploadFiles/" + attachement.FileName));
                    uow.AttachmentRepository.Delete(attachement);
                    uow.Save();
                }
                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {

                return Json(new
                {
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }
        protected string UploadMultipleFile(HttpPostedFileBase[] uploadedFiles, string[] Title, string UseWaterMark, string WaterMarkType, string HasMultiSize, string UseCompression, Int16 compressionLevel, int? FolderId, string LanguageId, bool PopUpAttachements, string controllerName, out string messages, out List<attachment> newAttachmentList, int? ProductId)
        {
            string userid = User.Identity.GetUserId();
            List<Guid> attachementId = new List<Guid>();
            messages = "";
            string HTMLString = "";
            newAttachmentList = new List<attachment>();
            try
            {
                ViewBag.HelpModuleSection = uow.HelpModuleSectionRepository.Get(x => x, x => x.ModuleId == 3 && x.Name == "افزودن فایل", null, "HelpModuleSectionFields").FirstOrDefault();

                int success = uploadedFiles.Count();
                int i = 1;
                foreach (var File in uploadedFiles)
                {
                    if (File != null && File.ContentLength > 0)//Check File Is Selected ?
                    {
                        if ((File.ContentLength / 1024) < 1024)
                        {
                            #region Get Extention
                            string extention = File.FileName.Substring(File.FileName.LastIndexOf("."));
                            var oFiletype = uow.FiletypeRepository.Get(x => x, x => x.FileTypeName.ToLower().Equals(extention)).SingleOrDefault();
                            #endregion
                            if (oFiletype != null)//Check Extension is Valid?
                            {
                                #region Get File From Uploader
                                byte[] FileByteArray = new byte[File.ContentLength];
                                File.InputStream.Read(FileByteArray, 0, File.ContentLength);
                                #endregion

                                #region Save To Database And Load Retated Object
                                attachment newAttchment = new attachment();
                                newAttchment.UseCount = 0;
                                newAttchment.InsertDate = DateTime.Now;
                                newAttchment.IsActive = true;
                                newAttchment.DisplaySort = 1;
                                newAttchment.LanguageId = Convert.ToInt16(LanguageId);
                                newAttchment.FileName = File.FileName;
                                newAttchment.FileTypeId = oFiletype.Id;
                                newAttchment.Capacity = File.ContentLength / 1024;
                                newAttchment.UserId = userid;
                                if (!string.IsNullOrEmpty(Title[(i - 1)]))
                                    newAttchment.Title = Title[(i - 1)];
                                else
                                    newAttchment.Title = " بی نام";
                                if (FolderId.HasValue)
                                    newAttchment.FolderId = FolderId.Value;
                                if (UseWaterMark == "on")
                                    newAttchment.HasWatermark = true;
                                else
                                    newAttchment.HasWatermark = false;
                                if (HasMultiSize == "on")
                                    newAttchment.HasMultiSize = true;
                                else
                                    newAttchment.HasMultiSize = false;
                                uow.AttachmentRepository.Insert(newAttchment);
                                uow.Save();

                                attachment upAttachement = uow.AttachmentRepository.GetByID(newAttchment.Id);
                                string fileName = "LG_" + newAttchment.Id.ToString() + newAttchment.FileType.FileTypeName;

                                if (controllerName != "filemanager")
                                    upAttachement.FileName = controllerName + "/" + fileName;
                                else
                                    upAttachement.FileName = fileName;
                                uow.Save();
                                attachementId.Add(upAttachement.Id);
                                #endregion

                                #region Upload File To Folder
                                string targetFolder = "";
                                if (controllerName != "filemanager")
                                    targetFolder = Server.MapPath("~/Content/UploadFiles/" + controllerName);
                                else
                                    targetFolder = Server.MapPath("~/Content/UploadFiles");
                                string targetPath = Path.Combine(targetFolder, fileName);
                                File.SaveAs(targetPath);
                                #endregion

                                #region Watermark Compression MultiSize
                                string UploadPath = "~/Content/UploadFiles/" + (controllerName != "filemanager" ? controllerName + "/" : "");
                                if (CoreLib.Infrastructure.Image.ImageClass.IsFileAnImage(UploadPath + fileName))
                                {
                                    int langId = int.Parse(LanguageId);
                                    Setting oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == langId, null, "Waterattachment,attachment").SingleOrDefault();
                                    if (newAttchment.HasWatermark)
                                    {
                                        string result = "";
                                        if (newAttchment.HasMultiSize)
                                        {
                                            if (UseCompression == "on")
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                            else
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, true, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        }
                                        else
                                        {
                                            if (UseCompression == "on")
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, true, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                            else
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, true, false, false, compressionLevel, oSetting.LargeImageWaremark, uow.AttachmentRepository.GetByID(oSetting.WaterMark).FileName, WaterMarkType);
                                        }
                                    }
                                    else
                                    {
                                        string result = "";
                                        if (newAttchment.HasMultiSize)
                                        {
                                            if (UseCompression == "on")
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, true, compressionLevel, oSetting.LargeImageWaremark);
                                            else
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, true, false, compressionLevel, oSetting.LargeImageWaremark);
                                        }
                                        else
                                        {
                                            if (UseCompression == "on")
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, true, compressionLevel, oSetting.LargeImageWaremark);
                                            else
                                                result = CoreLib.Infrastructure.Image.ImageClass.WaterMarkAndChangeSize(UploadPath, fileName, false, false, false, compressionLevel, oSetting.LargeImageWaremark);
                                        }

                                    }
                                }
                                #endregion


                                FileInfo fi = new FileInfo(Server.MapPath("~/Content/UploadFiles/" + newAttchment.FileName));
                                newAttchment.Capacity = Convert.ToInt32(fi.Length / 1024);
                                uow.AttachmentRepository.Update(newAttchment);
                                uow.Save();


                                newAttachmentList.Add(newAttchment);
                                messages += i.ToString() + "- " + "فایلِ " + File.FileName + " با موفقیت ذخیره شد. ";
                            }
                            else
                            {
                                success--;
                                messages += i.ToString() + "- " + " پسوندِ فایلِ  " + File.FileName + " مجاز نیست. ";
                            }
                        }
                        else
                        {
                            success--;
                            messages += i.ToString() + "- " + " حجم فایل باید کمتر از 1 مگابایت باشد ";
                        }
                    }
                    else
                    {
                        success--;
                        messages += i.ToString() + "- " + "  فایل خراب است،  ";
                    }
                    i++;



                }//for each

                if (success > 0)
                {

                    // ثبت فایل ها برای نظر محصول کاربر
                    if (ProductId.HasValue)
                    {
                        if (!uow.ProductCommentRepository.Any(x => x.Id, x => x.UserId == userid && x.ProductId == ProductId && x.IsActive == false && x.IsTemp == true))
                        {
                            ProductComment pc = new ProductComment()
                            {
                                IsTemp = true,
                                Visited = false,
                                IsActive = false,
                                ProductId = ProductId.Value,
                                UserId = User.Identity.GetUserId(),
                                Title = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter("---"),
                                Text = "---",
                                InsertDate = DateTime.Now,
                                IsBuy = uow.OrderRowRepository.Any(x => x.Id, x => x.Order.UserId == userid && x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.ProductId == ProductId)

                            };
                            pc.attachments = new List<attachment>();
                            foreach (var item in attachementId)
                            {
                                pc.attachments.Add(uow.AttachmentRepository.GetByID(item));
                            }
                            uow.ProductCommentRepository.Insert(pc);
                            uow.Save();
                        }
                        else
                        {
                            var pc = uow.ProductCommentRepository.Get(x => x, x => x.UserId == userid && x.ProductId == ProductId && x.IsActive == false && x.IsTemp == true, null, "attachments").First();
                            if (pc.attachments == null)
                                pc.attachments = new List<attachment>();
                            foreach (var item in attachementId)
                            {
                                pc.attachments.Add(uow.AttachmentRepository.GetByID(item));
                            }
                            uow.ProductCommentRepository.Update(pc);
                            uow.Save();
                        }
                    }

                    var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    if (PopUpAttachements)
                        HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulkSimple", newAttachmentList, this.ControllerContext);
                    else
                        HTMLString = CaptureHelper.RenderViewToString("_AttachmentBulk", newAttachmentList, this.ControllerContext);


                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "FileManager", "UplodMultiple", false, 200, "ایجاد گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion

                }

                return HTMLString;
            }
            catch (Exception x)
            {
                return "";
            }
        }

        public virtual JsonResult UplodMultipleFileFromComputer(HttpPostedFileBase[] uploadedFiles, int ProductId)
        {
            try
            {
                int? folderId = null, ufolderId = null;

                var user = uow.UserRepository.GetByID(User.Identity.GetUserId());
                string ufolder = "فایل های کاربران";
                var uufolder = uow.FolderRepository.Get(x => x, x => x.FolderName == ufolder).FirstOrDefault();
                if (uufolder != null)
                    ufolderId = uufolder.Id;
                else
                {

                    Folder newfolder = new Folder();
                    newfolder.FolderName = ufolder;
                    newfolder.LanguageId = uow.SettingRepository.Get(x => x).First().LanguageId;
                    uow.FolderRepository.Insert(newfolder);
                    uow.Save();
                    ufolderId = newfolder.Id;
                }

                string cfoldername = user.FirstName + " " + user.LastName;
                var folder = uow.FolderRepository.Get(x => x, x => x.FolderName == cfoldername).FirstOrDefault();
                if (folder != null)
                    folderId = folder.Id;
                else
                {
                    Folder newfolder = new Folder();
                    newfolder.FolderName = cfoldername;
                    newfolder.FolderID = ufolderId;
                    newfolder.LanguageId = uow.SettingRepository.Get(x => x).First().LanguageId;
                    uow.FolderRepository.Insert(newfolder);
                    uow.Save();
                    folderId = newfolder.Id;
                }

                string messages = "";
                List<attachment> newAttachmentList = new List<attachment>();
                var setting = uow.SettingRepository.Get(x => x).First();
                string result = UploadMultipleFile(uploadedFiles, uploadedFiles.Select(x => x.FileName.Substring(0, x.FileName.LastIndexOf("."))).ToArray(), "on", setting.WaterMarkPosition.ToString(), "on", "off", 9, folderId, setting.LanguageId.Value.ToString(), true, "filemanager", out messages, out newAttachmentList, ProductId);

                if (result != "")
                {


                    var p = ModulePermission.check(User.Identity.GetUserId(), 8);
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    string HTMLString = "";
                    HTMLString = CaptureHelper.RenderViewToString("_AttachmentJanebi", newAttachmentList, this.ControllerContext);


                    #region EventLogger
                    Infrastructure.EventLog.Logger.Add(2, "FileManager", "UplodMultiple", false, 200, "ایجاد گروهی فایل", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return Json(new
                    {
                        statusCode = 200,
                        successCounter = 1,
                        status = messages,
                        NewRow = HTMLString
                    }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new
                    {
                        statusCode = 400,
                        successCounter = 0,
                        status = messages
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception x)
            {

                #region EventLogger
                Infrastructure.EventLog.Logger.Add(5, "FileManager", "UplodMultiple", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                #region Unexpected Error
                return Json(new
                {
                    statusCode = 400,
                    successCounter = 0,
                    status = x.Message,
                }, JsonRequestBehavior.AllowGet);
                #endregion
            }


        }
        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" }; // add more if u like...

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            SendConfirmEmail,
            SendConfirmPhone,
            Error
        }

        #endregion
    }
}
