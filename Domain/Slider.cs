using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Slider : Object
    {
        #region Ctor
        public Slider()
        {

        }
        #endregion
        

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }


        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }


        /*
         * 1- content 
         * 2- category
         * 3- tag
         * 4- Product Category
         * 5- Gallery
         * 6- ContentType
         * 7- content main
         */
        [Required(ErrorMessage ="محل نمایش انتخاب نشده است")]
        [Display(Name = "محل نمایش(نوع صفحه)")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "محل نمایش انتخاب نشده است")]
        [Display(Name = "محل نمایش(صفحه)")]
        public int LinkId { get; set; }


        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16? LanguageId { get; set; }


        public  ICollection<SliderImage> SliderImages { get; set; }
        #endregion
    }
}
