using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class HelpModuleSectionField
    {
        #region Ctor
        public HelpModuleSectionField()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<HelpModuleSectionField>
        {
            public Configuration()
            {
                HasRequired(Current => Current.HelpModuleSection).WithMany(Current => Current.HelpModuleSectionFields).HasForeignKey(Current => Current.SectionId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "بخش")]
        public int SectionId { get; set; }
        public HelpModuleSection HelpModuleSection { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام قسمت")]
        [MaxLength(50, ErrorMessage = "حداکثر طول کارکتر ، 50")]
        public string Name { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیح کامل قسمت")]
        public string Data { get; set; }
        #endregion
    }
}
