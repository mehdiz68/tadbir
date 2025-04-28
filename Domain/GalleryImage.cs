using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class GalleryImage : Object
    {
        #region Ctor
        public GalleryImage()
        {

        }
        public GalleryImage(string title, int displaySort, string src, Guid? cover,int CatId)
        {
            this.Title = title;
            this.DisplaySort = displaySort;
            this.Src = src;
            this.Cover = cover;
            this.CatId = CatId;
        }
        public GalleryImage(string title, int displaySort, string src, Guid? cover)
        {
            this.Title = title;
            this.DisplaySort = displaySort;
            this.Src = src;
            this.Cover = cover;
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<GalleryImage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.GalleryCategory).WithMany(Current => Current.GalleryImages).HasForeignKey(Current => Current.CatId).WillCascadeOnDelete(true);
                HasOptional(Current => Current.attachment).WithMany(Current => Current.GalleryImages).HasForeignKey(Current => Current.Cover).WillCascadeOnDelete(false);
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

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }
    
        [Display(Name = "دسته بندی گالری")]
        public int CatId { get; set; }
        public  GalleryCategory GalleryCategory { get; set; }

        [Required]
        public int DisplaySort { get; set; }

        [NotMapped]
        public string Src { get; set; }

        #endregion
    }
}