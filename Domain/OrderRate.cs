using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OrderRate : Object
    {
        #region Ctor

        public OrderRate()
        {
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OrderRate>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Order).WithMany(Current => Current.OrderRates).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(true);
                HasRequired(Current => Current.orderRateItem).WithMany(Current => Current.OrderRates).HasForeignKey(Current => Current.orderRateId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "سفارش")]
        public Guid OrderId { get; set; }
        public  Order Order { get; set; }

        [Required]
        [Display(Name = "آیتم امتیازدهی")]
        public int orderRateId { get; set; }
        public OrderRateItem orderRateItem { get; set; }


        [Required]
        [Display(Name = "امتیاز")]
        public OrderRateStatus state { get; set; }

        [Required]
        [Display(Name = "تاریخ و زمان ثبت")]
        public DateTime LogDate { get; set; }

        #endregion
    }


    public enum OrderRateStatus
    {

        [Display(Name = "کاملا راضی")]
        کاملا_راضی,
        [Display(Name = "راضی")]
        راضی,
        [Display(Name = "نظری ندارم")]
        نظری_ندارم,
        [Display(Name = "ناراضی")]
        ناراضی,
        [Display(Name = "کاملا ناراضی")]
        کاملا_ناراضی
    }

}
