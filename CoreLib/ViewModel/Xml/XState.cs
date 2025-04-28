using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XStates"), XmlType("XStates")]
    public class XState
    {

        public XState()
        {

        }
        [Required(ErrorMessage = "آیدی باید وارد شود")]
        [Display(Name = "آیدی")]
        public int Id { get; set; }

        [Required(ErrorMessage = "نام باید وارد شود")]
        [Display(Name = "نام")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Name { get; set; }

        [Required(ErrorMessage = "انتخاب تنظیم ، اجباری است")]
        [Display(Name = "تنظیم وب سایت")]
        public int LanguageId { get; set; }

    }
}
