using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductImage
    {
        public ProductImage()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductImage>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Image).WithMany(Current => Current.ProductImages).HasForeignKey(Current => Current.AttachementId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductImages).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductPrice).WithMany(Current => Current.ProductImages).HasForeignKey(Current => Current.ProductPriceId).WillCascadeOnDelete(false);

            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "تصویر")]
        public Guid AttachementId { get; set; }
        public  attachment Image { get; set; }

        [Display(Name = "توضیحات")]
        public string Data { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تصویر یا فایل؟")]
        public bool IsImage { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عکس اصلی تنوع")]
        public bool IsMain { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Display(Name = "تنوع")]
        public int? ProductPriceId { get; set; }
        public  ProductPrice ProductPrice { get; set; }

        #endregion
    }
}
