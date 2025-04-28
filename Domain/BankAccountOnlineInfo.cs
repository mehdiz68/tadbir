using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class BankAccountOnlineInfo : Object
    {
        #region Ctor
        public BankAccountOnlineInfo()
        {

        }
        #endregion

        #region Properties

        [Key, ForeignKey("BankAccount")]
        public int OnlineInfoId { get; set; }

        [Required]
        [Display(Name = "شماره ترمینال")]
        public string TerminalId { get; set; }

        [Required]
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "صفحه Callback")]
        public string CallbackUrl { get; set; }

        public  BankAccount BankAccount { get; set; }
        #endregion
    }
}
