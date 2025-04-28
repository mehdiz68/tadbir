using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class SearchEngineFact : Object
    {
        #region Ctor
        public SearchEngineFact()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SearchEngineFact>
        {
            public Configuration()
            {
                HasOptional(Current => Current.attachment).WithMany(Current => Current.SearchEngineFacts).HasForeignKey(Current => Current.Cover);
                HasRequired(Current => Current.SearchEngineElementType).WithMany(Current => Current.SearchEngineFacts).HasForeignKey(Current => Current.ElementTypeID);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int SearchEngineFactID { get; set; }

        [Required]
        public int ElementID { get; set; }


        [Required]
        [Display(Name = "عنوان")]
        [MaxLength(300, ErrorMessage = "حداکثر طول کارکتر ، 300")]
        [Index("IX_SearchEngine_Title", IsClustered = false, IsUnique = false)]
        public string ElementTitle { get; set; }

        [Required]
        [Display(Name = "چکیده")]
        public string ElementAbstract { get; set; }


        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public  attachment attachment { get; set; }


        [Display(Name = "نوع سرچ")]
        public int ElementTypeID { get; set; }
        public  SearchEngineElementType SearchEngineElementType { get; set; }

        #endregion
    }
}
