using Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ahmadi.Models
{

    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool BrowserRemembered { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string About { get; set; }
        public bool? Gender { get; set; }
        public string LandlinePhone { get; set; }
        public string NationalCode { get; set; }
        public int State { get; set; }
        public string City { get; set; }
        public string CardNumber { get; set; }
        public int BonCount { get; set; }
        public int CodeCount { get; set; }
        public int NoticeCount { get; set; }
        public int FavCount { get; set; }
        public long wallet { get; set; }
        public string Address { get; set; }
        public bool IsInNewsLetter { get; set; }
        public DateTime? BirthDate { get; set; }
        public System.Guid? Avatar { get; set; }


        public Domain.attachment Avatarattachment { get; set; }
        public ApplicationUser AppUser { get; set; }
        public int MessageCout { get; set; }

        public int QuestionCount { get; set; }
        public int AnswerCount { get; set; }
        public int AnswerNotVisitedCount { get; set; }
    }
    public class EditProfileViewModel
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = " نام ")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }


        [Display(Name = "ایمیل ")]
        [EmailAddress(ErrorMessage = "ایمیل صحیح نیست.")]
        public string Email { get; set; }

        [Required(ErrorMessage = " نام خانوادگی باید وارد شود.")]
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage ="جنسیت را مشخص نمایید")]
        public bool? Gender { get; set; }


        [Display(Name = "آواتار ( 80 * 80 پیکسل)")]
        public System.Guid? Avatar { get; set; }
        public Domain.attachment Avatarattachment { get; set; }

        [Display(Name = "درباره")]
        public string About { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "تلفن ثابت")]
        public string LandlinePhone { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Required(ErrorMessage = " کد پستی باید وارد شود.")]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "استان")]
        public int State { get; set; }


        [Required(ErrorMessage = " شهر باید وارد شود.")]
        public int? CityId { get; set; }
        public City CityEntity { get; set; }

        [Display(Name = "شهر")]
        public string City { get; set; }

        [Required(ErrorMessage = " آدرس باید وارد شود.")]

        [Display(Name = "آدرس")]
        public string Address { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Required(ErrorMessage = " کد ملی باید وارد شود.")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "تاریخ تولد")]
        public DateTime? BirthDate { get; set; }


        [RegularExpression("^[0-9]*$", ErrorMessage = "فقط عدد")]
        [Display(Name = "شماره کارت بانکی")]
        public string CardNumber { get; set; }


        [Display(Name = "پلاک")]
        public string AddressNumber { get; set; }

        [Display(Name = "واحد")]
        public string AddressUnit { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "رمز عبور فعلی خالی است")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور فعلی")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور جدید")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ورود دوباره رمز عبورِ جدید")]
        [Compare("NewPassword", ErrorMessage = "رمز های عبور وارد شده یکسان نیستند.")]
        public string ConfirmPassword { get; set; }
    }
}