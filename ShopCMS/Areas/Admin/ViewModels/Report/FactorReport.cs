using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ahmadi.Areas.Admin.ViewModels.Report
{


    public class CustomerOrderRow
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public long RawPrice { get; set; }
        public long SumPrice { get; set; }
        public long OffPrice { get; set; }
        public long TaxPrice { get; set; }
        public long Price { get; set; }
        public long FinalPrice { get; set; }
    }
    public class CustomerOrderInfo
    {
        public string SerialNumber { get; set; }
        public string InsertDate { get; set; }
        public string Logo { get; set; }

        public string ShoppingName { get; set; }
        public string ShoppingProvience { get; set; }
        public string ShoppingCity { get; set; }
        public string ShoppingAddress { get; set; }
        public string ShoppingTele { get; set; }
        public string ShoppingPostalCode { get; set; }
        public string ShoppingTaxNumber { get; set; }
        public string ShoppingSenWay { get; set; }
        public string ShoppingPayWay { get; set; }
        public string ShoppingUserDescr { get; set; }
        public string ShoppingOrderId { get; set; }
        public long ShoppingSenWayPrice { get; set; }
        public long ShoppingTotalPrice { get; set; }

        public string CustomerName { get; set; }
        public string CustomerProvience { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerTele { get; set; }
        public string CustomerPostalCode { get; set; }
        public string CustomerTaxNumber { get; set; }
    }
}