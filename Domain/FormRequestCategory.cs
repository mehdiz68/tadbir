using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class FormRequestCategory : Object
    {
        #region Ctor
        public FormRequestCategory()
        {

        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "وارد کردن نام، اجباری است")]
        [Display(Name = "نام")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

      
        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }


        [Required]
        [Display(Name = "زبان")]
        public short LanguageId { get; set; }

        public ICollection<Domain.FormRequest> FormRequests { get; set; }
        #endregion
    }
}
