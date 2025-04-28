using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class City : Object
    {
        #region Ctor
        public City()
        {

        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<City>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Province).WithMany(Current => Current.Cities).HasForeignKey(Current => Current.ProvinceId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage = "وارد کردن استان، اجباری است")]
        [Display(Name = "استان")]
        public int ProvinceId { get; set; }
        public  Province Province { get; set; }

        [Required(ErrorMessage = "وارد کردن نام شهر ، اجباری است")]
        [Display(Name = "نام شهر")]
        [MaxLength(30, ErrorMessage = "حداکثر طول کارکتر ، 30")]
        public string Name { get; set; }

        public bool capital { get; set; }

        public  ICollection<Setting> Settings { get; set; }
        public  ICollection<ProductSendWayDetail> ProductSendWayDetails { get; set; }
        public  ICollection<FreeSendOfferState> FreeSendOfferStates { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
        public ICollection<UserAddress> UserAddresses { get; set; }

        #endregion
    }
}
