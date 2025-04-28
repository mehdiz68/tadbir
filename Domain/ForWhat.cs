using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ForWhat : Object
    {
        #region Ctor
        public ForWhat()
        {

        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        /*
        1- پرداخت سفارش
        2- شارژ کیف پول
        3- پرداخت هزینه اگهی
            */
        [Required]
        [Display(Name = "نوع")]
        public ForWhatType ForWhatType { get; set; }

        public  ICollection<Wallet> Wallets { get; set; }

        #endregion
    }
    public enum ForWhatType
    {
        پرداخت_سفارش = 1,
        شارژ_کیف_پول = 2,
        هزینه_پروموشن = 3,
        هزینه_ثبت_آگهی = 4
    }


}
