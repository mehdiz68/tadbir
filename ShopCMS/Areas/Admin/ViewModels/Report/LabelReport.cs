using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ahmadi.Areas.Admin.ViewModels.Report
{
  

    public class LabelReport
    {
        public string WebsiteName { get; set; }
        public string WebsiteAddress { get; set; }
        public string WebsitePhoneNumber { get; set; }
        public string Logo { get; set; }
        public string CustomerFullName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerProvience { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPostalCode { get; set; }
        public string CustomerTele { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string OrderSendWay { get; set; }
        public string OrderPayWay { get; set; }
    }

    public class Icons
    {
        public System.Drawing.Image Icon { get; set; }
    }


}