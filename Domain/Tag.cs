using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Tag : Object
    {
        #region Ctor
        public Tag()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Tag>
        {
            public Configuration()
            {
                Property(current => current.TagName).IsUnicode(true).HasMaxLength(100).IsVariableLength().IsRequired();
                Property(current => current.LanguageId).IsOptional();
                HasOptional(Current => Current.ProductCategory).WithMany(Current => Current.Tags).HasForeignKey(Current => Current.CatId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required(ErrorMessage ="نام برچسب را وارد نمایید")]
        [Display(Name = "برچسب")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        [Index("IX_TagName", IsClustered = false, IsUnique = true)]
        public string TagName { get; set; }

        [Required(ErrorMessage = "عنوان را وارد نمایید")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Required(ErrorMessage = "توضیحات متا را وارد نمایید")]
        [Display(Name = "توضیحات متا")]
        [MaxLength(160, ErrorMessage = "حداکثر طول کارکتر ، 160")]
        public string MetaDescription { get; set; }


        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }

        public  ICollection<Content> Content { get; set; }

        public  ICollection<Product> Product { get; set; }

        [Display(Name = "گروه و زیرگروه")]
        public int? CatId { get; set; }
        public  ProductCategory ProductCategory { get; set; }
        public ICollection<MetaSeo> MetaSeos { get; set; }
        #endregion
    }
}
