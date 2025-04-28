using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Site
{
    public class ProductSearchResult
    {
        public int ProductId { get; set; }
        public int Visits { get; set; }
        public int sellCount { get; set; }
        public int favCount { get; set; }
        public long price { get; set; }
        public long Sort { get; set; }
    }
}
