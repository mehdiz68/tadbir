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
    public class VideoCategoryController : BaseController
    {
        UnitOfWorkClass uow = null;
        // GET: category
        public ActionResult Index(int? id, int? page, string key, int? langid)
        {
            langid = langid.HasValue ? langid.Value : 1;
            ViewBag.langId = langid;
            uow = new UnitOfWork.UnitOfWorkClass();



            var setting = GetSetting(langid);


            if (id.HasValue)
            {
                var category = uow.CategoryRepository.Get(x => x, x => x.LanguageId == langid && x.IsActive && x.Id == id, null, "ChildCategory,Contents.Comments,attachment,Contents.Blogattachment").FirstOrDefault();
                ViewBag.category = category;

                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetContentSubCats @CatId", new SqlParameter("@CatId", category.Id)).ToList();

                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var contentType = readXML.ListOfXContentType().Where(x => x.LanguageId == langid && x.Id == category.ContentTypeId).SingleOrDefault();
                if (!contentType.IsVideo)
                    return RedirectPermanent(string.Format("~/Category/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(category.PageAddress)));

                if (contentType != null)
                {

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    if (String.IsNullOrEmpty(key))
                        ViewBag.LatestContent = uow.ContentRepository.GetQueryList().AsNoTracking().Include("Blogattachment").Include("attachment").Include("User").Include("Comments").Where(x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.ContentTypeId == contentType.Id && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);
                    else
                    {
                        ViewBag.LatestContent = uow.ContentRepository.GetQueryList().AsNoTracking().Include("Blogattachment").Include("attachment").Include("User").Include("Comments").Where(x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.ContentTypeId == contentType.Id && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false && (x.Title.Contains(key) || x.Descr.Contains(key) || x.Abstract.Contains(key))).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize).ToList();
                        ViewBag.key = key;
                    }

                    ViewBag.MostVisitContent = uow.ContentRepository.GetQueryList().AsNoTracking().Include("Blogattachment").Include("attachment").Include("User").Where(x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.ContentTypeId == contentType.Id && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false).OrderByDescending(x => x.Visits).Skip(() => 0).Take(() => 5).ToList();

                    ViewBag.Categories = uow.CategoryRepository.GetQueryList().AsNoTracking().Include("attachment").Where(x => x.LanguageId == langid && x.ParrentId == id.Value && x.ContentTypeId == contentType.Id && x.IsActive == true ).OrderBy(x => x.Sort).ToList();



                    ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.IsActive && x.TypeId == 2 && x.LinkId == id, null, "SliderImages.attachment");


                    ViewBag.setting = setting;

                    //Get Categories of Current ContentType
                    var date = DateTime.Now.Date;
                    ViewBag.Ads = uow.AdverestingRepository.GetQueryList().AsNoTracking().Include("attachment").Where(x => x.LanguageId == langid && x.TypeId == 2 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).ToList();

                    #region Bredcrumb
                    int Plusindex = 2, MinesIndex = 2;
                    string breadcrumbCat = "<li style='order:" + (Plusindex) + "' itemprop='itemListElement' class='breadcrumb-item'><a href='/videos'>" + contentType.Title + "</a><meta itemprop='position' content='" + (Plusindex) + "' /></li>";
                    Plusindex++;
                    MinesIndex--;
                    var currentCat = category;
                    while (currentCat.ParrentId.HasValue)
                    {
                        currentCat = currentCat.ParentCat;
                        breadcrumbCat += "<li style='order:" + MinesIndex + "'  itemprop='itemListElement' class='breadcrumb-item'><a href='" + Url.Action("Index", "VideoCategory", new { id = currentCat.Id, title = CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(currentCat.PageAddress) }) + "'>" + currentCat.Title + "</a><meta itemprop='position' content='" + Plusindex + "'></li>";
                        Plusindex++;
                        MinesIndex--;
                    }
                    breadcrumbCat += "<li style='order:" + Plusindex + "' itemprop='itemListElement' class='breadcrumb-item'><a href='" + Url.Action("Index", "VideoCategory", new { id = category.Id, title = CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(category.PageAddress) }) + "'>" + category.Title + "</a><meta itemprop='position' content='" + Plusindex + "'></li>";
                    ViewBag.breadcrumbCat = breadcrumbCat;
                    #endregion

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
                        oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/VideoCategory/" + category.Id + "/" + CommonFunctions.NormalizeAddress(category.PageAddress) + "?page=" + page);
                    else
                        oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/VideoCategory/" + category.Id + "/" + CommonFunctions.NormalizeAddress(category.PageAddress));

                    ViewBag.Meta = oMeta;
                    #endregion
                    if (langid == 1)
                        return View("Index", contentType);
                    else
                        return View("Index_en", contentType);

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
