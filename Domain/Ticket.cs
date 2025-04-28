using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Ticket : Object
    {
        #region Ctor
        public Ticket()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Ticket>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.Tickets).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ParentTicket).WithMany(Current => Current.ChildTickets).HasForeignKey(Current => Current.ParrentId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.TicketCategory).WithMany(Current => Current.Tickets).HasForeignKey(Current => Current.CatId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        public string Code { get; set; }


        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required(ErrorMessage = "وارد کردن پرسش، اجباری است")]
        [Display(Name = "پرسش")]
        public string Message { get; set; }


        [Display(Name = "دسته بندی")]
        public int? CatId { get; set; }
        public virtual TicketCategory TicketCategory { get; set; }

        [Display(Name = "پاسخ به تیکت")]
        public int? ParrentId { get; set; }
        public virtual Ticket ParentTicket { get; set; }
        public virtual ICollection<Ticket> ChildTickets { get; set; }

        [Display(Name = "دیده شده؟")]
        public bool IsVisit { get; set; }

        [Display(Name = "دیده شده؟")]
        public bool UserIsVisit { get; set; }

        [Display(Name = "پاسخ؟")]
        public bool Answer { get; set; }

        [Required]
        [Display(Name = "تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "تاریخ ویرایش")]
        public DateTime? UpdateDate { get; set; }

        #endregion
    }
}
