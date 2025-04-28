using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserOfferMessage
    {
        public UserOfferMessage()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserOfferMessage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Offer).WithMany(Current => Current.userOfferMessages).HasForeignKey(Current => Current.OfferId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }
                
        [Required(ErrorMessage ="متن وارد نشده است")]
        [Display(Name = "متن")]
        public string Text { get; set; }

        [Required(ErrorMessage = "تخفیف وارد نشده است")]
        [Display(Name = "تخفیف")]
        public int OfferId { get; set; }
        public  Offer Offer { get; set; }

        [Required(ErrorMessage = "تاریخ ثبت وارد نشده است")]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Required(ErrorMessage = "نوع پیام وارد نشده است")]
        [Display(Name = "نوع پیام")]
        public OfferMessageType OfferMessageType { get; set; }

        [Required(ErrorMessage = "وضعیت ارسال وارد نشده است")]
        [Display(Name = "وضعیت ارسال")]
        public bool state { get; set; }


        public ICollection<UserOfferMessageMember> UserOfferMessageMembers { get; set; }

        #endregion
    }

    public enum OfferMessageType
    {
        [Display(Name = "عمومی")]
        Public,
        [Display(Name = "خصوصی")]
        Private,
    }
}
