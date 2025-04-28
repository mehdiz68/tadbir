using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OrderState : Object
    {
        #region Ctor
        public OrderState(Guid orderId,OrderStatus state,DateTime logdate,string description=null)
        {
            this.OrderId = orderId;
            this.state = state;
            this.LogDate = logdate;
            this.Description = description;
        }
        public OrderState()
        {
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OrderState>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Order).WithMany(Current => Current.OrderStates).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(true);
                HasRequired(Current => Current.OrderDelivery).WithMany(Current => Current.OrderStates).HasForeignKey(Current => Current.OrderDeliveryId);
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

        /*
         0- در انتظار تایید
         1- تایید سفارش
         2- تایید پرداخت
         3- پردازش انبار
         4- آماده ارسال
         5- ارسال شده
         6- تحویل داده شده
         7- لغو شده
         8- مرجوعی
             */
        [Required]
        [Display(Name = "وضعیت")]
        public OrderStatus state { get; set; }

        [Required]
        [Display(Name = "تاریخ و زمان ثبت")]
        public DateTime LogDate { get; set; }

        [MaxLength(200,ErrorMessage ="حداکثر طول ، 200 کارکتر")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        public CancelOrderReson? cancelOrderReson { get; set; }

        public  OrderDelivery OrderDelivery { get; set; }
        public int OrderDeliveryId { get; set; }

        #endregion
    }


    public enum OrderStatus
    {

        [Display(Name = "در انتظار تایید")]
        در_انتظار_تایید,
        [Display(Name = "تایید سفارش")]
        تایید_سفارش,
        [Display(Name = "تایید پرداخت")]
        تایید_پرداخت,
        [Display(Name = "پردازش انبار")]
        پردازش_انبار,
        [Display(Name = "آماده ارسال")]
        آماده_ارسال,
        [Display(Name = "ارسال شده")]
        ارسال_شده,
        [Display(Name = "تحویل داده شده")]
        تحویل_داده_شده,
        [Display(Name = "درخواست لغو")]
        درخواست_لغو,
        [Display(Name = "عدم تایید درخواست لغو")]
        عدم_تایید_درخواست_لغو,
        [Display(Name = "لغو شده")]
        لغو_شده,
        [Display(Name = "درخواست مرجوعی")]
        درخواست_مرجوعی,
        [Display(Name = "عدم تایید درخواست مرجوعی")]
        عدم_تایید_درخواست_مرجوعی,
        [Display(Name = "جبران مرجوعی")]
        جبران_مرجوعی,
        [Display(Name = "مرجوعی")]
        مرجوعی,
        [Display(Name = "عدم تایید سفارش")]
        عدم_تایید_سفارش
    }

    public enum CancelOrderReson
    {
        [Display(Name = "میخواهم کالای خود را ویرایش کنم")]
        InvalidEdit,

        [Display(Name = "سفارش تکراری ثبت کرده ام")]
        DulicateOrder,

        [Display(Name = "قیمت کالا زیاد است")]
        InvalidPrice,

        [Display(Name = "هزینه ارسال زیاد است")]
        InvalidSendPrice,

        [Display(Name = "میخواهم شیوه پرداخت را تغییر دهم")]
        InvalidSendway,

        [Display(Name = "کد تخفیفم اعمال نشده است")]
        InvalidCodeGift,

        [Display(Name = "از خرید خود منصرف شده ام")]
        InvalidCancel
    }
}
