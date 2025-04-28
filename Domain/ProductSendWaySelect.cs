using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSendWaySelect
    {
        public ProductSendWaySelect()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductSendWaySelect>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductSendWay).WithMany(Current => Current.ProductSendWaySelects).HasForeignKey(Current => Current.ProductSendWayId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductSendWaySelects).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "روش ارسال")]
        public int ProductSendWayId { get; set; }
        public  ProductSendWay ProductSendWay { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        #endregion
    }
}
