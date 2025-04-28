using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductAttributeItem : Object
    {
        #region Ctor
        public ProductAttributeItem()
        {

        }
        public ProductAttributeItem(int attributeid,string value)
        {
            this.AttributeId = attributeid;
            this.Value = value;
       
        }
        public ProductAttributeItem(int id)
        {
            AttributeId = id;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductAttributeItem>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductAttribute).WithMany(Current => Current.ProductAttributeItems).HasForeignKey(Current => Current.AttributeId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "خصوصیت")]
        public int AttributeId { get; set; }
        public  ProductAttribute ProductAttribute { get; set; }
        
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "مقدار")]
        public string Value { get; set; }


        [Display(Name = "توضیحات")]
        public string Descr { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public Int16 DisplayOrder { get; set; }

        #endregion
    }
}