using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductLetmeknow
    {
        public ProductLetmeknow()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductLetmeknow>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.ProductLetmeknows).HasForeignKey(Current => Current.UserId);
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductLetmeknows).HasForeignKey(Current => Current.ProductId);
            }
        }
        #endregion

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [Display(Name = "کاربر")]
        public  ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Required]
        [Display(Name = "پیشنهاد شگفت انگیز")]
        public bool AmazingOffer { get; set; }

        [Required]
        [Display(Name = "موجود شدن")]
        public bool Available { get; set; }

        /*
         1- ایمیل
         2- اس ام اس
         3- سیستم پیام فروشگاه
         4- ایمیل و اس ام اس
         5- ایمیل و سیستم پیام فروشگاه
         6- اس ام اس و سیستم پیام فروشگاه
         7- ایمیل ، اس ام اس ، سیستم پیام فروشگاه
             */
        [Required]
        [Display(Name = "نوع اطلاع رسانی")]
        public Int16 NotificationType { get; set; }

        [Required]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }


        [Display(Name = "اطلاع رسانی شده")]
        public bool Notofied{ get; set; }

        [Display(Name = "اطلاع رسانی شده")]
        public bool NotofiedSms { get; set; }


        [Display(Name = "اطلاع رسانی شده")]
        public bool NotofiedEmail { get; set; }
    }
}
