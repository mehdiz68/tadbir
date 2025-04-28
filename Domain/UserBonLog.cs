using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserBonLog : Object
    {
        #region Ctor
        public UserBonLog()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserBonLog>
        {
            public Configuration()
            {
                HasRequired(Current => Current.UserBon).WithMany(Current => Current.UserBonLogs).HasForeignKey(Current => Current.UserBonId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Order).WithMany(Current => Current.UserBonLogs).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(false);
            }
        }
        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        [Required]
        [Display(Name = "بن تخفیف تخفیف")]
        public int UserBonId { get; set; }
        public  UserBon UserBon { get; set; }


        [Required]
        [Display(Name = "سفارش")]
        public Guid OrderId { get; set; }
        public  Order Order { get; set; }

        [Required]
        [Display(Name = "اعتبار مصرفی")]
        public long Value { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "وضعیت")]
        public bool state { get; set; }

        #endregion
    }
}
