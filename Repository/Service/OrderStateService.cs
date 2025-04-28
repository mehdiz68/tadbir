using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class OrderStateService : GenericRepository<OrderState>
    {
        public OrderStateService(ahmadiDbContext context) : base(context)
        {

        }

        

    }
}
