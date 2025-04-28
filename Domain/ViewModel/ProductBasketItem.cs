using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class ProductBasketItem
    {
        #region Ctor
        public ProductBasketItem()
        {

        }
        #endregion

        #region Properties
        public int? SenwayBoxId { get; set; }
        public ProductPackageType ProductPackageType { get; set; }
        public string PacakgeType { get; set; }
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
        public int UserQuantity { get; set; }
        public int ProductPriceQuantity { get; set; }
        public string PageAddress { get; set; }
        public ProductImage MainImage { get; set; }
        public ProductAttributeItemColor color { get; set; }
        public string model { get; set; }
        public string MainImageFileName { get; set; }
        public string size { get; set; }
        public ProductAttributeItem garanty { get; set; }
        public string weight { get; set; }
        public int extraprice { get; set; }
        public int productWeight { get; set; }
        public long rawprice { get; set; }
        public long Price { get; set; }
        public long finalPrice { get; set; }
        public long offValue{ get; set; }
        public long offFinalValue { get; set; }
        public int offerQuantity { get; set; }
        public int offerMaxQuantity { get; set; }
        public int? offerid { get; set; }
        public double tax { get; set; }

        public int productStateId { get; set; }
        public bool hasoff { get; set; }

        public string offTitle { get; set; }

        public string offExpireDate{ get; set; }
        public short offtype { get; set; }
        public int? TaxId { get; set; }
        public int DeliveryTimeout { get; set; }
        public int? Bon { get; set; }
        public int? MaxBon { get; set; }
        public string  code { get; set; }
        public DateTime?  InsertDate { get; set; }

        /*0 ارسال رایگان
          >0 باقی مانده
          -1  ارسال رایگان شامل نمیشود*/
        public int RemainOffPrice { get; set; }
        public int CatId { get; set; }

        #endregion
    }
}
