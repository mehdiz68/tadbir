using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OfferProductCategory : Object
    {
        #region Ctor
        public OfferProductCategory()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OfferProductCategory>
        {
            public Configuration()
            {
                HasRequired(Current => Current.offer).WithMany(Current => Current.offerProductCategories).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.productCategory).WithMany(Current => Current.offerProductCategories).HasForeignKey(Current => Current.CatId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "تخفیف")]
        public int OfferId { get; set; }
        public  Offer offer{ get; set; }


        [Display(Name = "گروه")]
        public int CatId { get; set; }
        public ProductCategory productCategory{ get; set; }

        #endregion
    }
}
