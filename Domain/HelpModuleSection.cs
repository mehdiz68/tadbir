using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class HelpModuleSection
    {
        #region Ctor
        public HelpModuleSection()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<HelpModuleSection>
        {
            public Configuration()
            {
                HasRequired(Current => Current.HelpModule).WithMany(Current => Current.HelpModuleSections).HasForeignKey(Current => Current.ModuleId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ماژول")]
        public int ModuleId { get; set; }
        public HelpModule HelpModule { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام بخش")]
        [MaxLength(50, ErrorMessage = "حداکثر طول کارکتر ، 50")]
        public string Name { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیح مختصر بخش")]
        public string Abstract { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیح کامل بخش")]
        public string Data { get; set; }

        public  ICollection<HelpModuleSectionField> HelpModuleSectionFields { get; set; }
        #endregion
    }
}
