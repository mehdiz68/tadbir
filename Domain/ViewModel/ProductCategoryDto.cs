using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ProductCategoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PageAddress { get; set; }
        public IEnumerable<ProductCategoryDto> ChildCategory { get; set; }
        public ProductCategoryDto ParentCat { get; set; }
        public int? ParrentId { get; set; }
        public int Sort { get; set; }
        public string Name { get; set; }
    }
}
