using Domain;
using ahmadi.Infrastructure.Security;
using ahmadi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CoreLib.Infrastructure.SMS;

namespace ahmadi.Controllers
{
    [Authorize]
    public partial class AccountController : BaseController
    {
        private UnitOfWork.UnitOfWorkClass uow = null;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> LoginJson(string username, string password, bool rememberme)
        {
            ApplicationUser user = await UserManager.FindAsync(username, password);
            var result = await SignInManager.PasswordSignInAsync(username, password, rememberme, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        user.LastActivityDate = DateTime.Now;
                        UserManager.Update(user); // Update DB field
                        return Json(true);
                    }
                case SignInStatus.LockedOut:
                case SignInStatus.RequiresVerification:
                case SignInStatus.Failure:
                    return Json(false);
                default:
                    break;
            }
            return Json(false);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            string UrlReferrer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : "";
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                IdentityManager im = new IdentityManager();
                if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support"))
                    return RedirectToLocal("/Admin");
                else if (!String.IsNullOrEmpty(returnUrl))
                    return RedirectToLocal(returnUrl);
                else if (!String.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else if (im.IsInRole(userId, "User"))
                    return RedirectToLocal("/Profile");
                else if (im.IsInRole(userId, "Seller"))
                    return RedirectToLocal("/Seller");
                else
                    return Redirect("~/");
            }

            ViewBag.UrlReferrer = Request.UrlReferrer;
            ViewBag.ReturnUrl = returnUrl;

            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " ورود به " + setting.WebSiteName;
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "ورود";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("Login", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion
            return View();



        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string UrlReferrer)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                IdentityManager im = new IdentityManager();
                if (im.IsInRole(userId, "Admin") || im.IsInRole(userId, "Security") || im.IsInRole(userId, "SuperUser") || im.IsInRole(userId, "Support"))
                    return RedirectToLocal("/Admin");
                else if (!String.IsNullOrEmpty(returnUrl))
                    return RedirectToLocal(returnUrl);
                else if (!String.IsNullOrEmpty(UrlReferrer))
                    return Redirect(UrlReferrer);
                else if (im.IsInRole(userId, "User"))
                    return RedirectToLocal("/Profile");
                else if (im.IsInRole(userId, "Seller"))
                    return RedirectToLocal("/Seller");
                else
                    return Redirect("~/");
            }

            System.Web.Helpers.AntiForgery.Validate();

            #region Get Setting & meta

            var setting = GetSetting();
            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " ورود به " + setting.WebSiteName;
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "ورود";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ResetPassword", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //if (!ahmadi.Infrastructure.Helper.Recaptcha.ReCaptchaPassed(Request.Form["Add"]))
            //{
            //    ModelState.AddModelError("", "دسترسی شما برای استفاده از این فرم توسط کپچای گوگل مسدود شد!");
            //    return View(model);
            //}
            //else
            //{

                //This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true

                ApplicationUser userOld = await UserManager.FindByNameAsync(model.Mobile);
                if (userOld != null)
                {
                    if (userOld.firstTime)
                    {
                        Random generator = new Random();
                        String ra = generator.Next(0, 1000000).ToString("D6");
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = userOld.UserName;
                        userOld.ActiveTempCode = ra;
                        userOld.ActiveTempCodeExpire = DateTime.Now.AddMinutes(10);
                        UserManager.Update(userOld);
                        iPhonemessage.Body = setting.WebSiteName + "\n" + userOld.FirstName + " " + "عزیز\n" + "کد فعال سازی تغییر کلمه عبور : " + ra;
                        await sms.SendSMSAsync(iPhonemessage, "NewForgotPassword", ra, null, null, userOld.FirstName);

                        return Redirect("~/Account/ResetPass?mobile=" + model.Mobile + "&old=true");
                    }
                }
                ApplicationUser user = await UserManager.FindAsync(model.Mobile, model.Password);
                if (user != null)
                {
                    if (user.Disable)
                    {
                        Random generator = new Random();
                        String ra = generator.Next(0, 1000000).ToString("D6");
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        iPhonemessage.Destination = user.UserName;
                        user.ActiveTempCode = ra;
                        user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(10);
                        UserManager.Update(user);
                        iPhonemessage.Body = setting.WebSiteName + "\n" + user.FirstName + " " + "عزیز\n" + "کد فعال سازی تغییر کلمه عبور : " + ra;
                        await sms.SendSMSAsync(iPhonemessage, "NewForgotPassword", ra, null, null, user.FirstName);
                        return Redirect("~/Account/ResetPass?mobile=" + model.Mobile);

                        //return View("Disable");
                    }
                    //else if (!user.EmailConfirmed)
                    //{
                    //    return View("EmailNotConfirmed");
                    //}
                    else
                    {
                        var result = await SignInManager.PasswordSignInAsync(model.Mobile, model.Password, model.RememberMe, shouldLockout: true);
                        switch (result)
                        {
                            case SignInStatus.Success:
                                //now we have access to the custom fields added.
                                user.LastActivityDate = DateTime.Now;
                                UserManager.Update(user); // Update DB field

                                //Delete more than 1 month Event Log
                                Infrastructure.EventLog.ClearLog.Clear();

                                if (returnUrl != null)
                                    return RedirectToLocal(returnUrl);
                                else
                                {

                                    IdentityManager im = new IdentityManager();
                                    if (im.IsInRole(user.Id, "Admin") || im.IsInRole(user.Id, "Security") || im.IsInRole(user.Id, "SuperUser") || im.IsInRole(user.Id, "Support"))
                                        return RedirectToLocal("/Admin");
                                    else if (!String.IsNullOrEmpty(returnUrl))
                                        return RedirectToLocal(returnUrl);
                                    else if (!String.IsNullOrEmpty(UrlReferrer))
                                        return Redirect(UrlReferrer);
                                    else if (im.IsInRole(user.Id, "User"))
                                        return RedirectToLocal("/Profile");
                                    else if (im.IsInRole(user.Id, "Seller"))
                                        return RedirectToLocal("/Seller");
                                    else
                                        return Redirect("~/");

                                }
                            case SignInStatus.LockedOut:
                                return View("Lockout");
                            case SignInStatus.RequiresVerification:
                                return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                            case SignInStatus.Failure:
                            default:
                                ModelState.AddModelError("", "نام کاربری یا رمز عبور صحیح نیست.");
                                return View(model);
                        }
                    }
                }
                else
                {
                    var result = await SignInManager.PasswordSignInAsync(model.Mobile, model.Password, model.RememberMe, shouldLockout: true);

                    switch (result)
                    {
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "نام کاربری یا رمز عبور صحیح نیست.");
                            return View(model);
                    }
                }
            //}
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public virtual ActionResult Register(string r)
        {

            #region Check License


            #endregion

            var setting = GetSetting();

            ViewBag.ProuctLisence = true;
            CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
            ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name");
            ViewBag.RegisterPage = uow.ContentRepository.Get(x => x, x => x.IsRegister).FirstOrDefault();

            List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
            ViewBag.GenderSelectListItem = GenderSelectListItem;

            #region Get Setting & meta
            ViewBag.setting = setting;
            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " ثبت نام در  " + setting.WebSiteName;
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = " ایجاد حساب کاربری ";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("Register", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            ViewBag.ReturnUrl = r;
            return View();

        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> Register(RegisterViewModel model, string IsNewsLetter, string Agree, string r)
        {


            var setting = GetSetting();

            #region Check License


            #endregion

            try
            {
                bool agree = Convert.ToBoolean(Agree);
                if (!ahmadi.Infrastructure.Helper.Recaptcha.ReCaptchaPassed(Request.Form["Add"]))
                {
                    ViewBag.Message = "دسترسی شما برای استفاده از این فرم توسط کپچای گوگل مسدود شد!";

                    ViewBag.ProuctLisence = true;
                    CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                    ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name");

                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;

                    #region Get Setting & meta

                    ViewBag.setting = setting;
                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                    oMeta.WebSiteMetaDescription = " ثبت نام در  " + setting.WebSiteName;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.WebSiteTitle = " ایجاد حساب کاربری ";
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.CanocicalUrl = Url.Action("Register", "Account");
                    oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
                    ViewBag.Meta = oMeta;
                    #endregion

                    ViewBag.setting = setting;

                    ViewBag.ReturnUrl = r;
                    return View(model);
                }

                else if (agree)
                {
                    if (!uow.UserRepository.Any(x => x, x => x.UserName == model.Mobile))
                    {

                        Random generator = new Random();
                        String ra = generator.Next(0, 1000000).ToString("D6");

                        if (model.FirstName != null)
                            model.FirstName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(model.FirstName);
                        if (model.LastName != null)
                            model.LastName = CoreLib.Infrastructure.CommonFunctions.CorrectArabianLetter(model.LastName);
                        var user = new ApplicationUser { UserName = model.Mobile, Email = model.Email };
                        user.CreationDate = DateTime.Now;
                        user.Disable = true;
                        user.EmailConfirmed = false;
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.LockoutEnabled = true;
                        user.PhoneNumber = model.Mobile;
                        user.PhoneNumberConfirmed = false;
                        user.About = "";
                        user.TwoFactorEnabled = false;
                        user.LandlinePhone = "";
                        user.PostalCode = "";
                        user.State = 1;
                        user.City = "";
                        user.Address = "";
                        user.ActiveTempCode = ra;
                        user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(10);

                        var result = await UserManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            IdentityManager im = new IdentityManager();
                            im.AddUserToRole(user.Id, "User");


                            #region Send SMS AND EMail
                            try
                            {

                                SmsService sms = new SmsService();
                                IdentityMessage iPhonemessage = new IdentityMessage();
                                iPhonemessage.Destination = user.UserName;
                                iPhonemessage.Body = setting.WebSiteName + "\n" + user.FirstName + " " + user.LastName + " " + "عزیز\n" + "با تشکر از ثبت نام شما\n" + "کد فعال سازی : " + ra;
                                await sms.SendSMSAsync(iPhonemessage, "NewUserSms", ra, null, null, user.FirstName, user.LastName);
                            }
                            catch (Exception)
                            {

                            }

                            return Redirect("~/Account/VerifyPhone?mobile=" + user.PhoneNumber);


                            #endregion

                        }
                        else
                        {
                            AddErrors(result);

                            ViewBag.ProuctLisence = true;
                            CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                            ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name");
                            ViewBag.RegisterPage = uow.ContentRepository.Get(x => x, x => x.IsRegister).FirstOrDefault();

                            List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                            ViewBag.GenderSelectListItem = GenderSelectListItem;

                            #region Get Setting & meta

                            ViewBag.setting = setting;
                            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                            oMeta.WebSiteMetaDescription = " ثبت نام در  " + setting.WebSiteName;
                            oMeta.WebSiteMetakeyword = "";
                            oMeta.WebSiteTitle = " ایجاد حساب کاربری ";
                            oMeta.Logo = setting.attachmentFileName;
                            oMeta.Favicon = setting.FaviconattachmentFileName;
                            oMeta.CanocicalUrl = Url.Action("Register", "Account");
                            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
                            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
                            ViewBag.Meta = oMeta;
                            #endregion

                            ViewBag.setting = setting;

                            ViewBag.ReturnUrl = r;
                            return View(model);
                        }
                    }
                    else
                    {

                        ViewBag.ProuctLisence = true;
                        CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                        ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name");
                        ViewBag.RegisterPage = uow.ContentRepository.Get(x => x, x => x.IsRegister).FirstOrDefault();

                        List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                        ViewBag.GenderSelectListItem = GenderSelectListItem;

                        #region Get Setting & meta
                        ViewBag.setting = setting;
                        ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                        oMeta.WebSiteMetaDescription = " ثبت نام در  " + setting.WebSiteName;
                        oMeta.WebSiteMetakeyword = "";
                        oMeta.WebSiteTitle = " ایجاد حساب کاربری ";
                        oMeta.Logo = setting.attachmentFileName;
                        oMeta.CanocicalUrl = Url.Action("Register", "Account");
                        oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
                        oMeta.Favicon = setting.FaviconattachmentFileName;
                        oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
                        ViewBag.Meta = oMeta;
                        #endregion


                        ViewBag.Message = "نام کاربری( شماره موبایل ) وجود دارد. اگر رمز عبور خود را فراموش کرده اید به صفحه فراموشی رمز عبور مراجعه نمایید.";

                        ViewBag.ReturnUrl = r;
                        return View(model);


                    }
                }
                else
                {
                    ViewBag.Message = " قوانین و شرایط را قبول نکرده اید. ";

                    ViewBag.ProuctLisence = true;
                    CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                    ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name");

                    List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                    ViewBag.GenderSelectListItem = GenderSelectListItem;

                    #region Get Setting & meta

                    ViewBag.setting = setting;
                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                    oMeta.WebSiteMetaDescription = " ثبت نام در  " + setting.WebSiteName;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.WebSiteTitle = " ایجاد حساب کاربری ";
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.CanocicalUrl = Url.Action("Register", "Account");
                    oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
                    ViewBag.Meta = oMeta;
                    #endregion

                    ViewBag.setting = setting;

                    ViewBag.ReturnUrl = r;
                    return View(model);
                }
            }
            catch (Exception ex)
            {

                ViewBag.ProuctLisence = true;
                CoreLib.ViewModel.Xml.XMLReader ReadXml = new CoreLib.ViewModel.Xml.XMLReader(setting.StaticContentDomain);
                ViewBag.State = new SelectList(ReadXml.ListOfXState(), "Id", "Name");

                ViewBag.Message = "خطایی رخ داد ";
                // If we got this far, something failed, redisplay form
                List<SelectListItem> GenderSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "زن", Value = "False" }, new SelectListItem() { Text = "مرد", Value = "True" } };
                ViewBag.GenderSelectListItem = GenderSelectListItem;

                #region Get Setting & meta

                ViewBag.setting = setting;
                ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                oMeta.WebSiteMetaDescription = " ثبت نام در  " + setting.WebSiteName;
                oMeta.WebSiteMetakeyword = "";
                oMeta.WebSiteTitle = " ایجاد حساب کاربری ";
                oMeta.Logo = setting.attachmentFileName;
                oMeta.CanocicalUrl = Url.Action("Register", "Account");
                oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
                oMeta.Favicon = setting.FaviconattachmentFileName;
                oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
                ViewBag.Meta = oMeta;
                #endregion

                ViewBag.setting = setting;

                ViewBag.ReturnUrl = r;
                return View(model);
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public virtual ActionResult VerifyPhone(string mobile)
        {
            var user = UserManager.FindByName(mobile);
            if (user == null)
                return RedirectToAction("Register");


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
            ViewBag.mobile = mobile;
            ViewBag.ActiveTempCodeExpire = user.ActiveTempCodeExpire;
            #endregion
            return View();

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public virtual ActionResult VerifyPhone(string userName, string code)
        {
            ViewBag.mobile = userName;

            try
            {
                var user = UserManager.FindByName(userName);
                ViewBag.ActiveTempCodeExpire = user.ActiveTempCodeExpire;

                if (code == null)
                {
                    ViewBag.message = "کد فعال سازی را وارد نمایید.";
                    return View();
                }
                else if (user == null)
                {
                    // Don't reveal that the user does not exist
                    ViewBag.Error = " نام کاربری ( شماره تلفن همراه) وارد شده ، صحیح نیست.";
                    return View();
                }
                else if (user.ActiveTempCode != code)
                {
                    // Don't reveal that the user does not exist
                    ViewBag.Error = " کد وارد شده معتبر نیست! ";
                    return View();
                }
                else if (user.ActiveTempCodeExpire <= DateTime.Now)
                {
                    // Don't reveal that the user does not exist
                    ViewBag.Error = " کد وارد شده منقضی شده است. دوباره درخواست تغییر رمز عبور دهید. ";
                    return View();
                }
                user.PhoneNumberConfirmed = true;
                user.ActiveTempCode = null;
                user.ActiveTempCodeExpire = null;
                user.Disable = false;
                user.LastActivityDate = DateTime.Now;
                UserManager.Update(user); // Update DB field

                //Delete more than 1 month Event Log
                Infrastructure.EventLog.ClearLog.Clear();

                SignInManager.SignIn(user, true, true);

                return Redirect("~/");
            }
            catch (Exception x)
            {
                ViewBag.message = x.Message;
                return View();
            }
        }


        [AllowAnonymous]
        public virtual JsonResult doesUserNameExist(string Email)
        {


            var user = uow.UserRepository.Get(u => u, u => u.UserName == Email);
            return Json(!user.Any(), JsonRequestBehavior.AllowGet);


        }

        [AllowAnonymous]
        public virtual JsonResult doesUserPhoneNumberExist(string PhoneNumber)
        {


            var user = uow.UserRepository.Get(u => u, u => u.UserName == PhoneNumber);
            return Json(!user.Any(), JsonRequestBehavior.AllowGet);


        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public virtual async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public virtual ActionResult ForgotPassword()
        {


            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " اقدام برای بازیابی رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "فراموشی رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ForgotPassword", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion
            return View();


        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " اقدام برای بازیابی رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "فراموشی رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ForgotPassword", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            ViewBag.Meta = oMeta;
            #endregion

            if (!ahmadi.Infrastructure.Helper.Recaptcha.ReCaptchaPassed(Request.Form["Add"]))
            {
                ModelState.AddModelError("", "دسترسی شما برای استفاده از این فرم توسط کپچای گوگل مسدود شد!");
                return View(model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var user = await UserManager.FindByNameAsync(model.Mobile);
                    if (user == null)
                    {
                        ViewBag.Error = " نام کاربری( شماره تلفن همراه) وارد شده وجود ندارد. ";
                        return View(model);
                    }
                    else
                    {
                        if (await UserManager.IsLockedOutAsync(user.Id))
                        {

                            ViewBag.Error = " حساب کاربری وارد شده به دلیل اشتباه وارد کردن رمز عبور به مدت 10 دقیقه قفل شده است. پس از آن اقدام نمایید.";
                            return View(model);
                        }
                        else
                        {

                            Random generator = new Random();
                            String ra = generator.Next(0, 1000000).ToString("D6");
                            SmsService sms = new SmsService();
                            IdentityMessage iPhonemessage = new IdentityMessage();
                            iPhonemessage.Destination = user.UserName;
                            user.ActiveTempCode = ra;
                            user.Disable = false;
                            user.ActiveTempCodeExpire = DateTime.Now.AddMinutes(10);
                            UserManager.Update(user);
                            iPhonemessage.Body = setting.WebSiteName + "\n" + user.FirstName + " " + "عزیز\n" + "کد فعال سازی تغییر کلمه عبور : " + ra;
                            await sms.SendSMSAsync(iPhonemessage, "NewForgotPassword", ra, null, null, user.FirstName);
                            //NikSmsManager.SingleSms(iPhonemessage.Body, user.UserName, "09128605712", "Aa@123456", "blacklist");


                            if (user.EmailConfirmed && !String.IsNullOrEmpty(user.Email))
                            {
                                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                                #region Create Html Body
                                string EmailBodyHtml = "";

                                var oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1, null, "attachment").SingleOrDefault();
                                CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(setting.attachmentFileName, "درخواست تغییر رمز عبور", "جهت تغییر رمز عبور خود، روی لینک مقابل کلیک کنید. در صورتیکه تمایل به تغییر آن ندارید این ایمیل را نادیده بگیرید <a href='" + callbackUrl + "'> لینک تغییر رمز عبور</a>", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                                EmailBodyHtml = Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);

                                #endregion
                                EmailService es = new EmailService();
                                IdentityMessage imessage = new IdentityMessage();
                                imessage.Body = EmailBodyHtml;
                                imessage.Destination = user.Email;
                                imessage.Subject = " درخواست تغییر رمز عبور ";
                                await es.SendAsync(imessage);
                            }
                            ViewBag.mobile = user.PhoneNumber;
                            ViewBag.message = "لینک تغییر رمز عبور به شماره موبایل و ایمیل شما ( در صورت تایید ) ارسال شد.";
                            return View(model);
                        }
                    }


                }
                else
                {
                    ViewBag.Error = " خطایی رخ داد. ";
                    return View(model);
                }
            }
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ForgotPasswordConfirmation()
        {



            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " تایید بازیابی رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "تایید فراموشی رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ForgotPasswordConfirmation", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            ViewBag.Meta = oMeta;
            #endregion
            return View();

        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public virtual ActionResult ResetPassword(string code)
        {



            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " تغییر رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "تغییر رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ResetPassword", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            ViewBag.Meta = oMeta;
            #endregion
            return code == null ? View("Error") : View();

        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Mobile);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ViewBag.Error = " نام کاربری ( شماره تلفن همراه) وارد شده ، صحیح نیست. ";
                return RedirectToAction("ResetPassword", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {

                ApplicationUser userr = uow.UserRepository.Get(x => x, x => x.UserName == model.Mobile && !x.EmailConfirmed).FirstOrDefault();
                if (userr != null)
                {
                    userr.EmailConfirmed = true;
                    uow.Save();
                }


                await SignInManager.PasswordSignInAsync(model.Mobile, model.Password, true, shouldLockout: true);
                user.LastActivityDate = DateTime.Now;
                UserManager.Update(user); // Update DB field

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            //AddErrors(result);
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " تغییر رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "تغییر رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ResetPassword", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            ViewBag.Meta = oMeta;

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }


            #endregion

            return View(model);
        }


        //
        // GET: /Account/ResetPass
        [AllowAnonymous]
        public virtual ActionResult ResetPass(string mobile, bool? old)
        {

            var user = UserManager.FindByName(mobile);
            if (user == null)
                return RedirectToAction("ForgotPassword");

            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " تغییر رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "تغییر رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ResetPass", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            ViewBag.Meta = oMeta;
            #endregion
            ViewBag.ActiveTempCodeExpire = user.ActiveTempCodeExpire;
            ViewBag.mobile = mobile;

            ViewBag.IsOld = old;
            ViewBag.name = user.FirstName;
            return View();

        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> ResetPass(ResetPasswordViewModel model, bool? old)
        {
            #region Get Setting & meta

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " تغییر رمز عبور ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "تغییر رمز عبور";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ResetPassword", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            ViewBag.Meta = oMeta;


            #endregion

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Mobile);
            ViewBag.ActiveTempCodeExpire = user.ActiveTempCodeExpire;
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ViewBag.Error = " نام کاربری ( شماره تلفن همراه) وارد شده ، صحیح نیست.";
                ViewBag.mobile = model.Mobile;
                return View(model);
            }
            else if (user.ActiveTempCode != model.Code)
            {
                // Don't reveal that the user does not exist
                ViewBag.Error = " کد وارد شده معتبر نیست! ";
                ViewBag.mobile = model.Mobile;
                return View(model);
            }
            else if (user.ActiveTempCodeExpire <= DateTime.Now)
            {
                // Don't reveal that the user does not exist
                ViewBag.Error = " کد وارد شده منقضی شده است. دوباره درخواست تغییر رمز عبور دهید. ";
                ViewBag.mobile = model.Mobile;
                return View(model);
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                user.ActiveTempCode = null;
                user.ActiveTempCodeExpire = null;
                user.firstTime = false;
                user.Disable = false;
                await SignInManager.PasswordSignInAsync(model.Mobile, model.Password, true, shouldLockout: true);
                user.LastActivityDate = DateTime.Now;
                UserManager.Update(user); // Update DB field

                //Delete more than 1 month Event Log
                Infrastructure.EventLog.ClearLog.Clear();

                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            //AddErrors(result);

            ViewBag.IsOld = old;
            ViewBag.name = user.FirstName;
            ViewBag.mobile = model.Mobile;
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public virtual ActionResult ResetPasswordConfirmation()
        {


            #region Get Setting & meta

            var setting = GetSetting();

            ahmadi.ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " رمز عبور تغییر یافت ";
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = "رمز عبور تغییر یافت";
            oMeta.Logo = setting.attachmentFileName;
            oMeta.CanocicalUrl = Url.Action("ResetPasswordConfirmation", "Account");
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName; oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            ViewBag.Meta = oMeta;
            #endregion
            return View();

        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return Redirect("~/");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
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

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect("~/");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }

        }
        #endregion
    }
}