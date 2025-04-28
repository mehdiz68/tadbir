using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserCodeGift : Object
    {
        #region Ctor
        public UserCodeGift()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserCodeGift>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserCodeGifts).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Offer).WithMany(Current => Current.UserCodeGifts).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "تخفیف")]
        public int OfferId { get; set; }
        public  Offer Offer { get; set; }

        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "ارزش تخفیف")]
        public int Value { get; set; }


        /*
         * 1- ثابت
         * 2- درصدی
         */
        [Required]
        [Display(Name = "نوع ارزش")]
        public Int16 CodeType { get; set; }

        [Required]
        [Display(Name = "کد تخفیف")]
        public string Code { get; set; }

        
        [Display(Name = "سقف مبلغ استفاده از کد تخفیف در هر سفارش")]
        public int MaxValue { get; set; }
        
        [Display(Name = "سقف تعداد استفاده از کد تخفیف")]
        public int CountUse{ get; set; }

        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }


        [Display(Name = "تاریخ و زمان انقضا")]
        public DateTime? ExpireDate { get; set; }

        public  ICollection<UserCodeGiftLog> UserCodeGiftLogs { get; set; }
        #endregion
    }
}
