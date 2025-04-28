using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;

namespace Domain
{
    public class ProductPriceGroupModification 
    {
        #region Ctor
        public ProductPriceGroupModification()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductPriceGroupModification>
        {
            public Configuration()
            {
                //Property(current => current.UserId).IsOptional();
                HasRequired(Current => Current.User).WithMany(Current => Current.ProductPriceGroupModifications).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Brand).WithMany(Current => Current.ProductPriceGroupModifications).HasForeignKey(Current => Current.BrandId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductCategory).WithMany(Current => Current.ProductPriceGroupModifications).HasForeignKey(Current => Current.CatId).WillCascadeOnDelete(false);

             
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "گروه و زیرگروه محصول")]
        public int? CatId { get; set; }
        public  ProductCategory ProductCategory { get; set; }

        [Display(Name = "برند")]
        public int? BrandId { get; set; }
        public  Brand Brand { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "ثابت یا درصدی؟")]
        public bool ValueType { get; set; }

        [Required]
        [Display(Name = "مقدار")]
        public int Value { get; set; }

        [Required]
        [Display(Name = "افزایش یا کاهش")]
        public bool IncreaseType { get; set; }

        [Required]
        [Display(Name = "برندهای گروه؟")]
        public bool BrandOrCat { get; set; }
        #endregion
    }
}
