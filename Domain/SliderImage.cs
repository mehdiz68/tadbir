using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SliderImage : Object
    {
        #region Ctor
        public SliderImage()
        {

        }
        public SliderImage(string title,string link,Guid cover,int displayorder,int sliderid)
        {
            Title = title;
            Link = link;
            Cover = cover;
            DisplaySort = displayorder;
            SliderId = sliderid;
        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<SliderImage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.attachment).WithMany(Current => Current.SliderImages).HasForeignKey(Current => Current.Cover);
                HasRequired(Current => Current.Slider).WithMany(Current => Current.SliderImages).HasForeignKey(Current => Current.SliderId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "عنوان اول")]
        [MaxLength(250, ErrorMessage = "حداکثر طول کارکتر ، 250")]
        public string Title { get; set; }

        [Display(Name = "عنوان دوم")]
        [MaxLength(250, ErrorMessage = "حداکثر طول کارکتر ، 250")]
        public string Title2 { get; set; }

        [Display(Name = "لینک")]
        //[MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        public string Link { get; set; }

        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "کاور(تصویر)")]
        public Guid Cover { get; set; }
        public  attachment attachment { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Display(Name = "اسلایدر")]
        public int SliderId{ get; set; }
        public  Slider Slider { get; set; }


        #endregion
    }
}
