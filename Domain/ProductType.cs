using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductType
    {
        public ProductType()
        {

        }

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        /*
         * 1- Single Product
         * 2- Multiple Product
         * 3- File
         * 4- Course
         * 5- Reserve
         */
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نوع محصول")]
        public int DataType { get; set; }


        public  ICollection<Product> Products { get; set; }
        public  ICollection<ProductCategory> ProductCategories { get; set; }
        public  ICollection<ProductPriceGroupModification> ProductPriceGroupModifications { get; set; }

        #endregion
    }
}
