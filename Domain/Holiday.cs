using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Holiday
    {
        public Holiday()
        {

        }

        #region Properties
        [Key]
        public int Id { get; set; }



        [Required]
        [Display(Name = "تاریخ ")]
        public DateTime InsertDate { get; set; }



        [Required]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }


        #endregion
    }
}
