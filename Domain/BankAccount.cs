using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BankAccount
    {
        public BankAccount()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<BankAccount>
        {
            public Configuration()
            {
                HasOptional(Current => Current.ProductSendWay).WithMany(Current => Current.BankAccounts).HasForeignKey(Current => Current.ProductSendWayId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "مشخصات صاحب حساب")]
        public string AccountName { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "شماره کارت")]
        public string CardNumber { get; set; }


        [Required]
        [Display(Name = "شماره حساب")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(Name = "پرداخت نقدی ( فیش بانکی )")]
        public bool HasFish { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت نقدی ( فیش بانکی ) ( ساعت)")]
        public int HasFishHours { get; set; }

        [Required]
        [Display(Name = "پرداخت کارت به کارت ( شتاب )")]
        public bool HasCardToCard { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش کارت به کارت ( ساعت)")]
        public int CardNumberHours { get; set; }


        [Required]
        [Display(Name = "پرداخت به پیک نقدی")]
        public bool HasCourierDeliveryCash { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت به پیک نقدی ( ساعت)")]
        public int HasCourierDeliveryCashHours { get; set; }

        [Required]
        [Display(Name = "پرداخت به پیک کارتخوان")]
        public bool HasCourierDeliveryPos { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت به پیک کارتخوان ( ساعت)")]
        public int HasCourierDeliveryPosHours { get; set; }

        public  BankAccountOnlineInfo BankAccountOnlineInfo { get; set; }

        [Display(Name = "درگاه پرداخت آنلاین غیر مستقیم ( زرین پال )")]
        public string MerchantId { get; set; }

        [Display(Name = "صفحه Callback")]
        public string CallbackUrl { get; set; }

        [Required]
        [Display(Name = "حداکثر زمان نگهداری سفارش پرداخت آنلاین ( ساعت)")]
        public int OnliePaymentHours { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplayOrder { get; set; }

        [Required]
        [Display(Name = "بانک")]
        public Int16 BankId { get; set; }


        [Required]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        public  ICollection<Wallet> Wallets { get; set; }


        [Display(Name = "روش ارسال")]
        public int? ProductSendWayId { get; set; }
        public ProductSendWay ProductSendWay { get; set; }
        #endregion
    }
}
