using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSendwayIrPostDetail
    {
        public ProductSendwayIrPostDetail()
        {

        }


        #region Properties

        [Display(Name = "روش ارسال")]
        [Key, ForeignKey("ProductSendWay")]
        public int ProductSendWayId { get; set; }


        public  ProductSendWay ProductSendWay { get; set; }


        //[Required]
        //[Display(Name = "از محدوده")]
        //public int From { get; set; }

        //[Required]
        //[Display(Name = "تا محدوده")]
        //public int To { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "درون استانی تا 1 کیلوگرم")]
        public int InnserState1 { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "برون استانی همجوار تا 1 کیلوگرم")]
        public int OuterNearState1 { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "برون استانی غیر همجوار تا 1 کیلوگرم")]
        public int OuterState1 { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "درون استانی مازاد بر 1 کیلو گرم هر کیلو و کسر آن")]
        public int InnserStateOver1 { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "برون استانی همجوار مازاد بر 1 کیلو گرم هر کیلو و کسر آن")]
        public int OuterNearStateOver1 { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "برون استانی غیر همجوار مازاد بر 1 کیلو گرم هر کیلو و کسر آن")]
        public int OuterStateOver1 { get; set; }



        #endregion
    }
}
