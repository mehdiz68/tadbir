using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserBon : Object
    {
        #region Ctor
        public UserBon()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserBon>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserBons).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Order).WithMany(Current => Current.UserBons).HasForeignKey(Current => Current.OrderId);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "سفارش")]
        public Guid OrderId { get; set; }
        public  Order Order { get; set; }

        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Required]
        [Display(Name = "تاریخ انقضا")]
        public DateTime ExpireDate { get; set; }

        [Required]
        [Display(Name = "اعتبار اولیه")]
        public int Value { get; set; }

        [Required]
        [Display(Name = "مقدار مصرفی")]
        public int UsedValue { get; set; }

        [Required]
        [Display(Name = "وضعیت")]
        public bool state { get; set; }

        public  ICollection<UserBonLog> UserBonLogs { get; set; }
        #endregion
    }
}
