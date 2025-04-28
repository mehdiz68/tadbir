using CoreLib.Infrastructure.DateTime;
using CoreLib.ViewModel.Xml;
using Domain;
using Microsoft.AspNet.Identity;
using ahmadi.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UnitOfWork;
using System.Data.SqlClient;
using ahmadi.Areas.Admin.ViewModels.Product;
using CoreLib.Infrastructure.ModelBinder;
using System.Data.Entity;
using PagedList;
using CoreLib.Infrastructure;
using Fasterflect;
using System.Threading.Tasks;
using ahmadi.Controllers;
using OfficeOpenXml;
using System.IO;

namespace ahmadi.Areas.Admin.Controllers
{
    //[Authorize(Roles = "Admin,Support,SuperUser")]
    public class ProductPriceController : BaseController
    {
        private UnitOfWorkClass uow;

        private IQueryable<ProductPrice> search(string sortOrder, string Quantity, string QuantityFilter, string ProductCatId, string ProductCatIdFilter, string ProductTypeId, string ProductTypeFilter, string LanguagenameString, string LanguagenameFilter, string NameString, string NameFilter, string IsActive, string IsActiveFilter, string ProductStateId, string ProductStateFilter, string StateId, string StateFilter, string ProductInsertStateId, string ProductInsertStateFilter, int langid)
        {
            #region search





            var ProductPrices = uow.ProductPriceRepository.GetQueryList().AsNoTracking().Include("ProductImages").Include("ProductImages.Image").Include("Product.User").Include("Product").Include("Product.ProductImages.Image").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectSize").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectweight").Include("Product.ProductCategories").Include("ProductState");

            //var ProductPrices = uow.ProductPriceRepository.GetByReturnQueryable(x => new ProductPriceViewModel()
            //{
            //    Id = x.Id,
            //    ProductId = x.ProductId,
            //    ProductImages = x.ProductImages,
            //    LatinName = x.Product.LatinName,
            //    Name = x.Product.Name,
            //    Code = x.code,
            //    ProductCode = x.Product.Code,
            //    IsActive = x.IsActive,
            //    IsDefault = x.IsDefault,
            //    ProductAttributeSelectColor = x.ProductAttributeSelectColor,
            //    ProductAttributeSelectColorId = x.ProductAttributeSelectColorId,
            //    ProductAttributeSelectGaranty = x.ProductAttributeSelectGaranty,
            //    ProductAttributeSelectGarantyId = x.ProductAttributeSelectGarantyId,
            //    ProductAttributeSelectModel = x.ProductAttributeSelectModel,
            //    ProductAttributeSelectModelId = x.ProductAttributeSelectModelId,
            //    ProductAttributeSelectSize = x.ProductAttributeSelectSize,
            //    ProductAttributeSelectSizeId = x.ProductAttributeSelectSizeId,
            //    ProductAttributeSelectWeight = x.ProductAttributeSelectweight,
            //    ProductAttributeSelectWeightId = x.ProductAttributeSelectWeightId,
            //    LanguageId = x.Product.LanguageId.Value,
            //    Quantity = x.Quantity,
            //    DeliveryTimeout = x.DeliveryTimeout,
            //    MaxBasketCount = x.MaxBasketCount,
            //    Price = x.Price,
            //    state = x.Product.state,
            //    ProductStateId = x.ProductStateId,
            //    ProductTypeId = x.Product.ProductTypeId,
            //    ProductStateTitle = x.ProductStateId != null ? x.ProductState.Title : "نامشخص",
            //    ProductCategories = x.Product.ProductCategories
            //},
            //null, x => x.OrderByDescending(s => s.Id),"",pageNumber,pageSize);

            if (!String.IsNullOrEmpty(NameString))
            {
                var color = uow.ProductAttributeItemColorRepository.Get(x => x, x => x.Color.Contains(NameString)).FirstOrDefault();
                string colorId = "0";
                if (color != null)
                    colorId = color.Id.ToString();
                var Garanty = uow.ProductAttributeItemRepository.Get(x => x, x => x.Value.Contains(NameString)).FirstOrDefault();
                string GarantyId = "0";
                if (Garanty != null)
                    GarantyId = Garanty.Id.ToString();
                //ProductPrices = from s in ProductPrices
                //                where
                //                (s.Product.LatinName != null && s.Product.LatinName.ToLower().Contains(NameString)) ||
                //                (s.Product.Name != null && s.Product.Name.ToLower().Contains(NameString)) ||
                //                (s.Product.Code != null && s.Product.Code.ToLower().Contains(NameString)) ||
                //                (s.code != null && s.code.ToLower().Contains(NameString)) ||
                //                (s.Product.Title != null && s.Product.Title.ToLower().Contains(NameString)) 
                //                select s;
                ProductPrices = from s in ProductPrices
                                where
                                (s.Product.LatinName != null && s.Product.LatinName.ToLower().Contains(NameString)) ||
                                (s.Product.Name != null && s.Product.Name.ToLower().Contains(NameString)) ||
                                (s.Product.Code != null && s.Product.Code.ToLower().Contains(NameString)) ||
                                (s.code != null && s.code.ToLower().Contains(NameString)) ||
                                (s.ProductAttributeSelectModelId != null && s.ProductAttributeSelectModel.Value.ToLower().Contains(NameString)) ||
                                (s.ProductAttributeSelectSizeId != null && s.ProductAttributeSelectSize.Value.ToLower().Contains(NameString)) ||
                                (s.ProductAttributeSelectWeightId != null && s.ProductAttributeSelectweight.Value.ToLower().Contains(NameString)) ||
                                (s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor.Value.ToLower() == colorId) ||
                                (s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId)
                                select s;
            }

            if (!String.IsNullOrEmpty(LanguagenameString))
                ProductPrices = ProductPrices.Where(s => s.Product.LanguageId == langid);
            if (!String.IsNullOrEmpty(ProductCatId))
            {
                int ctId = Convert.ToInt32(ProductCatId);
                if (ctId > 0)
                {

                    List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", ctId)).ToList();
                    ProductPrices = ProductPrices.Where(s => s.Product.ProductCategories.Any(x => CatIds.Contains(x.Id)));
                }
            }
            if (!String.IsNullOrEmpty(IsActive))
            {
                bool isActive = Convert.ToBoolean(IsActive);
                ProductPrices = ProductPrices.Where(s => s.IsActive == isActive);
            }
            if (!String.IsNullOrEmpty(ProductTypeId))
            {
                int value = int.Parse(ProductTypeId);
                ProductPrices = ProductPrices.Where(s => s.Product.ProductTypeId == value);
            }
            if (!String.IsNullOrEmpty(ProductStateId))
            {
                int value = int.Parse(ProductStateId);
                ProductPrices = ProductPrices.Where(s => s.ProductStateId == value);
            }
            if (!String.IsNullOrEmpty(ProductInsertStateId))
            {
                int value = int.Parse(ProductInsertStateId);
                ProductPrices = ProductPrices.Where(s => s.Product.state == value);
            }

            #endregion

            #region Sort
            switch (sortOrder)
            {
                case "IsDefault":
                    ProductPrices = ProductPrices.OrderBy(s => s.IsDefault);
                    ViewBag.CurrentSort = "IsDefault";
                    break;
                case "IsDefault_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.IsDefault);
                    ViewBag.CurrentSort = "IsDefault_desc";
                    break;
                case "Quantity":
                    ProductPrices = ProductPrices.OrderBy(s => s.Quantity);
                    ViewBag.CurrentSort = "Quantity";
                    break;
                case "Quantity_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.Quantity);
                    ViewBag.CurrentSort = "Quantity_desc";
                    break;
                case "DeliveryTimeout":
                    ProductPrices = ProductPrices.OrderBy(s => s.DeliveryTimeout);
                    ViewBag.CurrentSort = "DeliveryTimeout";
                    break;
                case "DeliveryTimeout_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.DeliveryTimeout);
                    ViewBag.CurrentSort = "DeliveryTimeout_desc";
                    break;
                case "MaxBasketCount":
                    ProductPrices = ProductPrices.OrderBy(s => s.MaxBasketCount);
                    ViewBag.CurrentSort = "MaxBasketCount";
                    break;
                case "MaxBasketCount_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.MaxBasketCount);
                    ViewBag.CurrentSort = "MaxBasketCount_desc";
                    break;
                case "price":
                    ProductPrices = ProductPrices.OrderBy(s => s.Price);
                    ViewBag.CurrentSort = "price";
                    break;
                case "price_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.Price);
                    ViewBag.CurrentSort = "price_desc";
                    break;
                case "code":
                    ProductPrices = ProductPrices.OrderBy(s => s.code);
                    ViewBag.CurrentSort = "code";
                    break;
                case "code_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.code);
                    ViewBag.CurrentSort = "code_desc";
                    break;
                case "ProductState":
                    ProductPrices = ProductPrices.OrderBy(s => s.ProductState.Title);
                    ViewBag.CurrentSort = "ProductState";
                    break;
                case "ProductState_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.ProductState.Title);
                    ViewBag.CurrentSort = "ProductState_desc";
                    break;
                case "state":
                    ProductPrices = ProductPrices.OrderBy(s => s.Product.state);
                    ViewBag.CurrentSort = "state";
                    break;
                case "state_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.Product.state);
                    ViewBag.CurrentSort = "state_desc";
                    break;
                case "IsActive":
                    ProductPrices = ProductPrices.OrderBy(s => s.IsActive);
                    ViewBag.CurrentSort = "IsActive";
                    break;
                case "IsActive_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.IsActive);
                    ViewBag.CurrentSort = "IsActive_desc";
                    break;
                case "Language":
                    ProductPrices = ProductPrices.OrderBy(s => s.Product.LanguageId);
                    ViewBag.CurrentSort = "Language";
                    break;
                case "Language_desc":
                    ProductPrices = ProductPrices.OrderByDescending(s => s.Product.LanguageId);
                    ViewBag.CurrentSort = "Language_desc";
                    break;
                default:  // Name ascending 
                    ProductPrices = ProductPrices.OrderByDescending(s => s.Updatedate).ThenByDescending(x => x.Id);
                    break;
            }

            #endregion

            return ProductPrices;
        }

        [CorrectArabianLetter(new string[] { "NameString", "NameFilter" })]
        // GET: Admin/ProductPrice
        public ActionResult List(string answer, int? PageSize, string sortOrder, string Quantity, string QuantityFilter, string ProductCatId, string ProductCatIdFilter, string ProductTypeId, string ProductTypeFilter, string LanguagenameString, string LanguagenameFilter, string NameString, string NameFilter, string IsActive, string IsActiveFilter, string ProductStateId, string ProductStateFilter, string StateId, string StateFilter, string ProductInsertStateId, string ProductInsertStateFilter, int? page)
        {
            uow = new UnitOfWorkClass();
            var setting = uow.SettingRepository.Get(s => s, s => s.LanguageId == 1, null, "attachment,Faviconattachment").SingleOrDefault();
            ViewBag.setting = setting;


            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {


                    #region Load DropDown List
                    int langid = 1;
                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;
                    if (LanguagenameString != null)
                        langid = int.Parse(LanguagenameString);

                    List<SelectListItem> IsActiveSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "فعال", Value = "True" }, new SelectListItem() { Text = "غیرفعال", Value = "False" } };
                    ViewBag.IsActive = IsActiveSelectListItem;
                    List<SelectListItem> stateSelectListItem = new List<SelectListItem>() { new SelectListItem() { Text = "در حال بررسی", Value = "1" }, new SelectListItem() { Text = "بررسی مجدد", Value = "2" }, new SelectListItem() { Text = "عدم تایید", Value = "3" }, new SelectListItem() { Text = "تایید", Value = "4" } };
                    ViewBag.ProductInsertStateId = stateSelectListItem;

                    ViewBag.TreeProductCategories = uow.ProductCategoryRepository.Get(x => x, c => c.ParrentId == null && c.LanguageId == langid, x => x.OrderBy(s => s.Sort), "attachment,ParentCat");
                    ViewBag.ProductTypeId = new SelectList(uow.ProductTypeRepository.Get(x => x), "Id", "Title", 1);
                    ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title");
                    ViewBag.ProductStateList = uow.ProductStateRepository.Get(x => x);


                    XMLReader readXML = new XMLReader(setting.StaticContentDomain);
                    ViewBag.LanguagenameString = new SelectList(readXML.ListOfXLanguage(), "Id", "Name");

                    #endregion

                    if (string.IsNullOrEmpty(Quantity))
                        Quantity = QuantityFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(ProductCatId))
                        ProductCatId = ProductCatIdFilter;
                    else
                        page = 1;
                    if (string.IsNullOrEmpty(NameString))
                        NameString = NameFilter;
                    else
                    {
                        page = 1;
                        NameString = NameString.ToLower();
                    }
                    if (string.IsNullOrEmpty(IsActive))
                        IsActive = IsActiveFilter;
                    else
                        page = 1;

                    if (string.IsNullOrEmpty(ProductTypeId))
                        ProductTypeId = ProductTypeFilter;
                    else
                        page = 1;

                    if (string.IsNullOrEmpty(ProductStateId))
                        ProductStateId = ProductStateFilter;
                    else
                        page = 1;

                    if (string.IsNullOrEmpty(LanguagenameString))
                        LanguagenameString = LanguagenameFilter;
                    else
                        page = 1;


                    ViewBag.QuantityFilter = Quantity;
                    ViewBag.ProductCatIdFilter = ProductCatId;
                    ViewBag.NameFilter = NameString;
                    ViewBag.IsActiveFilter = IsActive;
                    ViewBag.ProductTypeFilter = ProductTypeId;
                    ViewBag.ProductStateFilter = ProductStateId;
                    ViewBag.LanguagenameFilter = LanguagenameString;

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

                    var ProductPrices = search(sortOrder, Quantity, QuantityFilter, ProductCatId, ProductCatIdFilter, ProductTypeId, ProductTypeFilter, LanguagenameString, LanguagenameFilter, NameString, NameFilter, IsActive, IsActiveFilter, ProductStateId, ProductStateFilter, StateId, StateFilter, ProductInsertStateId, ProductInsertStateFilter, langid);

                    if (answer == "جستجو" || answer == null)
                    {

                        ViewBag.HelpModule = uow.HelpModuleRepository.Get(x => x, x => x.Name == "مدیریت تنوع محصولات", null, "HelpModuleSections").FirstOrDefault();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "Index", true, 200, " نمایش صفحه مدیریت تنوع ", DateTime.Now, User.Identity.GetUserId());
                        #endregion

                        return View(ProductPrices.ToPagedList(pageNumber, pageSize));
                    }
                    else
                    {
                        var GroupTitle = "Price-filter-" + DateTime.Now.ToString("HH:mm:ss");
                        ExcelPackage excel = new ExcelPackage();
                        var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                        workSheet.Cells[1, 1].LoadFromCollection(ProductPrices.Select(x => new { x.code, title = x.Product.Name + (x.ProductAttributeSelectModelId.HasValue ? " مدل " + x.ProductAttributeSelectModel.Value : "") + (x.ProductAttributeSelectSizeId.HasValue ? " سایز " + x.ProductAttributeSelectSize.Value : "") + (x.ProductAttributeSelectColorId.HasValue ? " " + x.ProductAttributeSelectColor.Value : ""),price= x.Price * 10,x.Quantity }).ToList());
                        using (var memoryStream = new MemoryStream())
                        {
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;  filename=" + GroupTitle + ".xlsx");
                            excel.SaveAs(memoryStream);
                            memoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();

                            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                        }
                    }
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



        [HttpPost]
        public virtual async Task<JsonResult> UpdatePrPrice(int PrId, long Price, int MaxBasketCount, int DeliveryTimeout, int Quantity, bool IsActive, int ProductStateId, bool IsDefault)
        {
            try
            {
                uow = new UnitOfWorkClass();
                var pr = uow.ProductPriceRepository.Get(x => x, x => x.Id == PrId, null, "ProductImages,Product.ProductImages,ProductAttributeSelectModel,ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ProductAttributeSelectColor,ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ProductAttributeSelectGaranty,ProductAttributeSelectweight,Product.ProductCategories,ProductState").FirstOrDefault();
                int oldState = pr.ProductStateId.Value;
                pr.Price = Price;
                pr.MaxBasketCount = MaxBasketCount;
                pr.DeliveryTimeout = DeliveryTimeout;
                pr.Quantity = Quantity;
                pr.IsActive = IsActive;
                pr.ProductState = uow.ProductStateRepository.GetByID(ProductStateId);
                pr.ProductStateId = ProductStateId;

                if (!IsDefault)
                {
                    if (uow.ProductPriceRepository.Get(x => x, x => x.ProductId == pr.ProductId && x.IsDefault && x.Id != pr.Id).Any())
                        pr.IsDefault = IsDefault;
                    uow.ProductPriceRepository.Update(pr);
                    uow.Save();
                }
                else
                {
                    pr.IsDefault = IsDefault;

                    string title = pr.Product.Name;
                    string pageaddress = pr.Product.Name;
                    if (pr.ProductAttributeSelectModelId.HasValue)
                    {
                        title += " مدل " + pr.ProductAttributeSelectModel.Value;
                        pageaddress += " " + pr.ProductAttributeSelectModel.Value;
                    }
                    if (pr.ProductAttributeSelectSizeId.HasValue)
                        title += " سایز " + pr.ProductAttributeSelectSize.Value + pr.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                    if (pr.ProductAttributeSelectColorId.HasValue)
                        title += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(pr.ProductAttributeSelectColorId).Value)).Color;
                    var product = pr.Product;
                    product.PageAddress = CommonFunctions.NormalizeAddressWithSpace(pageaddress);
                    product.Title = title;
                    uow.ProductRepository.Update(product);
                    uow.Save();


                    var prall = uow.ProductPriceRepository.Get(x => x, x => x.ProductId == pr.ProductId && x.Id != pr.Id);
                    foreach (var item in prall)
                    {
                        item.IsDefault = false;
                    }
                    uow.Save();

                }

                await CheckProductState(pr, oldState);
                return Json(new
                {
                    ProductStateId = pr.ProductStateId,
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public virtual async Task<JsonResult> UpdateProductoffer(int offerId, int ProductOfferId, int ProductPriceId, int codeType, int valuee, int Quantity, int maxQuantity)
        {
            uow = new UnitOfWorkClass();
            try
            {
                if (Quantity > 0 && maxQuantity > 0 && codeType > 0 && valuee > 0)
                {
                    var productprice = uow.ProductPriceRepository.GetByID(ProductPriceId);
                    if (ProductOfferId > 0)
                    {
                        var productOffer = uow.ProductOfferRepository.GetByID(ProductOfferId);
                        productOffer.Quantity = productprice.Quantity < Quantity ? productprice.Quantity : Quantity;
                        productOffer.MaxBasketCount = productprice.Quantity < maxQuantity ? productprice.Quantity : maxQuantity;
                        productOffer.CodeType = Convert.ToInt16(codeType);
                        productOffer.Value = valuee;
                        uow.ProductOfferRepository.Update(productOffer);
                    }
                    else
                    {
                        ProductOffer productOffer = new ProductOffer()
                        {
                            CodeType = Convert.ToInt16(codeType),
                            MaxBasketCount = productprice.Quantity < maxQuantity ? productprice.Quantity : maxQuantity,
                            OfferId = offerId,
                            ProductPriceId = ProductPriceId,
                            Quantity = productprice.Quantity < Quantity ? productprice.Quantity : Quantity,
                            Value = valuee
                        };
                        uow.ProductOfferRepository.Insert(productOffer);
                    }
                    uow.Save();


                    CheckState(offerId);

                    return Json(new
                    {
                        message = "ثبت شد.",
                        statusCode = 200
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (offerId > 0 && (Quantity <= 0 || maxQuantity <= 0 || codeType == 0 || valuee <= 0))
                {
                    try
                    {
                        var productprice = uow.ProductPriceRepository.GetByID(ProductPriceId);
                        var productOffer = uow.ProductOfferRepository.GetByID(ProductOfferId);
                        productOffer.Quantity = 0;
                        productOffer.MaxBasketCount = 0;
                        productOffer.CodeType = Convert.ToInt16(codeType);
                        productOffer.Value = valuee;
                        uow.ProductOfferRepository.Update(productOffer);



                        //var productOffer = uow.ProductOfferRepository.GetByID(ProductOfferId);
                        //uow.ProductOfferRepository.Delete(productOffer);
                        //uow.Save();

                        CheckState(offerId);
                        return Json(new
                        {
                            message = "تخفیف حذف شد.",
                            statusCode = 200
                        }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception)
                    {
                        return Json(new
                        {
                            message = "تخفیف حذف نشد. این تخفیف در سفارش استفاده شده است.",
                            statusCode = 500
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new
                    {
                        message = "تخفیف ثبت نشد. تعداد، حداکثر تعداد، نوع تخفیف و میزان تخفیف را وارد نمایید",
                        statusCode = 500
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public virtual async Task<JsonResult> AddProductofferGroup(int Id, int OfferId, int codeType, int valuee)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var date = DateTime.Now;
                List<int> CatIds = uow.ContentRepository.SqlQuery("exec GetSubCats @CatId", new SqlParameter("@CatId", Id)).ToList();
                var productprices = uow.ProductPriceRepository.Get(x => new { x.Id, x.Quantity, x.MaxBasketCount }, x => (x.ProductStateId == 1 || x.ProductStateId == 2) && x.Product.ProductCategories.Any(s => CatIds.Contains(s.Id)));
                foreach (var item in productprices)
                {
                    if (!uow.ProductOfferRepository.Any(x => x.Id, x => x.Offer.IsActive && x.Offer.state && ((x.Offer.ExpireDate != null && x.Offer.ExpireDate >= date) || x.Offer.ExpireDate == null) && ((x.Offer.StartDate != null && x.Offer.StartDate <= date) || x.Offer.StartDate == null) && x.ProductPriceId == item.Id))
                    {
                        ProductOffer productOffer = new ProductOffer()
                        {
                            CodeType = Convert.ToInt16(codeType),
                            MaxBasketCount = item.MaxBasketCount,
                            OfferId = OfferId,
                            ProductPriceId = item.Id,
                            Quantity = item.Quantity,
                            Value = valuee
                        };
                        uow.ProductOfferRepository.Insert(productOffer);
                    }
                }

                uow.Save();


                CheckState(OfferId);

                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public virtual async Task<JsonResult> AddProductofferBrand(int Id, int OfferId, int codeType, int valuee)
        {
            uow = new UnitOfWorkClass();
            try
            {

                var date = DateTime.Now;
                var productprices = uow.ProductPriceRepository.Get(x => new { x.Id, x.Quantity, x.MaxBasketCount }, x => (x.ProductStateId == 1 || x.ProductStateId == 2) && x.Product.BrandId == Id);
                foreach (var item in productprices)
                {
                    if (!uow.ProductOfferRepository.Any(x => x.Id, x => x.Offer.IsActive && x.Offer.state && ((x.Offer.ExpireDate != null && x.Offer.ExpireDate >= date) || x.Offer.ExpireDate == null) && ((x.Offer.StartDate != null && x.Offer.StartDate <= date) || x.Offer.StartDate == null) && x.ProductPriceId == item.Id))
                    {
                        ProductOffer productOffer = new ProductOffer()
                        {
                            CodeType = Convert.ToInt16(codeType),
                            MaxBasketCount = item.MaxBasketCount,
                            OfferId = OfferId,
                            ProductPriceId = item.Id,
                            Quantity = item.Quantity,
                            Value = valuee
                        };
                        uow.ProductOfferRepository.Insert(productOffer);
                    }
                }

                uow.Save();


                CheckState(OfferId);
                return Json(new
                {
                    statusCode = 200
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception x)
            {
                return Json(new
                {
                    message = x.Message,
                    statusCode = 500
                }, JsonRequestBehavior.AllowGet);
            }

        }


        private void CheckState(int offerId)
        {
            var offer = uow.OfferRepository.Get(x => x, x => x.Id == offerId, null, "ProductOffers,UserCodeGifts,FreeSendOffers").FirstOrDefault();

            if (offer.CodeTypeValueCode == 1 || offer.CodeTypeValueCode == 2)
            {
                if (offer.ProductOffers.Any() || offer.UserCodeGifts.Any())
                    offer.state = true;
                else
                    offer.state = false;
            }
            else
            {
                if (offer.FreeSendOffers.Any())
                    offer.state = true;
                else
                    offer.state = false;
            }
            uow.OfferRepository.Update(offer);
            uow.Save();
        }
        // GET: Admin/ProductPrice/1
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            uow = new UnitOfWorkClass();
            var product = uow.ProductRepository.GetByID(id);
            if (product == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.product = product;
                    ViewBag.ProductStateList = uow.ProductStateRepository.Get(x => x);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "Index", true, 200, " نمایش صفحه مدیریت تنوعِ " + product.Name, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.ProductPriceRepository.Get(x => x, x => x.ProductId == id.Value, x => x.OrderByDescending(o => o.Id), "ProductImages,ProductImages.Image,Product.ProductImages.Image,ProductAttributeSelectModel,ProductAttributeSelectSize,ProductAttributeSelectColor,ProductAttributeSelectGaranty,ProductAttributeSelectweight,Product.ProductCategories,ProductState"));
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


        // GET: Admin/ProductPrice/Create
        public virtual ActionResult Create(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {

                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    var product = uow.ProductRepository.Get(x => x, x => x.Id == id, null, "ProductPrices.ProductAttributeSelectGaranty,ProductPrices.ProductAttributeSelectColor,ProductPrices.ProductAttributeSelectSize,ProductPrices.ProductAttributeSelectModel,ProductPrices.ProductAttributeSelectweight,ProductCategories,ProductType,ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute,ProductImages.Image").FirstOrDefault();
                    if (product == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    ViewBag.product = product;
                    ViewBag.SellerList = new SelectList(uow.SellerRepository.Get(x => x, x => x.IsActive, null, "User"), "Id", "User.LastName");

                    var ProductAttributeSelects = product.ProductAttributeSelects;
                    if (ProductAttributeSelects != null)
                    {
                        if (ProductAttributeSelects.Any())
                        {
                            //Garanty
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15))
                            {
                                var garantyLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == id.Value && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                                var garantyList = uow.ProductAttributeItemRepository.Get(x => x, x => garantyLists.Contains(x.Id.ToString()));
                                ViewBag.GarantyList = new SelectList(garantyList, "Id", "Value");
                            }
                            //Color
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12))
                            {
                                var colorLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == id.Value && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                                var colorList = uow.ProductAttributeItemColorRepository.Get(x => x, x => colorLists.Contains(x.Id.ToString()));

                                ViewBag.ColorList = new SelectList(colorList, "Id", "Color");
                            }
                            //Size
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13))
                            {
                                ViewBag.SizeList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == id.Value && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13, x => x.OrderBy(s => s.Value)), "Id", "Value");
                            }
                            //Model
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14))
                            {
                                ViewBag.ModelList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == id.Value && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14, x => x.OrderBy(s => s.Value)), "Id", "Value");
                            }
                            //Weight
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16))
                            {
                                ViewBag.WeightList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == id.Value && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16, x => x.OrderBy(s => s.Value)), "Id", "Value");
                            }
                        }
                    }

                    ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title");
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "Create", true, 200, " نمایش صفحه ایجاد تنوع محصول", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "Create", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // PoST: Admin/ProductPrice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Create(ProductPrice ProductPrice, string[] janebi, string mainJanebi)
        {

            uow = new UnitOfWorkClass();
            var product = uow.ProductRepository.Get(x => x, x => x.Id == ProductPrice.ProductId, null, "ProductPrices.ProductAttributeSelectGaranty,ProductPrices.ProductAttributeSelectColor,ProductPrices.ProductAttributeSelectSize,ProductPrices.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ProductPrices.ProductAttributeSelectModel,ProductPrices.ProductAttributeSelectweight,ProductCategories,ProductType,ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute,ProductImages.Image").FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    if (ProductPrice.ProductAttributeSelectColorId.HasValue)
                        ProductPrice.ProductAttributeSelectColorId = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12 && x.Value == ProductPrice.ProductAttributeSelectColorId.ToString()).First().Id;

                    if (ProductPrice.ProductAttributeSelectGarantyId.HasValue)
                        ProductPrice.ProductAttributeSelectGarantyId = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15 && x.Value == ProductPrice.ProductAttributeSelectGarantyId.ToString()).First().Id;

                    if (!uow.ProductPriceRepository.Any(x => x, x => x.ProductAttributeSelectColorId == ProductPrice.ProductAttributeSelectColorId && x.ProductAttributeSelectModelId == ProductPrice.ProductAttributeSelectModelId && x.ProductAttributeSelectSizeId == ProductPrice.ProductAttributeSelectSizeId && x.ProductAttributeSelectWeightId == ProductPrice.ProductAttributeSelectWeightId && x.ProductAttributeSelectGarantyId == ProductPrice.ProductAttributeSelectGarantyId && x.SellerId == ProductPrice.SellerId && x.ProductId == ProductPrice.ProductId))
                    {
                        ProductPrice.InsertDate = DateTime.Now;
                        ProductPrice.Updatedate = DateTime.Now;
                        uow.ProductPriceRepository.Insert(ProductPrice);
                        uow.Save();

                        if (ProductPrice.IsDefault)
                        {
                            if (product.ProductPrices != null)
                            {
                                foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.ProductId == product.Id && x.Id != ProductPrice.Id))
                                {
                                    item.IsDefault = false;
                                    uow.ProductPriceRepository.Update(item);
                                    uow.Save();
                                }
                            }
                        }

                        #region images
                        var currentJanebi = uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == ProductPrice.Id);
                        if (janebi == null)
                            uow.ProductImageRepository.Delete(currentJanebi.ToList());
                        else
                        {
                            IEnumerable<string> AddedJanebi = janebi.Where(x => !currentJanebi.Select(s => s.AttachementId).ToList().Contains(new Guid(x)));
                            foreach (var item in currentJanebi)
                            {
                                item.DisplaySort = janebi.ToList().FindIndex(x => new Guid(x) == item.AttachementId);
                            }
                            int i = 0;
                            bool isdefaultset = false;
                            foreach (var item in AddedJanebi)
                            {
                                if (isdefaultset == false && item == mainJanebi)
                                {
                                    uow.ProductImageRepository.Insert(new ProductImage() { ProductId = ProductPrice.ProductId, ProductPrice = ProductPrice, ProductPriceId = ProductPrice.Id, AttachementId = new Guid(item), Data = product.Descr, IsImage = true, IsMain = true, DisplaySort = i });
                                    isdefaultset = true;
                                }
                                else
                                    uow.ProductImageRepository.Insert(new ProductImage() { ProductId = ProductPrice.ProductId, ProductPrice = ProductPrice, ProductPriceId = ProductPrice.Id, AttachementId = new Guid(item), Data = product.Descr, IsImage = true, IsMain = false, DisplaySort = i });
                                i++;
                            }
                            IEnumerable<ProductImage> NotExistJanebi = currentJanebi.Where(x => !janebi.Contains(x.AttachementId.ToString()));
                            uow.ProductImageRepository.Delete(NotExistJanebi.ToList());
                        }
                        uow.Save();
                        #endregion


                        if (ProductPrice.IsDefault)
                        {
                            string title = product.Name;
                            string pageaddress = product.Name;
                            if (ProductPrice.ProductAttributeSelectModelId.HasValue)
                            {
                                title += " مدل " + uow.ProductAttributeSelectRepository.GetByID(ProductPrice.ProductAttributeSelectModelId).Value;
                                pageaddress += " " + uow.ProductAttributeSelectRepository.GetByID(ProductPrice.ProductAttributeSelectModelId).Value;
                            }
                            if (ProductPrice.ProductAttributeSelectSizeId.HasValue)
                                title += " سایز " + uow.ProductAttributeSelectRepository.GetByID(ProductPrice.ProductAttributeSelectSizeId).Value + ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                            if (ProductPrice.ProductAttributeSelectColorId.HasValue)
                                title += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(ProductPrice.ProductAttributeSelectColorId).Value)).Color;
                            //ProductPrice.ProductAttributeSelectColor.ProductAttributeGroupSelect.ProductAttribute.ProductAttributeItemColors.Where(x => x.Id == int.Parse(uow.ProductAttributeSelectRepository.GetByID(ProductPrice.ProductAttributeSelectColorId).Value)).First().Color;
                            product.PageAddress = CommonFunctions.NormalizeAddressWithSpace(pageaddress);
                            product.Title = title;
                            uow.ProductRepository.Update(product);
                            uow.Save();
                        }
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductPrice", "Create", false, 200, " افزودن تنوع جدید برای محصول" + product.Name, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index", new { Id = product.Id });
                    }
                    else
                    {
                        ViewBag.Error = "تنوع وارد شده وجود دارد. حالت دیگری انتخاب نمایید !";
                    }
                }
                else
                {

                    ViewBag.Error = "خطایی رخ داد !";
                }
                ViewBag.product = product;
                ViewBag.SellerList = new MultiSelectList(uow.SellerRepository.Get(x => x, x => x.IsActive), "Id", "User.LastName", product.ProductSellers.Select(x => x.SellerId));

                var ProductAttributeSelects = product.ProductAttributeSelects;
                if (ProductAttributeSelects != null)
                {
                    if (ProductAttributeSelects.Any())
                    {
                        //Garanty
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15))
                        {
                            var garantyLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                            var garantyList = uow.ProductAttributeItemRepository.Get(x => x, x => garantyLists.Contains(x.Id.ToString()));
                            ViewBag.GarantyList = new SelectList(garantyList, "Id", "Value");
                        }
                        //Color
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12))
                        {
                            var colorLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                            var colorList = uow.ProductAttributeItemColorRepository.Get(x => x, x => colorLists.Contains(x.Id.ToString()));

                            ViewBag.ColorList = new SelectList(colorList, "Id", "Color");
                        }
                        //Size
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13))
                        {
                            ViewBag.SizeList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13, x => x.OrderBy(s => s.Value)), "Id", "Value");
                        }
                        //Model
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14))
                        {
                            ViewBag.ModelList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14, x => x.OrderBy(s => s.Value)), "Id", "Value");
                        }
                        //Weight
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16))
                        {
                            ViewBag.WeightList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16, x => x.OrderBy(s => s.Value)), "Id", "Value");
                        }
                    }
                }

                ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title", ProductPrice.ProductStateId);
                return View(ProductPrice);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "Create", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ProductPrice/Edit
        public virtual ActionResult Edit(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {

                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    var ProductPrice = uow.ProductPriceRepository.Get(x => x, x => x.Id == id, null, "ProductImages.Image").FirstOrDefault();
                    if (ProductPrice == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    var product = uow.ProductRepository.Get(x => x, x => x.Id == ProductPrice.ProductId, null, "ProductPrices.ProductAttributeSelectGaranty,ProductPrices.ProductAttributeSelectColor,ProductPrices.ProductAttributeSelectSize,ProductPrices.ProductAttributeSelectModel,ProductPrices.ProductAttributeSelectweight,ProductCategories,ProductType,ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute,ProductImages.Image").FirstOrDefault();
                    if (product == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);


                    ViewBag.product = product;
                    ViewBag.SellerList = new SelectList(uow.SellerRepository.Get(x => x, x => x.IsActive, null, "User"), "Id", "User.LastName", ProductPrice.SellerId);

                    var ProductAttributeSelects = product.ProductAttributeSelects;
                    if (ProductAttributeSelects != null)
                    {
                        if (ProductAttributeSelects.Any())
                        {
                            //Garanty
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15))
                            {
                                var garantyLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                                var garantyList = uow.ProductAttributeItemRepository.Get(x => x, x => garantyLists.Contains(x.Id.ToString()));
                                ViewBag.GarantyList = new SelectList(garantyList, "Id", "Value", ProductPrice.ProductAttributeSelectGaranty != null ? ProductPrice.ProductAttributeSelectGaranty.Value != null ? ProductPrice.ProductAttributeSelectGaranty.Value : "" : "");
                            }
                            //Color
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12))
                            {
                                var colorLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                                var colorList = uow.ProductAttributeItemColorRepository.Get(x => x, x => colorLists.Contains(x.Id.ToString()));

                                ViewBag.ColorList = new SelectList(colorList, "Id", "Color", ProductPrice.ProductAttributeSelectColor != null ? ProductPrice.ProductAttributeSelectColor.Value != null ? ProductPrice.ProductAttributeSelectColor.Value : "" : "");
                            }
                            //Size
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13))
                            {
                                ViewBag.SizeList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13, x => x.OrderBy(s => s.Value)), "Id", "Value", ProductPrice.ProductAttributeSelectSize != null ? ProductPrice.ProductAttributeSelectSize.Value != null ? ProductPrice.ProductAttributeSelectSize.Value : "" : "");
                            }
                            //Model
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14))
                            {
                                ViewBag.ModelList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14, x => x.OrderBy(s => s.Value)), "Id", "Value", ProductPrice.ProductAttributeSelectModel != null ? ProductPrice.ProductAttributeSelectModel.Value != null ? ProductPrice.ProductAttributeSelectModel.Value : "" : "");
                            }
                            //Weight
                            if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16))
                            {
                                ViewBag.WeightList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16, x => x.OrderBy(s => s.Value)), "Id", "Value", ProductPrice.ProductAttributeSelectweight != null ? ProductPrice.ProductAttributeSelectweight.Value != null ? ProductPrice.ProductAttributeSelectweight.Value : "" : "");
                            }
                        }
                    }

                    ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title", ProductPrice.ProductStateId);
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "Edit", true, 200, " نمایش صفحه ویرایش تتوع محصول", DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductPrice);

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "Edit", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // PoST: Admin/ProductPrice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult Edit(ProductPrice ProductPrice, int? ProductAttributeSelectColorIdEdit, int? ProductAttributeSelectGarantyIdEdit, string[] janebi, string mainJanebi)
        {

            uow = new UnitOfWorkClass();
            try
            {
                if (ModelState.IsValid)
                {
                    int oldState = ProductPrice.ProductStateId.Value;
                    if (ProductAttributeSelectColorIdEdit.HasValue)
                        ProductPrice.ProductAttributeSelectColorId = ProductAttributeSelectColorIdEdit = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12 && x.Value == ProductAttributeSelectColorIdEdit.ToString()).First().Id;

                    if (ProductAttributeSelectGarantyIdEdit.HasValue)
                        ProductPrice.ProductAttributeSelectGarantyId = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15 && x.Value == ProductAttributeSelectGarantyIdEdit.ToString()).First().Id;

                    if (!uow.ProductPriceRepository.Any(x => x, x => x.ProductAttributeSelectColorId == ProductPrice.ProductAttributeSelectColorId && x.ProductAttributeSelectModelId == ProductPrice.ProductAttributeSelectModelId && x.ProductAttributeSelectSizeId == ProductPrice.ProductAttributeSelectSizeId && x.ProductAttributeSelectWeightId == ProductPrice.ProductAttributeSelectWeightId && x.ProductAttributeSelectGarantyId == ProductPrice.ProductAttributeSelectGarantyId && x.SellerId == ProductPrice.SellerId && x.Id != ProductPrice.Id && x.ProductId == ProductPrice.ProductId))
                    {
                        ProductPrice.Updatedate = DateTime.Now;
                        uow.ProductPriceRepository.Update(ProductPrice);
                        uow.Save();

                        if (ProductPrice.IsDefault)
                        {

                            foreach (var item in uow.ProductPriceRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.Id != ProductPrice.Id))
                            {
                                item.IsDefault = false;
                                uow.ProductPriceRepository.Update(item);
                                uow.Save();
                            }

                        }
                        else
                        {

                            if (!uow.ProductPriceRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.IsDefault).Any())
                            {
                                var firstItem = uow.ProductPriceRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId).First();
                                firstItem.IsDefault = true;
                                uow.ProductPriceRepository.Update(firstItem);
                                uow.Save();
                            }

                        }

                        if (!ProductPrice.IsActive)
                        {

                            if (!uow.ProductPriceRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.IsActive).Any())
                            {
                                var firstItem = uow.ProductPriceRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId).First();
                                firstItem.IsActive = true;
                                uow.ProductPriceRepository.Update(firstItem);
                                uow.Save();
                            }

                        }

                        var product = uow.ProductRepository.GetByID(ProductPrice.ProductId);

                        #region images
                        var currentJanebi = uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == ProductPrice.Id);
                        if (janebi == null)
                            uow.ProductImageRepository.Delete(currentJanebi.ToList());
                        else
                        {
                            foreach (var item in currentJanebi)
                            {
                                item.DisplaySort = janebi.ToList().FindIndex(x => new Guid(x) == item.AttachementId);
                                if (item.AttachementId.ToString() == mainJanebi)
                                    item.IsMain = true;
                                else
                                    item.IsMain = false;
                                uow.ProductImageRepository.Update(item);
                            }
                            int i = 0;
                            bool isdefaultset = false;
                            IEnumerable<string> AddedJanebi = janebi.Where(x => !currentJanebi.Select(s => s.AttachementId).ToList().Contains(new Guid(x)));
                            foreach (var item in AddedJanebi)
                            {
                                if (isdefaultset == false && item == mainJanebi)
                                {
                                    uow.ProductImageRepository.Insert(new ProductImage() { ProductId = ProductPrice.ProductId, ProductPrice = ProductPrice, ProductPriceId = ProductPrice.Id, AttachementId = new Guid(item), Data = product.Descr, IsImage = true, IsMain = true, DisplaySort = i });
                                    isdefaultset = true;
                                }
                                else
                                    uow.ProductImageRepository.Insert(new ProductImage() { ProductId = ProductPrice.ProductId, ProductPrice = ProductPrice, ProductPriceId = ProductPrice.Id, AttachementId = new Guid(item), Data = product.Descr, IsImage = true, IsMain = false, DisplaySort = i });
                                i++;
                            }
                            uow.Save();


                            List<ProductImage> NotExistJanebi = currentJanebi.Where(x => !janebi.Contains(x.AttachementId.ToString())).ToList();
                            foreach (var item in NotExistJanebi)
                            {
                                uow.ProductImageRepository.Delete(item);
                            }

                            uow.Save();
                        }
                        #endregion

                        if (ProductPrice.IsDefault)
                        {
                            var prprice = uow.ProductPriceRepository.Get(x => x, x => x.Id == ProductPrice.Id, null, "ProductAttributeSelectModel,ProductAttributeSelectSize,ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ProductAttributeSelectColor").First();
                            string title = product.Name;
                            string pageaddress = product.Name;
                            if (prprice.ProductAttributeSelectModelId.HasValue)
                            {
                                title += " مدل " + prprice.ProductAttributeSelectModel.Value;
                                pageaddress += " " + prprice.ProductAttributeSelectModel.Value;
                            }
                            if (prprice.ProductAttributeSelectSizeId.HasValue)
                                title += " سایز " + prprice.ProductAttributeSelectSize.Value + prprice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                            if (prprice.ProductAttributeSelectColorId.HasValue)
                                title += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(ProductPrice.ProductAttributeSelectColorId).Value)).Color;
                            product.PageAddress = CommonFunctions.NormalizeAddressWithSpace(pageaddress);
                            product.Title = title;
                            uow.ProductRepository.Update(product);
                            uow.Save();
                        }

                        uow.Save();

                        CheckProductState(ProductPrice, oldState);
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductPrice", "Edit", false, 200, " ویرایش تنوع برای محصول" + product.Name, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Index", new { Id = product.Id });
                    }
                    else
                    {
                        ViewBag.Error = "تنوع وارد شده وجود دارد. حالت دیگری انتخاب نمایید !";
                    }
                }
                else
                {

                    ViewBag.Error = "خطایی رخ داد !";
                }

                var product2 = uow.ProductRepository.Get(x => x, x => x.Id == ProductPrice.ProductId, null, "ProductPrices.ProductAttributeSelectGaranty,ProductPrices.ProductAttributeSelectColor,ProductPrices.ProductAttributeSelectSize,ProductPrices.ProductAttributeSelectModel,ProductPrices.ProductAttributeSelectweight,ProductCategories,ProductType,ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute,ProductImages.Image").FirstOrDefault();

                ViewBag.product = product2;
                ViewBag.SellerList = new SelectList(uow.SellerRepository.Get(x => x, x => x.IsActive), "Id", "User.LastName", ProductPrice.SellerId);

                var ProductAttributeSelects = product2.ProductAttributeSelects;
                if (ProductAttributeSelects != null)
                {
                    if (ProductAttributeSelects.Any())
                    {
                        //Garanty
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15))
                        {
                            var garantyLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product2.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 15, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                            var garantyList = uow.ProductAttributeItemRepository.Get(x => x, x => garantyLists.Contains(x.Id.ToString()));
                            ViewBag.GarantyList = new SelectList(garantyList, "Id", "Value", ProductPrice.ProductAttributeSelectGaranty != null ? ProductPrice.ProductAttributeSelectGaranty.Value != null ? ProductPrice.ProductAttributeSelectGaranty.Value : "" : "");
                        }
                        //Color
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12))
                        {
                            var colorLists = uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product2.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 12, x => x.OrderBy(s => s.Value)).Select(x => x.Value).ToList();
                            var colorList = uow.ProductAttributeItemColorRepository.Get(x => x, x => colorLists.Contains(x.Id.ToString()));

                            ViewBag.ColorList = new SelectList(colorList, "Id", "Color", ProductPrice.ProductAttributeSelectColor != null ? ProductPrice.ProductAttributeSelectColor.Value != null ? ProductPrice.ProductAttributeSelectColor.Value : "" : "");
                        }
                        //Size
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13))
                        {
                            ViewBag.SizeList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product2.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 13, x => x.OrderBy(s => s.Value)), "Id", "Value", ProductPrice.ProductAttributeSelectSize != null ? ProductPrice.ProductAttributeSelectSize.Value != null ? ProductPrice.ProductAttributeSelectSize.Value : "" : "");
                        }
                        //Model
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14))
                        {
                            ViewBag.ModelList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product2.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 14, x => x.OrderBy(s => s.Value)), "Id", "Value", ProductPrice.ProductAttributeSelectModel != null ? ProductPrice.ProductAttributeSelectModel.Value != null ? ProductPrice.ProductAttributeSelectModel.Value : "" : "");
                        }
                        //Weight
                        if (ProductAttributeSelects.Any(x => x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16))
                        {
                            ViewBag.WeightList = new SelectList(uow.ProductAttributeSelectRepository.Get(x => x, x => x.ProductId == product2.Id && x.ProductAttributeGroupSelect.ProductAttribute.DataType == 16, x => x.OrderBy(s => s.Value)), "Id", "Value", ProductPrice.ProductAttributeSelectweight != null ? ProductPrice.ProductAttributeSelectweight.Value != null ? ProductPrice.ProductAttributeSelectweight.Value : "" : "");
                        }
                    }
                }

                ViewBag.ProductStateId = new SelectList(uow.ProductStateRepository.Get(x => x), "Id", "Title", ProductPrice.ProductStateId);
                return View(ProductPrice);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "Edit", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // GET: Admin/ProductPrice/Delete/5
        public virtual ActionResult Delete(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 3))
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    ProductPrice ProductPrice = uow.ProductPriceRepository.Get(x => x, x => x.Id == id, null, "OrderRows").SingleOrDefault();
                    if (ProductPrice == null)
                    {
                        return HttpNotFound();
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "Delete", true, 200, " نمایش صفحه حذف تنوعِ" + ProductPrice.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductPrice);
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "Delete", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                ProductPrice ProductPrice = uow.ProductPriceRepository.Get(x => x, x => x.Id == id, null, "OrderRows,Product").SingleOrDefault();
                CheckProductState(ProductPrice, ProductPrice.ProductStateId.Value);
                var Product = uow.ProductRepository.Get(x => x, x => x.Id == ProductPrice.ProductId, null, "ProductPrices").SingleOrDefault();
                if (!ProductPrice.OrderRows.Any())
                {
                    if (Product.ProductPrices.Where(x => x.IsDefault == true && x.IsActive == true && x.Id != id).Any())
                    {
                        if (Product.ProductPrices.Count >= 1)
                        {
                            int DefaultPriceId = Product.ProductPrices.Where(x => x.IsDefault == true && x.IsActive == true).First().Id;
                            foreach (var item in uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == ProductPrice.Id))
                            {
                                item.ProductPriceId = DefaultPriceId;
                                uow.ProductImageRepository.Update(item);
                            }
                            uow.Save();
                            int productid = ProductPrice.ProductId;
                            uow.ProductPriceRepository.Delete(ProductPrice);
                            uow.Save();
                            #region EventLogger
                            ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductPrice", "Delete", false, 200, "   حذف تنوع" + ProductPrice.Id, DateTime.Now, User.Identity.GetUserId());
                            #endregion
                            return RedirectToAction("Index", new { id = productid });
                        }
                        else
                        {
                            ViewBag.Error = "حداقل باید یک تنوع وجود داشته باشد !";
                            ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductPrice", "DeleteImage", false, 200, "   عدم تنوع " + ProductPrice.Id, DateTime.Now, User.Identity.GetUserId());
                            return View(ProductPrice);
                        }
                    }
                    else
                    {

                        ViewBag.Error = "حداقل باید یک تنوع پیش فرض وجود داشته باشد !";
                        ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductPrice", "DeleteImage", false, 200, "   عدم حذف تصویر" + ProductPrice.Id, DateTime.Now, User.Identity.GetUserId());
                        return View(ProductPrice);
                    }

                }
                ViewBag.Error = "به دلیل فروش این تنوع محصول و موجود بودن در سفارشات ، قادر به حذف این محصول نمی باشید";
                ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductPrice", "DeleteConfirmed", false, 200, "   عدم حذف تنوعِ" + ProductPrice.Id, DateTime.Now, User.Identity.GetUserId());
                return View(ProductPrice);

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ProductPrice/Images
        public ActionResult Images(int? id)
        {
            uow = new UnitOfWorkClass();
            var productPrice = uow.ProductPriceRepository.Get(x => x, x => x.Id == id, x => x.OrderBy(s => s.Id), "ProductImages.Image").Single();
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (productPrice == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.ProductPrice = productPrice;
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "Images", true, 200, " نمایش صفحه مدیریت تصاویر تنوعِ " + productPrice.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == id.Value, x => x.OrderByDescending(o => o.Id)));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "Images", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ProductPrice/SelectImage
        public ActionResult SelectImage(int? ProductId, int id)
        {
            uow = new UnitOfWorkClass();
            var product = uow.ProductRepository.GetByID(ProductId);
            if (ProductId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (product == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            try
            {

                var p = ModulePermission.check(User.Identity.GetUserId(), 22);
                if (p.Where(x => x == true).Any())
                {
                    ViewBag.product = product;
                    ViewBag.ProductPrice = uow.ProductPriceRepository.Get(x => x, x => x.Id == id).First();
                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "SelectImage", true, 200, " نمایش صفحه مدیریت انتخاب تصویر از تصاویر بلااستفاده محصولِ " + product.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(uow.ProductImageRepository.Get(x => x, x => x.ProductId == ProductId.Value && !x.ProductPriceId.HasValue, x => x.OrderByDescending(o => o.Id)));
                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "Products", "SelectImage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public ActionResult SelectImage(int ImageId, int ProductPriceId)
        {
            uow = new UnitOfWorkClass();
            try
            {
                var ProductImage = uow.ProductImageRepository.GetByID(ImageId);
                ProductImage.ProductPriceId = ProductPriceId;
                uow.ProductImageRepository.Update(ProductImage);
                uow.Save();
                if (ProductImage.IsMain)
                {
                    foreach (var item in uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == ProductPriceId && x.Id != ProductImage.Id))
                    {
                        item.IsMain = false;
                        uow.ProductImageRepository.Update(item);
                        uow.Save();
                    }
                }
                return Redirect("~/Admin/ProductPrice/SelectImage/?ProductId=" + ProductImage.ProductId + "&id=" + ProductPriceId);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "SelectImage", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }



        // GET: Admin/ProductPrice/CreateImage
        public virtual ActionResult CreateImage(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {

                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    var ProductPrice = uow.ProductPriceRepository.Get(x => x, x => x.Id == id, null, "Product").FirstOrDefault();
                    if (ProductPrice == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    ViewBag.product = ProductPrice.Product;
                    ViewBag.ProductPrice = ProductPrice;


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "CreateImage", true, 200, " نمایش صفحه ایجاد تصویر برای تنوعِ " + id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View();

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "CreateImage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        // PoST: Admin/ProductPrice/CreateImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult CreateImage([Bind(Include = "ProductId,AttachementId,Data,IsImage,IsMain,ProductPriceId")] ProductImage ProductImage)
        {
            uow = new UnitOfWorkClass();
            var product = uow.ProductRepository.Get(x => x, x => x.Id == ProductImage.ProductId).FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {

                    uow.ProductImageRepository.Insert(ProductImage);

                    if (product.ProductImages.Any())
                        ProductImage.DisplaySort = uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == ProductImage.ProductPriceId).Max(x => x.DisplaySort) + 1;

                    uow.Save();


                    if (ProductImage.IsMain)
                    {
                        if (product.ProductImages != null)
                        {
                            foreach (var item in uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == ProductImage.ProductPriceId && x.Id != ProductImage.Id))
                            {
                                item.IsMain = false;
                                uow.ProductImageRepository.Update(item);
                                uow.Save();
                            }
                        }
                    }

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductPrice", "CreateImage", false, 200, " افزودن تصویر جدید برای تنوعِ" + ProductImage.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Images", new { Id = ProductImage.ProductPriceId });

                }
                else
                {

                    ViewBag.Error = "خطایی رخ داد !";
                }

                ViewBag.product = ProductImage.Product;
                ViewBag.ProductPrice = uow.ProductPriceRepository.GetByID(ProductImage.ProductPriceId);



                return View(ProductImage);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "CreateImage", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ProductPrice/EditImage
        public virtual ActionResult EditImage(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {

                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {

                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    var ProductImage = uow.ProductImageRepository.Get(x => x, x => x.Id == id, null, "Product,ProductPrice,Image").FirstOrDefault();
                    if (ProductImage == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    ViewBag.product = ProductImage.Product;
                    ViewBag.ProductPrice = ProductImage.ProductPrice;


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "EditImage", true, 200, " نمایش صفحه ویرایش تصویر تنوعِ " + id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductImage);

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "EditImage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // PoST: Admin/ProductPrice/EditImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public virtual ActionResult EditImage([Bind(Include = "Id,ProductId,AttachementId,Data,IsImage,IsMain,ProductPriceId,DisplaySort")] ProductImage ProductImage)
        {
            uow = new UnitOfWorkClass();
            var product = uow.ProductRepository.Get(x => x, x => x.Id == ProductImage.ProductId).FirstOrDefault();
            try
            {
                if (ModelState.IsValid)
                {
                    uow.ProductImageRepository.Update(ProductImage);
                    uow.Save();

                    if (ProductImage.IsMain)
                    {
                        if (product.ProductImages != null)
                        {
                            foreach (var item in uow.ProductImageRepository.Get(x => x, x => x.ProductPriceId == ProductImage.ProductPriceId && x.Id != ProductImage.Id))
                            {
                                item.IsMain = false;
                                uow.ProductImageRepository.Update(item);
                                uow.Save();
                            }
                        }
                    }
                    //else
                    //{
                    //    if (product.ProductImages != null)
                    //    {
                    //        if (!product.ProductImages.Any(x => x.IsMain))
                    //        {
                    //            product.ProductImages.First().IsMain = true;
                    //            uow.ProductImageRepository.Update(product.ProductImages.First());
                    //            uow.Save();
                    //        }
                    //    }
                    //}

                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(2, "ProductPrice", "EditImage", false, 200, " ویرایش تصویر " + ProductImage.Id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return RedirectToAction("Images", new { Id = ProductImage.ProductPriceId });

                }
                else
                {

                    ViewBag.Error = "خطایی رخ داد !";
                }

                ViewBag.product = ProductImage.Product;
                ViewBag.ProductPrice = uow.ProductPriceRepository.GetByID(ProductImage.ProductPriceId);



                return View(ProductImage);

            }
            catch (Exception x)
            {

                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "EditImage", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // GET: Admin/ProductPrice/DeleteImage
        public virtual ActionResult DeleteImage(int? id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                if (ModulePermission.check(User.Identity.GetUserId(), 22, 1))
                {

                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    var ProductImage = uow.ProductImageRepository.Get(x => x, x => x.Id == id, null, "Product,ProductPrice,Image").FirstOrDefault();
                    if (ProductImage == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    ViewBag.product = ProductImage.Product;
                    ViewBag.ProductPrice = ProductImage.ProductPrice;


                    #region EventLogger
                    ahmadi.Infrastructure.EventLog.Logger.Add(1, "ProductPrice", "DeleteImage", true, 200, " نمایش صفحه حذف تصویر تنوعِ " + id, DateTime.Now, User.Identity.GetUserId());
                    #endregion
                    return View(ProductImage);

                }
                else
                    return RedirectToAction("Index", "AccessDenied", new System.Web.Routing.RouteValueDictionary(new { MouleName = "مدیریت محصولات" }));

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "DeleteImage", true, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }


        // POST: Admin/Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteImage(int id)
        {
            uow = new UnitOfWorkClass();
            try
            {
                ProductImage ProductImage = uow.ProductImageRepository.Get(x => x, x => x.Id == id, null, "Product,ProductPrice").SingleOrDefault();
                var Product = uow.ProductRepository.Get(x => x, x => x.Id == ProductImage.ProductId, null, "ProductImages").SingleOrDefault();
                if (Product.ProductImages.Where(x => x.IsMain == true && x.Id != id).Any())
                {
                    if (Product.ProductImages.Count >= 1)
                    {
                        int? ProductPriceId = ProductImage.ProductPriceId;
                        uow.ProductImageRepository.Delete(ProductImage);
                        uow.Save();
                        #region EventLogger
                        ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductPrice", "DeleteImage", false, 200, "   حذف تصویر" + ProductImage.Id, DateTime.Now, User.Identity.GetUserId());
                        #endregion
                        return RedirectToAction("Images", new { id = ProductPriceId });
                    }
                    else
                    {
                        ViewBag.product = ProductImage.Product;
                        ViewBag.ProductPrice = ProductImage.ProductPrice;
                        ViewBag.Error = "حداقل باید یک تصویر وجود داشته باشد !";
                        ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductPrice", "DeleteImage", false, 200, "   عدم حذف تصویر" + ProductImage.Id, DateTime.Now, User.Identity.GetUserId());
                        return View(ProductImage);
                    }
                }
                else
                {

                    ViewBag.product = ProductImage.Product;
                    ViewBag.ProductPrice = ProductImage.ProductPrice;
                    ViewBag.Error = "حداقل باید یک تصویر پیش فرض وجود داشته باشد !";
                    ahmadi.Infrastructure.EventLog.Logger.Add(4, "ProductPrice", "DeleteImage", false, 200, "   عدم حذف تصویر" + ProductImage.Id, DateTime.Now, User.Identity.GetUserId());
                    return View(ProductImage);
                }

            }
            catch (Exception x)
            {
                #region EventLogger
                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "DeleteConfirmed", false, 500, x.Message, DateTime.Now, User.Identity.GetUserId());
                #endregion
                return RedirectToAction("Index", "Error");
            }
        }

        protected async Task<bool> CheckProductState(ProductPrice ProductPrice, int oldState)
        {
            try
            {
                #region state
                if (ProductPrice.Quantity > 0) // اگر تعداد موجودی بیشتر از 0 است
                {
                    // تنوع موجود شود
                    if (ProductPrice.ProductStateId != 1 && ProductPrice.ProductStateId != 2)
                    {
                        ProductPrice.ProductState = uow.ProductStateRepository.GetByID(1);
                        ProductPrice.ProductStateId = 1;
                    }
                    uow.ProductPriceRepository.Update(ProductPrice);
                    uow.Save();
                    //if (ProductPrice.IsDefault)//اگر تنوع پیش فرض است
                    //{
                    //    //محصول هم موجود شود
                    //    var pr = ProductPrice.Product;
                    //    pr.ProductStateId = ProductPrice.ProductStateId.Value;
                    //    uow.ProductRepository.Update(pr);
                    //    uow.Save();
                    //}
                }
                else // اگر تعداد موجودی 0 شده است
                {
                    //if (ProductPrice.IsDefault)//اگر تنوع پیش فرض است
                    //{
                    //    var otherPrices2 = uow.ProductPriceRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.Id != ProductPrice.Id && x.ProductStateId == 1 && x.IsActive);
                    //    if (otherPrices2.Any())
                    //    {
                    //    }
                    //}
                    if (ProductPrice.ProductStateId == 1 || ProductPrice.ProductStateId == 2)
                    {
                        //تنوع ناموجود گردد
                        ProductPrice.ProductState = uow.ProductStateRepository.GetByID(5);
                        ProductPrice.ProductStateId = 5;
                        uow.ProductPriceRepository.Update(ProductPrice);
                        uow.Save();
                    }
                    //var otherPrices = uow.ProductPriceRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && x.Id != ProductPrice.Id && x.ProductStateId == 5 && x.IsActive);
                    //if (otherPrices.Any())// اگر بقیه تنوع ها هم ناموجود هستند
                    //{
                    //    if (ProductPrice.ProductStateId != 5)
                    //    {
                    //        //محصول مثل تنوع پیش فرض شود
                    //        var pr = ProductPrice.Product;
                    //        pr.ProductStateId = uow.ProductPriceRepository.Get(x => x, x => x.IsDefault && x.ProductId == ProductPrice.ProductId).First().ProductStateId.Value;
                    //        uow.ProductRepository.Update(pr);
                    //        uow.Save();
                    //    }
                    //    else
                    //    {
                    //        //محصول هم ناموجود شود
                    //        var pr = ProductPrice.Product;
                    //        pr.ProductStateId = 5;
                    //        uow.ProductRepository.Update(pr);
                    //        uow.Save();
                    //    }

                    //}
                    //else if (ProductPrice.IsDefault)
                    //{
                    //    //محصول هم ناموجود شود
                    //    var pr = ProductPrice.Product;
                    //    pr.ProductStateId = ProductPrice.ProductStateId == 3 || ProductPrice.ProductStateId == 4 ? ProductPrice.ProductStateId.Value : 5;
                    //    uow.ProductRepository.Update(pr);
                    //    uow.Save();
                    //}

                }
                #endregion

                #region LetmeKhow
                if (oldState > 2 && (ProductPrice.ProductStateId == 1 || ProductPrice.ProductStateId == 2))
                {

                    var ProductLetmeknows = uow.ProductLetmeknowRepository.Get(x => x, x => x.ProductId == ProductPrice.ProductId && (x.Notofied == false || x.NotofiedEmail == false || x.NotofiedSms == false) && x.Available == true, null, "User,Product");
                    if (ProductLetmeknows.Any())
                    {
                        var setting = GetSetting();
                        var smsProductLetmeknows = ProductLetmeknows.Where(x => (x.NotificationType == 2 || x.NotificationType == 4 || x.NotificationType == 6 || x.NotificationType == 7) && x.NotofiedSms == false);
                        var emailProductLetmeknows = ProductLetmeknows.Where(x => (x.NotificationType == 1 || x.NotificationType == 4 || x.NotificationType == 5 || x.NotificationType == 7) && x.NotofiedEmail == false);
                        var msgProductLetmeknows = ProductLetmeknows.Where(x => (x.NotificationType == 3 || x.NotificationType == 5 || x.NotificationType == 6 || x.NotificationType == 7) && x.Notofied == false);
                        #region sms
                        SmsService sms = new SmsService();
                        IdentityMessage iPhonemessage = new IdentityMessage();
                        foreach (var item in smsProductLetmeknows)
                        {
                            try
                            {
                                iPhonemessage.Destination = item.User.PhoneNumber;
                                iPhonemessage.Body = setting.WebSiteName + "\n" + item.User.FirstName + " " + "عزیز\n" + "کالای درخواستی شما ، هم اکنون آماده فروش شده است . جهت خرید کالا به لینک زیر مراجعه کنید :\n" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/CHP/" + item.ProductId + "/" + CommonFunctions.NormalizeAddress(item.Product.PageAddress);
                                await sms.SendSMSAsync(iPhonemessage, "NewLetmeKnowExist", ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/TFP/" + item.ProductId + "/" + CommonFunctions.NormalizeAddress(item.Product.PageAddress), null, null, item.User.FirstName, null);

                            }
                            catch (Exception ex)
                            {
                                #region EventLogger
                                ahmadi.Infrastructure.EventLog.Logger.Add(5, "ProductPrice", "CheckProductState", true, 500, ex.Message, DateTime.Now, User.Identity.GetUserId());
                                #endregion
                            }
                            item.NotofiedSms = true;
                        }
                        #endregion
                        #region email
                        foreach (var item in emailProductLetmeknows)
                        {
                            if (item.User.EmailConfirmed)
                            {

                                try
                                {
                                    #region Create Html Body
                                    string EmailBodyHtml = "";
                                    CoreLib.ViewModel.Email.Template emailBody = new CoreLib.ViewModel.Email.Template(setting.attachmentFileName, "موجودن شدن کالا", "<p>" + item.User.FirstName + " عزیز " + "</p><p> به درخواست شما ، کالای " + item.Product.Title + " هم اکنون در سایت ما موجود شده است . جهت خرید کالا به لینک زیر مراجعه کنید :</p><p><a href='" + ControllerContext.RequestContext.HttpContext.Request.Url.Host + "/TFP/" + item.ProductId + "/" + CommonFunctions.NormalizeAddress(item.Product.PageAddress) + "'>" + item.Product.Title + "</a></p>", setting.WebSiteName, HttpContext.Request.Url.Host, setting.WebSiteTitle);
                                    EmailBodyHtml = Infrastructure.Helper.CaptureHelper.RenderViewToString("_Email", emailBody, this.ControllerContext);

                                    #endregion
                                    EmailService es = new EmailService();
                                    IdentityMessage imessage = new IdentityMessage();
                                    imessage.Body = EmailBodyHtml;
                                    imessage.Destination = item.User.Email;
                                    imessage.Subject = "موجود شدن کالا";
                                    await es.SendAsync(imessage);
                                }
                                catch (Exception)
                                {

                                }
                                item.NotofiedEmail = true;
                            }
                        }
                        #endregion
                        #region usermessage
                        foreach (var item in msgProductLetmeknows)
                        {
                            UserMessage userMessage = new UserMessage()
                            {
                                InsertDate = DateTime.Now,
                                state = false,
                                Text = "<p>" + item.User.FirstName + " عزیز. به درخواست شما ، کالای " + item.Product.Title + " هم اکنون در سایت ما موجود شده است. جهت خرید کالا به لینک زیر مراجعه کنید : <a href='/TFP/" + item.ProductId + "/" + CommonFunctions.NormalizeAddress(item.Product.PageAddress) + "'>" + item.Product.Title + "</a></p>",
                                Title = "موجود شدن کالا",
                                UserId = User.Identity.GetUserId(),
                                UserIdTo = item.UserId
                            };
                            uow.UserMessageRepository.Insert(userMessage);
                            item.Notofied = true;
                        }
                        #endregion
                        uow.Save();
                    }
                }

                #endregion

                return true;

            }
            catch (Exception x)
            {
                return false;
            }
        }
    }


}