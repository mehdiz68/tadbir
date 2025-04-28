using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace ahmadi.ViewModels.Content
{
    public class ContentViewModel
    {
        #region Ctor
        public ContentViewModel()
        {

        }
        public ContentViewModel(Domain.Content content)
        {
            if (content != null)
            {
                this.Abstract = content.Abstract;
                this.Category = content.Category;
                this.CatId = content.CatId;
                this.attachment = content.attachment;
                this.Cover = content.Cover;
                this.BlogCover = content.BlogCover;
                this.ContentTypeId = content.ContentTypeId;
                this.Data = content.Data;
                this.Descr = content.Descr;
                this.Id = content.Id;
                this.IsActive = content.IsActive;
                this.BlogMain = content.BlogMain;
                this.LanguageId = content.LanguageId;
                this.OtherImages = content.OtherImages;
                this.Sources = content.Sources;
                this.Tags = content.Tags;
                this.Title = content.Title;
                this.User = content.User;
                this.UserId = content.UserId;
                this.IsDefault = content.IsDefault;
                this.IsAbout = content.IsAbout;
                this.IsContact = content.IsContact;
                this.HasContact = content.HasContact;
                this.IsRegister = content.IsRegister;
                this.IsSuperDeal = content.IsSuperDeal;
                this.PageAddress = content.PageAddress;
                this.Video = content.Video;
                this.VideoAttachment = content.VideoAttachment;
                this.Icon = content.Icon;
                this.ReadMinuts = content.ReadMinuts;
                this.Blogattachment = content.Blogattachment;
                this.BlogCover = content.BlogCover;
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

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

        [Display(Name = "چکیده")]
        public string Abstract { get; set; }

        [Display(Name = "محتوا(متن،عکس،فایل و ... )")]
        public string Data { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public attachment attachment { get; set; }

        [Display(Name = "کاور(تصویر صفحه اصلی وبلاگ)")]
        public Guid? BlogCover { get; set; }
        public attachment Blogattachment { get; set; }
        
        [Display(Name = "دسته بندی")]
        public int? CatId { get; set; }
        public Category Category { get; set; }

        [Display(Name = "نوع محتوا")]
        public int ContentTypeId { get; set; }


        [Display(Name = "زمان مطالعه ( دقیقه)")]
        public int ReadMinuts { get; set; }


        [Display(Name = "نویسنده")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        [Display(Name = "نمایش در صفحه اصلی وبلاگ")]
        public bool BlogMain { get; set; }

        [Required]
        [Display(Name = "صفحه اصلی")]
        public bool IsDefault { get; set; }

        [Required]
        [Display(Name = "صفحه درباره ما")]
        public bool IsAbout { get; set; }

        public bool IsSuperDeal { get; set; }

        [Required]
        [Display(Name = "صفحه تماس با ما")]
        public bool IsContact { get; set; }

        [Required]
        [Display(Name = "دارای فرم تماس با ما")]
        public bool HasContact { get; set; }

        [Required]
        [Display(Name = "محتوای شرایط ثبت نام")]
        public bool IsRegister { get; set; }

        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [Display(Name = "ویدئو")]
        public Guid? Video { get; set; }
        public virtual attachment VideoAttachment { get; set; }

        public ICollection<Source> Sources { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public ICollection<OtherImage> OtherImages { get; set; }

        #endregion
    }
}
