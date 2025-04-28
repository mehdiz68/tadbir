using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductRankSelectValue : Object
    {
        #region Ctor
        public ProductRankSelectValue()
        {

        }
        public ProductRankSelectValue(int id)
        {
            id = id;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductRankSelectValue>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.ProductRankSelectValues).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.ProductRankSelect).WithMany(Current => Current.ProductRankSelectValues).HasForeignKey(Current => Current.ProductRankSelectId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductComment).WithMany(Current => Current.ProductRankSelectValues).HasForeignKey(Current => Current.ProductCommentId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "انتخاب نوع امتیاز")]
        public int ProductRankSelectId { get; set; }
        public  ProductRankSelect ProductRankSelect { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نظر دهنده")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "مقدار")]
        public double Value { get; set; }


        [Display(Name = "نظر")]
        public int? ProductCommentId{ get; set; }
        public  ProductComment ProductComment { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "امتیاز مدیر فروشگاه")]
        public bool IsPrimary { get; set; }
        #endregion
    }
}
