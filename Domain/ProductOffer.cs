using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ProductOffer : Object
    {
        #region Ctor
        public ProductOffer()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductOffer>
        {
            public Configuration()
            {
                HasOptional(Current => Current.ProductPrice).WithMany(Current => Current.ProductOffers).HasForeignKey(Current => Current.ProductPriceId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Offer).WithMany(Current => Current.ProductOffers).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "تخفیف")]
        public int OfferId { get; set; }
        public  Offer Offer { get; set; }

        [Display(Name = "محصول")]
        public int? ProductPriceId { get; set; }
        public  ProductPrice ProductPrice { get; set; }

        [Display(Name = "موجودی در کمپین")]
        [Required(ErrorMessage = "موجودی در کمپین باید وارد گردد")]
        public int Quantity { get; set; }

        [Display(Name = "حداکثر تعداد موجود در سبد ")]
        [Required(ErrorMessage = "حداکثر تعداد موجود در سبد باید وارد گردد")]
        public int MaxBasketCount { get; set; }

        [Required]
        [Display(Name = "ارزش تخفیف")]
        public int Value { get; set; }


        /*
         * 1- ثابت
         * 2- درصدی
         */
        [Required]
        [Display(Name = "نوع ارزش")]
        public Int16 CodeType { get; set; }

        public  ICollection<OrderRow> OrderRows { get; set; }
        #endregion
    }
}
