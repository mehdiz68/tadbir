using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class OrderAttributeOrder
    {
        public OrderAttributeOrder()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OrderAttributeOrder>
        {
            public Configuration()
            {
                HasRequired(Current => Current.OrderDelivery).WithMany(Current => Current.OrderAttributeOrders).HasForeignKey(Current => Current.OrderDeliveryId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid OrderId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AttributeId { get; set; }

        [Required]
        public string Value { get; set; }

        public  Order Order { get; set; }

        public  OrderAttribute OrderAttribute { get; set; }

        public  OrderDelivery OrderDelivery { get; set; }


        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderDeliveryId { get; set; }



        #endregion
    }
}
