using System.ComponentModel.DataAnnotations;

namespace ahmadi.ViewModels.Content
{
    public class SourceViewModel
    {
        #region Ctor
        public SourceViewModel()
        {

        }
        public SourceViewModel(string source,string sourceLink)
        {
            this.Source = source;
            this.SourceLink = sourceLink;
        }
        #endregion

        #region Properties

        [Required]
        [Display(Name = "نام منبع")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Source { get; set; }

        [Display(Name = "آدرس منبع")]
        public string SourceLink { get; set; }

        #endregion
    }
}
