using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserOfferMessageMember
    {
        public UserOfferMessageMember()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserOfferMessageMember>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserOfferMessageMembers).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.UserOfferMessage).WithMany(Current => Current.UserOfferMessageMembers).HasForeignKey(Current => Current.UserOfferMessageId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "دریافت کننده وارد نشده است")]
        [Display(Name = "دریافت کننده")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required(ErrorMessage = "پیام وارد نشده است")]
        public int UserOfferMessageId { get; set; }
        public UserOfferMessage UserOfferMessage { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public OfferMessageSendMessageType state { get; set; }

        [Required(ErrorMessage = "تاریخ ارسال وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }


        #endregion
    }

    public enum OfferMessageSendMessageType
    {
        [Display(Name = "در انتظار ارسال")]
        Waiting,
        [Display(Name = "ارسال شد")]
        Sent,
        [Display(Name = "ارسال نشد-خطای ناشناخته")]
        UnkownError,
        [Display(Name = "ارسال نشد-پنل پیامک منقضی شده")]
        PanelExpire,
        [Display(Name = "ارسال نشد-شارژ پیامک به اتمام رسیده")]
        InsufficientCredit,
        [Display(Name = "ارسال نشد-پیام فیلتر شد")]
        Filtered,
        [Display(Name = "ارسال نشد-ارسال در ساعات غیرمجاز")]
        ForbiddenHours,
        [Display(Name = "ارسال نشد-نام کاربری یا رمز عبور اشتباه")]
        InvalidUserNameOrPass,
    }
}
