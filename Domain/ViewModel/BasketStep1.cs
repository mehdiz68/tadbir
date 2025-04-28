using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketStep1
    {
        public BasketStep1()
        {

        }
        public IEnumerable<ProductBasketItem> ProductBasketItems { get; set; }
        public List<ProductBox> ProductBoxes { get; set; }
        public bool validBuyOneTimePerOffer { get; set; }
    }
}
