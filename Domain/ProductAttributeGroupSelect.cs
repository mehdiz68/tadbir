using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductAttributeGroupSelect : Object
    {
        #region Ctor
        public ProductAttributeGroupSelect()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductAttributeGroupSelect>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductAttribute).WithMany(Current => Current.ProductAttributeGroupSelects).HasForeignKey(Current => Current.AttributeId).WillCascadeOnDelete(true);
                HasOptional(Current => Current.ProductAttributeGroup).WithMany(Current => Current.ProductAttributeGroupSelects).HasForeignKey(Current => Current.GroupId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductAttributeTab).WithMany(Current => Current.ProductAttributeGroupSelects).HasForeignKey(Current => Current.TabId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "خصوصیت")]
        [Index("IX_AttributeId", IsClustered = false, IsUnique = false)]
        public int AttributeId { get; set; }
        public  ProductAttribute ProductAttribute { get; set; }

        [Display(Name = "گروه دسته بندی")]
        [Index("IX_GroupId", IsClustered = false, IsUnique = false)]
        public int? GroupId { get; set; }
        public  ProductAttributeGroup ProductAttributeGroup { get; set; }

        [Display(Name = "تب")]
        [Index("IX_TabId", IsClustered = false, IsUnique = false)]
        public int? TabId { get; set; }
        public  ProductAttributeTab ProductAttributeTab { get; set; }


        [Required]
        [Display(Name = "ترتیب نمایش خصوصیت")]
        public int DisplayOrder { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش دسته")]
        public int DisplayGroupOrder { get; set; }

        public  IList<ProductAttributeSelect> ProductAttributeSelects { get; set; }

        #endregion

    }
}
