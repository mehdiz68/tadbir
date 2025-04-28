using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
namespace Domain.ViewModels
{
    public class Link
    {
        public Link()
        {

        }
        public int Id { get; set; }
        public int? ParrentId { get; set; }
        public string Name { get; set; }
        public string PageAddress { get; set; }
        public string Cover { get; set; }

    }

}
