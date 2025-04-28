using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Comment : Object
    {
        #region Ctor
        public Comment()
        {

        }
        public Comment(int id)
        {
            ContentId = id;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Comment>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Content).WithMany(Current => Current.Comments).HasForeignKey(Current => Current.ContentId);
                HasOptional(Current => Current.ParentComment).WithMany(Current => Current.ChildComment).HasForeignKey(Current => Current.ParrentId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = " ایمیل")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "آدرس ایمیل وارد نمایید")]
        public string Email { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "متن نظر")]
        public string Message { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام و نام خانوادگی")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string FullName { get; set; }


        [Required]
        [Display(Name = "امتیاز مثبت")]
        public int PositiveRating { get; set; }

        [Required]
        [Display(Name = "امتیاز منفی")]
        public int NegativeRating { get; set; }

        [Required]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Required]
        [Display(Name = "دیده شده؟")]
        public bool Visited { get; set; }


        [Required]
        [Display(Name = "دیده شده توسط نویسنده؟")]
        public bool IsAuthorSeen { get; set; }

        [Required]
        [Display(Name = "تایید شده؟")]
        public bool IsActive { get; set; }


        [Display(Name = "کامنت والد")]
        public int? ParrentId { get; set; }
        public virtual Comment ParentComment { get; set; }
        public virtual  ICollection<Comment> ChildComment { get; set; }


        [Display(Name = "انتخاب محتوا")]
        public int ContentId { get; set; }
        public  Content Content { get; set; }
        #endregion
    }
}
