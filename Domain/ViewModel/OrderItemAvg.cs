using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
namespace Domain.ViewModels
{
    public class OrderItemAvg
    {
        public OrderItemAvg()
        {

        }
        public string ItemName { get; set; }
        public int ItemTotalCount { get; set; }
        public int ItemCount0 { get; set; }
        public int ItemCount1 { get; set; }
        public int ItemCount2 { get; set; }
        public int ItemCount3 { get; set; }
        public int ItemCount4 { get; set; }

    }

}
