using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductQuestion : Object
    {
        #region Ctor
        public ProductQuestion()
        {

        }
        public ProductQuestion(int id)
        {
            ProductId = id;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductQuestion>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductQuestions).HasForeignKey(Current => Current.ProductId);
                HasOptional(Current => Current.ParentComment).WithMany(Current => Current.ChildComment).HasForeignKey(Current => Current.ParrentId);
                HasRequired(Current => Current.User).WithMany(Current => Current.ProductQuestions).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasMany(u => u.attachments).WithMany(m => m.ProductQuestions).Map(m =>
                {
                    m.ToTable("ProductQuestionFiles");
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
        [Display(Name = "متن نظر")]
        public string Message { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نظر دهنده")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Required]
        [Display(Name = "دیده شده؟")]
        public bool Visited { get; set; }


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public bool AdminAnswer { get; set; }
        public string FakeUserFullName { get; set; }


        [Required]
        [Display(Name = "لایک")]
        public int Like { get; set; }

        [Required]
        [Display(Name = "دیسلایک")]
        public int UnLike { get; set; }

        [Display(Name = "کامنت والد")]
        public int? ParrentId { get; set; }
        public virtual ProductQuestion ParentComment { get; set; }
        public virtual IList<ProductQuestion> ChildComment { get; set; }


        public ICollection<attachment> attachments { get; set; }


        [Display(Name = "انتخاب محصول")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        #endregion
    }
}
