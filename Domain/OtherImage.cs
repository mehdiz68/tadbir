using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OtherImage : Object
    {
        #region Ctor
        public OtherImage()
        {

        }

        public OtherImage(string title, int displaySort, Guid? cover)
        {
            this.Title = title;
            this.DisplaySort = displaySort;
            this.Cover = cover;
        }
        public OtherImage(string title, int displaySort, string src, Guid? cover, int Contentid)
        {
            this.Title = title;
            this.DisplaySort = displaySort;
            this.Src = src;
            this.Cover = cover;
            this.ContentId = Contentid;
        }
        public OtherImage(string title, int displaySort, string src, Guid? cover)
        {
            this.Title = title;
            this.DisplaySort = displaySort;
            this.Src = src;
            this.Cover = cover;
        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OtherImage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Content).WithMany(Current => Current.OtherImages).HasForeignKey(Current => Current.ContentId).WillCascadeOnDelete(true);
                HasOptional(Current => Current.attachment).WithMany(Current => Current.OtherImages).HasForeignKey(Current => Current.Cover).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public  attachment attachment { get; set; }

        [Required]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }
    
        [Display(Name = "انتخاب محتوا")]
        public int ContentId { get; set; }
        public  Content Content { get; set; }

        [Required]
        public int DisplaySort { get; set; }

        [NotMapped]
        public string Src { get; set; }

        #endregion
    }
}
