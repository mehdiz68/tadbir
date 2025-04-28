using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductMultipeItem
    {
        public ProductMultipeItem()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductMultipeItem>
        {
            public Configuration()
            {
                //Property(current => current.UserId).IsOptional();
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductMultipeMainItems).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductItem).WithMany(Current => Current.ProductMultipeSelectItems).HasForeignKey(Current => Current.ProductItemId).WillCascadeOnDelete(false);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "محصول اصلی")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Display(Name = "محصول انتخابی")]
        public int? ProductItemId { get; set; }
        public  Product ProductItem { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تعداد")]
        public int Quantity { get; set; }

        #endregion

    }
}
