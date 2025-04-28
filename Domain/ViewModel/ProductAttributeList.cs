using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ProductAttributeViewModel
    {
        public int AttrId { get; set; }
        public string AttrName { get; set; }
        public IEnumerable<ProductAttributeItemViewModel> ProductAttributeItemViewModels { get; set; }
    }
    public class ProductAttributeItemViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ColorCode { get; set; }
        public bool IsChecked { get; set; }
    }
}
