using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketShipping
    {
        public BasketShipping()
        {

        }
        public ProductPackageType ProductPackageType { get; set; }
        public int addressId { get; set; }
        public int sendwayId { get; set; }
        public int sendwayBoxId { get; set; }
        public int sendwayWorktimeId { get; set; }
        public string deliveryDate { get; set; }
        public bool extraprice { get; set; }

    }
    public class BasketShippingVireModel
    {
        public BasketShippingVireModel()
        {

        }
        public bool PasKeraye { get; set; }
        public int cost { get; set; }
        public int sendwayBoxId { get; set; }
        public ProductSendWayWorkTime productSendWayWorkTime { get; set; }
        public DateTime? deliveryDate { get; set; }
        public int InsucanceCost { get; set; }
        public int PackageCost { get; set; }
        public string Title { get; set; }

    }
}