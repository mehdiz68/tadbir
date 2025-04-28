using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class ProductDetailItem
    {
        #region Ctor
        public ProductDetailItem()
        {

        }
        #endregion

        #region Properties
        
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string LatinName { get; set; }

        public string Title { get; set; }


        public string PageAddress { get; set; }

        public string Descr { get; set; }
        public string Data { get; set; }
        public string Data2 { get; set; }
        public string Abstract { get; set; }

        public int quantity { get; set; }
        public int Basketquantity { get; set; }
        public  Brand Brand { get; set; }

        public int DeliveryTimeout { get; set; }

        public string Code { get; set; }


        public  ProductState ProductState { get; set; }

        public  ProductIcon ProductIcon { get; set; }

        public string MainImageFileName { get; set; }
        public ProductImage MainImage { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<ProductImage> OtherImages{ get; set; }
        public IEnumerable<ProductImage> ProductOtherImages { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<ProductPrice> ProductPrices { get; set; }
        public IEnumerable<ProductAttributeSelect> ProductAttributeSelects { get; set; }
        public IEnumerable<ProductAttributeItemColor> Colors { get; set; }
        public IEnumerable<ProductAttributeItem> Garanties { get; set; }
        public IEnumerable<ProductRankSelectValue> ProductRankSelectValues { get; set; }
        public IEnumerable<ProductRankSelectValue> AdminProductRankSelectValues { get; set; }
        public IEnumerable<attachment> buyerattachments { get; set; }

        public double avgRankValue { get; set; }
        public int countRankValue { get; set; }

        public long Price { get; set; }
        public long finalPrice { get; set; }
        public long offValue{ get; set; }
        public long offFinalValue { get; set; }
        public bool productoffertype { get; set; }
        public int offerQuantity { get; set; }
        public int offerMaxQuantity { get; set; }

        public double tax { get; set; }

        public bool hasoff { get; set; }

        public string offTitle { get; set; }

        public string offExpireDate{ get; set; }
        public short offtype { get; set; }
        public int Favorates { get; set; }
        public int LetmeKnows { get; set; }
        public string breadcrumb { get; set; }

        public string Video { get; set; }
        public IEnumerable<string> ProductAdvantage { get; set; }
        public IEnumerable<string> ProductCommentDisAdvantage { get; set; }
        #endregion
    }
}
