using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class PrCatViewModel
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<Adveresting> TopAdverestings { get; set; }
        public IEnumerable<Adveresting> BotomAdverestings { get; set; }
        public IEnumerable<Adveresting> LeftAdverestings { get; set; }
        public IEnumerable<Adveresting> RightAdverestings { get; set; }
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<ProductCategory> ChildProductCategories { get; set; }
        public IEnumerable<ProductItem> NewProductItems { get; set; }
        public IEnumerable<ProductItem> PopularProductItems { get; set; }
        public IEnumerable<ProductItem> MostOrderProductItems { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string PageAddress { get; set; }
        public string Description { get; set; }
        public string Abstract { get; set; }
        public string Data { get; set; }
        public string Breadcrumb { get; set; }
    }
}
