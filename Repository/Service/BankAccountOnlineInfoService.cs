using DataLayer;
using Domain;

namespace Repository.Service
{
    public class BankAccountOnlineInfoService : GenericRepository<BankAccountOnlineInfo>
    {
        public BankAccountOnlineInfoService(ahmadiDbContext context) : base(context)
        {
        }
    }
}
