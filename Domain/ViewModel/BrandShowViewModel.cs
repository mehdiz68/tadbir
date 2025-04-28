using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
   public class BrandShowViewModel
    {
        public int Id { get; set; }
        public string PersianName { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
