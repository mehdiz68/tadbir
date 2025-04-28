using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSendWayBox
    {
        public ProductSendWayBox()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductSendWayBox>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductSendWay).WithMany(Current => Current.ProductSendWayBoxes).HasForeignKey(Current => Current.ProductSendWayId).WillCascadeOnDelete(false);

                HasRequired(Current => Current.SendwayBox).WithMany(Current => Current.ProductSendWayBoxes).HasForeignKey(Current => Current.SendWayBoxID).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "روش ارسال")]
        public int ProductSendWayId { get; set; }
        public  ProductSendWay ProductSendWay { get; set; }


        [Required]
        [Display(Name = "باکس ارسال")]
        public int SendWayBoxID { get; set; }
        public  SendwayBox SendwayBox { get; set; }


        //[Required(ErrorMessage = "اجباری")]
        //[Display(Name = "بالاترین هزینه")]
        //public int MaxValue { get; set; }


        //[Required(ErrorMessage = "اجباری")]
        //[Display(Name = "پایین ترین هزینه")]
        //public int MinValue { get; set; }

        public ICollection<ProductSendWayDetail> ProductSendWayDetails { get; set; }


        #endregion
    }
}
