using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XCurrentInnerStates"), XmlType("XCurrentInnerStates")]
    public class XCurrentInnerState
    {

        public XCurrentInnerState()
        {

        }
        [Required(ErrorMessage = "آیدی استان باید وارد شود")]
        [Display(Name = "آیدی استان")]
        public int Id { get; set; }

    }
}
