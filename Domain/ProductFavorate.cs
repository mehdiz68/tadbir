using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductFavorate
    {
        public ProductFavorate()
        {

        }
        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductFavorate>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.ProductFavorates).HasForeignKey(Current => Current.UserId);
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductFavorates).HasForeignKey(Current => Current.ProductId);
            }
        }
        #endregion

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "کاربر")]
        public string UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "محصول")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }


        [Required]
        [Display(Name = "نام پوشه")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string FolderName { get; set; }

        [Required]
        [Display(Name = "تاریخ درج")]
        public DateTime InsertDate { get; set; }

        [Display(Name = "توضیحات")]
        public string Description { get; set; }

    }
}
