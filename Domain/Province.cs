using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Province : Object
    {
        #region Ctor
        public Province()
        {

        }
        #endregion     

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "وارد کردن نام استان ، اجباری است")]
        [Display(Name = "نام استان")]
        [MaxLength(30, ErrorMessage = "حداکثر طول کارکتر ، 30")]
        public string Name { get; set; }

        [Required(ErrorMessage = "وارد کردن کد ناحیه ، اجباری است")]
        [Display(Name = "کد ناحیه")]
        public int AreaCode { get; set; }

        public int OldId { get; set; }

        public  ICollection<City> Cities { get; set; }
        public  ICollection<Setting> Settings { get; set; }
        public  ICollection<SettingState> SettingStates { get; set; }
        #endregion
    }
}
