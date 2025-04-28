using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class FormRequest : Object
    {
        #region Ctor
        public FormRequest()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<FormRequest>
        {
            public Configuration()
            {
                HasRequired(Current => Current.FormRequestCategory).WithMany(Current => Current.FormRequests).HasForeignKey(Current => Current.CatId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "وارد کردن نام، اجباری است")]
        [Display(Name = "نام")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Name { get; set; }


        [Required(ErrorMessage = "وارد کردن نام خانوادگی، اجباری است")]
        [Display(Name = "نام خانوادگی")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Family { get; set; }


        [Display(Name = "شماره تماس")]
        [MaxLength(15, ErrorMessage = "حداکثر طول کارکتر ، 15")]
        public string Tele { get; set; }

        [Display(Name = "پیام")]
        public string Message { get; set; }

        [Display(Name = "دیده شده؟")]
        public bool IsVisit { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }
        
        [Display(Name = "دسته بندی")]
        public int CatId { get; set; }
        public  FormRequestCategory FormRequestCategory { get; set; }
        #endregion
    }
}
