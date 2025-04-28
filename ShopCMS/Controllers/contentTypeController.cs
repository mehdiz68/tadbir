using CoreLib.Infrastructure;
using CoreLib.ViewModel.Xml;
using Domain;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;

namespace ahmadi.Controllers
{
    public class contentTypeController : BaseController
    {
        UnitOfWork.UnitOfWorkClass uow = null;
        public contentTypeController()
        {

            uow = new UnitOfWork.UnitOfWorkClass();
        }
        // GET: contentType
        public ActionResult Index(int? id, string title, int? page, int? langid)
        {
            langid = langid.HasValue ? langid.Value : 1;
            ViewBag.langId = langid;

            if (Request.RawUrl.ToLower().Contains("contenttype"))
            {
                switch (id)
                {
                    case 0:
                    case 9:
                        return RedirectPermanent("~/pages");
                    case 1:
                    case 8:
                        return RedirectPermanent("~/blog");
                    case 2:
                    case 6:
                        return RedirectPermanent("~/services");
                    case 3:
                    case 7:
                        return RedirectPermanent("~/projects");
                    case 11:
                    case 12:
                        return RedirectPermanent("~/mservices");
                    case 5:
                    case 10:
                        return RedirectPermanent("~/videos");
                    case 13:
                    case 14:
                        return RedirectPermanent("~/Team");
                    default:
                        break;
                }
            }



            var setting = GetSetting(langid);

            if (id.HasValue)
            {

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var contentType = readXML.ListOfXContentType().Where(x => x.LanguageId == langid && x.Id == id).SingleOrDefault();
                if (contentType.IsVideo)
                    return RedirectPermanent(string.Format("~/videos"));

                if (contentType != null)
                {
                    ViewBag.page = page;
                    var date = DateTime.Now;
                    int pageSize = 12;
                    int pageNumber = (page ?? 1);

                    var ads = uow.AdverestingRepository.Get(x => x, x => x.LanguageId == langid && x.TypeId == 8 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");
                    ahmadi.ViewModels.Content.ContentType contentType1 = new ahmadi.ViewModels.Content.ContentType()
                    {
                        contentType = contentType,
                        Categories = uow.CategoryRepository.GetQueryList().Include("Contents").AsNoTracking().Where(x => x.ContentTypeId == contentType.Id && x.IsActive),
                        LatestContents = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.ContentTypeId == contentType.Id && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsRegister == false).Include("attachment").Include("Blogattachment").Include("Comments").OrderByDescending(s => s.Id).ToPagedList(pageNumber, pageSize),
                        LatestContentCount = uow.ContentRepository.GetQueryList().AsNoTracking().Count(x => x.ContentTypeId == contentType.Id && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsRegister == false),
                        Sliders = uow.SliderRepository.GetQueryList().AsNoTracking().Include("SliderImages").Include("SliderImages.attachment").Where(x => x.LanguageId == langid && x.IsActive && x.TypeId == 6 && x.LinkId == id).ToList(),
                        TopAdveresting = ads.Where(x => x.Position == 1),
                        RightAdveresting = ads.Where(x => x.Position == 2),
                        BottomAdveresting = ads.Where(x => x.Position == 3),
                        LeftAdveresting = ads.Where(x => x.Position == 4)
                    };

                    #region Get Setting & meta
                    string pageAdditionalText = "";
                    if (langid == 1)
                        pageAdditionalText = (page > 0 ? " صفحه " + page : "");
                    else
                        pageAdditionalText = (page > 0 ? " page " + page : "");
                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                    oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + setting.attachmentFileName;
                    oMeta.WebSiteMetaDescription = contentType.Abstract + pageAdditionalText;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.WebSiteTitle = contentType.Title + pageAdditionalText;
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.StaticContentUrl = setting.StaticContentDomain;
                    ViewBag.setting = setting;

                    string routname = "";
                    if (contentType.Id == 0)
                        routname = "/pages";
                    else if (contentType.Id == 9)
                        routname = "/En/pages";
                    if (contentType.Id == 1)
                        routname = "/blog";
                    else if (contentType.Id == 8)
                        routname = "/En/blog";
                    else if (contentType.Id == 2)
                        routname = "/services";
                    else if (contentType.Id == 6)
                        routname = "/En/services";
                    else if (contentType.Id == 3)
                        routname = "/projects";
                    else if (contentType.Id == 7)
                        routname = "/En/projects";
                    else if (contentType.Id == 11)
                        routname = "/mservices";
                    else if (contentType.Id == 12)
                        routname = "/En/mservices";
                    else if (contentType.Id == 5)
                        routname = "/videos";
                    else if (contentType.Id == 10)
                        routname = "/En/videos";
                    else if (contentType.Id == 13)
                        routname = "/team";
                    else if (contentType.Id == 14)
                        routname = "/En/team";


                    if (page > 0)
                        oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + routname + "?page=" + page);

                    oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + routname);


                    ViewBag.Meta = oMeta;
                    #endregion

                    if (contentType.Id == 0)
                        return View("Pages", contentType1);
                    else if (contentType.Id == 9)
                        return View("Pages_en", contentType1);
                    else if (contentType.Id == 1)
                        return View("Blogs", contentType1);
                    else if (contentType.Id == 8)
                        return View("Blogs_en", contentType1);
                    else if (contentType.Id == 2)
                        return View("Services", contentType1);
                    else if (contentType.Id == 6)
                        return View("Services_en", contentType1);
                    else if (contentType.Id == 3)
                        return View("Projects", contentType1);
                    else if (contentType.Id == 7)
                        return View("Projects_en", contentType1);
                    else if (contentType.Id == 11)
                        return View("mservices", contentType1);
                    else if (contentType.Id == 12)
                        return View("mservices_en", contentType1);
                    else if (contentType.Id == 5)
                        return View("Videos", contentType1);
                    else if (contentType.Id == 10)
                        return View("Videos_en", contentType1);
                    else if (contentType.Id == 13)
                        return View("Team", contentType1);
                    else if (contentType.Id == 14)
                        return View("Team_en", contentType1);
                    else
                        return View("Services", contentType1);

                }
                else
                    return Redirect("~/");
            }
            else
                return Redirect("~/");

        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
