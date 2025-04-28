using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("ContentTypes"), XmlType("ContentTypes")]
    public class XContentType
    {
        public XContentType(int id, string name, string title, int languageid, string abst, string shortName)
        {
            Id = id;
            Name = name;
            Title = title;
            LanguageId = languageid;
            Abstract = abst;
            shortName = ShortName;
        }

        public XContentType()
        {

        }

        [Required(ErrorMessage = "آیدی باید وارد شود")]
        [Display(Name = "آیدی")]
        public int Id { get; set; }

        [Required(ErrorMessage = "نام باید وارد شود")]
        [Display(Name = "نام")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Name { get; set; }

        [Required(ErrorMessage = "عنوان باید وارد شود")]
        [Display(Name = "عنوان برای نمایش")]
        public string Title { get; set; }


        [Required(ErrorMessage = "نام کوتاه باید وارد شود")]
        [Display(Name = "نام کوتاه")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "چکیده باید وارد شود")]
        [Display(Name = "چکیده")]
        public string Abstract { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نمایش در جست و جوی سایت")]
        public bool InSearch { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ویدئویی؟")]
        public bool IsVideo { get; set; }


        [Display(Name = "تصویر")]
        public string Cover { get; set; }

        [Required(ErrorMessage = "انتخاب تنظیم ، اجباری است")]
        [Display(Name = "تنظیم وب سایت")]
        public int LanguageId { get; set; }

    }
}
