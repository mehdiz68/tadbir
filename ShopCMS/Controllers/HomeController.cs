using CoreLib.Infrastructure;
using CoreLib.Infrastructure.Captcha;
using CoreLib.Infrastructure.ModelBinder;
using CoreLib.ViewModel.Xml;
using Domain;
using Domain.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ahmadi.Infrastructure.Helper;
using ahmadi.ViewModels.Home;
using UnitOfWork;
using System.IO;

namespace ahmadi.Controllers
{
    public class HomeController : BaseController
    {
        private UnitOfWorkClass uow = null;

        public HomeController()
        {
            uow = new UnitOfWorkClass();

        }
        public ActionResult Index(int? langid)
        {
            if (langid.HasValue)
                langid = langid.Value;
            else if (Request.Url.Host.ToLower().Contains("tadbirpoyan"))
                langid = 1;
            else
                langid = 2;

            //langid = langid.HasValue ? langid.Value : 1;

            if (Request.RawUrl.ToLower().Contains("index"))
                return RedirectPermanent("~/");

            ViewBag.langId = langid;

            #region Get Setting

            var setting = GetSetting(langid);


            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = setting.WebSiteMetaDescription;
            oMeta.WebSiteMetakeyword = setting.WebSiteMetakeyword;
            oMeta.WebSiteTitle = setting.WebSiteTitle;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Logo = setting.attachmentFileName;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", ""));
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            ViewBag.Meta = oMeta;
            #endregion

            #region Get HomePage
            Content HomePage = null, AboutPage = null;
            if (Session["HomePage" + langid] == null)
            {
                HomePage = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsDefault == true).SingleOrDefault();
                Session["HomePage" + langid] = HomePage;
            }
            else
                HomePage = Session["HomePage" + langid] as Content;

            int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);

            if (Session["AboutPage" + langid] == null)
            {
                AboutPage = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsAbout == true).SingleOrDefault();
                Session["AboutPage" + langid] = AboutPage;
            }
            else
                AboutPage = Session["AboutPage" + langid] as Content;

            #endregion

            HomePage oHomePage = new HomePage();
            if (uow.SliderRepository.GetQueryList().Any(x => x.IsActive && (x.TypeId == 1 || x.TypeId == 7) && x.LinkId == HomePageId))
                oHomePage.SliderImages = uow.SliderRepository.GetQueryList().Include("SliderImages.attachment").Where(x => x.IsActive && (x.TypeId == 1 || x.TypeId == 7) && x.LinkId == HomePageId).First().SliderImages.ToList();
            else
                oHomePage.SliderImages = new List<SliderImage>();
            oHomePage.MainServices = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive && (x.ContentTypeId == 11 || x.ContentTypeId == 12)).Include("attachment").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 3);
            oHomePage.Services = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive && (x.ContentTypeId == 2 || x.ContentTypeId == 6)).Include("attachment").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 9);
            oHomePage.Projects = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive && (x.ContentTypeId == 3 || x.ContentTypeId == 7)).Include("attachment").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 9);
            oHomePage.Blogs = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive && (x.ContentTypeId == 1 || x.ContentTypeId == 8)).Include("attachment").Include("Comments").OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 3);
            oHomePage.Comments = uow.CommentRepository.GetQueryList().AsNoTracking().Where(x => x.Content.LanguageId == setting.LanguageId && x.IsActive).OrderByDescending(s => s.Id).Skip(() => 0).Take(() => 10);
            oHomePage.StaticTextCategories = uow.StaticTextCategoryRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive).Include("StaticTextContents").ToList();
            oHomePage.Video = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == setting.LanguageId && x.IsActive && (x.ContentTypeId == 5 || x.ContentTypeId == 10)).Include("attachment").Include("VideoAttachment").OrderBy(s => s.DisplaySort).Skip(() => 0).Take(() => 1).SingleOrDefault();


            var date = DateTime.Now;
            if (uow.AdverestingRepository.Any(x => x.Position == 1 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)))
                oHomePage.TopAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 1 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").OrderBy(r => r.DisplaySort).ToList();
            if (uow.AdverestingRepository.Any(x => x.Position == 2 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)))
                oHomePage.RightAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 2 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").OrderBy(r => r.DisplaySort).ToList();
            if (uow.AdverestingRepository.Any(x => x.Position == 3 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)))
                oHomePage.BottomAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 3 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").OrderBy(r => r.DisplaySort).ToList();
            if (uow.AdverestingRepository.Any(x => x.Position == 4 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)))
                oHomePage.LeftAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.Position == 4 && x.TypeId == 7 && x.LinkId == HomePage.Id && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").OrderBy(r => r.DisplaySort).ToList();



            if (langid == 1)
                return View("Index", oHomePage);
            else
                return View("IndexEn", oHomePage);
        }



        #region jsTools Layout

        public PartialViewResult GetJsTools(int? langId)
        {
            #region Get Setting

            var setting = GetSetting(langId);
            #endregion
            ViewModels.Home.JsTools oJsTools = new ViewModels.Home.JsTools();
            oJsTools.AnalyticsVerification = setting.AnalyticsVerification;
            oJsTools.WebmasterVerification = setting.WebmasterVerification;
            return PartialView("_JsTools", oJsTools);
        }
        #endregion

        #region Header Footer Layout

        public PartialViewResult GetHeader(short? langId)
        {
            if (langId.HasValue)
                langId = langId.Value;
            else if (Request.Url.Host.ToLower().Contains("tadbirpoyan"))
                langId = 1;
            else
                langId = 2;
            #region Get Language
            if (!langId.HasValue)
            {
                langId = 1;
                if (Session["langId" + langId] == null)
                {
                    langId = uow.SettingRepository.GetQueryList().AsNoTracking().Include("attachment").Include("Faviconattachment").Where(x => x.LanguageId == langId).FirstOrDefault().LanguageId.Value;
                    Session["langId" + langId] = langId;
                }
                else
                    langId = Convert.ToInt16(Session["langId" + langId]);
            }
            else
                Session["langId" + langId] = langId;

            #endregion

            #region Get Setting

            var setting = GetSetting(langId);
            #endregion
            XMLReader readXML = new XMLReader(setting.StaticContentDomain);


            #region Get HomePage
            Content HomePage = null;
            if (Session["HomePage" + langId] == null)
            {
                HomePage = uow.ContentRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.LanguageId == langId && x.IsDefault == true);
                Session["HomePage" + langId] = HomePage;
            }
            else
                HomePage = Session["HomePage" + langId] as Content;

            int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);
            #endregion

            Master oMasterTheme = new Master();
            var controller = ControllerContext.ParentActionViewContext.RouteData.Values["Controller"] as string;
            var action = ControllerContext.ParentActionViewContext.RouteData.Values["Action"] as string;
            if (controller.ToLower() == "home" && action.ToLower() == "index")
                oMasterTheme.root = true;
            else
                oMasterTheme.root = false;
            if (User.Identity.IsAuthenticated)
                oMasterTheme.User = uow.UserRepository.GetQueryList().AsNoTracking().FirstOrDefault(x => x.UserName == User.Identity.Name);
            var date = DateTime.Now.Date;
            #region Get Languages
            List<XLanguage> oLanguages = null;
            if (Session["Languages"] == null)
            {
                oLanguages = readXML.ListOfXLanguage();
                Session["Languages"] = oLanguages;
            }
            else
                oLanguages = Session["Languages"] as List<XLanguage>;
            #endregion

            oMasterTheme.Email = setting.Email;
            oMasterTheme.WebsiteAddress = setting.Address;
            oMasterTheme.WebsiteAddress2 = setting.Address2;
            oMasterTheme.StaticContentUrl = setting.StaticContentDomain;
            oMasterTheme.HeaderLogo = setting.attachmentFileName;
            oMasterTheme.ContentTypes = readXML.ListOfXContentType();

            oMasterTheme.HeaderSocialNetworks = uow.SocialRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langId && x.IsActive).ToList();

            #region Popup
            HttpCookie popUpCookie = HttpContext.Request.Cookies["popUpCookie"];
            if (popUpCookie == null)
            {
                popUpCookie = new HttpCookie("popUpCookie");
                popUpCookie.Value = string.Format("{0},{1}", setting.PopUpEditVersion, "1");
                popUpCookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Response.Cookies.Add(popUpCookie);
                oMasterTheme.PopUpActive = setting.PopUpActive;
                oMasterTheme.PopUpMessage = setting.PopUpMessage;
                oMasterTheme.PopUpType = setting.PopUpType;
            }
            else
            {
                string popUpCookieValue = popUpCookie.Value;
                if (popUpCookieValue.ToString().IndexOf(",") > 0)
                {
                    int EditVersion = Convert.ToInt32(popUpCookieValue.ToString().Substring(0, popUpCookieValue.ToString().IndexOf(",")));
                    int value = Convert.ToInt32(popUpCookieValue.ToString().Substring(popUpCookieValue.ToString().IndexOf(",") + 1));
                    if (value < 2 || EditVersion < setting.PopUpEditVersion)
                    {

                        oMasterTheme.PopUpActive = setting.PopUpActive;
                        oMasterTheme.PopUpMessage = setting.PopUpMessage;
                        oMasterTheme.PopUpType = setting.PopUpType;
                        value++;
                        popUpCookie.Value = string.Format("{0},{1}", setting.PopUpEditVersion, (EditVersion < setting.PopUpEditVersion ? "1" : value.ToString()));
                        HttpContext.Response.Cookies.Add(popUpCookie);

                    }
                    else
                        oMasterTheme.PopUpActive = false;
                }
                else
                    oMasterTheme.PopUpActive = false;
            }
            #endregion

            #region Menu
            oMasterTheme.HeaderMainMenu = new List<DisplayMenu>();
            foreach (var item in uow.MenuRepository.GetQueryList().AsNoTracking().Include("attachment").Where(x => x.LanguageId == langId && x.IsActive == true && (x.PlaceShow == 3 || x.PlaceShow == 1)).ToList())
            {
                DisplayMenu dm = new ViewModels.Home.DisplayMenu();
                dm.Id = item.Id;
                if (item.Cover != null)
                    dm.Cover = item.attachment.FileName;
                else
                    dm.Cover = null;
                dm.IsRoot = (item.LinkId.HasValue ? (item.LinkId == HomePageId && item.TypeId == 11 ? true : false) : false);
                dm.Link = GenerateLink(item.Id, item.TypeId, item.LinkId, item.LinkUniqIdentifier, item.Title, item.OffLink, item.ParrentMenu, setting.StaticContentDomain, langId.Value);
                dm.PlaceShow = item.PlaceShow;
                dm.DisplayOrder = item.DisplaySort;
                dm.Title = item.Title;
                dm.parentId = item.ParrentMenu != null ? item.ParrentMenu.Id : 0;
                oMasterTheme.HeaderMainMenu.Add(dm);
            }
            #endregion


            if (setting != null)
            {
                oMasterTheme.WebSiteName = setting.WebSiteName;
                oMasterTheme.WebSiteTitle = setting.WebSiteTitle;
            }



            //Get Layouts
            if (langId == 1)
                return PartialView("_Header", oMasterTheme);
            else
                return PartialView("_HeaderEn", oMasterTheme);

        }

        public PartialViewResult GetHeaderVideo(short? langId)
        {
            #region Get Language
            if (!langId.HasValue)
            {
                langId = 1;
                if (Session["langId" + langId] == null)
                {
                    langId = uow.SettingRepository.GetQueryList().AsNoTracking().Include("attachment").Include("Faviconattachment").Where(x => x.LanguageId == langId).FirstOrDefault().LanguageId.Value;
                    Session["langId" + langId] = langId;
                }
                else
                    langId = Convert.ToInt16(Session["langId" + langId]);
            }
            else
                Session["langId" + langId] = langId;

            #endregion
            #region Get Setting

            var setting = GetSetting(langId);
            #endregion
            XMLReader readXML = new XMLReader(setting.StaticContentDomain);


            #region Get HomePage
            Content HomePage = null;
            if (Session["HomePage" + langId] == null)
            {
                HomePage = uow.ContentRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.LanguageId == langId && x.IsDefault == true);
                Session["HomePage" + langId] = HomePage;
            }
            else
                HomePage = Session["HomePage" + langId] as Content;

            int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);
            #endregion

            Master oMasterTheme = new Master();
            var controller = ControllerContext.ParentActionViewContext.RouteData.Values["Controller"] as string;
            var action = ControllerContext.ParentActionViewContext.RouteData.Values["Action"] as string;
            if (controller.ToLower() == "home" && action.ToLower() == "index")
                oMasterTheme.root = true;
            else
                oMasterTheme.root = false;
            if (User.Identity.IsAuthenticated)
                oMasterTheme.User = uow.UserRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.UserName == User.Identity.Name);
            var date = DateTime.Now.Date;
            #region Get Languages
            List<XLanguage> oLanguages = null;
            if (Session["Languages"] == null)
            {
                oLanguages = readXML.ListOfXLanguage();
                Session["Languages"] = oLanguages;
            }
            else
                oLanguages = Session["Languages"] as List<XLanguage>;
            #endregion

            oMasterTheme.StaticContentUrl = setting.StaticContentDomain;
            oMasterTheme.HeaderLogo = setting.attachmentFileName;
            oMasterTheme.HeaderSocialNetworks = uow.SocialRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langId && x.IsActive).ToList();

            oMasterTheme.ContentTypes = readXML.ListOfXContentType();

            #region Popup
            HttpCookie popUpCookie = HttpContext.Request.Cookies["popUpCookie"];
            if (popUpCookie == null)
            {
                popUpCookie = new HttpCookie("popUpCookie");
                popUpCookie.Value = string.Format("{0},{1}", setting.PopUpEditVersion, "1");
                popUpCookie.Expires = DateTime.Now.AddDays(7);
                HttpContext.Response.Cookies.Add(popUpCookie);
                oMasterTheme.PopUpActive = setting.PopUpActive;
                oMasterTheme.PopUpMessage = setting.PopUpMessage;
                oMasterTheme.PopUpType = setting.PopUpType;
            }
            else
            {
                string popUpCookieValue = popUpCookie.Value;
                if (popUpCookieValue.ToString().IndexOf(",") > 0)
                {
                    int EditVersion = Convert.ToInt32(popUpCookieValue.ToString().Substring(0, popUpCookieValue.ToString().IndexOf(",")));
                    int value = Convert.ToInt32(popUpCookieValue.ToString().Substring(popUpCookieValue.ToString().IndexOf(",") + 1));
                    if (value < 2 || EditVersion < setting.PopUpEditVersion)
                    {

                        oMasterTheme.PopUpActive = setting.PopUpActive;
                        oMasterTheme.PopUpMessage = setting.PopUpMessage;
                        oMasterTheme.PopUpType = setting.PopUpType;
                        value++;
                        popUpCookie.Value = string.Format("{0},{1}", setting.PopUpEditVersion, (EditVersion < setting.PopUpEditVersion ? "1" : value.ToString()));
                        HttpContext.Response.Cookies.Add(popUpCookie);

                    }
                    else
                        oMasterTheme.PopUpActive = false;
                }
                else
                    oMasterTheme.PopUpActive = false;
            }
            #endregion

            #region Menu
            oMasterTheme.HeaderMainMenu = new List<DisplayMenu>();
            foreach (var item in uow.MenuRepository.GetQueryList().AsNoTracking().Include("attachment").Where(x => x.LanguageId == langId && x.IsActive == true && (x.PlaceShow == 7 || x.PlaceShow == 9)).ToList())
            {
                DisplayMenu dm = new ViewModels.Home.DisplayMenu();
                dm.Id = item.Id;
                if (item.Cover != null)
                    dm.Cover = item.attachment.FileName;
                else
                    dm.Cover = null;
                dm.IsRoot = (item.LinkId.HasValue ? (item.LinkId == HomePageId && item.TypeId == 11 ? true : false) : false);
                dm.Link = GenerateLink(item.Id, item.TypeId, item.LinkId, item.LinkUniqIdentifier, item.Title, item.OffLink, item.ParrentMenu, setting.StaticContentDomain, langId.Value);
                dm.PlaceShow = item.PlaceShow;
                dm.DisplayOrder = item.DisplaySort;
                dm.Title = item.Title;
                dm.parentId = item.ParrentMenu != null ? item.ParrentMenu.Id : 0;
                oMasterTheme.HeaderMainMenu.Add(dm);
            }
            #endregion

            if (setting != null)
            {
                oMasterTheme.WebSiteName = setting.WebSiteName;
                oMasterTheme.WebSiteTitle = setting.WebSiteTitle;
            }

            //Get Layouts
            if (langId == 1)
                return PartialView("_HeaderVideo", oMasterTheme);
            else
                return PartialView("_HeaderVideoEn", oMasterTheme);
        }

        public PartialViewResult GetRegisterHeader(short? langId)
        {
            #region Get Language
            if (!langId.HasValue)
            {
                langId = 1;
                if (Session["langId" + langId] == null)
                {
                    langId = uow.SettingRepository.GetQueryList().AsNoTracking().Include("attachment").Include("Faviconattachment").Where(x => x.LanguageId == langId).FirstOrDefault().LanguageId.Value;
                    Session["langId" + langId] = langId;
                }
                else
                    langId = Convert.ToInt16(Session["langId" + langId]);
            }
            else
                Session["langId" + langId] = langId;
            #endregion
            #region Get Setting

            var setting = GetSetting(langId);
            #endregion
            XMLReader readXML = new XMLReader(setting.StaticContentDomain);


            #region Get HomePage
            Content HomePage = null;
            if (Session["HomePage" + langId] == null)
            {
                HomePage = uow.ContentRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.LanguageId == langId && x.IsDefault == true);
                Session["HomePage" + langId] = HomePage;
            }
            else
                HomePage = Session["HomePage" + langId] as Content;

            int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);
            #endregion

            Master oMasterTheme = new Master();
            var controller = ControllerContext.ParentActionViewContext.RouteData.Values["Controller"] as string;
            var action = ControllerContext.ParentActionViewContext.RouteData.Values["Action"] as string;
            if (controller.ToLower() == "home" && action.ToLower() == "index")
                oMasterTheme.root = true;
            else
                oMasterTheme.root = false;
            if (User.Identity.IsAuthenticated)
                oMasterTheme.User = uow.UserRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.UserName == User.Identity.Name);
            var date = DateTime.Now.Date;
            #region Get Languages
            List<XLanguage> oLanguages = null;
            if (Session["Languages"] == null)
            {
                oLanguages = readXML.ListOfXLanguage();
                Session["Languages"] = oLanguages;
            }
            else
                oLanguages = Session["Languages"] as List<XLanguage>;
            #endregion

            oMasterTheme.StaticContentUrl = setting.StaticContentDomain;
            oMasterTheme.HeaderLogo = setting.attachmentFileName;
            if (setting != null)
            {
                oMasterTheme.WebSiteName = setting.WebSiteName;
                oMasterTheme.WebSiteTitle = setting.WebSiteTitle;
            }

            //Get Layouts
            if (langId == 1)
                return PartialView("_RegisterHeader", oMasterTheme);
            else
                return PartialView("_RegisterHeaderEn", oMasterTheme);

        }
        public PartialViewResult GetFooter(short? langId)
        {
            if (langId.HasValue)
                langId = langId.Value;
            else if (Request.Url.Host.ToLower().Contains("tadbirpoyan"))
                langId = 1;
            else
                langId = 2;
            if (true)
            //if (oModulesList.Any(x => x.Id > 0 && x.HasAccess))
            {
                #region Get Language
                if (!langId.HasValue)
                {
                    langId = 1;
                    if (Session["langId" + langId] == null)
                    {
                        langId = uow.SettingRepository.GetQueryList().AsNoTracking().Include("attachment").Include("Faviconattachment").Where(x => x.LanguageId == langId).FirstOrDefault().LanguageId.Value;
                        Session["langId" + langId] = langId;
                    }
                    else
                        langId = Convert.ToInt16(Session["langId" + langId]);
                }
                else
                    Session["langId" + langId] = langId;

                #endregion

                #region Get Setting

                var setting = GetSetting(langId);
                #endregion

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                #region Get HomePage
                Content HomePage = null, AboutPage = null;
                if (Session["HomePage" + langId] == null)
                {
                    HomePage = uow.ContentRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.LanguageId == langId && x.IsDefault == true);
                    Session["HomePage" + langId] = HomePage;
                }
                else
                    HomePage = Session["HomePage" + langId] as Content;

                if (Session["AboutPage" + langId] == null)
                {
                    AboutPage = uow.ContentRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.LanguageId == langId && x.IsAbout == true);
                    Session["AboutPage" + langId] = AboutPage;
                }
                else
                    AboutPage = Session["AboutPage" + langId] as Content;

                int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);
                #endregion

                Master oMasterTheme = new Master();

                #region Get Languages
                List<XLanguage> oLanguages = null;
                if (Session["Languages"] == null)
                {
                    oLanguages = readXML.ListOfXLanguage();
                    Session["Languages"] = oLanguages;
                }
                else
                    oLanguages = Session["Languages"] as List<XLanguage>;
                #endregion

                oMasterTheme.Email = setting.Email;
                oMasterTheme.services = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.IsActive && x.LanguageId == langId && (x.ContentTypeId == 2 || x.ContentTypeId == 6));
                oMasterTheme.FooterGoogleMapLatitude = setting.FooterGoogleMapLatitude;
                oMasterTheme.FooterGoogleMapLongitude = setting.FooterGoogleMapLongitude;
                oMasterTheme.FooterGoogleMapZoom = setting.FooterGoogleMapZoom;
                oMasterTheme.HeaderSocialNetworks = uow.SocialRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langId && x.IsActive).ToList();
                oMasterTheme.StaticContentUrl = setting.StaticContentDomain;
                oMasterTheme.HeaderLogo = setting.attachmentFileName;
                if (AboutPage != null)
                    oMasterTheme.AboutPageAbstract = AboutPage.Abstract;
                else
                    oMasterTheme.AboutPageAbstract = "";
                if (User.Identity.IsAuthenticated)
                    oMasterTheme.User = uow.UserRepository.GetQueryList().AsNoTracking().FirstOrDefault(x => x.UserName == User.Identity.Name);
                oMasterTheme.WebsiteAddress = setting.Address;
                oMasterTheme.WebsiteAddress2 = setting.Address2;
                oMasterTheme.WebsitePhoneNumber = setting.WebSitePhoneNumber;
                oMasterTheme.WebsiteTele = setting.Tele;
                oMasterTheme.WebsiteTele2 = setting.Tele2;
                oMasterTheme.WebsiteTele3 = setting.Tele3;


                #region Menu
                oMasterTheme.FooterMainMenu = new List<DisplayMenu>();
                foreach (var item in uow.MenuRepository.GetQueryList().Include("attachment").AsNoTracking().Where(x => x.LanguageId == langId && x.IsActive == true && x.PlaceShow == 2).ToList())
                {
                    DisplayMenu dm = new ViewModels.Home.DisplayMenu();
                    dm.Id = item.Id;
                    if (item.Cover != null)
                        dm.Cover = item.attachment.FileName;
                    else
                        dm.Cover = null;
                    dm.IsRoot = (item.LinkId.HasValue ? (item.LinkId == HomePageId && item.TypeId == 11 ? true : false) : false);
                    dm.Link = GenerateLink(item.Id, item.TypeId, item.LinkId, item.LinkUniqIdentifier, item.Title, item.OffLink, item.ParrentMenu, setting.StaticContentDomain, langId.Value);
                    dm.PlaceShow = item.PlaceShow;
                    dm.DisplayOrder = item.DisplaySort;
                    dm.Title = item.Title;
                    dm.parentId = item.ParrentMenu != null ? item.ParrentMenu.Id : 0;
                    oMasterTheme.FooterMainMenu.Add(dm);
                }
                #endregion

                if (setting != null)
                {
                    oMasterTheme.WebSiteName = setting.WebSiteName;
                    oMasterTheme.WebSiteTitle = setting.WebSiteTitle;
                }

                ViewBag.FormRequestCat = uow.FormRequestCategoryRepository.Get(x => x, x => x.LanguageId == langId.Value, x => x.OrderBy(a => a.Title));

                //Get Layouts
                if (langId == 1)
                    return PartialView("_Footer", oMasterTheme);
                else
                    return PartialView("_FooterEn", oMasterTheme);
            }
            else
                return PartialView("_Footer");
        }


        public PartialViewResult GetFooterVideo(int? langId)
        {
            #region Get Language
            if (!langId.HasValue)
            {
                langId = 1;
                if (Session["langId" + langId] == null)
                {
                    langId = uow.SettingRepository.GetQueryList().AsNoTracking().Include("attachment").Include("Faviconattachment").Where(x => x.LanguageId == langId).FirstOrDefault().LanguageId.Value;
                    Session["langId" + langId] = langId;
                }
                else
                    langId = Convert.ToInt16(Session["langId" + langId]);
            }
            else
                Session["langId" + langId] = langId;
            #endregion

            #region Get Setting

            var setting = GetSetting(langId);
            #endregion

            XMLReader readXML = new XMLReader(setting.StaticContentDomain);
            #region Get HomePage
            Content HomePage = null, AboutPage = null;
            if (Session["HomePage" + langId] == null)
            {
                HomePage = uow.ContentRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.LanguageId == langId && x.IsDefault == true);
                Session["HomePage" + langId] = HomePage;
            }
            else
                HomePage = Session["HomePage" + langId] as Content;

            if (Session["AboutPage" + langId] == null)
            {
                AboutPage = uow.ContentRepository.GetQueryList().AsNoTracking().SingleOrDefault(x => x.LanguageId == langId && x.IsAbout == true);
                Session["AboutPage" + langId] = AboutPage;
            }
            else
                AboutPage = Session["AboutPage" + langId] as Content;

            int HomePageId = (HomePage != null ? Convert.ToInt32(HomePage.Id) : 0);
            #endregion

            Master oMasterTheme = new Master();

            #region Get Languages
            List<XLanguage> oLanguages = null;
            if (Session["Languages"] == null)
            {
                oLanguages = readXML.ListOfXLanguage();
                Session["Languages"] = oLanguages;
            }
            else
                oLanguages = Session["Languages"] as List<XLanguage>;
            #endregion

            oMasterTheme.HeaderSocialNetworks = uow.SocialRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langId && x.IsActive).ToList();
            oMasterTheme.StaticContentUrl = setting.StaticContentDomain;
            oMasterTheme.HeaderLogo = setting.attachmentFileName;
            if (AboutPage != null)
                oMasterTheme.AboutPageAbstract = AboutPage.Abstract;
            else
                oMasterTheme.AboutPageAbstract = "";



            #region Menu
            oMasterTheme.FooterMainMenu = new List<DisplayMenu>();
            foreach (var item in uow.MenuRepository.Get(x => x, x => x.LanguageId == langId && x.IsActive == true && (x.PlaceShow == 8 || x.PlaceShow == 9), null, "attachment").ToList())
            {
                DisplayMenu dm = new ViewModels.Home.DisplayMenu();
                dm.Id = item.Id;
                if (item.Cover != null)
                    dm.Cover = item.attachment.FileName;
                else
                    dm.Cover = null;
                dm.IsRoot = (item.LinkId.HasValue ? (item.LinkId == HomePageId && item.TypeId == 11 ? true : false) : false);
                dm.Link = GenerateLink(item.Id, item.TypeId, item.LinkId, item.LinkUniqIdentifier, item.Title, item.OffLink, item.ParrentMenu, setting.StaticContentDomain, langId.Value);
                dm.PlaceShow = item.PlaceShow;
                dm.DisplayOrder = item.DisplaySort;
                dm.Title = item.Title;
                dm.parentId = item.ParrentMenu != null ? item.ParrentMenu.Id : 0;
                oMasterTheme.FooterMainMenu.Add(dm);
            }
            #endregion


            if (setting != null)
            {
                oMasterTheme.WebSiteName = setting.WebSiteName;
                oMasterTheme.WebSiteTitle = setting.WebSiteTitle;
            }
            oMasterTheme.VideoCat1 = uow.CategoryRepository.Get(x => new Link { Id = x.Id, Name = x.Title, PageAddress = x.PageAddress }, x => x.IsVideo == true && x.ParrentId == null, x => x.OrderBy(s => s.Sort), "", 0, 1, true).FirstOrDefault();
            oMasterTheme.VideoCat2 = uow.CategoryRepository.Get(x => new Link { Id = x.Id, Name = x.Title, PageAddress = x.PageAddress }, x => x.IsVideo == true && x.ParrentId == null, x => x.OrderBy(s => s.Sort), "", 1, 1, true).FirstOrDefault();
            oMasterTheme.VideoCat3 = uow.CategoryRepository.Get(x => new Link { Id = x.Id, Name = x.Title, PageAddress = x.PageAddress }, x => x.IsVideo == true && x.ParrentId == null, x => x.OrderBy(s => s.Sort), "", 2, 1, true).FirstOrDefault();
            if (oMasterTheme.VideoCat1 != null)
                oMasterTheme.Videos1 = uow.ContentRepository.Get(x => new Link { Id = x.Id, Name = x.Title, PageAddress = x.PageAddress }, x => x.CatId == oMasterTheme.VideoCat1.Id && x.IsActive, x => x.OrderByDescending(s => s.Id), "", 0, 7, true);
            if (oMasterTheme.VideoCat2 != null)
                oMasterTheme.Videos2 = uow.ContentRepository.Get(x => new Link { Id = x.Id, Name = x.Title, PageAddress = x.PageAddress }, x => x.CatId == oMasterTheme.VideoCat2.Id && x.IsActive, x => x.OrderByDescending(s => s.Id), "", 0, 7, true);
            if (oMasterTheme.VideoCat3 != null)
                oMasterTheme.Videos3 = uow.ContentRepository.Get(x => new Link { Id = x.Id, Name = x.Title, PageAddress = x.PageAddress }, x => x.CatId == oMasterTheme.VideoCat3.Id && x.IsActive, x => x.OrderByDescending(s => s.Id), "", 0, 7, true);
            //Get Layouts
            if (langId == 1)
                return PartialView("_FooterVideo", oMasterTheme);
            else
                return PartialView("_FooterVideoEn", oMasterTheme);


        }

        #endregion

        #region NewsLetter
        [HttpPost]
        public async Task<JsonResult> JoinNewsLetter(NewsLetter NewsLetter)
        {
            try
            {
                HttpCookie NewLetterCookie = HttpContext.Request.Cookies["NewsLetter"];
                if (NewLetterCookie == null)
                {
                    NewLetterCookie = new HttpCookie("NewsLetter");
                    NewLetterCookie["NewsLetter"] = "False";
                }
                if (NewLetterCookie["NewsLetter"] == "False")
                {

                    var newsletteremail = uow.NewsLetterEmailRepository.Get(x => x, x => x.Email == NewsLetter.Email);
                    if (!newsletteremail.Any())
                    {
                        NewsLetterEmail oEmail = new NewsLetterEmail();
                        oEmail.Email = NewsLetter.Email;
                        oEmail.InsertDate = DateTime.Now;
                        oEmail.LanguageId = 1;
                        oEmail.IsVerified = false;
                        uow.NewsLetterEmailRepository.Insert(oEmail);
                        uow.Save();

                        //Add Cookie

                        NewLetterCookie["NewsLetter"] = "True";
                        NewLetterCookie.Expires = DateTime.Now.AddMinutes(5);
                        HttpContext.Response.Cookies.Add(NewLetterCookie);

                        #region Create Html Body
                        string EmailBodyHtml = "";

                        Setting oSetting = null;
                        if (Session["settingPersian"] == null)
                        {
                            oSetting = uow.SettingRepository.Get(x => x, null, null, "attachment").FirstOrDefault();
                            Session["settingPersian"] = oSetting;
                        }
                        else
                        {
                            oSetting = Session["settingPersian"] as Setting;
                        }

                        Guid VerifyCode = Guid.NewGuid();
                        Session["VerifyNewsletterCode"] = VerifyCode;
                        string VerifytUrl = "http://" + HttpContext.Request.Url.Host + "/Home/VerifyEmail?id=" + VerifyCode + "&email=" + NewsLetter.Email;
                        CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید ایمیل شما", "بازدید کننده گرامی ، " + NewsLetter.Email + "، ایمیل شما در خبرنامه سایت ما ثبت شد . برای تایید ایمیل خود روی لینک زیر کلیک نمایید : <br/> <a href='" + VerifytUrl + "'>لینک فعال سازی ایمیل برای عضویت در خبرنامه</a>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                        EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                        #endregion

                        #region SendMail
                        EmailService es = new EmailService();
                        IdentityMessage imessage = new IdentityMessage();
                        imessage.Body = EmailBodyHtml;
                        imessage.Destination = NewsLetter.Email;
                        imessage.Subject = "تایید ایمیل شما";
                        await es.SendAsync(imessage);

                        #endregion
                        return Json(new
                        {
                            Message = "ایمیل شما ثبت شد. اما برای تایید آن ، به ایمیل خود مراجعه کنید.تنها 20 دقیقه فرصت دارید.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (!newsletteremail.First().IsVerified)
                        {
                            #region Create Html Body
                            string EmailBodyHtml = "";

                            Setting oSetting = null;
                            if (Session["settingPersian"] == null)
                            {
                                oSetting = uow.SettingRepository.Get(x => x, null, null, "attachment").FirstOrDefault();
                                Session["settingPersian"] = oSetting;
                            }
                            else
                            {
                                oSetting = Session["settingPersian"] as Setting;
                            }

                            Guid VerifyCode = Guid.NewGuid();
                            Session["VerifyNewsletterCode"] = VerifyCode;
                            string VerifytUrl = "http://" + HttpContext.Request.Url.Host + "/Home/VerifyEmail?id=" + VerifyCode + "&email=" + NewsLetter.Email;
                            CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "تایید ایمیل شما", "بازدید کننده گرامی ، " + NewsLetter.Email + "، ایمیل شما در خبرنامه سایت ما ثبت شد . برای تایید ایمیل خود روی لینک زیر کلیک نمایید : <br/> <a href='" + VerifytUrl + "'>لینک فعال سازی ایمیل برای عضویت در خبرنامه</a>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                            EmailBodyHtml = Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                            #endregion

                            #region SendMail
                            EmailService es = new EmailService();
                            IdentityMessage imessage = new IdentityMessage();
                            imessage.Body = EmailBodyHtml;
                            imessage.Destination = NewsLetter.Email;
                            imessage.Subject = "تایید ایمیل شما";
                            await es.SendAsync(imessage);

                            #endregion
                            return Json(new
                            {
                                Message = "ایمیل وارد شده قبلا در خبرنامه وارد شده است. اما تایید نشده است. برای تایید آن ، به ایمیل خود مراجعه کنید.تنها 20 دقیقه فرصت دارید",
                                statusCode = 200
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new
                            {
                                Message = "ایمیل وارد شده قبلا در خبرنامه وارد شده است.",
                                statusCode = 500
                            }, JsonRequestBehavior.AllowGet);
                        }

                    }

                }
                else
                {
                    return Json(new
                    {
                        Message = "5 دقیقه دیگر امتحان نمایید",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region request form
        [HttpPost]
        public JsonResult NewRequest(string Name, string Family, string Mobile, int CatId, string Message)
        {
            try
            {
                FormRequest t = new FormRequest { Name = Name.Trim(), Family = Family.Trim(), CatId = CatId, Tele = Mobile.Trim(), Message = Message, InsertDate = DateTime.Now };
                uow.FormRequestRepository.Insert(t);
                uow.Save();


                return Json(new
                {
                    Id = t.Id,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = "خطایی رخ داد" + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Captcha

        public ActionResult Captcha()
        {
            zCaptcha c = new zCaptcha();
            Session["captcha"] = c.GetCurrentValue();
            return PartialView("_Captcha", c);
        }
        public ActionResult RefreshCaptcha()
        {
            zCaptcha c = new zCaptcha();
            Session["captcha"] = c.GetCurrentValue();
            return Json(new
            {
                data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                statusCode = 200
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        [HttpPost]
        public JsonResult LoadMorePr()
        {
            try
            {


                var setting = GetSetting();

                var ProductRandomSettings = uow.ProductRandomSettingRepository.Get(x => x.ProductCatId, x => x.SettingId == setting.Id);
                List<int> randomCatIds = new List<int>();
                foreach (var item in ProductRandomSettings)
                    randomCatIds.AddRange(uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", item)).ToList());

                var Ids = uow.ProductRepository.Get(x => x.Id, x => x.ProductCategories.Any(s => x.IsActive && (x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2) && x.LanguageId == setting.LanguageId && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4 && randomCatIds.Contains(s.Id)), x => x.OrderBy(s => Guid.NewGuid()), "", 0, 20);

                var ProductRecommends = uow.ProductRepository.ProductItemList(x => x.IsActive && (x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2) && x.LanguageId == setting.LanguageId && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4 && Ids.Contains(x.Id), x => x.OrderBy(s => s.Id), 0, 20);
                ViewBag.w = 232;
                ViewBag.h = 232;
                ViewBag.size = "SM";
                return Json(new
                {
                    NewRow = CaptureHelper.RenderViewToString("_RandomProducts", ProductRecommends, this.ControllerContext),
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Message = " خطایی رخ داد. " + ex.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult VerifyEmail(Guid id, string email)
        {
            try
            {
                if (Session["VerifyNewsletterCode"] != null)
                {
                    Guid CurrentId = new Guid(Session["VerifyNewsletterCode"].ToString());
                    if (CurrentId == id)
                    {
                        var newsletterEmail = uow.NewsLetterEmailRepository.Get(x => x, x => x.Email == email.Trim());
                        if (newsletterEmail.Any())
                        {
                            newsletterEmail.First().IsVerified = true;
                            uow.Save();
                            return RedirectToAction("Index", "Home", new { NewsLettersVerifystate = 1 });
                        }
                        else
                            return RedirectToAction("Index", "Home", new { NewsLettersVerifystate = -3 });
                    }
                    else
                        return RedirectToAction("Index", "Home", new { NewsLettersVerifystate = -2 });
                }
                else
                    return RedirectToAction("Index", "Home", new { NewsLettersVerifystate = -1 });
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home", new { NewsLettersVerifystate = 0 });
            }
        }


        [HttpPost]
        [CorrectArabianLetter(new string[] { "keyword" })]
        public JsonResult Search(string keyword)
        {
            try
            {
                #region Get Setting

                var setting = GetSetting();
                #endregion


                List<ViewModels.Home.SearchList> SearchLists = new List<ViewModels.Home.SearchList>();


                #region Content
                {
                    foreach (var content in uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.IsActive && x.LanguageId == 1 && (x.PageAddress.Contains(keyword) || x.Title.Contains(keyword) || x.Abstract.Contains(keyword))).OrderBy(s => s.Title).Skip(() => 0).Take(() => 5).ToList())
                        SearchLists.Add(new ViewModels.Home.SearchList(content.Id, content.Title, content.Abstract, content.ContentTypeId, content.Cover, Url.Content("~/Content/UploadFiles/")));
                }
                #endregion

                #region Category
                {
                    foreach (var content in uow.CategoryRepository.GetQueryList().AsNoTracking().Where(x => x.IsActive && x.LanguageId == 1 && (x.PageAddress.Contains(keyword) || x.Title.Contains(keyword) || x.Abstract.Contains(keyword))).OrderBy(s => s.Title).Skip(() => 0).Take(() => 5).ToList())
                        SearchLists.Add(new ViewModels.Home.SearchList(content.Id, content.Title, content.Abstract, 0, content.Cover, Url.Content("~/Content/UploadFiles/")));
                }
                #endregion

                #region Tag
                {

                    foreach (var content in uow.TagRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == 1 && (x.TagName.Contains(keyword))).OrderBy(s => s.TagName).Skip(() => 0).Take(() => 5).ToList())
                        SearchLists.Add(new ViewModels.Home.SearchList(content.Id, content.TagName, content.TagName, -1, null, Url.Content("~/Content/UploadFiles/")));
                }
                #endregion

                #region Product Category
                {
                    foreach (var content in uow.ProductCategoryRepository.GetQueryList().AsNoTracking().Where(x => x.IsActive && x.LanguageId == 1 && (x.PageAddress.Contains(keyword) || x.Title.Contains(keyword) || x.Abstract.Contains(keyword))).OrderBy(s => s.Title).Skip(() => 0).Take(() => 5).ToList())
                        SearchLists.Add(new ViewModels.Home.SearchList(content.Id, content.Name, content.Abstract, -3, content.Cover, Url.Content("~/Content/UploadFiles/")));
                }
                #endregion

                #region Product 
                {
                    //foreach (var content in uow.ProductRepository.Get(x => x, x => x.IsActive && x.LanguageId == 1 && (x.Title.Contains(keyword) || x.Name.Contains(keyword) || x.LatinName.Contains(keyword) || x.ProductPrices.Any(s => s.code.Contains(keyword))), o => o.OrderBy(s => s.Title), "", 0, 5))
                    foreach (var content in uow.ProductRepository.ProductItemList(x => x.IsActive && x.LanguageId == 1 && (x.Title.Contains(keyword) || x.Name.Contains(keyword) || x.LatinName.Contains(keyword)), o => o.OrderBy(s => s.Title), 0, 5).ToList())
                        if (content.MainImage != null)
                            SearchLists.Add(new ViewModels.Home.SearchList(content.Id, content.PageAddress, content.Title, -2, content.MainImage.AttachementId, Url.Content("~/Content/UploadFiles/")));
                        else
                            SearchLists.Add(new ViewModels.Home.SearchList(content.Id, content.PageAddress, content.Title, -2, null, Url.Content("~/Content/UploadFiles/")));
                }
                #endregion





                return Json(new
                {
                    data = SearchLists,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new
                {
                    data = "Exception Erorr",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        [CorrectArabianLetter(new string[] { "keyword" })]
        public JsonResult MainSearch(string keyword)
        {
            try
            {
                //search
                List<MainSearch> SearchLists = uow.ContentRepository.Sql(x => new MainSearch(), "exec MainSearch @keyword,@BrandCount,@CatCount,@ProductCount", new SqlParameter("@keyword", keyword.Trim().ToLower()), new SqlParameter("@BrandCount", 2), new SqlParameter("@CatCount", 2), new SqlParameter("@ProductCount", 15)).ToList();
                SearchLists.ForEach(x => x.PageAddress = CommonFunctions.NormalizeAddress(x.PageAddress));
                //log
                uow.SearchLogRepository.Insert(new SearchLog
                {
                    insertDate = DateTime.Now,
                    keyword = keyword.Trim(),
                    UserId = User.Identity.IsAuthenticated ? User.Identity.GetUserId() : null,
                    ClientIP = Request.UserHostAddress,
                    Browser = Request.Browser.Browser,
                    UserAgent = GetUserPlatform(Request)
                });
                uow.Save();

                return Json(new
                {
                    data = SearchLists,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                return Json(new
                {
                    data = "Exception Erorr",
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ContactUs(ContactUs ContactUs, int CaptchaValue, int LangId)
        {
            try
            {
                if (CaptchaValue == Convert.ToInt32(Session["captcha"]))
                {
                    zCaptcha c = new zCaptcha();
                    Session["captcha"] = c.GetCurrentValue();
                    ContactUs.InsertDate = DateTime.Now;
                    uow.ContactUsRepository.Insert(ContactUs);
                    uow.Save();


                    var content = uow.ContentRepository.GetByID(ContactUs.ContentId);
                    //Send Mail To Related Admin
                    #region Create Html Body
                    string EmailBodyHtml = "";

                    var oSetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == LangId, null, "attachment").FirstOrDefault();
                    string contentUrl = "http://" + HttpContext.Request.Url.Host;
                    if (!content.IsDefault)
                        contentUrl = "http://" + HttpContext.Request.Url.Host + "/content/" + content.Id + "/" + CommonFunctions.NormalizeAddress(content.Title);
                    CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(oSetting.attachment.FileName, "ثبت پیام تماس با مای جدید در سایت", "مدیر گرامی پیام تماس بامای  جدیدی توسط ، " + ContactUs.FullName + "، در سایت Submited . شما میتوانید با مراجعه به پنل مدیریت آن را ببینید.لینک صفحه مربوط به کامنت : <br/> <a href='" + contentUrl + "'>" + content.Title + "</a><br/>پیام : <br/><p>" + ContactUs.Message + "</p>  ", oSetting.WebSiteName, HttpContext.Request.Url.Host, oSetting.WebSiteTitle);
                    EmailBodyHtml = ahmadi.Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);
                    #endregion

                    #region SendMail
                    var emails = uow.AdministratorPermissionRepository.Get(x => x, x => x.ModuleId == 4 && x.NotificationEmail == true).Select(x => x.User.Email).Distinct();

                    EmailService es = new EmailService();
                    await es.SendMultiDestinationAsync(" ثبت پیام تماس با مای جدید در سایت ", EmailBodyHtml, emails.ToList());


                    #endregion

                    return Json(new
                    {
                        data = CaptureHelper.RenderViewToString("_Captcha", c, this.ControllerContext),
                        Message = LangId == 1 ? "ارسال شد" : "Sent",
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
                        Message =LangId==1? "پاسخ اشتباه":"Invalid Captcha",
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
                    Message = LangId == 1 ? "ارسال شد" : "Sent",
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
        }

        public string GenerateLink(int id, int? typeid, int? linkid, Guid? LinkUniqIdentifier, string title, string offlink, Menu ParrentMenu, string Url, int languageId)
        {
            string pre = "";
            if (languageId == 2)
                pre = "/En";

            XMLReader readXML = new XMLReader(Url);
            string FinallLink = "";
            if (typeid.HasValue)
            {
                switch (typeid.Value)
                {
                    case 1:
                    case 11:
                        var content = uow.ContentRepository.GetByID(linkid);
                        FinallLink = pre + (content != null ? readXML.DetailOfXContentType(content.ContentTypeId).IsVideo ? "Video" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.ContentRepository.GetByID(linkid).PageAddress) : content.IsSuperDeal ? "/SuperDeal" : "/content/" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.ContentRepository.GetByID(linkid).PageAddress) : "#"); break;
                    case 2: FinallLink = pre + (uow.AttachmentRepository.Any(LinkUniqIdentifier) ? "/Content/UploadFiles/" + uow.AttachmentRepository.GetByID(LinkUniqIdentifier).FileName : "#"); break;
                    case 3:
                        var category = uow.CategoryRepository.GetByID(linkid);
                        FinallLink = pre + (category != null ? readXML.DetailOfXContentType(category.ContentTypeId.Value).IsVideo ? "category" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.CategoryRepository.GetByID(linkid).PageAddress) : "/category/" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.CategoryRepository.GetByID(linkid).PageAddress) : "#"); break;
                    case 4: FinallLink = pre + (uow.TagRepository.Any(linkid) ? "/tag/" + linkid + "/" + CommonFunctions.NormalizeAddress(uow.TagRepository.GetByID(linkid).TagName) : "#"); break;
                    //case 5: dm.Link = db.Sliders.Find(item.LinkId); break;
                    case 6: FinallLink = uow.SocialRepository.GetByID(linkid).Link; break;
                    case 7:
                        FinallLink = pre + (readXML.ListOfXContentType().Any(x => x.Id == linkid) ? (linkid == 0 || linkid == 9) ? "/pages" : (linkid == 1 || linkid == 8) ? "/blog" : (linkid == 2 || linkid == 6) ? "/services" : (linkid == 3 || linkid == 7) ? "/projects" : (linkid == 5 || linkid == 10) ? "/videos" : (linkid == 11 || linkid == 12) ? "/mservices" : (linkid == 11 || linkid == 12) ? "/team" : "#" : "#"); break;
                    case 8:
                        var cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == linkid, null, "ProductType").SingleOrDefault();
                        if (cat != null)
                        {
                            if (!cat.ParrentId.HasValue)
                            {
                                switch (cat.ProductType.DataType)
                                {
                                    case 1:
                                    case 2: FinallLink = pre + "/TFC/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress); break;
                                    case 3: FinallLink = pre + "/FileCategory/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress); break;
                                    case 4: FinallLink = pre + "/CourseCategory/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress); break;
                                    case 5: FinallLink = pre + "/TourCategory/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress); break;
                                    default: FinallLink = pre + "/TFC/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress); break;
                                }
                            }
                            else
                            {
                                switch (cat.ProductType.DataType)
                                {
                                    case 1:
                                    case 2: FinallLink = pre + "/ProductSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2); break;
                                    case 3: FinallLink = pre + "/FileSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2); break;
                                    case 4: FinallLink = pre + "/CourseSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2); break;
                                    case 5: FinallLink = pre + "/TourSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2); break;
                                    default: FinallLink = pre + "/ProductSearch/" + linkid + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2); break;
                                }
                            }
                        }
                        else
                            FinallLink = "#";
                        break;
                    case 9: FinallLink = pre + "/Galleries"; break;
                    case 10:
                        if (ParrentMenu != null)
                        {
                            cat = uow.ProductCategoryRepository.Get(x => x, x => x.Id == ParrentMenu.LinkId, null, "ProductType").SingleOrDefault();
                            if (cat != null)
                            {
                                switch (cat.ProductType.DataType)
                                {
                                    case 1:
                                    case 2: FinallLink = pre + "/ProductSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2) + "/List-" + linkid; break;
                                    case 3: FinallLink = pre + "/FileSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2) + "/List-" + linkid; break;
                                    case 4: FinallLink = pre + "/CourseSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2) + "/List-" + linkid; break;
                                    case 5: FinallLink = pre + "/TourSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2) + "/List-" + linkid; break;
                                    default: FinallLink = pre + "/ProductSearch/" + ParrentMenu.LinkId + "/" + CommonFunctions.NormalizeAddress(cat.PageAddress2) + "/List-" + linkid; break;
                                }
                            }
                            else
                                FinallLink = "#";
                        }
                        else
                            FinallLink = "#";
                        break;
                    case 12:
                        FinallLink = pre + "/Page/" + id + "/" + CommonFunctions.NormalizeAddress(title); break;
                }
            }
            else
                FinallLink = offlink;

            return FinallLink;
        }


        public String GetUserPlatform(HttpRequestBase request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }

        public String GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }

        //[OutputCache(Duration = 3600, VaryByParam = "f")]
        //public void Thumbnail(string f, int w, int h, int q)
        //{
        //    WebImage img = new WebImage(Server.MapPath("~/Content/UploadFiles/" + f));
        //    img.Resize(w, h);
        //    img.Write();
        //}

        //[OutputCache(Duration = 3600, VaryByParam = "f")]
        //public ActionResult Thumbnail(string f, int? w, int? h, int? q)
        //{
        //    using (Image image = Image.FromFile(Server.MapPath("~/Content/UploadFiles/"+f)))
        //    {
        //        return File(image.BestFit(w, h).ConvertToByteArray(), ".jpg");
        //    }
        //}


    }



}