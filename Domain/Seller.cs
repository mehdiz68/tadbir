using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Seller
    {
        public Seller()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Seller>
        {
            public Configuration()
            {
                HasRequired(Current => Current.User).WithMany(Current => Current.Sellers).HasForeignKey(Current => Current.UserId);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }


        [Display(Name = "حساب کاربری")]
        public String UserId { get; set; }
        public  ApplicationUser User { get; set; }

        [Required(ErrorMessage ="اجباری")]
        [Display(Name = "فعال")]
        public bool IsActive{ get; set; }

        public  ICollection<ProductPrice> ProductPrices { get; set; }

        public  ICollection<ProductSeller> ProductSellers { get; set; }
        #endregion
    }
}
