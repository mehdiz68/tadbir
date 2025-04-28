using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class StaticTextContent : Object
    {
        #region Ctor
        public StaticTextContent()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<StaticTextContent>
        {
            public Configuration()
            {
                HasRequired(Current => Current.staticTextCategory).WithMany(Current => Current.StaticTextContents).HasForeignKey(Current => Current.CatId).WillCascadeOnDelete(false);

                HasRequired(Current => Current.attachment).WithMany(Current => Current.StaticTextContents).HasForeignKey(Current => Current.Cover).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Display(Name = "دسته بندی")]
        public int CatId { get; set; }
        public StaticTextCategory staticTextCategory{ get; set; }

        [Display(Name = "نوع محتوا")]
        public StaticTextContentType staticTextContentType { get; set; }

        [Display(Name = "تصویر")]
        public Guid Cover { get; set; }
        public attachment attachment { get; set; }


        [Display(Name = "محتوا(متن،عکس،فایل و ... )")]
        public string Data { get; set; }

        #endregion
    }
}

public enum StaticTextContentType
{

    [Display(Name = "متن اول درخواست مشاوره")]
    متن_اول_درخواست_مشاوره,
    [Display(Name = "متن دوم درخواست مشاوره")]
    متن_دوم_درخواست_مشاوره,
    [Display(Name = "متن اول سمت چپ قسمت آمار")]
    متن_اول_سمت_چپ_قسمت_آمار,
    [Display(Name = "متن دوم سمت چپ قسمت آمار")]
    متن_دوم_سمت_چپ_قسمت_آمار,
    [Display(Name = "متن سوم سمت چپ قسمت آمار")]
    متن_سوم_سمت_چپ_قسمت_آمار,
    [Display(Name = "متن چهارم سمت چپ قسمت آمار")]
    متن_چهارم_سمت_چپ_قسمت_آمار,
    [Display(Name = "آیکن اول سمت راست قسمت آمار")]
    آیکن_اول_سمت_راست_قسمت_آمار,
    [Display(Name = "عدد آیکن اول سمت راست قسمت آمار")]
    عدد_آیکن_اول_سمت_راست_قسمت_آمار,
    [Display(Name = "متن آیکن اول سمت راست قسمت آمار")]
    متن_آیکن_اول_سمت_راست_قسمت_آمار,
    [Display(Name = "آیکن دوم سمت راست قسمت آمار")]
    آیکن_دوم_سمت_راست_قسمت_آمار,
    [Display(Name = "عدد آیکن دوم سمت راست قسمت آمار")]
    عدد_آیکن_دوم_سمت_راست_قسمت_آمار,
    [Display(Name = "متن آیکن دوم سمت راست قسمت آمار")]
    متن_آیکن_دوم_سمت_راست_قسمت_آمار,
    [Display(Name = "متن اول سمت چپ قسمت نظرات مشتریان")]
    متن_اول_سمت_چپ_قسمت_نظرات_مشتریان,
    [Display(Name = "متن دوم سمت چپ قسمت نظرات مشتریان")]
    متن_دوم_سمت_چپ_قسمت_نظرات_مشتریان,
}
