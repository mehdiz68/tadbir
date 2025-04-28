using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// جدول میانی برای ارتباط بین دسته بندی و گروه های مربوط به محصولات 
    /// </summary>
  public  class ProductAttributeGroupProductCategory
    {
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductAttributeGroupProductCategory>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductAttributeGroup).WithMany(Current => Current.ProductAttributeGroupProductCategorys).HasForeignKey(Current => Current.ProductAttributeGroupId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.ProductCategory).WithMany(Current => Current.ProductAttributeGroupProductCategorys).HasForeignKey(Current => Current.ProductCategoryId).WillCascadeOnDelete(false);
         
            }
        }

        #endregion

        [Key]
        public int Id { get; set; }
        public ProductAttributeGroup ProductAttributeGroup { get; set; }
        public int ProductAttributeGroupId { get; set; }

        public ProductCategory ProductCategory { get; set; }
        public int ProductCategoryId { get; set; }

    }
}
