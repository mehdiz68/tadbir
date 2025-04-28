using DataLayer;
using Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class UserBonService : GenericRepository<UserBon>
    {
        public UserBonService(ahmadiDbContext context) : base(context)
        {

        }
       

    }
}
