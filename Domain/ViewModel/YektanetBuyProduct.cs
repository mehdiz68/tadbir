using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;
using Domain.ViewModel;

namespace Domain.ViewModels
{
    public class YektanetBuyProduct
    {

        #region Properties


        public string sku { get; set; }
        public int quantity { get; set; }
        public long price { get; set; }
        public string currency { get; set; }
        #endregion
    }
}
