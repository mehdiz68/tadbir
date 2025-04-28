using CoreLib.Infrastructure;
using CoreLib.ViewModel.Xml;
using Domain;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace ahmadi.Controllers
{
    public class contentTagController : BaseController
    {
        // GET: contentTag
        UnitOfWork.UnitOfWorkClass uow = null;
        public ActionResult Index(int? id, int? contentTypeId, int? page, int? langid)
        {
            uow = new UnitOfWork.UnitOfWorkClass();

            langid = langid.HasValue ? langid.Value : 1;
            ViewBag.langId = langid;

            var setting = GetSetting(langid);


            if (id.HasValue && contentTypeId.HasValue)
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var tag = uow.TagRepository.Get(x => x, x => x.LanguageId == langid && x.Id == id, null, "Content.Comments,Content.attachment,Content.User").SingleOrDefault();
                if (tag != null)
                {

                    int pageSize = 10;
                    int pageNumber = (page ?? 1);

                    ViewBag.LatestContent = tag.Content.Where(y => y.LanguageId == langid && y.ContentTypeId == contentTypeId && y.ContentTypeId == contentTypeId && y.IsAbout == false && y.IsContact == false && y.IsActive == true && y.IsDefault == false && y.IsRegister == false).OrderByDescending(x => x.Id).ToPagedList(pageNumber, pageSize);
                    ViewBag.contentTypeId = contentTypeId.Value;

                    ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.LanguageId == langid && x.IsActive && x.TypeId == 3 && x.LinkId == id, null, "SliderImages.attachment");

                    var date = DateTime.Now.Date;
                    ViewBag.Ads = uow.AdverestingRepository.Get(x => x, x => x.LanguageId == langid && x.TypeId == 3 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");


                    ViewBag.setting = setting;
                    #region Get Setting & meta


                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();

                    string pre = langid.HasValue ? "/En" : "";
                    string pageAdditionalText = (page > 0 ? " صفحه " + page : "");
                    oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + setting.attachmentFileName;
                    oMeta.WebSiteName = setting.WebSiteName;
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    var contentTypeName = readXML.DetailOfXContentType(contentTypeId.Value).Name;
                    oMeta.WebSiteMetaDescription = langid == 1 ? " محتواهای مرتبط با : " + tag.TagName + " در " + contentTypeName + pageAdditionalText : "related post about : " + tag.TagName + " in " + contentTypeName + pageAdditionalText;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.WebSiteTitle = tag.TagName + " در  " + contentTypeName + pageAdditionalText;
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.StaticContentUrl = setting.StaticContentDomain;
                    if (page > 0)
                        oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/contentTag/" + contentTypeId.Value + "/" + id.Value + "/" + CommonFunctions.NormalizeAddress(tag.TagName) + "?page=" + page);
                    else
                        oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/contentTag/" + contentTypeId.Value + "/" + id.Value + "/" + CommonFunctions.NormalizeAddress(tag.TagName));
                    oMeta.WebSiteName = setting.WebSiteName;
                    ViewBag.Meta = oMeta;
                    #endregion

                    if (langid == 1)
                        return View("Index", tag);
                    else
                        return View("Index_en", tag);

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
