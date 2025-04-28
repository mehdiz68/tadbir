
using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductAttributeItemColor : Object
    {
        #region Ctor
        public ProductAttributeItemColor()
        {

        }
        public ProductAttributeItemColor(int attributeid,string value)
        {
            this.AttributeId = attributeid;
            this.Value = value;
       
        }
        public ProductAttributeItemColor(int id)
        {
            AttributeId = id;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductAttributeItemColor>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductAttribute).WithMany(Current => Current.ProductAttributeItemColors).HasForeignKey(Current => Current.AttributeId).WillCascadeOnDelete(false);
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
        [Display(Name = "رنگ")]
        public string Color { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "مقدار")]
        public string Value { get; set; }


        #endregion
    }
}