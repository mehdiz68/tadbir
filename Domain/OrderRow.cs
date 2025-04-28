using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Domain
{
    public class OrderRow
    {
        public OrderRow()
        {

        }
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OrderRow>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.ProductId);
                HasRequired(Current => Current.Order).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.OrderId);
                HasOptional(Current => Current.ProductPrice).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.ProductPriceId);
                HasOptional(Current => Current.ProductOffer).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.ProductOfferId);
                HasOptional(Current => Current.ProductCourseLesson).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.ProductCourseLessonId);
                HasRequired(Current => Current.OrderDelivery).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.OrderDeliveryId);
                //HasRequired(Current => Current.ProductAttributeSelect).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.ColorId);
                //HasRequired(Current => Current.ProductAttributeSelect).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.ModelId);
                //HasRequired(Current => Current.ProductAttributeSelect).WithMany(Current => Current.OrderRows).HasForeignKey(Current => Current.SizeId);
            }
        }
        #endregion

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "سفارش")]
        public Guid OrderId { get; set; }
        public  Order Order { get; set; }

        [Required]
        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Display(Name = "تنوع محصول")]
        public int? ProductPriceId { get; set; }
        public  ProductPrice ProductPrice { get; set; }

        
        [Display(Name = "مالیات بر ارزش افزوده")]
        public int taxValue { get; set; }

        [Display(Name = "تک قسمت دوره آموزشی")]
        public int? ProductCourseLessonId { get; set; }
        public  ProductCourseLesson ProductCourseLesson { get; set; }

        [Required]
        [Display(Name = "تعداد")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "قیمت")]
        public long Price { get; set; }


        [Required]
        [Display(Name = "قیمت اصلی")]
        public long RawPrice { get; set; }

        //[Required]
        //[Display(Name = "تاریخ ارسال مرسوله به مشتری")]
        //public DateTime RequestDate { get; set; }


        public  ProductOffer ProductOffer { get; set; }
        public int? ProductOfferId { get; set; }


        public  OrderDelivery OrderDelivery { get; set; }
        public int OrderDeliveryId { get; set; }






    }
}
