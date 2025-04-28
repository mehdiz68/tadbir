using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class OrderDelivery : Object
    {
        #region Ctor
        public OrderDelivery()
        {
        }
        #endregion

        #region Configuration
        public class Configuration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<OrderDelivery>
        {
            public Configuration()
            {
                HasOptional(Current => Current.ProductSendWay).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.SendWayId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.UserAddress).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.UserAddressId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.Order).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.OrderId).WillCascadeOnDelete(false);
                HasOptional(Current => Current.ProductSendWayWorkTime).WithMany(Current => Current.OrderDeliveries).HasForeignKey(Current => Current.ProductSendWayWorkTimeId).WillCascadeOnDelete(false);
            }
        }

        #endregion

        #region Properties

        [Key]
        [Required]
        public int Id { get; set; }

        [Display(Name = "سفارش")]
        public Guid? OrderId { get; set; }
        public  Order Order { get; set; }

        [Display(Name = "تاریخ ارسال مرسوله به مشتری")]
        public DateTime? RequestDate { get; set; }

        [Display(Name = "زمان ارسال مرسوله به مشتری")]
        public int? ProductSendWayWorkTimeId { get; set; }

        [Display(Name ="وضعیت تحویل")]
        public DeliveryState? DeliveryState { get; set; }


        [Display(Name = "آدرس")]
        public int? UserAddressId { get; set; }
        public  UserAddress UserAddress { get; set; }

        [Display(Name = "روش ارسال")]
        public int? SendWayId { get; set; }
        public  ProductSendWay ProductSendWay { get; set; }


        [Display(Name = "ماهیت باکس")]
        public ProductPackageType ProductPackageType { get; set; }

        public bool Insurance { get; set; }

        public  ProductSendWayWorkTime ProductSendWayWorkTime { get; set; }
        public  ICollection<OrderRow> OrderRows { get; set; }
        public  ICollection<OrderAttributeOrder> OrderAttributeOrders { get; set; }
        public  ICollection<WalletAttributeWallet> WalletAttributeWallets { get; set; }
        public  ICollection<OrderState> OrderStates { get; set; }
        public  ICollection<labelIcon> labelIcons { get; set; }

        #endregion
    }

    public enum DeliveryState
    {
        تحویل_به_موقع,
        تحویل_با_تاخیر
    }
}
