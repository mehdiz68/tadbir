using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductPrice
    {
        public ProductPrice()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductPrice>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductPrices).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Seller).WithMany(Current => Current.ProductPrices).HasForeignKey(Current => Current.SellerId).WillCascadeOnDelete(false);

                HasOptional(Current => Current.ProductAttributeSelectColor).WithMany(Current => Current.ProductColorPrices).HasForeignKey(Current => Current.ProductAttributeSelectColorId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductAttributeSelectModel).WithMany(Current => Current.ProductModelPrices).HasForeignKey(Current => Current.ProductAttributeSelectModelId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductAttributeSelectSize).WithMany(Current => Current.ProductSizePrices).HasForeignKey(Current => Current.ProductAttributeSelectSizeId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductAttributeSelectGaranty).WithMany(Current => Current.ProductGarantyPrices).HasForeignKey(Current => Current.ProductAttributeSelectGarantyId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductAttributeSelectweight).WithMany(Current => Current.ProductWeightPrices).HasForeignKey(Current => Current.ProductAttributeSelectWeightId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductState).WithMany(Current => Current.ProductPrices).HasForeignKey(Current => Current.ProductStateId).WillCascadeOnDelete(false);

            }
        }

        #endregion


        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Display(Name = "رنگ")]
        public int? ProductAttributeSelectColorId { get; set; }
        public  ProductAttributeSelect ProductAttributeSelectColor { get; set; }


        [Display(Name = "مدل")]
        public int? ProductAttributeSelectModelId { get; set; }
        public  ProductAttributeSelect ProductAttributeSelectModel { get; set; }


        [Display(Name = "سایز")]
        public int? ProductAttributeSelectSizeId { get; set; }
        public  ProductAttributeSelect ProductAttributeSelectSize { get; set; }


        [Display(Name = "وزن")]
        public int? ProductAttributeSelectWeightId { get; set; }
        public  ProductAttributeSelect ProductAttributeSelectweight { get; set; }


        [Display(Name = "گارانتی")]
        public int? ProductAttributeSelectGarantyId { get; set; }
        public  ProductAttributeSelect ProductAttributeSelectGaranty { get; set; }


        [Display(Name = "قیمت فروش")]
        [Required(ErrorMessage = "قیمت فروش باید وارد گردد")]
        public long Price { get; set; }


        [Display(Name = "کد فروش")]
        public string code { get; set; }

        [Display(Name = "فروشنده (انبار)")]
        [Required(ErrorMessage = "فروشنده(انباری) باید وارد گردد")]
        public int SellerId { get; set; }
        public  Seller Seller { get; set; }

        [Display(Name = "تعداد موجودی این محصول نزد شما")]
        [Required(ErrorMessage = "تعداد موجودی نزد شما باید وارد گردد")]
        public int Quantity { get; set; }
        
        [Display(Name = "بازه زمانی ارسال ( روز )")]
        [Required(ErrorMessage = "بازه زمانی ارسال باید وارد گردد")]
        public int DeliveryTimeout { get; set; }

        [Display(Name = "حداکثر تعدادی که مشتریان در یک سبد بتوانند سفارش دهند ")]
        [Required(ErrorMessage = "حداکثر تعداد موجود در سبد باید وارد گردد")]
        public int MaxBasketCount { get; set; }

        [Display(Name = "تنوع پیش فرض")]
        [Required(ErrorMessage = "تنوع پیش فرض باید وارد گردد")]
        public bool IsDefault { get; set; }


        [Display(Name = "وضعیت موجودی")]
        public int? ProductStateId { get; set; }
        public  ProductState ProductState { get; set; }


        [Required]
        [Display(Name = "فعال برای فروش در سایت")]
        public bool IsActive { get; set; }

        public DateTime InsertDate { get; set; }
        public DateTime Updatedate { get; set; }

        public  ICollection<ProductImage> ProductImages { get; set; }

        public  ICollection<OrderRow> OrderRows { get; set; }
        public ICollection<ProductOffer> ProductOffers { get; set; }

        #endregion
    }
}
