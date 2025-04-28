using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketProduct
    {
        public BasketProduct()
        {

        }
        public int Id { get; set; }
        public int productId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Lenght { get; set; }
        public int ProductWeight { get; set; }

        public int quantity { get; set; }

    }

}
