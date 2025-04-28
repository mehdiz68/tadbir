using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OrderRateItem : Object
    {
        #region Ctor

        public OrderRateItem()
        {
        }
        #endregion

  

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "توضیح")]
        public string Descr { get; set; }
        public ICollection<OrderRate> OrderRates { get; set; }

        #endregion
    }

}
