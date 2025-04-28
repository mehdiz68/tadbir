using CoreLib.ViewModel.Xml;
using Domain;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UnitOfWork;

namespace ahmadi.Controllers
{
    public class SearchResultController : BaseController
    {
        private UnitOfWorkClass uow = null;
        // GET: SearchResult
        public ActionResult Index(string title, int? id, int? page)
        {
            if (title == null || !id.HasValue)
            {
                return Redirect("~/");
            }
            uow = new UnitOfWorkClass();
            #region Get Setting

            var setting = GetSetting();

            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = "صفحه نتایج جستجو - " + title;
            oMeta.WebSiteMetakeyword = "صفحه نتایج جستجو - " + title;
            oMeta.WebSiteTitle = "صفحه نتایج جستجو - " + title;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Logo = setting.attachmentFileName;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", ""));
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            ViewBag.Meta = oMeta;
            #endregion


            int pageSize = 12;

            ViewBag.PageSize = pageSize;
            int pageNumber = (page ?? 1);
            ViewBag.keyword = title;

            if (id == -3)
                return View("ProductCategoryIndex", uow.ProductCategoryRepository.Get(x => x, x => x.IsActive && x.LanguageId == setting.LanguageId && (x.PageAddress.Contains(title) || x.Title.Contains(title) || x.Abstract.Contains(title)), x => x.OrderByDescending(s => s.Id), "attachment").ToPagedList(pageNumber, pageSize));
            else if (id == -2)
            {
                var prs = uow.ProductRepository.ProductItemList(x => x.IsActive && x.LanguageId == 1 && (x.Title.Contains(title) || x.Name.Contains(title) || x.LatinName.Contains(title) || x.Code.Contains(title) || x.ProductPrices.Any(s => s.code.Contains(title)) || x.Abstract.Contains(title)), x => x.OrderByDescending(o => o.Id)).ToPagedList(pageNumber, pageSize);
                return View("ProductIndex", prs);
            }
            else if (id == 0)
                return View("CategoryIndex", uow.CategoryRepository.GetByReturnQueryable(x => x, x => x.IsActive && x.LanguageId == 1 && (x.Title.Contains(title) || x.Abstract.Contains(title)), o => o.OrderByDescending(s => s.Id), "attachment").ToPagedList(pageNumber, pageSize));
            else if (id > 0)
            {
                ViewBag.ContentTypeId = id.Value;
                return View("ContentIndex", uow.ContentRepository.GetByReturnQueryable(x => x, x => x.IsActive && x.LanguageId == 1 && (x.Title.Contains(title) || x.Abstract.Contains(title)), o => o.OrderByDescending(s => s.Id), "attachment").ToPagedList(pageNumber, pageSize));
            }
            else
                return Redirect("~/");
        }
    }
}