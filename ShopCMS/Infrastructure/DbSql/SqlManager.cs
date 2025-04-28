using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;
using DataLayer;

namespace ahmadi.Infrastructure.DbSql
{
    public static class SqlManager
    {
        public static bool CheckTicketState(IEnumerable<Ticket> childItems)
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass();
            var isChildExisting = false;
            foreach (Ticket item in childItems)
            {
                if (item.ChildTickets.Where(x => x.Answer == false && !x.ChildTickets.Any(s => s.Answer)).Any())
                {
                    isChildExisting = true;
                    return isChildExisting;
                }
                else
                {
                    return CheckTicketState(item.ChildTickets);
                }
            }
            return isChildExisting;
        }
        public static List<int> GetAllSubCat(int CatId)
        {
            DataLayer.ahmadiDbContext db = new DataLayer.ahmadiDbContext();
            try
            {
                List<int> subCatIds = db.Database.SqlQuery<int>(string.Format("select * from GetAllSubCat({0})", CatId)).ToList();
                return subCatIds;
            }
            catch (Exception x)
            {
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }
    }
}