using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class BrandVM
    {
        public IEnumerable<ProductItem> Products { get; set; }
        public string Breadcrumb { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string RawName { get; set; }
        public string Name { get; set; }
        public string PersianName { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }
        public string Cover { get; set; }
    }
}
