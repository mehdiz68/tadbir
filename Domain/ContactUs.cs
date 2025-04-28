using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ContactUs : Object
    {
        #region Ctor
        public ContactUs()
        {

        }
        public ContactUs(int id)
        {
            ContentId = id;
        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ContactUs>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Content).WithMany(Current => Current.ContactUs).HasForeignKey(Current => Current.ContentId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "وارد کردن نام و نام خانوادگی ، اجباری است")]
        [Display(Name = "نام و نام خانوادگی")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "وارد کردن ایمیل ، اجباری است")]
        [Display(Name = " ایمیل")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "آدرس ایمیل وارد نمایید")]
        public string Email { get; set; }

        [Display(Name = "شماره تماس")]
        [MaxLength(15, ErrorMessage = "حداکثر طول کارکتر ، 15")]
        public string Tele { get; set; }

        [Required(ErrorMessage = "وارد کردن پیام، اجباری است")]
        [Display(Name = "پیام")]
        public string Message { get; set; }

        [Display(Name = "دیده شده؟")]
        public bool IsVisit { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }
        
        [Display(Name = "انتخاب محتوا")]
        public int ContentId { get; set; }
        public  Content Content { get; set; }
        #endregion
    }
}
