using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GalleryCategory : Object
    {
        #region Ctor
        public GalleryCategory()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<GalleryCategory>
        {
            public Configuration()
            {
                Property(current => current.Title).IsUnicode(true).HasMaxLength(100).IsVariableLength().IsRequired();
                //Property(current => current.ParrentId).IsOptional();
                Property(current => current.Descr).IsUnicode(true).HasMaxLength(150).IsVariableLength().IsRequired();
                //HasOptional(Current => Current.ParentCat).WithMany(Current => Current.ChildCategory).HasForeignKey(Current => Current.ParrentId);
                HasOptional(Current => Current.attachment).WithMany(Current => Current.GalleryCategories).HasForeignKey(Current => Current.Cover);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }



        [Required(ErrorMessage = "عنوان سئو باید وارد شود")]
        [Display(Name = "عنوان")]
        [MaxLength(80, ErrorMessage = "حداکثر طول کارکتر ، 80")]
        public string PageAddress { get; set; }

        //[Display(Name = "دسته بندی اصلی")]
        //public int? ParrentId { get; set; }
        //public  GalleryCategory ParentCat { get; set; }
        //public  IList<GalleryCategory> ChildCategory { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیحات متای گوگل")]
        [MaxLength(150, ErrorMessage = "حداکثر طول کارکتر ، 150")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "چکیده")]
        public string Abstract { get; set; }

        [Display(Name = "محتوا(متن،عکس،فایل و ... )")]
        public string Data { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public attachment attachment { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int Sort { get; set; }


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        public ICollection<GalleryImage> GalleryImages { get; set; }
        #endregion
    }
}