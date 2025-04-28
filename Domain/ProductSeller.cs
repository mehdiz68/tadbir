using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSeller
    {
        public ProductSeller()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductSeller>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Seller).WithMany(Current => Current.ProductSellers).HasForeignKey(Current => Current.SellerId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductSellers).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }


        [Display(Name = "فروشنده")]
        public int SellerId { get; set; }
        public  Seller Seller { get; set; }



        #endregion
    }
}
