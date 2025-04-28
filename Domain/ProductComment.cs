using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductComment : Object
    {
        #region Ctor
        public ProductComment()
        {

        }
        public ProductComment(int id)
        {

            ProductId = id;
        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductComment>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductComments).HasForeignKey(Current => Current.ProductId);
                HasRequired(Current => Current.User).WithMany(Current => Current.ProductComments).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasMany(u => u.attachments).WithMany(m => m.ProductComments).Map(m =>
                {
                    m.ToTable("ProductCommentFiles");
                    m.MapLeftKey("AttachementId");  // because it is the "left" column, isn't it?
                    m.MapRightKey("ProductCommentId"); // because it is the "right" column, isn't it?
                });
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نظر دهنده")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "متن")]
        public string Text { get; set; }


        [Required]
        [Display(Name = "دیده شده؟")]
        public bool Visited { get; set; }


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        [Display(Name = "موقت")]
        public bool IsTemp { get; set; }

        [Display(Name = "رضایت از خرید")]
        public ProductCommentSatisfaction? Satisfaction { get; set; }

        [Display(Name = "خریدار محصول؟")]
        public bool IsBuy { get; set; }

        [Required]
        [Display(Name = "مفید")]
        public int Useful { get; set; }

        [Required]
        [Display(Name = "غیر مفید")]
        public int Unuseful { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Required]
        [Display(Name = "تخصیص کد تخفیف")]
        public bool SetGiftCode { get; set; }

        public  ICollection<attachment> attachments { get; set; }
        public  IList<ProductCommentAdvantage> ProductCommentAdvantages { get; set; }

        public  IList<ProductCommentDisAdvantage> ProductCommentDisAdvantages { get; set; }
        public  ICollection<ProductRankSelectValue> ProductRankSelectValues { get; set; }
        #endregion
    }

    public enum ProductCommentSatisfaction
    {
        کاملا_راضی_و_پیشنهاد_میکنم,
        راضی_و_پیشنهاد_می_کنم,
        نظری_ندارم,
        ناراضی_و_پیشنهاد_نمی_کنم,
        کاملا_ناراضی_و_پیشنهاد_نمی_کنم
    }
}
