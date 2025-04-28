using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ProductRankSelect : Object
    {
        #region Ctor
        public ProductRankSelect()
        {

        }
        public ProductRankSelect(int id)
        {
            ProductId = id;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductRankSelect>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductRankSelects).HasForeignKey(Current => Current.ProductId);
                HasRequired(Current => Current.ProductRankGroupSelect).WithMany(Current => Current.ProductRankSelects).HasForeignKey(Current => Current.ProductRankGroupId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "انتخاب محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Display(Name = "انتخاب نوع امتیاز")]
        public int ProductRankGroupId { get; set; }
        public  ProductRankGroupSelect ProductRankGroupSelect { get; set; }

        public  ICollection<ProductRankSelectValue> ProductRankSelectValues { get; set; }

        #endregion
    }
}
