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
using UnitOfWork;

namespace ahmadi.Controllers
{
    public class categoryController : BaseController
    {
        UnitOfWorkClass uow = null;
        public categoryController()
        {

            uow = new UnitOfWorkClass();
        }
        // GET: category
        public ActionResult Index(int? id, string title, int? page, int? langid)
        {

            var setting = GetSetting(langid);

            langid = langid.HasValue ? langid.Value : 1;
            ViewBag.langId = langid;


            if (id.HasValue)
            {
                var category = uow.CategoryRepository.Get(x => x, x => x.IsActive && x.LanguageId == langid && x.Id == id, null, "ChildCategory.Contents,Contents,attachment,Contents.Blogattachment").FirstOrDefault();
                if (category != null)
                {
                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    int contentTypeId = category.ContentTypeId.Value;
                    var ContentType = readXML.DetailOfXContentType(category.ContentTypeId.Value);
                    if (ContentType.IsVideo)
                        return RedirectPermanent(string.Format("~/VideoCategory/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(category.PageAddress)));

                    int pageSize = 12;
                    int pageNumber = (page ?? 1);

                    List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetContentSubCats @CatId", new SqlParameter("@CatId", category.Id)).ToList();
                    ViewBag.LatestContent = uow.ContentRepository.GetQueryList().AsNoTracking().Include("Blogattachment").Include("attachment").Include("User").Where(x => x.LanguageId==langid && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false && CatIds.Contains(x.CatId.Value)).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);

                    ViewBag.ContentType = ContentType;

                    ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.LanguageId == langid && x.IsActive && x.TypeId == 2 && x.LinkId == id, null, "SliderImages.attachment");

                    var date = DateTime.Now.Date;
                    ViewBag.Ads = uow.AdverestingRepository.Get(x => x, x => x.LanguageId == langid && x.TypeId == 2 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");

                    ViewBag.setting = setting;


                    #region Get Setting & meta

                    string pageAdditionalText = "";
                    if (langid == 1)
                        pageAdditionalText = (page > 0 ? " صفحه " + page : "");
                    else
                        pageAdditionalText = (page > 0 ? " page " + page : "");

                    string pre = langid.HasValue ? "/En" : "";


                    ahmadi.ViewModels.Home.Meta oMeta = new ahmadi.ViewModels.Home.Meta();
                    if (category.attachment != null)
                        oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + category.attachment.FileName;
                    else
                        oMeta.PageCover = setting.StaticContentDomain + "/images/default-thumbnail.jpg";
                    oMeta.WebSiteName = setting.WebSiteName;
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.WebSiteMetaDescription = category.Descr + pageAdditionalText;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.WebSiteTitle = category.Title + pageAdditionalText;
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.StaticContentUrl = setting.StaticContentDomain;
                    if (page > 0)
                        oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/category/" + category.Id + "/" + CommonFunctions.NormalizeAddress(category.PageAddress) + "?page=" + page);
                    else
                        oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/category/" + category.Id + "/" + CommonFunctions.NormalizeAddress(category.PageAddress));

                    ViewBag.Meta = oMeta;
                    #endregion


                    if (category.ContentTypeId == 0)
                        return View("Pages", category);
                    else if (category.ContentTypeId == 9)
                        return View("Pages_en", category);
                    else if (category.ContentTypeId == 1)
                        return View("Blogs", category);
                    else if (category.ContentTypeId == 8)
                        return View("Blogs_en", category);
                    else if (category.ContentTypeId == 2)
                        return View("Services", category);
                    else if (category.ContentTypeId == 6)
                        return View("Services_en", category);
                    else if (category.ContentTypeId == 3)
                        return View("Projects", category);
                    else if (category.ContentTypeId == 7)
                        return View("Projects_en", category);
                    else if (category.ContentTypeId == 11)
                        return View("mservices", category);
                    else if (category.ContentTypeId == 12)
                        return View("mservices_en", category);
                    else if (category.ContentTypeId == 5)
                        return View("Videos", category);
                    else if (category.ContentTypeId == 10)
                        return View("Videos_en", category);
                    else if (category.ContentTypeId == 13)
                        return View("Team", category);
                    else if (category.ContentTypeId == 14)
                        return View("Team_en", category);
                    else
                        return View("Services", category);

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
