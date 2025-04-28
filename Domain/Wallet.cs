using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Wallet : Object
    {
        #region Ctor
        public Wallet()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Wallet>
        {
            public Configuration()
            {
                HasOptional(Current => Current.ApplicationUser).WithMany(Current => Current.Wallets).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.ForWhat).WithMany(Current => Current.Wallets).HasForeignKey(Current => Current.ForWhatId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.BankAccount).WithMany(Current => Current.Wallets).HasForeignKey(Current => Current.BankAccountId).WillCascadeOnDelete(false);
              
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="نوع تراکنش اجباری است")]
        [Display(Name = "نوع تراکنش")]
        public int ForWhatId { get; set; }
        public  ForWhat ForWhat { get; set; }

        [Display(Name = "توضیحات کاربر")]
        public string Description { get; set; }

        [Display(Name = "توضیحات سیستم")]
        public string SystemDescription { get; set; }

        
        //[Required(ErrorMessage = "کاربر اجباری است")]
        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        [Required(ErrorMessage ="اجباری")]
        public DateTime InsertDate { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name ="تایید شده؟")]
        public bool State { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "مبلغ تراکنش")]
        public long Price { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name ="وضعیت")]
        public bool DepositOrWithdrawal { get; set; }

        /*
         * 1- پرداخت آنلاین ملت
         * 2- پرداخت آنلاین غیر مستقیم ( زرین پال )
         * 3- کارت به کارت
         * 4- پرداخت به پیک کارتخوان
         * 5- پرداخت به پیک نقدی
         * 6- فیش بانکی نقدی
         */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نوع پرداخت")]
        public int? PaymentType { get; set; }

        [Required(ErrorMessage = "حساب بانکی اجباری است")]
        [Display(Name = "حسابن بانکی")]
        public int? BankAccountId { get; set; }
        public  BankAccount BankAccount { get; set; }


        [Required]
        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }


        public  ICollection<OrderWallet> OrderWallets { get; set; }


        public  ICollection<WalletAttributeWallet> WalletAttributeWallets { get; set; }
        #endregion
    }
}
