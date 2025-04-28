using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Source : Object
    {
        #region Ctor
        public Source()
        {

        }
        public Source(string source,string link)
        {
            this.Title = source;
            this.Link = link;
        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Source>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Content).WithMany(Current => Current.Sources).HasForeignKey(Current => Current.ContentId);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "نام منبع")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }
        
        [Display(Name = "آدرس منبع")]
        public string Link { get; set; }
        
        [Display(Name = "انتخاب محتوا")]
        public int ContentId { get; set; }
        public  Content Content { get; set; }
        #endregion
    }
}
