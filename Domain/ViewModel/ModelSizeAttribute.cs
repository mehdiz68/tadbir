using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ModelSizeAttribute
    {
        public int AttrId { get; set; }
        public string AttrName { get; set; }
        public List<string> Values { get; set; }
    }
}
