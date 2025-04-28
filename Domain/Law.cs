using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Law : BaseEntityFullAutoId
    {
        #region Ctor
        public Law()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Law>
        {
            public Configuration()
            {
                HasOptional(Current => Current.ParentLaw).WithMany(Current => Current.ChildLaws).HasForeignKey(Current => Current.ParrentId);
            }
        }

        #endregion

        #region Properties

        [Required(ErrorMessage = "عنوان باید وارد شود")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }


        [Display(Name = "مصوب")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string date { get; set; }

        [Display(Name = "توضیح")]
        public string Data { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16? LanguageId { get; set; }

        [Required]
        [Display(Name = "تعداد بازدید")]
        public int Visits { get; set; }


        [Display(Name = "دسته بندی اصلی")]
        public int? ParrentId { get; set; }
        public Law ParentLaw { get; set; }
        public ICollection<Law> ChildLaws { get; set; }


        #endregion
    }
}
