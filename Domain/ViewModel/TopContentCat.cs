using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
namespace Domain.ViewModels
{
    public class TopContentCat
    {
        public TopContentCat()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string PageAddress { get; set; }
        public int Quantity { get; set; }
        public Content TopContent { get; set; }
        public List<Content> TopContentList { get; set; }

    }

}
