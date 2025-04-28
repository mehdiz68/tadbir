using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductAttributeGroup
    {

        #region Ctor
        public ProductAttributeGroup()
        {

        }
        public ProductAttributeGroup( string Title, Int16 DisplayOrder, Int16 LanguageId)
        {
            this.Title = Title;
            this.DisplayOrder = DisplayOrder;
            this.LanguageId = LanguageId;

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductAttributeGroup>
        {
            public Configuration()
            {
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان گروه")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public Int16 DisplayOrder { get; set; }

        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }

        [Required]
        [Display(Name = "اصلی")]
        public bool Primary { get; set; }

        public  ICollection<ProductAttributeGroupSelect> ProductAttributeGroupSelects { get; set; }
        public  ICollection<ProductAttributeGroupProductCategory> ProductAttributeGroupProductCategorys { get; set; }
        #endregion
    }
}
