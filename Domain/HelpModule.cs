using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class HelpModule
    {
        #region Ctor
        public HelpModule()
        {

        }
        #endregion

        #region Properties

        [Key]
        [Required(ErrorMessage ="اجباری")]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [MaxLength(50, ErrorMessage = "حداکثر طول کارکتر ، 50")]
        [Display(Name = "نام ماژول")]
        public string Name { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیح مختصر ماژول")]
        public string Abstract { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "توضیح کامل ماژول")]
        public string Data { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "گروه نمایش")]
        public int DisplayGroup { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public int DisplayOrder { get; set; }


        public  ICollection<HelpModuleSection> HelpModuleSections { get; set; }
        #endregion
    }
}
