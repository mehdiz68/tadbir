using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductCourse
    {
        public ProductCourse()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductCourse>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductCourses).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
            }
        }
        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "محصول اصلی")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان سرفصل")]
        public string Title { get; set; }


        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        public  ICollection<ProductCourseLesson> ProductCourseLessons { get; set; }

        #endregion
    }
}
