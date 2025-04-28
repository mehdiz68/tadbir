using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class AdministratorModule
    {
        #region Ctor
        public AdministratorModule()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<AdministratorModule>
        {
            public Configuration()
            {
                Property(current => current.Name).IsUnicode(true).HasMaxLength(50).IsVariableLength().IsRequired();
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام ماژول")]
        public string Name { get; set; }



        public  ICollection<AdministratorPermission> AdministratorPermissions { get; set; }
        #endregion
    }
}
