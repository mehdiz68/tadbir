using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductState
    {
        public ProductState()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductState>
        {
            public Configuration()
            {
                HasOptional(Current => Current.attachment).WithMany(Current => Current.ProductStates).HasForeignKey(Current => Current.Cover);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }


        [Display(Name = "کاور(تصویر)")]
        public Guid? Cover { get; set; }
        public  attachment attachment { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "مجوز حذف؟")]
        public bool DeleteAccess { get; set; }


        public  ICollection<Product> Products { get; set; }
        public  ICollection<ProductPrice> ProductPrices { get; set; }
        #endregion
    }
}
