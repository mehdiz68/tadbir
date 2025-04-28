using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductDisAdvantage : Object
    {
        #region Ctor
        public ProductDisAdvantage()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductDisAdvantage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductDisAdvantages).HasForeignKey(Current => Current.ProductId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product{ get; set; }

        #endregion
    }
}
