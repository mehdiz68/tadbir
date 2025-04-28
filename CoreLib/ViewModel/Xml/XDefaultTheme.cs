using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("XDefaultThemes"), XmlType("XDefaultThemes")]
    public class XDefaultTheme
    {
        #region Ctor
        public XDefaultTheme(int id, string title, Int16 TypeId, Int16 TypeOrderShow, string LinkId, Int16 NumberOfRecord, int DisplaySort, Int16 LanguageId, string HtmlData)
        {
            this.Id = id;
            this.Title = title;
            this.TypeId = TypeId;
            this.TypeOrderShow = TypeOrderShow;
            this.LinkId = LinkId;
            this.NumberOfRecord = NumberOfRecord;
            this.DisplaySort = DisplaySort;
            this.LanguageId = LanguageId;
            this.HtmlData = HtmlData;
        }
        public XDefaultTheme()
        {

        }
        #endregion
        
        #region Properties

        [Key]
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }


        [Display(Name = "عنوان جهت نمایش در قسمت")]
        public string Title { get; set; }

        /*
         * 1- content types
         * 2- tags
         * 3- category
         * 4- social
         * 5- html data
         * 6- Product
         * 7- Product Category
         * 8- Brand
         * 9- Gallery
         * 10-Slider
         * 11-Adveresting
         * 12-Amazing Offer
         */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "انتخاب نوع محتوا برای نمایش")]
        public Int16 TypeId { get; set; }

        /*
         * 1- New-Old
         * 2- Old-New
         * 3- A-Z
         * 4- Z-A
         * 5- Most Visit
         * 6- Least Visit
         * 7- Most Discuss
         * 8- Least Discuss
         * 9- Order by Display Sort
         * 10- Order by Random
         * 11- Order by OrderRow
         */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نوع نمایش")]
        public Int16 TypeOrderShow { get; set; }


        [Display(Name = "انتخاب لیست محتوا")]
        public string LinkId { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تعداد برای نمایش")]
        public Int16 NumberOfRecord { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }

        [Display(Name = "محتوای اچ تی ام ال")]
        public string HtmlData { get; set; }
        
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "المنت نگه دارنده ( HTML ID )")]
        public string Layout { get; set; }


        #endregion
    }
}