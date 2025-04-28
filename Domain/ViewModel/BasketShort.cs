using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketShort
    {
        public BasketShort()
        {

        }
        public IEnumerable<ProductBasketItem> ProductBasketItems { get; set; }
        public long remainPrice { get; set; }
        public bool validBuyOneTimePerOffer { get; set; }

    }

}
