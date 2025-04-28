using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class NewsLetterEmail : Object
    {
        #region Ctor
        public NewsLetterEmail()
        {

        }
        #endregion

        


        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "وارد کردن ایمیل ، اجباری است")]
        [Display(Name = " ایمیل")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "آدرس ایمیل وارد نمایید")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        [Required]
        [Display(Name = "تاریخ عضویت")]
        public DateTime InsertDate { get; set; }

        [Required]
        [Display(Name = "تایید شده")]
        public bool IsVerified { get; set; }

        [Required]
        [Display(Name = "تایید شده")]
        public bool IsVerified2 { get; set; }

        #endregion
    }
}
