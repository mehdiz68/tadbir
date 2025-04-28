using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Tax
    {
        public Tax()
        {

        }

        #region Tax

        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "درصد")]
        public int TaxPercent { get; set; }

        public  ICollection<ProductSendWay> ProductSendWays { get; set; }

        public  ICollection<Product> Products { get; set; }

        #endregion
    }
}
