using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketStep2
    {
        public BasketStep2()
        {

        }
        public UserAddress UserDefaultAddress { get; set; }
        public List<UserAddress> UserAddresses { get; set; }

        public BasketStep1 basketStep1 { get; set; }
    }
}
