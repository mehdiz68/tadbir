using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductCommentAdvantage : Object
    {
        #region Ctor
        public ProductCommentAdvantage()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductCommentAdvantage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductComment).WithMany(Current => Current.ProductCommentAdvantages).HasForeignKey(Current => Current.ProductCommentId);
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
