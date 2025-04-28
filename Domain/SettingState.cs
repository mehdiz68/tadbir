using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreLib.Infrastructure.Cryptography;
using Microsoft.AspNet.Identity;

namespace Domain
{

    public class SettingState 
    {
        #region Ctor
        public SettingState()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SettingState>
        {
            public Configuration()
            {
         
                HasRequired(Current => Current.Setting).WithMany(Current => Current.settingStates).HasForeignKey(Current => Current.SettingId);
                HasRequired(Current => Current.Province).WithMany(Current => Current.SettingStates).HasForeignKey(Current => Current.ProvinceId);
            }
        }
        #endregion

        #region Properties
        
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "استان اجباری است")]
        [Display(Name = "استان")]
        public int? ProvinceId { get; set; }
        public Province Province { get; set; }

        public Setting Setting { get; set; }

        [Required(ErrorMessage = "انتخاب تنظیم ، اجباری است")]
        [Display(Name = "تنظیم وب سایت")]
        public int SettingId { get; set; }
        #endregion
    }
}
