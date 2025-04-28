using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductCourseLesson
    {
        public ProductCourseLesson()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductCourseLesson>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductCourse).WithMany(Current => Current.ProductCourseLessons).HasForeignKey(Current => Current.ProductCourseId).WillCascadeOnDelete(true);
                HasRequired(Current => Current.Attachement).WithMany(Current => Current.ProductCourseLessons).HasForeignKey(Current => Current.AttachementId).WillCascadeOnDelete(false);

            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "انتخاب سرفصل")]
        public int ProductCourseId { get; set; }
        public  ProductCourse ProductCourse { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "فایل")]
        public Guid AttachementId { get; set; }
        public  attachment Attachement { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "حجم فایل(کیلوبایت)")]
        public double Capacity { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "مدت زمان ( ثانیه )")]
        public int Duration { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Display(Name = "رایگان؟")]
        public bool IsFree { get; set; }

        [Display(Name = "قیمت")]
        public long price { get; set; }


        public  ICollection<OrderRow> OrderRows { get; set; }

        #endregion
    }


}
