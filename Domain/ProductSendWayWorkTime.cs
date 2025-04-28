using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSendWayWorkTime
    {
        public ProductSendWayWorkTime()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductSendWayWorkTime>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductSendWay).WithMany(Current => Current.ProductSendWayWorkTimes).HasForeignKey(Current => Current.ProductSendWayId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "روش ارسال")]
        public int ProductSendWayId { get; set; }
        public  ProductSendWay ProductSendWay { get; set; }

        /*
         0- شنبه
         ...
         6- جمعه
             */

        [Required]
        [Display(Name = "روز هفته")]
        public short WeekDay { get; set; }


        [Required]
        [Display(Name = "ساعت شروع")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "ساعت پایان")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public ICollection<OrderDelivery> OrderDeliveries { get; set; }

        #endregion
    }
}
