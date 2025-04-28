using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class MetaSeo : Object
    {
        #region Ctor
        public MetaSeo()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<MetaSeo>
        {
            public Configuration()
            {
                HasOptional(Current => Current.Tag).WithMany(Current => Current.MetaSeos).HasForeignKey(Current => Current.TagId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان را وارد نمایید")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Required(ErrorMessage = "توضیحات متا را وارد نمایید")]
        [Display(Name = "توضیحات متا")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string MetaDescription { get; set; }

        public Tag Tag { get; set; }
        public int? TagId { get; set; }
        #endregion
    }
}
