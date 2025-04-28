using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ahmadi.Areas.Admin.ViewModels.Product
{

    public class ProductPriceViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public IEnumerable<ProductImage> ProductImages { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public string LatinName { get; set; }
        public string Name { get; set; }
        public string code { get; set; }
        public string ProductCode { get; set; }
        public int? ProductAttributeSelectModelId { get; set; }
        public ProductAttributeSelect ProductAttributeSelectModel { get; set; }

        public int? ProductAttributeSelectColorId { get; set; }
        public ProductAttributeSelect ProductAttributeSelectColor { get; set; }

        public int? ProductAttributeSelectSizeId { get; set; }
        public ProductAttributeSelect ProductAttributeSelectSize { get; set; }

        public int? ProductAttributeSelectGarantyId { get; set; }
        public ProductAttributeSelect ProductAttributeSelectGaranty { get; set; }

        public int? ProductAttributeSelectWeightId { get; set; }
        public ProductAttributeSelect ProductAttributeSelectWeight { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public short LanguageId { get; set; }
        public int ProductTypeId { get; set; }
        public int? ProductStateId { get; set; }
        public int state { get; set; }
        public int DeliveryTimeout { get; set; }
        public int MaxBasketCount { get; set; }
        public long Price { get; set; }
        public string ProductStateTitle { get; set; }


    }
}