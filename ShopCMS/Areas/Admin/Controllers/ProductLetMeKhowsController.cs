using CoreLib.Infrastructure.DateTime;
using CoreLib.ViewModel.Xml;
using Domain;
using Microsoft.AspNet.Identity;
using PagedList;
using ahmadi.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UnitOfWork;
using System.Data.SqlClient;
using CoreLib.Infrastructure;
using ahmadi.Areas.Admin.ViewModel.AdminPanel;
using System.Data.Entity.Core.Mapping;
using System.Linq.Expressions;

namespace ahmadi.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Support,SuperUser")]
    public class ProductLetMeKhowsController : Controller
    {
        private UnitOfWorkClass uow;
        public ProductLetMeKhowsController()
        {

            uow = new UnitOfWorkClass();
        }

        // GET: Admin/Products
        public ActionResult Index(int? PageSize, string sortOrder, string ProductCatId, string ProductCatIdFilter, string notified, string notifiedFilter, int? page)
        {

            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;
            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.AddPermission = p.First();
                    ViewBag.EditPermission = p.Skip(1).First();
                    ViewBag.DeletePermission = p.Skip(2).First();

                    #region Load DropDown List


                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);

                    List<SelectListItem> VisitedSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "ارسال نشده ها", Value = "false" }, new SelectListItem() { Text = "ارسال شده ها", Value = "true" } };
                    ViewBag.VisitedSelectListItem = VisitedSelectListItem;


                    #endregion

                    #region search
                    if (string.IsNullOrEmpty(ProductCatId))
                        ProductCatId = ProductCatIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(notified))
                        notified = notifiedFilter;
                    else
                        page = 1;

                    ViewBag.ProductCatIdFilter = ProductCatId;
                    ViewBag.notifiedFilter = notifiedFilter;



                    if (!String.IsNullOrEmpty(ProductCatId) && String.IsNullOrEmpty(notified))
                    {
                        int ctId = Convert.ToInt32(ProductCatId);
                        if (ctId > 0)
                        {
                            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ctId)).ToList();
                            var Products = uow.ProductRepository.ProductItemListWithCategories(x => x.ProductLetmeknows.Any() && x.ProductCategories.Any(s => CatIds.Contains(x.Id)));
                            #region Sort
                            switch (sortOrder)
                            {
                                case "count":
                                    Products = Products.OrderBy(s => s.Code);
                                    ViewBag.CurrentSort = "count";
                                    break;
                                case "count_desc":
                                    Products = Products.OrderByDescending(s => s.Code);
                                    ViewBag.CurrentSort = "count_desc";
                                    break;
                                case "Name":
                                    Products = Products.OrderBy(s => s.Name);
                                    ViewBag.CurrentSort = "Name";
                                    break;
                                case "Name_desc":
                                    Products = Products.OrderByDescending(s => s.Name);
                                    ViewBag.CurrentSort = "Name_desc";
                                    break;
                                default:  // Name ascending 
                                    Products = Products.OrderByDescending(s => s.LetmeKnows);
                                    break;
                            }

                            #endregion
                            int pageSize = 10;
                            if (PageSize.HasValue)
                            {
                                if (PageSize.Value > 100)
                                    pageSize = 100;
                                else if (pageSize < 10)
                                    pageSize = 10;
                                else
                                    pageSize = PageSize.Value;
                            }
                            ViewBag.PageSize = pageSize;
                            int pageNumber = (page ?? 1);

                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductLetMeKhows", "Index", true, 200, " نمایش صفحه مدیریت محصولات مورد علاقه", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View(Products.ToPagedList(pageNumber, pageSize));
                        }
                    }
                    else if (!String.IsNullOrEmpty(notified) && String.IsNullOrEmpty(ProductCatId))
                    {
                        bool ntf = Convert.ToBoolean(notified);
                        var Products = uow.ProductRepository.ProductItemListWithCategories(x => x.ProductLetmeknows.Any(a => a.Notofied == ntf));
                        #region Sort
                        switch (sortOrder)
                        {
                            case "count":
                                Products = Products.OrderBy(s => s.Code);
                                ViewBag.CurrentSort = "count";
                                break;
                            case "count_desc":
                                Products = Products.OrderByDescending(s => s.Code);
                                ViewBag.CurrentSort = "count_desc";
                                break;
                            case "Name":
                                Products = Products.OrderBy(s => s.Name);
                                ViewBag.CurrentSort = "Name";
                                break;
                            case "Name_desc":
                                Products = Products.OrderByDescending(s => s.Name);
                                ViewBag.CurrentSort = "Name_desc";
                                break;
                            default:  // Name ascending 
                                Products = Products.OrderByDescending(s => s.LetmeKnows);
                                break;
                        }

                        #endregion
                        int pageSize = 10;
                        if (PageSize.HasValue)
                        {
                            if (PageSize.Value > 100)
                                pageSize = 100;
                            else if (pageSize < 10)
                                pageSize = 10;
                            else
                                pageSize = PageSize.Value;
                        }
                        ViewBag.PageSize = pageSize;
                        int pageNumber = (page ?? 1);

                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductLetMeKhows", "Index", true, 200, " نمایش صفحه مدیریت محصولات مورد علاقه", DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return View(Products.ToPagedList(pageNumber, pageSize));
                    }
                    else if (!String.IsNullOrEmpty(notified) && !String.IsNullOrEmpty(ProductCatId))
                    {
                        int ctId = Convert.ToInt32(ProductCatId);
                        if (ctId > 0)
                        {
                            List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ctId)).ToList();
                            bool ntf = Convert.ToBoolean(notified);
                            var Products = uow.ProductRepository.ProductItemListWithCategories(x => x.ProductCategories.Any(s => CatIds.Contains(x.Id)) && x.ProductLetmeknows.Any(a => a.Notofied == ntf));
                            #region Sort
                            switch (sortOrder)
                            {
                                case "count":
                                    Products = Products.OrderBy(s => s.Code);
                                    ViewBag.CurrentSort = "count";
                                    break;
                                case "count_desc":
                                    Products = Products.OrderByDescending(s => s.Code);
                                    ViewBag.CurrentSort = "count_desc";
                                    break;
                                case "Name":
                                    Products = Products.OrderBy(s => s.Name);
                                    ViewBag.CurrentSort = "Name";
                                    break;
                                case "Name_desc":
                                    Products = Products.OrderByDescending(s => s.Name);
                                    ViewBag.CurrentSort = "Name_desc";
                                    break;
                                default:  // Name ascending 
                                    Products = Products.OrderByDescending(s => s.LetmeKnows);
                                    break;
                            }

                            #endregion
                            int pageSize = 10;
                            if (PageSize.HasValue)
                            {
                                if (PageSize.Value > 100)
                                    pageSize = 100;
                                else if (pageSize < 10)
                                    pageSize = 10;
                                else
                                    pageSize = PageSize.Value;
                            }
                            ViewBag.PageSize = pageSize;
                            int pageNumber = (page ?? 1);

                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductLetMeKhows", "Index", true, 200, " نمایش صفحه مدیریت محصولات مورد علاقه", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View(Products.ToPagedList(pageNumber, pageSize));
                        }
                        else
                        {
                            bool ntf = Convert.ToBoolean(notified);
                            var Products = uow.ProductRepository.ProductItemListWithCategories(x => x.ProductLetmeknows.Any(a => a.Notofied == ntf));
                            #region Sort
                            switch (sortOrder)
                            {
                                case "count":
                                    Products = Products.OrderBy(s => s.Code);
                                    ViewBag.CurrentSort = "count";
                                    break;
                                case "count_desc":
                                    Products = Products.OrderByDescending(s => s.Code);
                                    ViewBag.CurrentSort = "count_desc";
                                    break;
                                case "Name":
                                    Products = Products.OrderBy(s => s.Name);
                                    ViewBag.CurrentSort = "Name";
                                    break;
                                case "Name_desc":
                                    Products = Products.OrderByDescending(s => s.Name);
                                    ViewBag.CurrentSort = "Name_desc";
                                    break;
                                default:  // Name ascending 
                                    Products = Products.OrderByDescending(s => s.LetmeKnows);
                                    break;
                            }

                            #endregion
                            int pageSize = 10;
                            if (PageSize.HasValue)
                            {
                                if (PageSize.Value > 100)
                                    pageSize = 100;
                                else if (pageSize < 10)
                                    pageSize = 10;
                                else
                                    pageSize = PageSize.Value;
                            }
                            ViewBag.PageSize = pageSize;
                            int pageNumber = (page ?? 1);

                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductLetMeKhows", "Index", true, 200, " نمایش صفحه مدیریت محصولات مورد علاقه", DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return View(Products.ToPagedList(pageNumber, pageSize));
                        }
                    }

                    var Products2 = uow.ProductRepository.ProductItemListWithCategories(x => x.ProductLetmeknows.Any());

                    #region Sort
                    switch (sortOrder)
                    {
                        case "count":
                            Products2 = Products2.OrderBy(s => s.Code);
                            ViewBag.CurrentSort = "count";
                            break;
                        case "count_desc":
                            Products2 = Products2.OrderByDescending(s => s.Code);
                            ViewBag.CurrentSort = "count_desc";
                            break;
                        case "Name":
                            Products2 = Products2.OrderBy(s => s.Name);
                            ViewBag.CurrentSort = "Name";
                            break;
                        case "Name_desc":
                            Products2 = Products2.OrderByDescending(s => s.Name);
                            ViewBag.CurrentSort = "Name_desc";
                            break;
                        default:  // Name ascending 
                            Products2 = Products2.OrderByDescending(s => s.LetmeKnows);
                            break;
                    }

                    #endregion
                    int pageSize2 = 10;
                    if (PageSize.HasValue)
                    {
                        if (PageSize.Value > 100)
                            pageSize2 = 100;
                        else if (pageSize2 < 10)
                            pageSize2 = 10;
                        else
                            pageSize2 = PageSize.Value;
                    }
                    ViewBag.PageSize = pageSize2;
                    int pageNumber2 = (page ?? 1);

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductLetMeKhows", "Index", true, 200, " نمایش صفحه مدیریت محصولات مورد علاقه", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(Products2.ToPagedList(pageNumber2, pageSize2));


                    #endregion


                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "Index", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Details(int? id, int? page)
        {
            if (!id.HasValue)
                return RedirectToAction("Index");
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;

            var p = ModulePermission.check(User.Identity.GetUserId(), 22);
            if (p.Where(x => x == true).Any())
            {
                int pageNumber = (page ?? 1);
                var pf = uow.ProductLetmeknowRepository.Get(x => x, x => x.ProductId == id.Value, null, "Product").First();
                ViewBag.Title = pf.Product.Title;
                ViewBag.Id = pf.ProductId;
                return View(uow.ProductLetmeknowRepository.Get(x => x.User, x => x.ProductId == id.Value, null, "User.Avatarattachment").ToPagedList(pageNumber, 8));
            }
            else
                return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

        }
    }
}

