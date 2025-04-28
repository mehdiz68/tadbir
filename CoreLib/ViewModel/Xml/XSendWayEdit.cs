using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XSendWayEdits"), XmlType("XSendWayEdits")]
    public class XSendWayEdit
    {

        public XSendWayEdit()
        {

        }
        [Required(ErrorMessage = "آیدی باید وارد شود")]
        [Display(Name = "آیدی")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "ویرایش شده ، باید وارد شود")]
        [Display(Name = "ویرایش شده؟")]
        public int Edit { get; set; }

    }
}
