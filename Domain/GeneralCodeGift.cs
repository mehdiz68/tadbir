using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class GeneralCodeGift : Object
    {
        #region Ctor
        public GeneralCodeGift()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<GeneralCodeGift>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Offer).WithMany(Current => Current.generalCodeGifts).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);
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



        [Required]
        [Display(Name = "نوع کد تخفیف")]
        public GeneralCodeType generalCode { get; set; }

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

        public ICollection<GeneralCodeGiftLog> GeneralCodeGiftLogs { get; set; }

        #endregion
    }

    public enum GeneralCodeType
    {
        [Display(Name = "تخفیف سفارش عمومی")]
        تخفیف_سفارش_عمومی,
        [Display(Name = "تخفیف اولین سفارش")]
        تخفیف_اولین_سفارش,
        [Display(Name = "تخفیف ارسال رایگان")]
        تخفیف_ارسال_رایگان,

    }
}
