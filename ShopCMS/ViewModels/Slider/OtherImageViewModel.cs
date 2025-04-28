using System;
using System.ComponentModel.DataAnnotations;

namespace ahmadi.ViewModels.Slider
{
    public class OtherImageViewModel
    {
        #region Ctor
        public OtherImageViewModel()
        {

        }
      
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }

        [Required]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }
        
        [Required]
        public int DisplaySort { get; set; }

        public string Src { get; set; }
        public string Link { get; set; }

        #endregion
    }
}
