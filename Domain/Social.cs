using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Social : Object
    {
        #region Ctor
        public Social()
        {

        }
        #endregion
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Social>
        {
            public Configuration()
            {
                HasOptional(Current => Current.attachment).WithMany(Current => Current.Socials).HasForeignKey(Current => Current.Cover);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        public Guid? Cover { get; set; }
        public  attachment attachment { get; set; }
        
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Title { get; set; }

        [Display(Name = "لینک")]
        [MaxLength(255, ErrorMessage = "حداکثر طول کارکتر ، 255")]
        public string Link { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Required]
        [Display(Name = "فعال؟")]
        public bool IsActive { get; set; }

      
        [Display(Name = "آیکن")]
        public string Icon { get; set; }

        [Required]
        [Display(Name = "زبان ( وب سایت)")]
        public Int16? LanguageId { get; set; }

        #endregion
    }
}
