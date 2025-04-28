using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserAddress : Object
    {
        #region Ctor
        public UserAddress()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserAddress>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserAddresses).HasForeignKey(Current => Current.UserId);
                HasOptional(Current => Current.CityEntity).WithMany(Current => Current.UserAddresses).HasForeignKey(Current => Current.CityId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "وارد کردن نام و نام خانوادگی، اجباری است")]
        [Display(Name = "نام و نام خانوادگی  تحویل گیرنده")]
        public string FullName { get; set; }

        [Required(ErrorMessage = " تلفن همراه باید وارد شود.")]
        [Display(Name = "تلفن همراه  تحویل گیرنده")]
        [RegularExpression(@"(0)([ ]|,|-|[()]){0,2}9[0|1|2|3|4|9]([ ]|,|-|[()]){0,2}(?:[0-9]([ ]|,|-|[()]){0,2}){8}", ErrorMessage = "شماره موبایل صحیح نیست !")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "تلفن ثابت باید وارد شود.")]
        [Display(Name = "تلفن ثابت  تحویل گیرنده")]
        public string LandlinePhone { get; set; }

        [Required(ErrorMessage = "کد پستی باید وارد شود.")]
        [Display(Name = "کد پستی")]
        public string PostalCode { get; set; }

        public int? CityId { get; set; }
        public  City CityEntity { get; set; }

        [Required(ErrorMessage = "آدرس باید وارد شود.")]
        [Display(Name = "آدرس پستی")]
        public string Address { get; set; }


        [Display(Name = "پلاک")]
        public string AddressNumber { get; set; }

        [Display(Name = "واحد")]
        public string AddressUnit { get; set; }


        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }


        public  ICollection<OrderDelivery> OrderDeliveries { get; set; }
        #endregion
    }
}
