using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductAccessory
    {
        public ProductAccessory()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductAccessory>
        {
            public Configuration()
            {
                HasRequired(Current => Current.MainProduct).WithMany(Current => Current.ProductMainAccessory).HasForeignKey(Current => Current.MainProductId).WillCascadeOnDelete(false);
                //HasOptional(Current => Current.AcceessoryProduct).WithMany(Current => Current.ProductAccessory).HasForeignKey(Current => Current.AcceessoryProductId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "محصول اصلی")]
        public int MainProductId { get; set; }
        public  Product MainProduct { get; set; }

        
        //[Display(Name = "محصول جانبی")]
        //public int? AcceessoryProductId { get; set; }
        //public  Product AcceessoryProduct { get; set; }

        #endregion
    }
}
