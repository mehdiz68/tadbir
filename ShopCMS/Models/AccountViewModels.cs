using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ahmadi.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "نام کاربری(شماره تلفن همراه)")]
        public string Mobile { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "نام کاربری(شماره تلفن همراه)")]
        public string Mobile { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage = "نام کاربری(شماره تلفن همراه) را وارد نمایید")]
        [RegularExpression(@"(0)([ ]|,|-|[()]){0,2}9[0|1|2|3|4|9]([ ]|,|-|[()]){0,2}(?:[0-9]([ ]|,|-|[()]){0,2}){8}", ErrorMessage = "شماره موبایل صحیح نیست !")]
        [Display(Name = "نام کاربری(شماره تلفن همراه)")]
        public string Mobile { get; set; }

        [StringLength(100, ErrorMessage = "طول رمز عبور حداقل باید 6 کارکتر باشد.", MinimumLength = 6)]
        [Required(ErrorMessage = "رمز عبور را وارد نمایید")]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class UserRegisterViewModel
    {

        [Required(ErrorMessage = " نام باید وارد شود.")]
        [Display(Name = "نام ")]
        public string FirstName { get; set; }


        [Display(Name = "درباره خود بنویسید")]
        public string About { get; set; }

        [Required(ErrorMessage = " نام خانوادگی باید وارد شود.")]
        [Display(Name = "نام خانوادگی ")]
        public string LastName { get; set; }

        [Required(ErrorMessage = " جنسیت باید وارد شود")]
        [Display(Name = "جنسیت")]
        public bool Gender { get; set; }



        [Required(ErrorMessage = "نام کاربری(شماره تلفن همراه) باید وارد شود.")]
        [Display(Name = "نام کاربری(شماره تلفن همراه)")]
        [Infrastructure.CustomAttribute.MyRemoteAttribute("doesUserNameExist", "Account", ErrorMessage = "نام کاربری (شماره تلفن همراه) وارد شده ، تکراری است")]
        public string Mobile { get; set; }


        [Display(Name = "ایمیل ")]
        public string Email { get; set; }



        [Required(ErrorMessage = "تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه")]
        [Infrastructure.CustomAttribute.MyRemoteAttribute("doesUserPhoneNumberExist", "Account", ErrorMessage = "تلفن همراه وارد شده ، تکراری است")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "رمز عبور باید وارد شود.")]
        [StringLength(100, ErrorMessage = "طول رمز عبور حداقل باید 6 کارکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "تایید رمز عبور، باید وارد شود.")]
        [DataType(DataType.Password)]
        [Display(Name = "ورود دوباره رمز")]
        [Compare("Password", ErrorMessage = "رمزهای عبور یکسان نیست")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage = "تلفن ثابت باید وارد شود.")]
        [Display(Name = "تلفن ثابت")]
        public string LandlinePhone { get; set; }


        [Required(ErrorMessage = "کد پستی باید وارد شود.")]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "استان باید انتخاب شود.")]
        [Display(Name = "استان")]
        public int State { get; set; }

        [Required(ErrorMessage = "شهر باید وارد شود.")]
        [Display(Name = "شهر")]
        public string City { get; set; }

        [Required(ErrorMessage = "آدرس باید وارد شود.")]
        [Display(Name = "آدرس")]
        public string Address { get; set; }
    }
    public class RegisterViewModel
    {
        [Display(Name = "ایمیل ")]
        public string Email { get; set; }

        [Required(ErrorMessage = " نام باید وارد شود.")]
        [Display(Name = "نام ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = " نام خانوادگی باید وارد شود.")]
        [Display(Name = "نام خانوادگی ")]
        public string LastName { get; set; }

        [Required(ErrorMessage = " جنسیت باید وارد شود")]
        [Display(Name = "جنسیت")]
        public bool Gender { get; set; }

        [RegularExpression(@"(0)([ ]|,|-|[()]){0,2}9[0|1|2|3|4|9]([ ]|,|-|[()]){0,2}(?:[0-9]([ ]|,|-|[()]){0,2}){8}", ErrorMessage = "شماره موبایل صحیح نیست !")]
        //[RegularExpression(@"^(?:(\u0660\u0669[\u0660-\u0669][\u0660-\u0669]{8})|(\u06F0\u06F9[\u06F0-\u06F9][\u06F0-\u06F9]{8})|(09[0-9][0-9]{8}))$", ErrorMessage = "شماره موبایل صحیح نیست !")]
        [Required(ErrorMessage = "نام کاربری(شماره تلفن همراه) باید وارد شود.")]
        [Display(Name = "نام کاربری(شماره تلفن همراه)")]
        [Infrastructure.CustomAttribute.MyRemoteAttribute("doesUserNameExist", "Account", ErrorMessage = "نام کاربری (شماره تلفن همراه) وارد شده ، تکراری است")]
        public string Mobile { get; set; }

        [Required]
        [Display(Name = "ایمیل تایید شده است")]
        public bool EmailConfirmed { get; set; }


        [Required]
        [Display(Name = "تلفن همراه تایید شده است")]
        public bool PhoneNumberConfirmed { get; set; }

        [Required]
        [Display(Name = "غیرفعال")]
        public bool Disable { get; set; }

        [Required(ErrorMessage = "رمز عبور باید وارد شود.")]
        [StringLength(100, ErrorMessage = "طول رمز عبور حداقل باید 6 کارکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }


        [Required(ErrorMessage = "تایید رمز عبور، باید وارد شود.")]
        [DataType(DataType.Password)]
        [Display(Name = "ورود دوباره رمز")]
        [Compare("Password", ErrorMessage = "رمزهای عبور یکسان نیست")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "آواتار ( 80 * 80 پیکسل )")]
        public System.Guid? Avatar { get; set; }

        [Display(Name = "درباره")]
        public string About { get; set; }



        [Display(Name = "تلفن ثابت")]
        public string LandlinePhone { get; set; }


        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "استان")]
        public int State { get; set; }

        [Display(Name = "شهر")]
        public string City { get; set; }

        [Display(Name = "آدرس")]
        public string Address { get; set; }


        [Display(Name = "پلاک")]
        public string AddressNumber { get; set; }

        [Display(Name = "واحد")]
        public string AddressUnit { get; set; }

        [Display(Name = "شهر")]
        public int CityId { get; set; }

    }

    public class EditUserViewModel
    {
        public string Id { get; set; }


        [Required(ErrorMessage = "نام کاربری (شماره تلفن همراه) باید وارد شود.")]
        [Display(Name = "نام کاربری(شماره تلفن همراه)")]
        public string Mobile { get; set; }

        [Display(Name = "ایمیل ")]
        [EmailAddress(ErrorMessage = "ایمیل صحیح نیست.")]
        public string Email { get; set; }

        [Required(ErrorMessage = " نام باید وارد شود.")]
        [Display(Name = "نام ")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = " نام خانوادگی باید وارد شود.")]
        [Display(Name = "نام خانوادگی ")]
        public string LastName { get; set; }


        [Required(ErrorMessage = " جنسیت باید وارد شود")]
        [Display(Name = "جنسیت")]
        public bool? Gender { get; set; }

        [Required]
        [Display(Name = "ایمیل تایید شده است")]
        public bool EmailConfirmed { get; set; }

        [Required]
        [Display(Name = "تلفن همراه تایید شده است")]
        public bool PhoneNumberConfirmed { get; set; }

        [Required]
        [Display(Name = "غیرفعال")]
        public bool Disable { get; set; }

        [Display(Name = "آواتار ( 80 * 80 پیکسل )")]
        public System.Guid? Avatar { get; set; }

        [Display(Name = "درباره")]
        public string About { get; set; }



        [Required(ErrorMessage = "تلفن ثابت باید وارد شود.")]
        [Display(Name = "تلفن ثابت")]
        public string LandlinePhone { get; set; }


        [Required(ErrorMessage = "کد پستی باید وارد شود.")]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        [Display(Name = "استان")]
        public int State { get; set; }

        [Display(Name = "شهر")]
        public string City { get; set; }

        [Required(ErrorMessage = "آدرس باید وارد شود.")]
        [Display(Name = "آدرس")]
        public string Address { get; set; }


        [Display(Name = "پلاک")]
        public string AddressNumber { get; set; }

        [Display(Name = "واحد")]
        public string AddressUnit { get; set; }


        [Required(ErrorMessage = "شهر باید وارد شود.")]
        [Display(Name = "شهر")]
        public int? CityId { get; set; }

        [Required(ErrorMessage = "استان باید وارد شود.")]
        [Display(Name = "استان")]
        public int? ProvienceId { get; set; }

        public Domain.attachment attachment { get; set; }


    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "نام کاربری (شماره تلفن همراه) باید وارد شود.")]
        [RegularExpression(@"(0)([ ]|,|-|[()]){0,2}9[0|1|2|3|4|9]([ ]|,|-|[()]){0,2}(?:[0-9]([ ]|,|-|[()]){0,2}){8}", ErrorMessage = "شماره موبایل صحیح نیست !")]
        [Display(Name = "نام کاربری ( شماره تلفن همراه )")]
        public string Mobile { get; set; }


        [Required(ErrorMessage = "رمز عبور باید وارد شود.")]
        [StringLength(100, ErrorMessage = "طول رمز عبور حداقل باید 6 کارکتر باشد.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Required(ErrorMessage = "تایید رمز عبور، باید وارد شود.")]
        [DataType(DataType.Password)]
        [Display(Name = "ورود دوباره رمز")]
        [Compare("Password", ErrorMessage = "رمزهای عبور یکسان نیست")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "نام کاربری (شماره تلفن همراه) باید وارد شود.")]
        [RegularExpression(@"(0)([ ]|,|-|[()]){0,2}9[0|1|2|3|4|9]([ ]|,|-|[()]){0,2}(?:[0-9]([ ]|,|-|[()]){0,2}){8}", ErrorMessage = "شماره موبایل صحیح نیست !")]
        [Display(Name = "نام کاربری(شماره تلفن همراه)")]
        public string Mobile { get; set; }
    }
}
