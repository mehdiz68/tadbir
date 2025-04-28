using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class FreeSendOffer : Object
    {
        #region Ctor
        public FreeSendOffer()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<FreeSendOffer>
        {
            public Configuration()
            {
                HasOptional(Current => Current.Product).WithMany(Current => Current.FreeSendOffers).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductCategory).WithMany(Current => Current.FreeSendOffers).HasForeignKey(Current => Current.CatId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Brand).WithMany(Current => Current.FreeSendOffers).HasForeignKey(Current => Current.BrandId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Offer).WithMany(Current => Current.FreeSendOffers).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);

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
        public int? ProductId { get; set; }
        public  Product Product { get; set; }
        
        [Display(Name = "گروه محصول")]
        public int? CatId { get; set; }
        public  ProductCategory ProductCategory { get; set; }

        [Display(Name = "برند")]
        public int? BrandId { get; set; }
        public  Brand Brand { get; set; }

        [Display(Name = "حداکثر هزینه ارسال")]
        public long MaxCost { get; set; }

        [Display(Name = "مبلغ سفارش")]
        public long? OrderSum { get; set; }

        public  ICollection<FreeSendOfferState> FreeSendOfferStates { get; set; }

        #endregion
    }
}
