using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreLib.Infrastructure.Cryptography;

namespace Domain
{

    public class SmsSender : BaseEntityCommon
    {
        #region Ctor
        public SmsSender()
        {

        }
        public SmsSender(string username, string password, string senderNumber, string domainName,Int16 companyId, int settingId)
        {
            this.Username = username;
            this.Password = password;
            this.SenderNumber = senderNumber;
            this.DomainName = domainName;
            this.CompanyId = companyId;
            this.SettingId = settingId;
        }
        public SmsSender(string username, string password, string senderNumber, Int16 companyId, int settingId)
        {
            this.Username = username;
            this.Password = password;
            this.SenderNumber = senderNumber;
            this.CompanyId = companyId;
            this.SettingId = settingId;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SmsSender>
        {
            public Configuration()
            {
                Property(current => current.Username).IsUnicode(false).HasMaxLength(50).IsVariableLength().IsRequired();
                Property(current => current.Password).IsUnicode(false).IsVariableLength().IsRequired();
                Property(current => current.SenderNumber).IsUnicode(false).IsVariableLength().IsRequired();
                Property(current => current.DomainName).IsUnicode(true).HasMaxLength(255).IsVariableLength().IsOptional();
                Property(current => current.SettingId).IsRequired();
                HasRequired(Current => Current.Setting).WithMany(Current => Current.SmsSenders).HasForeignKey(Current => Current.SettingId);
            }
        }
        #endregion

        #region Properties
        [Required(ErrorMessage = "وارد کردن نام کاربری ، اجباری است")]
        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور ، اجباری است")]
        [Display(Name = " رمز عبور")]
        public string Password { get; set; }


        [NotMapped]
        public string DecryptedPassword { get { return StringCipher.Decrypt(Password); } }
        public void EncryptPassword()
        {
            Password = StringCipher.Encrypt(Password);
        }
        public void DecryptPassword()
        {
            Password = StringCipher.Decrypt(Password);
        }

        [Required(ErrorMessage = "وارد کردن شماره ارسال ، اجباری است")]
        [Display(Name = "شماره ارسال یا APIKEY")]
        public string SenderNumber { get; set; }


        [Display(Name = "دامنه(اختیاری)")]
        public string DomainName { get; set; }


        [Required(ErrorMessage = "انتخاب شرکت سرویس دهنده ،اجباری است")]
        [Display(Name = "شرکت سرویس دهنده")]
        public Int16 CompanyId { get; set; }

        public Setting Setting { get; set; }

        [Required(ErrorMessage = "انتخاب تنظیم ، اجباری است")]
        [Display(Name = "تنظیم وب سایت")]
        public int SettingId { get; set; }

        [Display(Name = "انتخاب برای اطلاع رسانی عمومی")]
        public bool IsPublic { get; set; }
        #endregion
    }
}
