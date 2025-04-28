using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModels
{
    public class CatAttribute
    {
        public IEnumerable<Domain.ProductAttributeGroupSelect> MainCatGroupAttributes { get; set; }
    }

    public class AttributeGroup
    {
        public int? AttributeId { get; set; }
        public int? GroupId { get; set; }
        public int? CatId { get; set; }
        public short? TabId { get; set; }
    }
}
