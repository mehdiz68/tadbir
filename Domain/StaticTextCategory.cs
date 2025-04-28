using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class StaticTextCategory : Object
    {
        #region Ctor
        public StaticTextCategory()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<StaticTextCategory>
        {
            public Configuration()
            {
                HasRequired(Current => Current.attachment).WithMany(Current => Current.StaticTextCategories).HasForeignKey(Current => Current.Cover);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "نوع محتوا")]
        public StaticTextType staticTextType { get; set; }

        [Display(Name = "تصویر")]
        public Guid Cover { get; set; }
        public attachment attachment { get; set; }

        [Required]
        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        public short LanguageId { get; set; }

        public ICollection<StaticTextContent> StaticTextContents { get; set; }

        #endregion
    }
}
public enum StaticTextType
{

    [Display(Name = "قسمت اول،درخواست مشاوره")]
    قسمت_اول_درخواست_مشاوره,
    [Display(Name = "قسمت دوم، آمار")]
    قسمت_دوم_آمار,
    [Display(Name = "قسمت سوم، نظرات مشتریان")]
    قسمت_سوم_نظرات_مشتریان,
}
