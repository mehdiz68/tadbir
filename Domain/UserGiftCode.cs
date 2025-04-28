using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserGiftCode : Object
    {
        #region Ctor
        public UserGiftCode()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserBon>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserBons).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Required]
        [Display(Name = "تاریخ و زمان انقضا")]
        public DateTime ExpireDate { get; set; }

        [Required]
        [Display(Name = "ارزش کد")]
        public int Value { get; set; }

        [Required]
        [Display(Name = "نوع تخفیف درصدی یا ثابت")]
        public bool CodeType { get; set; }

        [Required]
        [Display(Name = "وضعیت")]
        public bool state { get; set; }

        #endregion
    }
}
