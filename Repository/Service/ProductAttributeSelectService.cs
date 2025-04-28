using DataLayer;
using Domain;
using Domain.ViewModel;
using Domain.ViewModel.Site;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Repository.Service
{
    public class ProductAttributeSelectService : GenericRepository<ProductAttributeSelect>
    {
        public ProductAttributeSelectService(ahmadiDbContext context) : base(context)
        {
        }

        public IQueryable<ProductAttributeSelect> CheckAtrribute(IQueryable<ProductAttributeSelect> productAttributeSelects, List<AttributeViewModel> attributeViewModels, List<int> cateIds)
        {

            string querys = string.Empty;
            string catQuery = string.Empty;
            int index = 1;
            if (attributeViewModels == null)
                attributeViewModels = new List<AttributeViewModel>();
            else
                querys = " AND (";
            foreach (var item in attributeViewModels)
            {
                querys += $"([Extent2].[AttributeId] = {item.Key1}) AND([Extent1].[Value] = '{item.Value}')";
                querys += index != attributeViewModels.Count ? " OR " : ")";
                index += 1;
            }
            index = 1;
            foreach (var item in cateIds)
            {
                catQuery += $"[Extent3].[CatId] IN ({item})";
                catQuery += index != cateIds.Count ? " OR " : "";
            }
            var sqlQuery = string.Concat(@"SELECT 
                            [Extent1].[Id] AS [Id], 
                            [Extent1].[ProductAttributeCategorySelectId] AS [ProductAttributeCategorySelectId], 
                            [Extent1].[ProductId] AS [ProductId], 
                            [Extent1].[Value] AS [Value], 
                            [Extent1].[DisplayOrder] AS [DisplayOrder], 
                            [Extent2].[Id] AS [Id1], 
                            [Extent2].[Name] AS [Name], 
                            [Extent2].[LatinName] AS [LatinName], 
                            [Extent2].[Title] AS [Title], 
                            [Extent2].[PageAddress] AS [PageAddress], 
                            [Extent2].[Descr] AS [Descr], 
                            [Extent2].[Abstract] AS [Abstract], 
                            [Extent2].[Data] AS [Data], 
                            [Extent2].[Data2] AS [Data2], 
                            [Extent2].[BrandId] AS [BrandId], 
                            [Extent2].[ProductTypeId] AS [ProductTypeId], 
                            [Extent2].[Code] AS [Code], 
                            [Extent2].[ScratchCode] AS [ScratchCode], 
                            [Extent2].[ProductStateId] AS [ProductStateId], 
                            [Extent2].[ProductIconId] AS [ProductIconId], 
                            [Extent2].[Height] AS [Height], 
                            [Extent2].[Width] AS [Width], 
                            [Extent2].[Lenght] AS [Lenght], 
                            [Extent2].[ProductWeight] AS [ProductWeight], 
                            [Extent2].[ExtraSendPrice] AS [ExtraSendPrice], 
                            [Extent2].[TaxId] AS [TaxId], 
                            [Extent2].[bon] AS [bon], 
                            [Extent2].[bonMaxBasket] AS [bonMaxBasket], 
                            [Extent2].[state] AS [state], 
                            [Extent2].[LanguageId] AS [LanguageId], 
                            [Extent2].[Visits] AS [Visits], 
                            [Extent2].[UserId] AS [UserId], 
                            [Extent2].[LogPrice] AS [LogPrice], 
                            [Extent2].[Video] AS [Video], 
                            [Extent2].[Catalog] AS [Catalog], 
                            [Extent2].[IsOriginality] AS [IsOriginality], 
                            [Extent2].[InsertDate] AS [InsertDate], 
                            [Extent2].[UpdateDate] AS [UpdateDate], 
                            [Extent2].[IsActive] AS [IsActive], 
                            [Extent2].[DisplaySort] AS [DisplaySort], 
                            [Extent2].[attachment_Id] AS [attachment_Id]
                            FROM  [dbo].[ProductAttributeSelects] AS [Extent1]
                            INNER JOIN [dbo].[Products] AS [Extent2] ON [Extent1].[ProductId] = [Extent2].[Id]
                            WHERE  EXISTS (SELECT 
                                1 AS [C1]
                                FROM [dbo].[ProductCategorySelect] AS [Extent3]
                                WHERE ([Extent1].[ProductId] = [Extent3].[ProductId]) AND (", catQuery, @")
                            )", querys);
            var result = context.Database.SqlQuery<ProductAttributeSelect>(sqlQuery).AsQueryable();
            return result;
        }

        public IQueryable<ProductAttributeSelect> CheckAtrribute2(IQueryable<ProductAttributeSelect> productAttributeSelects, List<AttributeViewModel> a)
        {
            var productAttributeSelects2 = productAttributeSelects.ToList();
            if (a == null || a.Count == 0)
                return productAttributeSelects;
            else if (a.Count == 1)
            {
                var k1 = a[0].Key1;
                var a1 = a[0].Value;

                productAttributeSelects = productAttributeSelects.Where(x => x.ProductAttributeGroupSelect.AttributeId == k1 && x.Value == a1);
            }
            else if (a.Count == 2)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value));
            else if (a.Count == 3)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value));
            else if (a.Count == 4)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[3].Key1 && x.Value == a[3].Value));
            else if (a.Count == 5)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[3].Key1 && x.Value == a[3].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[4].Key1 && x.Value == a[4].Value));
            else if (a.Count == 6)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[3].Key1 && x.Value == a[3].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[4].Key1 && x.Value == a[4].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[5].Key1 && x.Value == a[5].Value));
            else if (a.Count == 7)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[3].Key1 && x.Value == a[3].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[4].Key1 && x.Value == a[4].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[5].Key1 && x.Value == a[5].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[6].Key1 && x.Value == a[6].Value));
            else if (a.Count == 8)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value));
            else if (a.Count == 9)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value));
            else if (a.Count == 10)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value));
            else if (a.Count == 11)
                productAttributeSelects = productAttributeSelects.Where(x => (x.ProductAttributeGroupSelect.AttributeId == a[0].Key1 && x.Value == a[0].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[1].Key1 && x.Value == a[1].Value) || (x.ProductAttributeGroupSelect.AttributeId == a[2].Key1 && x.Value == a[2].Value));


            return productAttributeSelects;
        }

        public IQueryable<ProductAttributeSelect> CheckSeller(IQueryable<ProductAttributeSelect> productAttributeSelects, List<SellerConditionViewModel> b)
        {
            //productAttributeSelects = productAttributeSelects .Where(c=>c.Product.ProductPrices.Any(c=> c.SellerId ==  b[0].Value ))
            if (b == null || b.Count == 0)
                return productAttributeSelects;
            else if (b.Count == 1)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value));
            else if (b.Count == 2)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)));
            else if (b.Count == 3)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)));
            else if (b.Count == 4)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[3].Value)));
            else if (b.Count == 5)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[3].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[4].Value)));
            else if (b.Count == 6)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[3].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[4].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[5].Value)));
            else if (b.Count == 7)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[3].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[4].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[5].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[6].Value)));
            else if (b.Count == 8)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)));
            else if (b.Count == 9)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)));
            else if (b.Count == 10)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)));
            else if (b.Count == 11)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.ProductPrices.Any(c => c.SellerId == b[0].Value) || (x.Product.ProductPrices.Any(c => c.SellerId == b[1].Value)) || (x.Product.ProductPrices.Any(c => c.SellerId == b[2].Value)));

            return productAttributeSelects;
        }



        public IQueryable<ProductAttributeSelect> CheckBbrands(IQueryable<ProductAttributeSelect> productAttributeSelects, List<BrandViewModel> b)
        {

            if (b == null || b.Count == 0)
                return productAttributeSelects;
            else if (b.Count == 1)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value);
            else if (b.Count == 2)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value));
            else if (b.Count == 3)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value));
            else if (b.Count == 4)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value) || (x.Product.BrandId == b[3].Value));
            else if (b.Count == 5)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value) || (x.Product.BrandId == b[3].Value) || (x.Product.BrandId == b[4].Value));
            else if (b.Count == 6)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value) || (x.Product.BrandId == b[3].Value) || (x.Product.BrandId == b[4].Value) || (x.Product.BrandId == b[5].Value));
            else if (b.Count == 7)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value) || (x.Product.BrandId == b[3].Value) || (x.Product.BrandId == b[4].Value) || (x.Product.BrandId == b[5].Value) || (x.Product.BrandId == b[6].Value));
            else if (b.Count == 8)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value));
            else if (b.Count == 9)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value));
            else if (b.Count == 10)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value));
            else if (b.Count == 11)
                productAttributeSelects = productAttributeSelects.Where(x => x.Product.BrandId == b[0].Value || (x.Product.BrandId == b[1].Value) || (x.Product.BrandId == b[2].Value));

            return productAttributeSelects;
        }



        /// <summary>
        /// نتایج سرچ و دسته بندی برای محصولات
        /// </summary>
        /// <param name="totalFilters"></param>
        /// <param name="catIds"></param>
        /// <returns></returns>
        public IQueryable<ProductSearchResult> GetSearchResult(getUrlParameter totalFilters, List<int> catIds = null, bool? searchpage = null, bool? noAttributeSelect = null)
        {
            string attrQuerys = string.Empty;
            string dblistQuerys = string.Empty;
            string catQuery = string.Empty;
            string brandQuery = string.Empty;
            string originalyQuery = string.Empty;
            string SearchQuery = string.Empty;
            string productStateQuery = string.Empty;
            string sellerQuery = string.Empty;
            string onlyInStoreQuery = string.Empty;
            string sortByQuery = string.Empty;
            string sortfieldNameQuery = string.Empty;


            // کوئری مروبط به رنج قیمت
            string PriceRangeQuery = string.Empty;

            int index = 1;

            if (catIds != null)
            {
                // دسته بندی ای دی ها 
                foreach (var item in catIds)
                {
                    catQuery += $"ProductCategorySelect.CatId={item}";
                    catQuery += index != catIds.Count ? " OR " : "";
                    index += 1;
                }
            }
            else
                catQuery += " 1=1 ";
            index = 1;

            if (noAttributeSelect != null && noAttributeSelect.Value == false)
            {
                if (totalFilters.AttributeViewModels != null && totalFilters.AttributeViewModels.Count > 0)
                {
                    attrQuerys = " AND (";
                    foreach (var item in totalFilters.AttributeViewModels)
                    {
                        attrQuerys += $"(ProductAttributeGroupSelects.AttributeId={item.Key1} AND ProductAttributeSelects.Value='{item.Value}')";
                        attrQuerys += index != totalFilters.AttributeViewModels.Count ? " OR " : ")";
                        index += 1;
                    }
                }
                index = 1;
                if (totalFilters.DbListViewModels != null && totalFilters.DbListViewModels.Count > 0)
                {
                    dblistQuerys = " AND (";
                    foreach (var item in totalFilters.DbListViewModels)
                    {
                        dblistQuerys += $"(ProductAttributeGroupSelects.AttributeId={item.Key1} AND ProductAttributeSelects.Value='{item.Value}')";
                        dblistQuerys += index != totalFilters.DbListViewModels.Count ? " OR " : ")";
                        index += 1;
                    }
                }

                index = 1;
                if (totalFilters.LogicViewModels != null && totalFilters.LogicViewModels.Count > 0)
                {
                    attrQuerys = " AND (";
                    foreach (var item in totalFilters.LogicViewModels)
                    {
                        attrQuerys += $"(ProductAttributeGroupSelects.AttributeId={item.Key} AND ProductAttributeSelects.Value='1')";
                        attrQuerys += index != totalFilters.LogicViewModels.Count ? " OR " : ")";
                        index += 1;
                    }
                }
            }

            // چک کردن برند
            index = 1;
            if (totalFilters.BrandViewModels != null && totalFilters.BrandViewModels.Count > 0)
            {
                brandQuery = " and (";
                foreach (var item in totalFilters.BrandViewModels)
                {

                    brandQuery += $"Products.BrandId={item.Value}";
                    brandQuery += index != totalFilters.BrandViewModels.Count ? " OR " : ")";
                    index += 1;
                }
            }

            // چک کردن تگ
            if (totalFilters.productTagId > 0)
            {
                originalyQuery = "   and (ProductTags.TagId=" + totalFilters.productTagId + ") ";
            }

            // چک کردن فقط کالا های اصل

            if (totalFilters.IsOriginality)
            {
                originalyQuery = "  and (Products.IsOriginality=1) ";
            }
            if (totalFilters.SearchStr != null)
            {
                if (searchpage.HasValue)
                {
                    if (searchpage.Value == true)
                        SearchQuery = $" and( products.Title LiKE N'%" + totalFilters.SearchStr + "%' OR products.Name LiKE N'%" + totalFilters.SearchStr + "%' OR Products.LatinName LiKE N'%" + totalFilters.SearchStr + "%' OR Products.Descr LiKE N'%" + totalFilters.SearchStr + "%' OR Products.Abstract LiKE N'%" + totalFilters.SearchStr + "%' OR ProductAttributeSelects.ProductId in (select productid from productprices where  ProductPrices.ProductAttributeSelectModelId IN(select id from ProductAttributeSelects where Value LIKE N'%" + totalFilters.SearchStr + "%') ) OR ProductAttributeSelects.ProductId in (select productid from productprices where  ProductPrices.ProductAttributeSelectSizeId IN(select id from ProductAttributeSelects where Value LIKE N'%" + totalFilters.SearchStr + "%')) ) ";
                    else
                        SearchQuery = $" and(products.Title LiKE N'%" + totalFilters.SearchStr + "%' OR products.Name LiKE N'%" + totalFilters.SearchStr + "%' OR Products.LatinName LiKE N'%" + totalFilters.SearchStr + "%') ";
                }
                else
                    SearchQuery = $" and(products.Title LiKE N'%" + totalFilters.SearchStr + "%' OR products.Name LiKE N'%" + totalFilters.SearchStr + "%' OR Products.LatinName LiKE N'%" + totalFilters.SearchStr + "%') ";
            }
            if (totalFilters.ProductStateId > 0)
            {
                productStateQuery = $"  and (ProductPrices.ProductStateId={totalFilters.ProductStateId})";
            }

            if (totalFilters.IsExit)
            {
                productStateQuery = $"  and (ProductPrices.ProductStateId=1 OR  ProductPrices.ProductStateId=2)";
            }
            // چک کردن فروشنده
            index = 1;
            if (totalFilters.SellerConditionViewModels != null && totalFilters.SellerConditionViewModels.Count > 0)
            {
                sellerQuery = " and (";
                foreach (var item in totalFilters.SellerConditionViewModels)
                {

                    sellerQuery += $"ProductPrices.SellerId={item.Value}";
                    sellerQuery += index != totalFilters.SellerConditionViewModels.Count ? " OR " : ")";
                    index += 1;
                }
            }
            if (totalFilters.PriceRange != null)
            {
                var startRange = Math.Min(totalFilters.PriceRange.StartPrice, totalFilters.PriceRange.EndPrice);
                var endRage = Math.Max(totalFilters.PriceRange.StartPrice, totalFilters.PriceRange.EndPrice);
                PriceRangeQuery = $" and (price  between {startRange} and {endRage} ) ";
            }
            //if (totalFilters.OnlyInStore)
            //{
            //    onlyInStoreQuery = " and (StoreRows.InputState=1 and StoreRows.IsComplete=0)  ";
            //}

            var sortby = totalFilters.sortby;
            sortByQuery = sortby == 1 ? " order by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end, Visits desc " :
                sortby == 2 ? " order by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end, sellCount desc " :
                sortby == 3 ? " order by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end, favCount desc " :
                sortby == 4 ? " order by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end, ProductAttributeSelects.ProductId desc " :   // جدیدترین
                sortby == 5 ? "  order by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end, price  " :  //  ارزانترین
                sortby == 6 ? " order by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end,price desc  " :  //  گرانترین
              " order by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end,Visits desc ";


            sortfieldNameQuery = sortby == 1 ? "Visits" :
            sortby == 2 ? "sellCount" :
            sortby == 3 ? "favCount" :
            sortby == 4 ? "ProductAttributeSelects.ProductId" :     // جدیدترین
            sortby == 5 ? "price" :         //  ارزانترین
            sortby == 6 ? "price" :         //  گرانترین
          "Visits";

            var sqlQuery = string.Concat(@"select ROW_NUMBER() OVER(" + sortByQuery + @") AS Sort,ProductAttributeSelects.ProductId,avg(products.Visits)Visits,avg(products.sellCount)sellCount,avg(products.favCount)favCount, (select price from getprice(ProductAttributeSelects.ProductId,ProductPrices.Price)) price from  ProductAttributeSelects

             	            LEFT OUTER JOIN ProductCategorySelect ON ProductCategorySelect.ProductId=ProductAttributeSelects.ProductId
	                        LEFT OUTER JOIN ProductAttributeGroupSelects ON ProductAttributeGroupSelects.Id=ProductAttributeSelects.ProductAttributeCategorySelectId 
	                        LEFT OUTER JOIN Products ON products.Id=ProductAttributeSelects.ProductId
	                        LEFT OUTER JOIN ProductFavorates ON ProductAttributeSelects.ProductId=ProductFavorates.ProductId
	                        LEFT OUTER JOIN ProductPrices ON ProductAttributeSelects.ProductId = ProductPrices.ProductId" + (totalFilters.productTagId > 0 ? " LEFT OUTER JOIN ProductTags ON ProductPrices.ProductId = ProductTags.ProductId " : " ") + "where ProductPrices.IsDefault=1 and ProductPrices.IsActive=1 and Products.state=4 and Products.languageid=1 AND ProductAttributeSelects.ProductId in (select productid from productprices where isdefault=1) AND (", catQuery, @")
                             and (Products.IsActive=1) 
                            " + brandQuery + @"
                             " + originalyQuery + @" 
                             " + attrQuerys + @" 
                             " + dblistQuerys + @" 
                           " + SearchQuery + @" 
                           " + productStateQuery + @" 
                           " + sellerQuery + @" 
                           " + PriceRangeQuery + @" 
                           " + onlyInStoreQuery + @" 
                            group by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end,ProductAttributeSelects.ProductId,price," + sortfieldNameQuery + " " + sortByQuery);
            if (noAttributeSelect != null && noAttributeSelect.Value == true)
            {
                sqlQuery = string.Concat(@"select ROW_NUMBER() OVER(" + sortByQuery + @") AS Sort,ProductPrices.ProductId,avg(products.Visits)Visits,avg(products.sellCount)sellCount,avg(products.favCount)favCount, (select price from getprice(ProductPrices.ProductId,ProductPrices.Price)) price from  ProductPrices

             	            LEFT OUTER JOIN ProductCategorySelect ON ProductCategorySelect.ProductId=ProductPrices.ProductId
	                        LEFT OUTER JOIN Products ON products.Id=ProductPrices.ProductId
	                        LEFT OUTER JOIN ProductFavorates ON ProductPrices.ProductId=ProductFavorates.ProductId
                            where ProductPrices.IsDefault=1 and ProductPrices.IsActive=1 and Products.state=4 and Products.languageid=1 AND ProductPrices.ProductId in (select productid from productprices where isdefault=1) AND 
                            (", catQuery, @")
                             and (Products.IsActive=1) 
                            " + brandQuery + @"
                             " + originalyQuery + @" 
                             " + attrQuerys + @" 
                             " + dblistQuerys + @" 
                           " + SearchQuery + @" 
                           " + productStateQuery + @" 
                           " + sellerQuery + @" 
                           " + PriceRangeQuery + @" 
                           " + onlyInStoreQuery + @" 
                            group by case ProductPrices.ProductStateId when 1 then 1 when 2 then 1 else ProductPrices.ProductStateId end,ProductPrices.ProductId,price," + sortfieldNameQuery + " " + sortByQuery);
            }

            var result = context.Database.SqlQuery<ProductSearchResult>(sqlQuery).AsQueryable();
            return result;

        }


        public IQueryable<ProductSearchResult> GetSearchResultPages(getUrlParameter totalFilters, List<int> catIds)
        {
            string attrQuerys = string.Empty;
            string catQuery = string.Empty;
            string brandQuery = string.Empty;
            string originalyQuery = string.Empty;
            string SearchQuery = string.Empty;
            string productStateQuery = string.Empty;
            string sellerQuery = string.Empty;
            string onlyInStoreQuery = string.Empty;
            string sortByQuery = string.Empty;


            // pageSetting
            int skip = (totalFilters.pagenum - 1) * totalFilters.perpage;
            if (skip < 0)
                skip = 0;
            if (totalFilters.perpage == 0)
            {
                totalFilters.perpage = 24;
            }
            // کوئری مروبط به رنج قیمت
            string PriceRangeQuery = string.Empty;

            int index = 1;

            if (catIds != null)
            {
                // دسته بندی ای دی ها 
                foreach (var item in catIds)
                {
                    catQuery += $"ProductCategorySelect.CatId={item}";
                    catQuery += index != catIds.Count ? " OR " : "";
                    index += 1;
                }
            }
            else
                catQuery += " 1=1 ";
            index = 1;
            var attributeViewModels = new List<AttributeViewModel>();
            if (totalFilters.AttributeViewModels != null)
                attrQuerys = " AND (";
            foreach (var item in attributeViewModels)
            {
                attrQuerys += $"(ProductAttributeGroupSelects.AttributeId={item.Key1} AND ProductAttributeSelects.Value='{item.Value}')";
                attrQuerys += index != attributeViewModels.Count ? " OR " : ")";
                index += 1;
            }
            // چک کردن برند
            index = 1;
            if (totalFilters.BrandViewModels != null && totalFilters.BrandViewModels.Count > 0)
            {
                brandQuery = " and (";
                foreach (var item in totalFilters.BrandViewModels)
                {

                    brandQuery += $"Products.BrandId={item.Value}";
                    brandQuery += index != totalFilters.BrandViewModels.Count ? " OR " : ")";
                    index += 1;
                }
            }


            // چک کردن فقط کالا های اصل

            if (totalFilters.IsOriginality)
            {
                originalyQuery = "  and (Products.IsOriginality=1) ";
            }
            if (totalFilters.SearchStr != null)
            {
                SearchQuery = $" and(products.Title LiKE N'%" + totalFilters.SearchStr + "%' OR products.Name LiKE N'%" + totalFilters.SearchStr + "%' OR Products.LatinName LiKE N'%" + totalFilters.SearchStr + "%') ";
            }
            if (totalFilters.ProductStateId > 0)
            {
                productStateQuery = $"  and (ProductPrices.ProductStateId={totalFilters.ProductStateId})";
            }

            if (totalFilters.IsExit)
            {
                productStateQuery = $"  and (ProductPrices.ProductStateId=1  OR  ProductPrices.ProductStateId=2)";
            }
            // چک کردن فروشنده
            index = 1;
            if (totalFilters.SellerConditionViewModels != null && totalFilters.SellerConditionViewModels.Count > 0)
            {
                sellerQuery = " and (";
                foreach (var item in totalFilters.SellerConditionViewModels)
                {

                    sellerQuery += $"roductPrices.SellerId={item.Value}";
                    sellerQuery += index != totalFilters.BrandViewModels.Count ? " OR " : ")";
                    index += 1;
                }
            }
            if (totalFilters.PriceRange != null)
            {
                var startRange = Math.Min(totalFilters.PriceRange.StartPrice, totalFilters.PriceRange.EndPrice);
                var endRage = Math.Max(totalFilters.PriceRange.StartPrice, totalFilters.PriceRange.EndPrice);
                PriceRangeQuery = $" and ((select price from getprice(ProductPrices.ProductId,ProductPrices.Price)) between {startRange} and {endRage} ) ";
            }
            //if (totalFilters.OnlyInStore)
            //{
            //    onlyInStoreQuery = " and (StoreRows.InputState=1 and StoreRows.IsComplete=0)  ";
            //}
            var sortby = totalFilters.sortby;
            sortByQuery = sortby == 1 ? " order by Visits desc " :
                sortby == 2 ? " order by sellCount desc " :
                sortby == 3 ? " order by favCount desc " :
                sortby == 4 ? " order by ProductId desc " :   // جدیدترین
                sortby == 5 ? "  order by price  " :  //  ارزانترین
                sortby == 6 ? " order by price desc  " :  //  گرانترین
              " order by Visits desc ";

            var sqlQuery = string.Concat(@"select ProductAttributeSelects.ProductId,avg(products.Visits)Visits,avg(products.sellCount)sellCount,avg(products.favCount)favCount, (select price from getprice(ProductAttributeSelects.ProductId,ProductPrices.Price)) price from  ProductAttributeSelects

                            LEFT OUTER JOIN ProductCategorySelect ON ProductCategorySelect.ProductId=ProductAttributeSelects.ProductId
                            LEFT OUTER JOIN ProductAttributeGroupSelects ON ProductAttributeGroupSelects.Id=ProductAttributeSelects.ProductAttributeCategorySelectId 
                            LEFT OUTER JOIN Products ON products.Id=ProductAttributeSelects.ProductId
                            LEFT OUTER JOIN ProductFavorates ON ProductFavorates.ProductId=ProductAttributeSelects.ProductId
                            LEFT OUTER JOIN ProductPrices ON ProductPrices.ProductId=ProductAttributeSelects.ProductId
                            where
                            (", catQuery, @")
                             and (Products.IsActive=1) 
                            " + brandQuery + @"
                           " + originalyQuery + @" 
                           " + SearchQuery + @" 
                           " + productStateQuery + @" 
                           " + sellerQuery + @" 
                           " + PriceRangeQuery + @" 
                           " + onlyInStoreQuery + @" 
                            group by ProductAttributeSelects.ProductId,price,favCount
                             " + sortByQuery + @"
                               
                                OFFSET  " + skip + @"  ROWS  
                                FETCH NEXT  " + totalFilters.perpage + @"  ROWS ONLY;");
            var result = context.Database.SqlQuery<ProductSearchResult>(sqlQuery).AsQueryable();
            return result;
        }


    }
}
