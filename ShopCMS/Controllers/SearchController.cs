using CoreLib.Infrastructure;
using CoreLib.Infrastructure.ModelBinder;
using CoreLib.ViewModel.Xml;
using Domain;
using Domain.ViewModel;
using Domain.ViewModels;
using Fasterflect;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using UnitOfWork;

namespace ahmadi.Controllers
{
    public class SearchController : BaseController
    {
        private readonly UnitOfWorkClass uow;
        public SearchController()
        {
            uow = new UnitOfWorkClass();
        }

        [CorrectArabianLetter(new string[] { "q", })]
        public ActionResult Index(string q, int? page, int? perpage, int? sort)
        {
            #region Get Setting
            var setting = GetSetting();
            ViewModels.Home.Meta oMeta = new ViewModels.Home.Meta();
            oMeta.WebSiteMetaDescription = " جستجوی " + q;
            oMeta.WebSiteMetakeyword = "";
            oMeta.WebSiteTitle = q;
            oMeta.StaticContentUrl = setting.StaticContentDomain;
            oMeta.Logo = setting.attachmentFileName;
            oMeta.Favicon = setting.FaviconattachmentFileName;
            oMeta.CanocicalUrl = Url.Content((setting.HasHttps ? "https" : "http") + "://www." + HttpContext.Request.Url.Host.Replace("www.", "") + "/Search?q=" + q);
            oMeta.PageCover = HttpContext.Request.Url.Host + Url.Content("~/Content/Uploadfiles/" + setting.attachmentFileName);
            oMeta.WebSiteName = setting.WebSiteName;
            ViewBag.Meta = oMeta;
            #endregion


            #region Products
            var totlaFilters = new getUrlParameter();
            totlaFilters.pagenum = page == null ? 1 : page.Value;
            totlaFilters.perpage = perpage == null ? 15 : perpage.Value;
            totlaFilters.sortby = sort.HasValue ? sort.Value : 4;
            if (sort.HasValue)
                ViewBag.sort = sort.Value;
            totlaFilters.SearchStr = q;
            ViewBag.q = q;

            List<SearchResultSort> SearchResultSorts = new List<SearchResultSort>();
            List<ProductItem> sortedProductItems = new List<ProductItem>();
            var productSearchResults = uow.ProductAttributeSelectRepository.GetSearchResult(totlaFilters,null,true);
            // pageSetting
            var totalCount = productSearchResults.Count();
            ViewBag.TotalCount = totalCount;
            int skip = (totlaFilters.pagenum - 1) * totlaFilters.perpage;
            if (skip < 0)
                skip = 0;
            if (totlaFilters.perpage == 0)
            {
                totlaFilters.perpage = 1;
            }
            var paggingModel = new PagingViewModel
            {
                Count = totalCount,
                CurentPage = totlaFilters.pagenum,
                PerPage = totlaFilters.perpage,
                RawUrl = Request.RawUrl
            };
            TempData["PaggingModel"] = paggingModel;

            productSearchResults = productSearchResults.Skip(skip).Take(totlaFilters.perpage);
            SearchResultSorts = productSearchResults.ToList().Select(c => new SearchResultSort { Sort = c.Sort, ProductId = c.ProductId }).ToList();
            List<int> productids = SearchResultSorts.Select(s => s.ProductId).ToList();
            var productItems = uow.ProductRepository.ProductItemList(x => productids.Contains(x.Id)).ToList();
            foreach (var item in SearchResultSorts.OrderBy(s => s.Sort))
            {
                sortedProductItems.Add(productItems.Where(x => x.Id == item.ProductId).FirstOrDefault());
            }
            #endregion

            return View(sortedProductItems);
        }
    }

}