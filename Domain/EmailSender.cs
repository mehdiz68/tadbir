using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CoreLib.Infrastructure.Cryptography;

namespace Domain
{

    public class EmailSender : BaseEntityCommon
    {
        #region Ctor
        public EmailSender()
        {

        }
        public EmailSender(string email, string password, string mailServer, int settingId)
        {
            this.Email = email;
            this.Password = password;
            this.MailServer = mailServer;
            this.SettingId = settingId;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<EmailSender>
        {
            public Configuration()
            {
                Property(current => current.Email).IsUnicode(false).HasMaxLength(255).IsVariableLength().IsRequired();
                Property(current => current.Password).IsUnicode(false).IsVariableLength().IsRequired();
                Property(current => current.MailServer).IsUnicode(true).HasMaxLength(255).IsVariableLength().IsRequired();
                Property(current => current.SettingId).IsRequired();
                HasRequired(Current => Current.Setting).WithMany(Current => Current.EmailSenders).HasForeignKey(Current => Current.SettingId);
            }
        }
        #endregion

        #region Properties
        [Required(ErrorMessage = "وارد کردن ایمیل ، اجباری است")]
        [Display(Name = " ایمیل روی دامنه وب سایت ")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "آدرس ایمیل وارد نمایید")]
        public string Email { get; set; }

        [Required(ErrorMessage = "وارد کردن رمز عبور ایمیل ، اجباری است")]
        [Display(Name = " رمز عبور ایمیل")]
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

        [Required(ErrorMessage = "وارد کردن نام سرور ایمیل ، اجباری است")]
        [Display(Name = " میل سرور روی دامنه وب سایت ")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        [RegularExpression(@"([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", ErrorMessage = "آدرس دامنه اینترنتی وارد نمایید. بدون http://")]
        public string MailServer { get; set; }

        [Required(ErrorMessage = "وارد کردن پورت، اجباری است")]
        [Display(Name = "پورت میل سرور")]
        [RegularExpression("([0-9]+)", ErrorMessage = "برای پورت فقط عدد وارد نمایید")]
        public int Port { get; set; }

        public Setting Setting { get; set; }

        [Required(ErrorMessage = "انتخاب تنظیم ، اجباری است")]
        [Display(Name = "تنظیم وب سایت")]
        public int SettingId { get; set; }
        #endregion
    }
}
