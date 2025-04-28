using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// جدول میانی برای ارتباط بین  اتریبیوت ها و دسته بندی محصولات
    /// </summary>
    public class ProductCategoryAttribute
    {
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductCategoryAttribute>
        {
            public Configuration()
            {
                //Property(current => current.UserId).IsOptional();
                HasRequired(Current => Current.ProductCategory).WithMany(Current => Current.ProductCategoryAttributes).HasForeignKey(Current => Current.ProductCategoryId);
                HasRequired(Current => Current.ProductAttribute).WithMany(Current => Current.ProductCategoryAttributes).HasForeignKey(Current => Current.ProductAttributeId);
             
            }
        }
        #endregion

        [Key]
        public int Id { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public int ProductCategoryId { get; set; }

        public  ProductAttribute ProductAttribute { get; set; }

        public int ProductAttributeId { get; set; }
        /// <summary>
        /// انتخاب میکند که ایا این اتریبیوت در سرچ آورده شود  یا نه 
        /// </summary>
        public bool IsSearchAbe { get; set; }
    }
}
