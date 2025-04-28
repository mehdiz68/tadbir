using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class WalletAttributeWallet
    {
        public WalletAttributeWallet()
        {

        }

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<WalletAttributeWallet>
        {
            public Configuration()
            {
                HasRequired(Current => Current.OrderDelivery).WithMany(Current => Current.WalletAttributeWallets).HasForeignKey(Current => Current.OrderDeliveryId);
            }
        }
        #endregion

        #region Properties

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WalletId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int WalletAttributeId { get; set; }

        [Required]
        public string Value { get; set; }

        public  Wallet Wallet { get; set; }

        public  WalletAttribute WalletAttribute { get; set; }


        public  OrderDelivery OrderDelivery { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int OrderDeliveryId { get; set; }

        #endregion
    }
}
