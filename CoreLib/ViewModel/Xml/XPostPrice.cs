using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XPostPrices"), XmlType("XPostPrice")]
    public class XPostPrice
    {

        public XPostPrice()
        {

        }
        [Required(ErrorMessage = "آیدی باید وارد شود")]
        [Display(Name = "آیدی")]
        public int Id { get; set; }
        [Required]
        [Display(Name ="شهری تا 250 گرم")]
        public int Post_Inner_City_250 { get; set; }
        [Required]
        [Display(Name = "شهری از 251 گرم تا 500 گرم")]
        public int Post_Inner_City_500 { get; set; }
        [Required]
        [Display(Name = "شهری از 501 گرم تا 1000 گرم")]
        public int Post_Inner_City_1000 { get; set; }
        [Required]
        [Display(Name = "شهری از 1001 گرم تا 2000 گرم")]
        public int Post_Inner_City_2000 { get; set; }
        [Required]
        [Display(Name = "شهری مازاد بر 2 کیلوگرم هر کیلو و کسر آن")]
        public int Post_Inner_City_More_2000 { get; set; }
        [Required]
        [Display(Name = "درون استانی تا 250 گرم")]
        public int Post_Inner_State_250 { get; set; }
        [Required]
        [Display(Name = "درون استانی از 251 گرم تا 500 گرم")]
        public int Post_Inner_State_500 { get; set; }
        [Required]
        [Display(Name = "درون استانی از 501 گرم تا 1000 گرم")]
        public int Post_Inner_State_1000 { get; set; }
        [Required]
        [Display(Name = "درون استانی از 1001 گرم تا 2000 گرم")]
        public int Post_Inner_State_2000 { get; set; }
        [Required]
        [Display(Name = "درون استانی مازاد بر 2 کیلوگرم هر کیلو و کسر آن")]
        public int Post_Inner_State_More_2000 { get; set; }
        [Required]
        [Display(Name = "برون استانی همجوار تا 250 گرم")]
        public int Post_Outer_State_Neighbor_250 { get; set; }
        [Required]
        [Display(Name = "برون استانی همجوار از 251 گرم تا 500 گرم")]
        public int Post_Outer_State_Neighbor_500 { get; set; }
        [Required]
        [Display(Name = "برون استانی همجوار از 501 گرم تا 1000 گرم")]
        public int Post_Outer_State_Neighbor_1000 { get; set; }
        [Required]
        [Display(Name = "برون استانی همجوار از 1001 گرم تا 2000 گرم")]
        public int Post_Outer_State_Neighbor_2000 { get; set; }
        [Required]
        [Display(Name = "برون استانی همجوار مازاد بر 2 کیلوگرم هر کیلو و کسر آن")]
        public int Post_Outer_State_Neighbor_More_2000 { get; set; }
        [Required]
        [Display(Name = "برون استانی غیر همجوار تا 250 گرم")]
        public int Post_Outer_State_NoNeighbor_250 { get; set; }
        [Required]
        [Display(Name = "برون استانی غیر همجوار از 251 گرم تا 500 گرم")]
        public int Post_Outer_State_NoNeighbor_500 { get; set; }
        [Required]
        [Display(Name = "برون استانی غیر همجوار از 501 گرم تا 1000 گرم")]
        public int Post_Outer_State_NoNeighbor_1000 { get; set; }
        [Required]
        [Display(Name = "برون استانی غیر همجوار از 1001 گرم تا 2000 گرم")]
        public int Post_Outer_State_NoNeighbor_2000 { get; set; }
        [Required]
        [Display(Name = "برون استانی غیر همجوار مازاد بر 2 کیلوگرم هر کیلو و کسر آن")]
        public int Post_Outer_State_NoNeighbor_More_2000 { get; set; }


    }
}
