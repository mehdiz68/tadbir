using CoreLib.Infrastructure;
using CoreLib.ViewModel.Xml;
using Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ahmadi.Controllers
{
    public class tagController : BaseController
    {
        UnitOfWork.UnitOfWorkClass uow = null;

        // GET: tag
        public ActionResult Index(int? id, int? langid)
        {
            uow = new UnitOfWork.UnitOfWorkClass();

            langid = langid.HasValue ? langid.Value : 1;
            ViewBag.langId = langid;

            var setting = GetSetting(langid);


            if (id.HasValue)
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var tag = uow.TagRepository.Get(x => x, x => x.LanguageId == langid && x.Id == id, null, "Content.Comments,Content.attachment,Content.User").SingleOrDefault();
                if (tag != null)
                {

                    var contentTypes = readXML.ListOfXContentType().Where(x => x.LanguageId == langid).OrderBy(x => x.Name);
                    ViewBag.ContentTypes = contentTypes;
                    //calculate number of records
                    int numbers = contentTypes.Count() * 10;
                    ViewBag.LatestContent = tag.Content.Where(x => x.LanguageId == langid && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false).OrderByDescending(x => x.Id).Take(numbers);
                    ViewBag.MostVisitsContent = tag.Content.Where(x => x.LanguageId == langid && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false).OrderByDescending(x => x.Visits).Take(5);


                    ViewBag.Sliders = uow.SliderRepository.Get(x => x, x => x.LanguageId == langid && x.IsActive && x.TypeId == 3 && x.LinkId == id, null, "SliderImages.attachment");

                    var date = DateTime.Now.Date;
                    ViewBag.Ads = uow.AdverestingRepository.Get(x => x, x => x.LanguageId == langid && x.TypeId == 3 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null), null, "attachment");


                    ViewBag.setting = setting;
                    #region Get Setting & meta


                    string pre = langid.HasValue ? "/En" : "";
                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                    oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + setting.attachmentFileName;
                    oMeta.WebSiteName = setting.WebSiteName;
                    oMeta.WebSiteMetaDescription = langid == 1 ? " محتواهای مرتبط با : " + tag.TagName : "related post about : " + tag.TagName;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.WebSiteTitle = tag.TagName;
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.StaticContentUrl = setting.StaticContentDomain;
                    oMeta.CanocicalUrl = (setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + Url.Action("Index", "tag", new { id = tag.Id, title = CommonFunctions.NormalizeAddress(tag.TagName) });

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
