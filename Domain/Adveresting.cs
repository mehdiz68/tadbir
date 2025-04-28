using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Adveresting : Object
    {
        #region Ctor
        public Adveresting()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Adveresting>
        {
            public Configuration()
            {
                HasRequired(Current => Current.attachment).WithMany(Current => Current.Adverestings).HasForeignKey(Current => Current.Cover);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }


        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        /*
         * 1- content 
         * 2- category
         * 3- tag
         * 4- Product Category
         * 5- Gallery
         * 6- Product Search
         * 7- base content 
         * 8- Content Type
         */
        [Required(ErrorMessage = "محل نمایش انتخاب نشده است")]
        [Display(Name = "محل نمایش(نوع صفحه)")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "محل نمایش انتخاب نشده است")]
        [Display(Name = "محل نمایش(صفحه)")]
        public int LinkId { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16? LanguageId { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ شروع(اختیاری)")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "تاریخ پایان(اختیاری)")]
        public DateTime? ExpireDate { get; set; }

        [NotMapped]
        public string ExpireDatestr { get; set; }



        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "کاور(تصویر)")]
        public Guid Cover { get; set; }
        public  attachment attachment { get; set; }


        [Display(Name = "اندازه")]
        public short AdverestingSizeId { get; set; }

        /*
     * 1- بالا 
     * 2- راست
     * 3- پایین
     * 4- چپ
     */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "موقعیت نمایش")]
        public short Position { get; set; }

        [Required(ErrorMessage ="اجباری")]
        [RegularExpression(@"^http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", ErrorMessage = "لینک درست نیست")]
        [Display(Name = "لینک")]
        public string Link { get; set; }

        /*
         true- لینک غیر مستقیم و پیشرفته همراه با آمار
         false- لینک ساده بدون آمار
             */
        [Required]
        [Display(Name = "نوع لینک")]
        public bool TypeLink { get; set; }

        public  IList<AdverestingLog> AdverestingLogs { get; set; }

      
        #endregion
    }
}
