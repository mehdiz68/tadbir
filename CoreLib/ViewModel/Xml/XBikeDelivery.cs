using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XBikeDeliveries"), XmlType("XBikeDeliveries")]
    public class XBikeDelivery
    {

        public XBikeDelivery()
        {

        }
        [Required(ErrorMessage = "آیدی باید وارد شود")]
        [Display(Name = "آیدی")]
        public int Id { get; set; }

        [Required(ErrorMessage = "هزینه باید وارد شود")]
        [Display(Name = "هزینه")]
        public int Price { get; set; }

        [Required(ErrorMessage = "انتخاب استان جاری فروشگاه ، اجباری است")]
        [Display(Name = "استان جاری فروشگاه")]
        public int CurrentState { get; set; }

    }
}
