using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class GeneralCodeGiftLog : Object
    {
        #region Ctor
        public GeneralCodeGiftLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<GeneralCodeGiftLog>
        {
            public Configuration()
            {
                HasRequired(Current => Current.GeneralCodeGift).WithMany(Current => Current.GeneralCodeGiftLogs).HasForeignKey(Current => Current.GeneralCodeGiftId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Order).WithMany(Current => Current.GeneralCodeGiftLogs).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        [Required]
        [Display(Name = "کد تخفیف عمومی")]
        public int GeneralCodeGiftId { get; set; }
        public  GeneralCodeGift GeneralCodeGift { get; set; }


        [Required]
        [Display(Name = "سفارش")]
        public Guid OrderId { get; set; }
        public  Order Order { get; set; }

        [Required]
        [Display(Name = "مقدار مصرفی")]
        public long Value { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }


        [Display(Name = "وضعیت")]
        public bool state { get; set; }

        #endregion
    }
}
