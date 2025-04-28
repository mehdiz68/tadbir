using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class ProductRank : Object
    {
        #region Ctor
        public ProductRank()
        {

        }
        #endregion


        #region Properties

        [Key]
        [Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "عنوان")]
        [MaxLength(100, ErrorMessage = "حداکثر طول کارکتر ، 100")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "ترتیب نمایش")]
        public int DisplaySort { get; set; }

        [Display(Name = "زبان ( وب سایت )")]
        public Int16 LanguageId { get; set; }

        public  ICollection<ProductRankGroupSelect> ProductRankGroupSelects { get; set; }

        #endregion
    }
}
