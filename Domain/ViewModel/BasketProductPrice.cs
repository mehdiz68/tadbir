using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class Basket
    {
        public Basket()
        {

        }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public long Price { get; set; }
        public int Quantity { get; set; }
        public int? SendBoxId { get; set; }


        public List<BasketShipping> shippings { get; set; }

        public string UserCodeGift { get; set; }

        public int UserBon { get; set; }

        public DateTime? InsertDate { get; set; }
    }
}
