using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Order : BaseEntityDate
    {
        public Order()
        {
            Id = Guid.NewGuid();
            InsertDate = DateTime.Now;
        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Order>
        {
            public Configuration()
            {
                //HasMany(u => u.Wallets).WithMany(m => m.Orders).Map(m =>
                //{
                //    m.ToTable("OrderWallets");
                //    m.MapLeftKey("WalletId");  // because it is the "left" column, isn't it?
                //    m.MapRightKey("OrderId"); // because it is the "right" column, isn't it?
                //});

                HasOptional(Current => Current.Offer).WithMany(Current => Current.Orders).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.User).WithMany(Current => Current.Orders).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties


        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        [Required]
        [Display(Name = "تخصیص کد تخفیف نظر سفارش")]
        public bool SetGiftCode { get; set; }


        [Required]
        [Display(Name = "تکمیل نظرسنجی")]
        public bool compeleteRate{ get; set; }

        [Required]
        [Display(Name = "ارسال پیام ادمین")]
        public bool AdminNotification { get; set; }


        public bool CallbackSeen { get; set; }
        public bool CallbackFailedBank { get; set; }

        [Required]
        [Display(Name = "منقضی")]
        public bool IsExpire { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required]
        [Display(Name = "مشتری")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }




        [Required]
        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        [Required]
        [Display(Name = "مشاهده شده؟")]
        public bool Visited { get; set; }


        [Required]
        [Display(Name = "جدید؟")]
        public bool New { get; set; }


        [Display(Name = "تخفیف")]
        public int? OfferId { get; set; }
        public Offer Offer { get; set; }

        [Display(Name = "واحد پولی")]
        public short CurrencyId { get; set; }

        [Index("IX_BankOrderId", IsClustered = false, IsUnique = true)]
        public long BankOrderId { get; set; }
        public string CustomerOrderId { get; set; }

        [Display(Name = "تاریخ انقضا")]
        public DateTime ExpireDate { get; set; }

        public bool IsOld { get; set; }

        public PayUnSuccessfulReason? payUnSuccessfulReason  { get; set; }

        public ICollection<OrderWallet> OrderWallets { get; set; }
        public ICollection<OrderAttributeOrder> OrderAttributeSelects { get; set; }
        /// <summary>
        /// وضعیت سفارش چیست
        /// </summary>
        public ICollection<OrderState> OrderStates { get; set; }
        public ICollection<OrderRow> OrderRows { get; set; }
        public ICollection<UserBon> UserBons { get; set; }
        public ICollection<UserCodeGiftLog> UserCodeGiftLogs { get; set; }
        public ICollection<GeneralCodeGiftLog> GeneralCodeGiftLogs { get; set; }
        public ICollection<UserBonLog> UserBonLogs { get; set; }
        public ICollection<OrderDelivery> OrderDeliveries { get; set; }
        public ICollection<OrderRate> OrderRates { get; set; }

        #endregion
    }

    public enum PayUnSuccessfulReason
    {

        [Display(Name = "مشکل کارت اعتباری و رمز دوم")]
        CodeCardError,
        [Display(Name = "خطا در درگاه اینترنتی بانک")]
        TerminalError,
        [Display(Name = "از خرید منصرف شدم")]
        CancelOrderError
    }
}
