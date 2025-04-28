using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductCommentDisAdvantage : Object
    {
        #region Ctor
        public ProductCommentDisAdvantage()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductCommentDisAdvantage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductComment).WithMany(Current => Current.ProductCommentDisAdvantages).HasForeignKey(Current => Current.ProductCommentId);
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
        [Display(Name = "نظر کاربر")]
        public int ProductCommentId { get; set; }
        public  ProductComment ProductComment { get; set; }

        #endregion
    }
}
