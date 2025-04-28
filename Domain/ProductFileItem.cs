using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductFileItem
    {
        public ProductFileItem()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductFileItem>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductFileInfo).WithMany(Current => Current.ProductFileItems).HasForeignKey(Current => Current.ProductFileInfoId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.File).WithMany(Current => Current.ProductFileItems).HasForeignKey(Current => Current.AttachementId).WillCascadeOnDelete(false);

            }
        }
        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "فایل محصول")]
        public int ProductFileInfoId { get; set; }
        public  ProductFileInfo ProductFileInfo { get; set; }


        [Display(Name = "فایل")]
        public Guid AttachementId { get; set; }
        public  attachment File { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "حجم فایل(کیلوبایت)")]
        public int Capacity { get; set; }


        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }


        #endregion
    }


}
