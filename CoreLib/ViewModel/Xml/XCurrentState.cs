using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XCurrentStates"), XmlType("XCurrentStates")]
    public class XCurrentState
    {

        public XCurrentState()
        {

        }
        [Required(ErrorMessage = "نام استان باید وارد شود")]
        [Display(Name = "استان")]
        public int StateId { get; set; }

    }
}
