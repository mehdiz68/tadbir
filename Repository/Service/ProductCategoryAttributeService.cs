using DataLayer;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service
{
    public class ProductCategoryAttributeService : GenericRepository<ProductCategoryAttribute>
    {
        public ProductCategoryAttributeService(ahmadiDbContext context) : base(context)
        {
        }
    }
}
