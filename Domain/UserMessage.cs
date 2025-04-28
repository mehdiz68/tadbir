using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserMessage
    {
        public UserMessage()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<UserMessage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.UserMessages).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.UserTo).WithMany(Current => Current.UserMessage2s).HasForeignKey(Current => Current.UserIdTo).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="عنوان وارد نشده است")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }
        
        [Required(ErrorMessage ="متن وارد نشده است")]
        [Display(Name = "متن")]
        public string Text { get; set; }

        [Required]
        [Display(Name = "وضعیت")]
        public bool state { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Required]  
        [Display(Name = "کاربر فرستنده")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Display(Name = "کابر گیرنده")]
        public string UserIdTo { get; set; }
        public  ApplicationUser UserTo { get; set; }

        #endregion
    }
}
