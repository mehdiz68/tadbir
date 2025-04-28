using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class AdministratorPermission
    {
        #region Ctor
        public AdministratorPermission()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<AdministratorPermission>
        {
            public Configuration()
            {
                Property(current => current.UserId).IsUnicode(true).HasMaxLength(128).IsVariableLength().IsRequired();
                HasRequired(current => current.User).WithMany(user => user.AdministratorPermissions).HasForeignKey(x => x.UserId).WillCascadeOnDelete(true);
                HasRequired(current => current.AdministratorModule).WithMany(m => m.AdministratorPermissions).HasForeignKey(x => x.ModuleId).WillCascadeOnDelete(true);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }


        [Required]
        public int ModuleId { get; set; }
        public  AdministratorModule AdministratorModule { get; set; }

        [Required]
        [Display(Name = "سطح دسترسی")]
        public Int16 TypeAccess { get; set; }


        [Required]
        [Display(Name = "دریافت ایمیلِ اطلاع رسانی")]
        public bool NotificationEmail { get; set; }

        #endregion
    }
}
