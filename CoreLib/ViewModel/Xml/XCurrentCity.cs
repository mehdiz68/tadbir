using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XCurrentCities"), XmlType("XCurrentCities")]
    public class XCurrentCity
    {

        public XCurrentCity()
        {

        }
        [Required(ErrorMessage = "نام شهر باید وارد شود")]
        [Display(Name = "نام شهر")]
        public string Name { get; set; }

    }
}
