using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class CourseRating : Object
    {
        #region Ctor
        public CourseRating()
        {

        }
        #endregion

        #region CourseRating
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<CourseRating>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.CourseRatings).WillCascadeOnDelete(true);
                HasRequired(Current => Current.User).WithMany(Current => Current.CourseRatings).HasForeignKey(Current => Current.UserId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        public double value { get; set; }

        #endregion
    }
}
