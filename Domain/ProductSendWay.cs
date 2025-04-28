using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSendWay
    {
        public ProductSendWay()
        {

        }
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductSendWay>
        {
            public Configuration()
            {
                HasOptional(Current => Current.Image).WithMany(Current => Current.ProductSendWays).HasForeignKey(Current => Current.AttachementId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Tax).WithMany(Current => Current.ProductSendWays).HasForeignKey(Current => Current.TaxId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage = "عنوان روش ارسال باید وارد شود")]
        [Display(Name = "عنوان روش ارسال")]
        [MaxLength(200, ErrorMessage = "حداکثر طول کارکتر ، 200")]
        public string Title { get; set; }


        [Display(Name = "توضیحات روش ارسال")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "شروع زمان انتظار باید وارد شود")]
        [Display(Name = "شروع زمان انتظار ( روز )")]
        public int DeliveryStartDay { get; set; }

        [Required(ErrorMessage = "شروع انتظار باید وارد شود")]
        [Display(Name = "شروع زمان انتظار ( ساعت )")]
        public int DeliveryStartHour{ get; set; }


        [Required(ErrorMessage = "پایان زمان انتظار باید وارد شود")]
        [Display(Name = "پایان زمان انتظار ( روز )")]
        public int DeliveryHourDay { get; set; }

        [Display(Name = "لوگو")]
        public Guid? AttachementId { get; set; }
        public  attachment Image { get; set; }

        [Display(Name = "آدرس اینترنتی برای پیگیری")]
        public string TrackingUrl { get; set; }

        /*در صورت ترو شدن، از تنظیمات حمل و نقل عددش وارد میشود*/
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "اضافه کردن هزینه جابجایی")]
        public bool HasExtraPrice { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "حمل رایگان")]
        public bool IsFree { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "پس کرایه")]
        public bool PasKeraye { get; set; }

        /*بر اساس گرم یا قیمت؟*/
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "صورت حساب")]
        public bool InvoiceType { get; set; }

        [Display(Name = "مالیات")]
        public int? TaxId { get; set; }
        public  Tax Tax { get; set; }

        ///*تصمیم در مورد خارج از محدوده فعالیت. ترو اعمال بالاترین هزینه ، فالس غیر فعال کردن روش ارسال*/
        //[Required(ErrorMessage = "اجباری")]
        //[Display(Name = "خارج از محدوده فعالیت ها")]
        //public bool OutOfService { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        [Display(Name = "پیش فرض")]
        public bool IsDefault { get; set; }



        [Display(Name = "محاسبه قیمت برمبنای سیستم پست ایران")]
        public bool IsIrPost{ get; set; }


        [Display(Name = "قابلیت انتخاب بازه دریافت")]
        public bool DeliverSelectable { get; set; }


        [Display(Name = "توضیح زمان تحویل")]
        public string DeliverSelectDescr { get; set; }

        
        [Display(Name = "روش ارسال پیش فرض در زمان تخفیف ارسال رایگان")]
        public bool FreeOff { get; set; }

        public ICollection<OrderDelivery> OrderDeliveries { get; set; }
        public ICollection<ProductSendWaySelect> ProductSendWaySelects { get; set; }
        public ICollection<ProductSendWayBox> ProductSendWayBoxes { get; set; }
        public ICollection<ProductSendWayWorkTime> ProductSendWayWorkTimes { get; set; }
        public  ProductSendwayIrPostDetail ProductSendwayIrPostDetail { get; set; }
        public ICollection<BankAccount> BankAccounts { get; set; }

        #endregion
    }
}
