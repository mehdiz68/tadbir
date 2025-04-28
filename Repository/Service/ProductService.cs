using DataLayer;
using Domain;
using Domain.ViewModel;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CoreLib;

namespace Repository.Service
{
    public class ProductService : GenericRepository<Product>
    {
        private ProductCategoryService _productCategoryService;
        // private readonly StoreRowService _storeRowService;
        public ProductService(ahmadiDbContext context, ProductCategoryService productCategoryService) : base(context)
        {
            _productCategoryService = productCategoryService;
        }

        public string packagename = "";
        public ProductPackageType ProductPackageType;
        public string offexpiredate = "";
        public int? offerId = null;
        public string offtitle = "";
        public long cprice = 0;
        public long coffvalue = 0;
        public long coffvaluefinal = 0;
        public double ctaxtvalue = 0;
        public bool chasoff = false;
        public bool ProductOfferType = false;
        public int offerQuantity = 0;
        public int offerMaxQuantity = 0;
        public short cofftype = 3;


        public IEnumerable<ProductItem> ProductItemList(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress,
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId

            });
        }
        public IEnumerable<ProductItemRanks> ProductItemListRanks(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Include("ProductRankSelects.ProductRankGroupSelect.ProductRank").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItemRanks()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress,
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId,
                ProductRanks=x.ProductRankSelects.Select(s=>s.ProductRankGroupSelect).Select(a=>a.ProductRank)

            });
        }

        public IEnumerable<ProductItem> ProductItemListWithCategories(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("Brand").Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress,
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                ProductCategories = x.ProductCategories,
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId

            });
        }

        public TotobItem TorobList(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices").Include("ProductPrices.ProductAttributeSelectGaranty").Include("ProductPrices.ProductState").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            TotobItem item = new TotobItem();
            item.count = Get(x => filter).Count();
            item.max_pages = item.count / 100;
            item.products = result.Select(x => new TorobProduct()
            {
                availability = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 ? "instock" : "",
                category_name = x.ProductCategories.First().Name,
                old_price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                current_price = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                guarantee = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGarantyId != null ? GetGarantyItem(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGaranty.Value) : null,
                image_link = "https://www.tfshops.com/Content/UploadFiles/" + GetMainImageFileName(x.Id),
                page_url = "https://www.tfshops.com/TFP/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress),
                page_unique = x.Id,
                short_desc = x.Descr,
                subtitle = x.LatinName,
                title = x.Title


            }).ToList();

            return item;
        }

        public YektanetProduct YektanetProductDetail(int id)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("Brand").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(x => x.Id == id);
            var result = q.ToList();


            return result.Select(x => new YektanetProduct()
            {
                sku = "C-" + x.Id,
                image = "https://www.tfshops.com/Content/UploadFiles/" + GetMainImageFileName(x.Id),
                title = x.Title,
                isAvailable = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2,
                category = new List<string>() { x.ProductCategories.First().Name },
                price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                discount = coffvalue,
                currency = "IRT",
                brand = x.Brand.Name,
                averageVote = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count() : 0,
                totalVotes = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                expiration = 0

            }).First();


        }

        public List<YektanetBuyProduct> YektanetBuyProduct(IEnumerable<OrderRow> OrderRows)
        {


            return OrderRows.Select(x => new YektanetBuyProduct()
            {
                sku = "C-" + x.ProductId,
                quantity = x.Quantity,
                price = x.Price * x.Quantity,
                currency = "IRT"

            }).ToList();


        }

        public IEnumerable<ProductItem> ProductItemList2(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {

            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress,
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                // //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                // //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                ////Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                // countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                // Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                // //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId

            });
        }


        public IEnumerable<ProductItem> ProductItemListPrice(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {

            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices").Include("ProductPrices.ProductState").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();



            return q.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice)

            });
        }

        public ProductItem ProductItem(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress,
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //    Brand = x.Brand,
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,


            }).SingleOrDefault();
        }



        public List<string> GetOtherImageFileName(int id)
        {
            var sqlQuery = @"                SELECT  attachments.FileName FROM ProductPrices AS pp
                       LEFT OUTER   JOIN ProductImages ON pp.ProductId = ProductImages.ProductId
                        LEFT OUTER JOIN attachments ON ProductImages.AttachementId = attachments.Id

                        WHERE 
                         pp.ProductId = 15 AND pp.IsDefault =1 AND  ProductImages.IsImage = 1 AND ProductImages.IsMain = 0
                        ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc";

            var queryResult = context.Database.SqlQuery<string>(sqlQuery).AsQueryable();
            return queryResult.ToList();
        }
        /// <summary>
        /// کوئری برای ئیدا کردن اولین تصویر یک کالا
        /// </summary>
        /// <param name="productPrices"></param>
        /// <returns></returns>
        public string GetMainImageFileName(int pId)
        {

            var sqlQuery = @" 
                SELECT top(1) FIRST_VALUE( attachments.FileName) OVER ( ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc)  FROM ProductPrices AS pp
                       LEFT OUTER   JOIN ProductImages ON pp.Id = ProductImages.ProductPriceId
                        LEFT OUTER JOIN attachments ON ProductImages.AttachementId = attachments.Id

                        WHERE 
                         pp.ProductId =  " + pId + @" AND  ProductImages.IsImage = 1
                        ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc";
            var queryResult = context.Database.SqlQuery<string>(sqlQuery).AsQueryable();
            return queryResult.FirstOrDefault();

        }

        public Domain.ProductImage GetMainImage(IEnumerable<Domain.ProductPrice> productPrices)
        {
            if (productPrices.Any(x => x.ProductImages.Any()))
            {
                if (productPrices.Any(x => x.IsDefault))
                {
                    if (productPrices.Where(x => x.IsDefault).First().ProductImages.Any())
                    {
                        if (productPrices.Where(x => x.IsDefault).First().ProductImages.Any(x => x.IsMain)) // تنوع پیش فرض ، عکس اصلی
                        {
                            return productPrices.Where(x => x.IsDefault).First().ProductImages.Where(x => x.IsMain).First();
                        }
                        else // تنوع پیش فرض ، اولین عکس
                        {
                            return productPrices.Where(x => x.IsDefault).First().ProductImages.First();
                        }
                    }
                    else // اولین تنوع، اولین عکس
                    {

                        return productPrices.SelectMany(x => x.ProductImages).First();
                    }
                }
                else // اولین تنوع، اولین عکس
                {
                    return productPrices.SelectMany(x => x.ProductImages).First();
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<string> GetColor(IEnumerable<string> ColorIds)
        {
            if (ColorIds != null)
            {
                return context.ProductAttributeItemColors.Where(x => ColorIds.Contains(x.Id.ToString())).Select(x => x.Value);
            }
            else
                return null;
        }
        public string GetColor(int ColorId)
        {

            return context.ProductAttributeItemColors.Where(x => x.Id == ColorId).Select(x => x.Value).FirstOrDefault();

        }
        public ProductAttributeItemColor GetColorItem(int ColorId)
        {

            return context.ProductAttributeItemColors.Where(x => x.Id == ColorId).FirstOrDefault();
        }
        public IEnumerable<ProductAttributeItemColor> GetColorItems(IEnumerable<string> ColorIds)
        {
            if (ColorIds != null)
            {
                return context.ProductAttributeItemColors.Where(x => ColorIds.Contains(x.Id.ToString()));
            }
            else
                return null;
        }
        public string GetGaranty(int garantyId)
        {

            return context.ProductAttributeItems.Where(x => x.Id == garantyId).Select(x => x.Value).FirstOrDefault();

        }
        private IEnumerable<string> GetGaranty(IEnumerable<string> GarantyIds)
        {
            if (GarantyIds != null)
            {
                return context.ProductAttributeItems.Where(x => GarantyIds.Contains(x.Id.ToString())).Select(x => x.Value);
            }
            else
                return null;
        }
        public ProductAttributeItem GetGarantyItem(int GarantyId)
        {

            return context.ProductAttributeItems.Where(x => x.Id == GarantyId).FirstOrDefault();

        }
        public IEnumerable<ProductAttributeItem> GetGarantyItems(IEnumerable<string> GarantyIds)
        {
            if (GarantyIds != null)
            {
                return context.ProductAttributeItems.Where(x => GarantyIds.Contains(x.Id.ToString()));
            }
            else
                return null;
        }
        public string GetGarantyItem(string garantyId)
        {
            if (garantyId != null)
            {
                return context.ProductAttributeItems.Where(x => x.Id.ToString() == garantyId).First().Value;
            }
            else
                return null;
        }
        public long GetPrice(int ProductId, long price, int? taxid)
        {

            //if (!taxid.HasValue)
            //{
            //    ctaxtvalue = 0;
            //    cprice = price;
            //    cprice = Convert.ToInt64(Math.Ceiling(cprice * 0.001) * 1000);
            //    return cprice;
            //}
            //else
            //{
            //    ctaxtvalue = ((context.Taxes.Find(taxid.Value).TaxPercent * 0.01) * price);
            //    double prc = ctaxtvalue + price;
            //    cprice = Convert.ToInt64(Math.Ceiling(prc * 0.001) * 1000);
            //    return cprice;

            //}
            ctaxtvalue = 0;
            cprice = price;
            cprice = Convert.ToInt64(Math.Ceiling(cprice * 0.001) * 1000);
            return cprice;

        }

        public GetLongRange GetPriceRaneg(int catIds)
        {
            var prices = SqlQueryLong("exec GetPriceProductCategory @catid", new SqlParameter("@catid", catIds)).ToList();
            //var sss = context.ProductPrices.Where(c => c.IsDefault && c.IsActive && c.Product.IsActive && c.Product.state==4 && c.Product.LanguageId==1 && c.Product.ProductCategories.Any(s => catIds.Contains(s.Id))).Select(s => s.Price);
            if (prices.Any())
            {
                return new GetLongRange
                {
                    R1 = prices.Min(),
                    R2 = prices.Max()
                };
            }
            else
            {
                return new GetLongRange
                {
                    R1 = 0,
                    R2 = 0
                };
            }
        }
        public GetLongRange GetPriceRanegGetPriceTag(int tagId)
        {
            var prices = SqlQueryLong("exec GetPriceTag @TagId", new SqlParameter("@TagId", tagId)).ToList();
            if (prices.Any())
            {
                return new GetLongRange
                {
                    R1 = prices.Min(),
                    R2 = prices.Max()
                };
            }
            else
                return new GetLongRange
                {
                    R1 = 0,
                    R2 = 0
                };

        }

        public long GetOff(int ProductPriceId, int CatId, int? BrandId, long price)
        {
            chasoff = false;
            var productoffer = context.ProductOffers.Include("Offer").Include("ProductPrice").Where(s => s.Quantity > 0 && s.ProductPrice.ProductStateId < 3 && s.ProductPriceId == ProductPriceId && s.Offer.IsActive && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= DateTime.Now) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= DateTime.Now) || s.Offer.StartDate == null)).FirstOrDefault();
            if (productoffer != null)
            {
                offexpiredate = productoffer.Offer.ExpireDate.HasValue ? productoffer.Offer.ExpireDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + " " + productoffer.Offer.ExpireDate.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : "";
                ProductOfferType = productoffer.Offer.CodeTypeValueCode == 2 ? false : true;
                offtitle = productoffer.Offer.Title;
                cofftype = productoffer.CodeType;
                offerQuantity = productoffer.Quantity;
                offerMaxQuantity = productoffer.MaxBasketCount;
                chasoff = true;
                offerId = productoffer.Id;
                return ComputeOffVaule(productoffer.CodeType, productoffer.Value, price);
            }
            else
            {
                chasoff = false;
                cofftype = 3;
                coffvalue = 0;
                coffvaluefinal = 0;
                return 0;
            }
        }
        public long ComputeOffVaule(short codeType, int value, long price)
        {
            if (codeType == 2)
            {
                coffvaluefinal = Convert.ToInt64(price * (value * 0.01));
                coffvaluefinal = Convert.ToInt64(Math.Ceiling(coffvaluefinal * 0.001) * 1000);
                coffvalue = value;
                return coffvaluefinal;
            }
            else if (codeType == 1)
            {
                coffvaluefinal = value;
                coffvaluefinal = Convert.ToInt64(Math.Ceiling(coffvaluefinal * 0.001) * 1000);
                coffvalue = value;
                return coffvaluefinal;
            }
            else
            {
                coffvalue = 0;
                coffvaluefinal = 0;
                return 0;
            }
        }

        public string GerProductBreadcrumb(int productId)
        {
            string breadcrumb = "";
            List<ProductCategory> breadcrumbList = new List<ProductCategory>();
            var prcats = Get(x => new { ProductCategory = x.ProductCategories.First(), Id = x.Id, PrTitle = x.Title }, x => x.Id == productId, null, "ProductCategories");
            if (!prcats.First().ProductCategory.ParrentId.HasValue)
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFC/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='1' /></li>", prcats.First().ProductCategory.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.First().ProductCategory.PageAddress), prcats.First().ProductCategory.Name);
            else
            {
                var parent = prcats.First().ProductCategory.ParentCat;
                while (parent != null)
                {
                    breadcrumbList.Add(new ProductCategory() { Id = parent.Id, Name = parent.Name, PageAddress = parent.PageAddress });
                    parent = parent.ParentCat;
                }
                int i = 1;
                foreach (var item in breadcrumbList.OrderBy(x => x.Id))
                {

                    string prefix = "TFS";
                    if (!item.ParrentId.HasValue)
                        prefix = "TFC";

                    breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/" + prefix + "/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", item.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(item.PageAddress), item.Name, i);
                    i++;
                }
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFS/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", prcats.First().ProductCategory.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.First().ProductCategory.PageAddress), prcats.First().ProductCategory.Name, i);
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFP/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", prcats.First().Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.First().PrTitle), prcats.First().PrTitle, i + 1);
            }
            return breadcrumb;
        }


        public string GerProductBreadcrumbBrand(int BrandId, string Title, string Name, int? CatId)
        {
            string breadcrumb = "";
            if (CatId.HasValue)
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFB/{0}/{1}/{2}'><span itemprop='name'>{3}</span></a><meta itemprop='position' content='{4}' /></li>", BrandId, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(Name), CatId.Value, Name, 1);
            else
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFB/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", BrandId, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(Name), Name, 1);
            return breadcrumb;
        }


        public async Task addProductFavorate(int productid)
        {
            var pr = GetByID(productid);
            pr.FavCount++;
            Update(pr);
            await context.SaveChangesAsync();
        }
        public async Task removeProductFavorate(int productid)
        {
            var pr = GetByID(productid);
            pr.FavCount--;
            Update(pr);
            await context.SaveChangesAsync();
        }
        public ProductReviewGoogleList GetProductReviewGoogleList(int productid)
        {
            return Get(x => new ProductReviewGoogleList
            {
                @context = "https://schema.org/",
                @type = "Product",
                name = x.Title,
                image = x.ProductImages.Where(sx => sx.IsImage).OrderBy(s => s.IsMain).Select(s => "https://www.tfshops.com/tf-Products/" + s.Image.FileName.GetFileName() + "/" + s.Image.FileName.GetFolderName() + "/1280/1280/LG"),
                description = x.Abstract,
                sku = x.Id,
                brand = new brand() { @type = "Brand", name = x.Brand.Name },
                review = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.User).Select(s => new reviewitem() { @type = "Review", author = new author { type = "Person", name = s.Key.FirstName + " " + s.Key.LastName }, reviewBody = x.ProductComments.Where(a => a.UserId == s.Key.Id).Any() ? x.ProductComments.Where(a => a.UserId == s.Key.Id).First().Title : "", reviewRating = new reviewRating() { @type = "Rating", ratingValue = s.Average(a => a.Value), bestRating = 5, worstRating = 1 } }),
                aggregateRating = new aggregateRating() { @type = "AggregateRating", ratingValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? Math.Round((x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count()), 2) : 3, reviewCount = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 1 },
                offers = new offers()
                {
                    @type = "Offer",
                    url = "https://www.tfshops.com/TFP/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress),
                    priceCurrency = "IRR",
                    price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId)*10,
                    availability = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 ? "https://schema.org/InStock" : "https://schema.org/OutOfStock",
                    priceValidUntil=DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm")
                }




            }, x => x.Id == productid && x.IsActive && x.ProductRankSelects.Any(), x => x.OrderByDescending(s => s.Id), "ProductPrices,ProductIcon,ProductImages.Image,Brand,ProductRankSelects.ProductRankSelectValues.User,ProductComments").SingleOrDefault();
        }

        public bool CheckBuyer(int productId, string userid)
        {
            return Any(x => x.Id, x => x.OrderRows.Any(s => s.ProductId == productId) && x.OrderRows.Any(s => s.Order.UserId == userid) && x.OrderRows.Any(s=>s.Order.OrderStates.Any(a=>a.state==OrderStatus.تایید_پرداخت)) && x.OrderRows.Any(s => s.Order.OrderStates.Any(a => a.state == OrderStatus.تحویل_داده_شده)));
        }
    }

}
