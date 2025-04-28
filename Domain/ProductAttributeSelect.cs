using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductAttributeSelect
    {
        #region Ctor
        public ProductAttributeSelect()
        {

        }
        public ProductAttributeSelect(int productAttributeCategorySelectId, int productId, string value)
        {
            ProductAttributeCategorySelectId = productAttributeCategorySelectId;
            ProductId = productId;
            Value = value;
            DisplayOrder = 0;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductAttributeSelect>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductAttributeGroupSelect).WithMany(Current => Current.ProductAttributeSelects).HasForeignKey(Current => Current.ProductAttributeCategorySelectId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductAttributeSelects).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        //public int AttributeId { get; set; }
        //public  ProductAttribute ProductAttribute { get; set; }
        [Display(Name = "خصوصیت")]
        public int ProductAttributeCategorySelectId { get; set; }
        public  ProductAttributeGroupSelect ProductAttributeGroupSelect { get; set; }


        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "مقدار")]
        public string Value { get; set; }
        
        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplayOrder { get; set; }


        public  ICollection<ProductPrice> ProductColorPrices { get; set; }
        public  ICollection<ProductPrice> ProductSizePrices { get; set; }
        public  ICollection<ProductPrice> ProductModelPrices { get; set; }
        public  ICollection<ProductPrice> ProductGarantyPrices { get; set; }
        public  ICollection<ProductPrice> ProductWeightPrices { get; set; }
        #endregion

    }
}
