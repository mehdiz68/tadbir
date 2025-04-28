using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ProductSendWayDetail
    {
        public ProductSendWayDetail()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<ProductSendWayDetail>
        {
            public Configuration()
            {
                HasRequired(Current => Current.ProductSendWayBox).WithMany(Current => Current.ProductSendWayDetails).HasForeignKey(Current => Current.ProductSendWayBoxId).WillCascadeOnDelete(false);
                HasRequired(Current => Current.City).WithMany(Current => Current.ProductSendWayDetails).HasForeignKey(Current => Current.CityId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties
        [Key]
        [Required]
        public int Id { get; set; }


        public  City City { get; set; }
        [Display(Name = "شهر")]
        public int CityId { get; set; }


        [Required]
        [Display(Name = "باکس ارسال")]
        public int ProductSendWayBoxId { get; set; }
        public  ProductSendWayBox ProductSendWayBox { get; set; }


        //[Required]
        //[Display(Name = "از محدوده")]
        //public int From { get; set; }

        //[Required]
        //[Display(Name = "تا محدوده")]
        //public int To { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "هزینه")]
        public int Value { get; set; }


        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "محدودیت ارسال، تعداد در ساعت")]
        public int Limitation { get; set; }

        public bool IsActive { get; set; }


        #endregion
    }
}
