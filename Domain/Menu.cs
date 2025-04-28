using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Menu : Object
    {
        #region Ctor
        public Menu()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Menu>
        {
            public Configuration()
            {
                HasOptional(Current => Current.ParrentMenu).WithMany(Current => Current.ChildMenu).HasForeignKey(Current => Current.MenuID);
                HasOptional(Current => Current.attachment).WithMany(Current => Current.Menus).HasForeignKey(Current => Current.Cover);
                HasOptional(Current => Current.Homeattachment).WithMany(Current => Current.Menu2s).HasForeignKey(Current => Current.HomeCover);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="عنوان را وارد نمایید")]
        [Display(Name = "عنوان منو")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Display(Name = "منوی اصلی")]
        public int? MenuID { get; set; }
        public virtual  Menu ParrentMenu { get; set; }

        /*
        * 1- content 
        * 2- attachement
        * 3- category
        * 4- tag
        * 5- slider
        * 6- social
        * 7- contentType
        * 8- Product Category
        * 9- Gallery
        * 10- Attribute
        * 11- Base content
        * 12- Html Editor
        * 13- Product Search Category
        */
        [Display(Name = "نوع لینک داخلی")]
        public int? TypeId { get; set; }

        [Display(Name = "انتخاب محتوای داخلی")]
        public int? LinkId { get; set; }

        [Display(Name = "انتخاب محتوای داخلی")]
        public Guid? LinkUniqIdentifier { get; set; }

        [Display(Name = "لینک خارجی")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        public string OffLink { get; set; }

        [Display(Name = "محتوای دستی منو")]
        public string MenuData { get; set; }


        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        [Display(Name = "تصویر پشت زمینه منو")]
        public Guid? Cover { get; set; }
        public  attachment attachment { get; set; }


        [Display(Name = " آیکون منو در صفحه اصلی برای موبایل و تبلت ( در صورت استفاده در تم)")]
        public Guid? HomeCover { get; set; }
        public attachment Homeattachment { get; set; }

        /*
         * 1- Header
         * 2- Footer
         * 3- Both
         * 4- blog1
         * 5- blog2
         * 6- blogBoth
         * 7- Video1
         * 8- Video2
         * 9- VideoBoth
         * 
         */
        [Required]
        [Display(Name = "محل نمایش")]
        public Int16 PlaceShow { get; set; }

        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16? LanguageId { get; set; }

        public virtual  ICollection<Menu> ChildMenu{ get; set; }
        #endregion
    }
}
