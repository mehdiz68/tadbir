using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Content : BaseEntityFullAutoId
    {
        #region Ctor
        public Content()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Content>
        {
            public Configuration()
            {
                HasOptional(Current => Current.attachment).WithMany(Current => Current.Contents).HasForeignKey(Current => Current.Cover);
                HasOptional(Current => Current.Blogattachment).WithMany(Current => Current.Content3s).HasForeignKey(Current => Current.BlogCover);
                HasOptional(Current => Current.VideoAttachment).WithMany(Current => Current.Content2s).HasForeignKey(Current => Current.Video);
                HasOptional(Current => Current.Category).WithMany(Current => Current.Contents).HasForeignKey(Current => Current.CatId);
                Property(current => current.UserId).IsOptional();
                HasOptional(Current => Current.User).WithMany(Current => Current.Contents).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasMany(u => u.Tags).WithMany(m => m.Content).Map(m =>
                {
                    m.ToTable("ContentTags");
                    m.MapLeftKey("ContentId");  // because it is the "left" column, isn't it?
                    m.MapRightKey("TagId"); // because it is the "right" column, isn't it?
                });
            }
        }

        #endregion

        #region Properties

        [Required(ErrorMessage ="عنوان باید وارد شود")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Required(ErrorMessage = "عنوان سئو باید وارد شود")]
        [Display(Name = "عنوان")]
        [MaxLength(80, ErrorMessage = "حداکثر طول کارکتر ، 80")]
        public string PageAddress { get; set; }

        [Required(ErrorMessage = "توضیحات متای گوگل باید وارد شود")]
        [Display(Name = "توضیحات متای گوگل")]
        [MaxLength(150, ErrorMessage = "حداکثر طول کارکتر ، 150")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "چکیده باید وارد شود")]
        [Display(Name = "چکیده")]
        public string Abstract { get; set; }

        [Display(Name = "محتوا(متن،عکس،فایل و ... )")]
        public string Data { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public  attachment attachment { get; set; }


        [Display(Name = "کاور(تصویر برای صفحه اصلی وبلاگ)")]
        public Guid? BlogCover { get; set; }
        public  attachment Blogattachment { get; set; }

        [Display(Name = "ویدئو")]
        public Guid? Video { get; set; }
        public  attachment VideoAttachment { get; set; }

        [Required]
        [Display(Name = "تعداد بازدید")]
        public int Visits { get; set; }

        [Display(Name = "زمان مطالعه ( دقیقه)")]
        public int ReadMinuts { get; set; }

        [Required]
        [Display(Name = "تعداد لایک")]
        public int Link { get; set; }

        [Required]
        [Display(Name = "تعداد دیسلایک")]
        public int DisLike { get; set; }


        [Required]
        [Display(Name = "صفحه اصلی")]
        public bool IsDefault { get; set; }

        [Required]
        [Display(Name = "صفحه درباره ما")]
        public bool IsAbout { get; set; }

        [Required]
        [Display(Name = "صفحه تماس با ما")]
        public bool IsContact { get; set; }


        [Required]
        [Display(Name = "صفحه تخفیف")]
        public bool IsSuperDeal { get; set; }

        [Required]
        [Display(Name = "نمایش در صفحه اصلی بلاگ")]
        public bool BlogMain { get; set; }


        [Required]
        [Display(Name = "دارای فرم تماس با ما")]
        public bool HasContact { get; set; }

        [Required]
        [Display(Name = "محتوای شرایط ثبت نام")]
        public bool IsRegister { get; set; }


        [Display(Name = "دسته بندی")]
        public int? CatId { get; set; }
        public  Category Category { get; set; }

        [Display(Name = "نوع محتوا")]
        public int ContentTypeId { get; set; }


        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [Display(Name = "نویسنده")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        public  ICollection<Source> Sources { get; set; }

        public  ICollection<Tag> Tags { get; set; }

        public  ICollection<OtherImage> OtherImages { get; set; }

        public  ICollection<Comment> Comments { get; set; }

        public  ICollection<ContactUs> ContactUs { get; set; }
        public ICollection<Ticket> Tickets { get; set; }

        public  ContentRating ContentRating { get; set; }

        #endregion
    }
}
