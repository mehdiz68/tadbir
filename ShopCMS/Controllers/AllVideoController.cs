using CoreLib.Infrastructure;
using CoreLib.ViewModel.Xml;
using Domain;
using Domain.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace ahmadi.Controllers
{
    public class AllVideoController : BaseController
    {
        UnitOfWork.UnitOfWorkClass uow = null;
        public AllVideoController()
        {

            uow = new UnitOfWork.UnitOfWorkClass();
        }
        // GET: contentType
        public ActionResult Index(int? id, string key, int? langid)
        {

            langid = langid.HasValue ? langid.Value : 1;
            ViewBag.langId = langid;

            var setting = GetSetting(langid);


            if (id.HasValue)
            {
                XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                var contentType = readXML.ListOfXContentType().Where(x => x.LanguageId == langid && x.Id == id).SingleOrDefault();
                if (contentType != null)
                {
                    if (!contentType.IsVideo)
                    {
                        switch (contentType.Id)
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
                                return RedirectPermanent(string.Format("~/ContentType/{0}/{1}", id.Value, CommonFunctions.NormalizeAddress(contentType.Title))); break;
                        }
                    }
                    DateTime date = new DateTime();
                    Domain.ViewModels.Videos videos = new Domain.ViewModels.Videos();
                    videos.TopAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.TypeId == 8 && x.Position == 1 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null));
                    videos.Sliders = uow.SliderRepository.GetQueryList().AsNoTracking().Include("SliderImages").Include("SliderImages.attachment").Where(x => x.LanguageId == langid && x.IsActive && x.TypeId == 6 && x.LinkId == id);
                    var categories = uow.CategoryRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.ContentTypeId == contentType.Id && x.IsActive && x.ParrentId == null).Include("ChildCategory").OrderBy(s => s.Sort).Skip(() => 0).Take(() => 10).ToList();
                    videos.TopCatContentsByNew = new List<TopContentCat>();
                    videos.TopCatContentsByVisit = new List<TopContentCat>();
                    foreach (var item in categories)
                    {
                        List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetContentSubCats @CatId", new SqlParameter("@CatId", item.Id)).ToList();
                        videos.TopCatContentsByNew.Add(new Domain.ViewModels.TopContentCat()
                        {
                            Id = item.Id,
                            Name = item.Title,
                            PageAddress = item.PageAddress,
                            Quantity = uow.ContentRepository.Count(x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false),
                            TopContentList = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderByDescending(x => x.Id).Skip(() => 0).Take(() => 20).ToList()

                        });
                        videos.TopCatContentsByVisit.Add(new Domain.ViewModels.TopContentCat()
                        {
                            Id = item.Id,
                            Name = item.Title,
                            PageAddress = item.PageAddress,
                            Quantity = uow.ContentRepository.Count(x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false),
                            TopContentList = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && CatIds.Contains(x.CatId.Value) && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderByDescending(x => x.Visits).Skip(() => 0).Take(() => 20).ToList()
                        });
                    }
                    videos.RightAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.TypeId == 8 && x.Position == 2 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").ToList();
                    videos.News = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.ContentTypeId == 1 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderByDescending(x => x.Id).Skip(() => 0).Take(() => 3).ToList();
                    videos.Events = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.ContentTypeId == 6 && x.IsAbout == false && x.IsContact == false && x.IsActive == true && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderByDescending(x => x.Id).Skip(() => 0).Take(() => 9).ToList();
                    videos.LeftAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.TypeId == 8 && x.Position == 4 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").ToList();
                    videos.TopContentsByRandom1 = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.BlogMain == true && x.ContentTypeId == contentType.Id && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderByDescending(x => new Guid()).Skip(() => 0).Take(() => 9).ToList();
                    List<int> Random1 = new List<int>();
                    if (videos.TopContentsByRandom1.Any())
                        Random1 = videos.TopContentsByRandom1.Select(x => x.Id).ToList();
                    videos.TopContentsByRandom2 = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && !Random1.Contains(x.Id) && x.ContentTypeId == contentType.Id && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderBy(x => new Guid()).Skip(() => 0).Take(() => 8).ToList();
                    List<int> Random2 = new List<int>();
                    if (videos.TopContentsByRandom2.Any())
                        Random2 = videos.TopContentsByRandom2.Select(x => x.Id).ToList();
                    videos.TopContentsByRandom3 = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && !Random2.Contains(x.Id) && x.ContentTypeId == contentType.Id && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderBy(x => new Guid()).Skip(() => 0).Take(() => 8).ToList();
                    List<int> Random3 = new List<int>();
                    if (videos.TopContentsByRandom3.Any())
                        Random3 = videos.TopContentsByRandom3.Select(x => x.Id).ToList();
                    videos.TopContentsByRandom4 = uow.ContentRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && !Random3.Contains(x.Id) && x.ContentTypeId == contentType.Id && x.IsActive && x.IsAbout == false && x.IsContact == false && x.IsDefault == false && x.IsRegister == false).Include("attachment").Include("User").OrderBy(x => new Guid()).Skip(() => 0).Take(() => 8).ToList();
                    videos.BottomAdveresting = uow.AdverestingRepository.GetQueryList().AsNoTracking().Where(x => x.LanguageId == langid && x.TypeId == 8 && x.Position == 3 && (x.LinkId == id || x.LinkId == 0) && x.IsActive && ((x.ExpireDate != null && x.ExpireDate >= date) || x.ExpireDate == null) && ((x.StartDate != null && x.StartDate <= date) || x.StartDate == null)).Include("attachment").ToList();
                    videos.Name = contentType.Name;


                    #region Get Setting & meta
                    string pre = langid.HasValue ? "/En" : "";
                    ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
                    oMeta.PageCover = setting.StaticContentDomain + "/Uploadfiles/" + setting.attachmentFileName;
                    oMeta.WebSiteMetaDescription = contentType.Abstract;
                    oMeta.WebSiteMetakeyword = "";
                    oMeta.Favicon = setting.FaviconattachmentFileName;
                    oMeta.WebSiteTitle = contentType.Title;
                    oMeta.Logo = setting.attachmentFileName;
                    oMeta.StaticContentUrl = setting.StaticContentDomain;
                    ViewBag.setting = setting;

                    oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + pre + "/AllVideo/" + contentType.Id + "/" + CommonFunctions.NormalizeAddress(contentType.Title));


                    ViewBag.Meta = oMeta;
                    #endregion
                    if (langid == 1)
                        return View("Index", videos);
                    else
                        return View("Index_en", videos);

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
