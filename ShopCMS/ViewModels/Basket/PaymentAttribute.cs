using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ahmadi.ViewModels.Basket
{
    public class PaymentAttribute
    {
        public string FinalBasketPrice { get; set; }
        public IEnumerable<BankAccount> BankAccounts { get; set; }
        public int BonSum { get; set; }
    }
}