using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ProductRandomSetting : Object
    {
        #region Ctor
        public ProductRandomSetting()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductRandomSetting>
        {
            public Configuration()
            {
                HasRequired(Current => Current.setting).WithMany(Current => Current.ProductRandomSettings).HasForeignKey(Current => Current.SettingId);
                HasRequired(Current => Current.productCategory).WithMany(Current => Current.ProductRandomSettings).HasForeignKey(Current => Current.ProductCatId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        public int SettingId { get; set; }
        public Setting setting { get; set; }


        [Display(Name = "گروه محصول")]
        public int ProductCatId { get; set; }
        public ProductCategory productCategory { get; set; }

        #endregion
    }
}
