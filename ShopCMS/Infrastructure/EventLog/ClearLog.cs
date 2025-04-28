using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnitOfWork;

namespace ahmadi.Infrastructure.EventLog
{
    public static class ClearLog
    {
        public static void Clear()
        {
            DateTime LastMonthDate = DateTime.Now.AddDays(-7);
            using (DataLayer.ahmadiDbContext db = new DataLayer.ahmadiDbContext())
            {
                db.Database.ExecuteSqlCommandAsync("DELETE FROM ELMAH_Error WHERE TimeUtc < '" + LastMonthDate.ToString("yyyy-MM-dd") + "'");
            }

            UnitOfWorkClass uow = new UnitOfWorkClass();
            IEnumerable<Domain.EventLog> oldLog = uow.EventLogRepository.Get(x => x, x => x.LogDateTime < LastMonthDate);
            uow.EventLogRepository.Delete(oldLog.ToList());
            uow.Save();
        }
    }
}