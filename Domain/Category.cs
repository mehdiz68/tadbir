using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Category : Object
    {
        #region Ctor
        public Category()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Category>
        {
            public Configuration()
            {
                Property(current => current.Title).IsUnicode(true).HasMaxLength(100).IsVariableLength().IsRequired();
                Property(current => current.ParrentId).IsOptional();
                Property(current => current.Descr).IsUnicode(true).HasMaxLength(150).IsVariableLength().IsRequired();
                HasOptional(Current => Current.ParentCat).WithMany(Current => Current.ChildCategory).HasForeignKey(Current => Current.ParrentId);
                HasOptional(Current => Current.attachment).WithMany(Current => Current.Categories).HasForeignKey(Current => Current.Cover);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }



        [Required(ErrorMessage = "عنوان سئو باید وارد شود")]
        [Display(Name = "عنوان")]
        [MaxLength(80, ErrorMessage = "حداکثر طول کارکتر ، 80")]
        public string PageAddress { get; set; }

        [Display(Name = "دسته بندی اصلی")]
        public int? ParrentId { get; set; }
        public virtual Category ParentCat { get; set; }
        public virtual  IList<Category> ChildCategory { get; set; }

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
        public Int16? LanguageId { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public attachment attachment { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int Sort { get; set; }


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public bool IsVideo { get; set; }

        [Display(Name = "نمایش در صفحه اصلی")]
        public bool ShowInDefault { get; set; }

        [Required]
        [Display(Name = "نوع محتوا")]
        public int? ContentTypeId { get; set; }

        public  ICollection<Content> Contents { get; set; }
        #endregion
    }
}
