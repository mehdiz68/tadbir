using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XFreightPrices"), XmlType("XFreightPrices")]
    public class XFreightPrice
    {

        public XFreightPrice()
        {

        }
        [Required(ErrorMessage = "آیدی باید وارد شود")]
        [Display(Name = "آیدی")]
        public int Id { get; set; }

        [Required(ErrorMessage = "هزینه باید وارد شود")]
        [Display(Name = "هزینه")]
        public int Price { get; set; }


    }
}
