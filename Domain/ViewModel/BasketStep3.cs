using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class BasketStep3
    {
        public BasketStep3()
        {

        }
        public List<BankAccount> BankAccounts { get; set; }

        public BasketStep1 basketStep1 { get; set; }

        public List<BasketShippingVireModel> basketShippings { get; set; }


        public string UserCodeGift { get; set; }

        public int UserBon { get; set; }
        public int UserCurrentBon { get; set; }
        public int? UserMaxBon { get; set; }

        public long OffPrice { get; set; }
        public CodeGiftType OffPriceType { get; set; }

        public bool IsEstelam { get; set; }

    }

    public enum CodeGiftType
    {
        محصول,
        کد_بن_تخفیف,
        کدتخفیف_ارسال_رایگان
    }

}
