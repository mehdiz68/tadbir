using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("Languages"), XmlType("Languages")]
    public class XLanguage
    {
        public XLanguage(Int16 id,string name,string shortName)
        {
            Id=id;
            Name=name;
            ShortName = shortName;
        }

        public XLanguage()
        {

        }

        [Required]
        public Int16 Id { get; set; }

        [Required(ErrorMessage = "نام زبان اجباری است")]
        [Display(Name = "نام زبان")]
        public string Name { get; set; }

        [Required(ErrorMessage = "نام کوتاه اجباری است")]
        [Display(Name = "نام کوتاه")]
        public string ShortName { get; set; }

    }
}
