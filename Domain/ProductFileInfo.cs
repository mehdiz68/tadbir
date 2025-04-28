using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductFileInfo
    {
        public ProductFileInfo()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductFileInfo>
        {
            public Configuration()
            {
                HasRequired(Current => Current.Product).WithMany(Current => Current.ProductFileInfos).HasForeignKey(Current => Current.ProductId).WillCascadeOnDelete(false);
            }
        }
        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "محصول اصلی")]
        public int ProductId { get; set; }
        public  Product Product { get; set; }

        /*
         * 0 unlimited
         */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تعداد دانلودهای مجاز")]
        public int QuantityLimit { get; set; }

        /*
         * null unlimited
         */
        [Display(Name = "تاریخ انقضا")]
        public DateTime? DateLimit { get; set; }


        /*
         * 0 unlimited
         */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "تعداد روزهای دانلود مجاز")]
        public int DayLimit { get; set; }


        public  ICollection<ProductFileItem> ProductFileItems { get; set; }

        #endregion
    }
}
