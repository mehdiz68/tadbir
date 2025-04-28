using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace CoreLib.ViewModel.Xml
{
    [Serializable]
    [XmlRoot("InternalThemes"), XmlType("InternalThemes")]
    public class XInternalTheme
    {
        #region Ctor
        public XInternalTheme(int id, Int16 viewid, Int16 LanguageId,int contenttypeid)
        {
            this.Id = id;
            this.ViewId=viewid;
            this.LanguageId=LanguageId;
            this.ContentTypeId = contenttypeid;
        }
        public XInternalTheme(int id, Int16 viewid, Int16 LanguageId)
        {
            this.Id = id;
            this.ViewId = viewid;
            this.LanguageId = LanguageId;
        }
        public XInternalTheme()
        {

        }
        #endregion

        #region Properties

        
        [Display(Name = "نوع محتوا")]
        public int ContentTypeId { get; set; }

        /*
        * 1- خبر
        * 2- مقاله
        * 3- بلاگ
        * 4- ...
        * 5- ...
        * .
        * .
        * .
        * .
        * -1 - دسته بندی
        * -2 - محتوا
        * -3 - برچسب انواع محتوا
        * -5 -  برچسب 
        * -6 -  محصول 
        * -7 -  دسته بندی محصول 
        * -8 -  لیست محصول 
        * -9 -  سبد 
        * -10 -  گالری 
        * -11 -  دسته بندی گالری 
        * -12 -  برند
        */
        [Required(ErrorMessage = "اجباری")]
        public int Id { get; set; }


        /*
          * 1- یک ستونه ، مدل 1
          * 2- دو ستونه ، مدل 2
          * 3- سه ستونه ، مدل 3
          */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "انتخاب صفحه")]
        public Int16 ViewId { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16 LanguageId { get; set; }



        #endregion
    }
}