using DataLayer;
using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.SqlClient;

namespace Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal ahmadiDbContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(ahmadiDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }
        public IQueryable<TEntity> GetQueryList()
        {
            return dbSet;
        }


        public virtual List<int> SqlQuery(string sql, SqlParameter p)
        {
            return context.Database.SqlQuery<int>(sql, p).ToList();
        }
        public virtual List<int> SqlQuery(string sql, SqlParameter p, SqlParameter p2)
        {
            return context.Database.SqlQuery<int>(sql, p, p2).ToList();
        }
        public virtual List<long> SqlQueryLong(string sql, SqlParameter p)
        {
            return context.Database.SqlQuery<long>(sql, p).ToList();
        }
        public virtual IEnumerable<TResult> Sql<TResult>(Func<TEntity, TResult> selector, string sql, SqlParameter p1 = null, SqlParameter p2 = null, SqlParameter p3 = null, SqlParameter p4 = null, SqlParameter p5 = null, SqlParameter p6 = null, SqlParameter p7 = null, SqlParameter p8 = null)
        {
            if (p1 == null && p2 == null && p3 == null && p4 == null && p5 == null && p6 == null && p7 == null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql).ToList();
            else if (p1 != null && p2 == null && p3 == null && p4 == null && p5 == null && p6 == null && p7 == null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql, p1).ToList();
            else if (p1 != null && p2 != null && p3 == null && p4 == null && p5 == null && p6 == null && p7 == null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql, p1, p2).ToList();
            else if (p1 != null && p2 != null && p3 != null && p4 == null && p5 == null && p6 == null && p7 == null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql, p1, p2, p3).ToList();
            else if (p1 != null && p2 != null && p3 != null && p4 != null && p5 == null && p6 == null && p7 == null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql, p1, p2, p3, p4).ToList();
            else if (p1 != null && p2 != null && p3 != null && p4 != null && p5 != null && p6 == null && p7 == null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql, p1, p2, p3, p4, p5).ToList();
            else if (p1 != null && p2 != null && p3 != null && p4 != null && p5 != null && p6 != null && p7 == null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql, p1, p2, p3, p4, p5, p6).ToList();
            else if (p1 != null && p2 != null && p3 != null && p4 != null && p5 != null && p6 != null && p7 != null && p8 == null)
                return context.Database.SqlQuery<TResult>(sql, p1, p2, p3, p4, p5, p6, p7).ToList();
            else if (p1 != null && p2 != null && p3 != null && p4 != null && p5 != null && p6 != null && p7 != null && p8 != null)
                return context.Database.SqlQuery<TResult>(sql, p1, p2, p3, p4, p5, p6, p7, p8).ToList();
            else
                return context.Database.SqlQuery<TResult>(sql).ToList();
        }

        public virtual IEnumerable<TResult> Get<TResult>(
            Func<TEntity, TResult> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", int skipRecord = 0, int takeRecorf = 0, bool asNoTracking = false)
        {
            IQueryable<TEntity> query = dbSet;
            try
            {

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }

                if (orderBy != null)
                {
                    query = orderBy(query);
                }

                if (skipRecord > 0)
                    query = query.Skip(() => skipRecord);
                if (takeRecorf > 0)
                {
                    if (skipRecord == 0)
                        query = query.Skip(() => 0).Take(() => takeRecorf);
                    else
                        query = query.Take(() => takeRecorf);
                }
                if (asNoTracking)
                    return query.AsNoTracking().Select(selector).ToList();
                else
                    return query.Select(selector).ToList();
            }
            catch (Exception ex)
            {
                return query.Select(selector).ToList();
            }
        }

        public virtual IQueryable<TResult> GetByReturnQueryable<TResult>(
            Func<TEntity, TResult> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", int skipRecord = 0, int takeRecorf = 0)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skipRecord > 0)
                query = query.Skip(() => skipRecord);
            if (takeRecorf > 0)
            {
                if (skipRecord == 0)
                    query = query.Skip(() => 0).Take(() => takeRecorf);
                else
                    query = query.Take(() => takeRecorf);
            }

            return query.Select(selector).AsQueryable();
        }


        public virtual IEnumerable<TResult> PagedResult<TResult>(
            Func<TEntity, TResult> selector,
            int? pageNum, int pageSize, out int rowsCount,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (pageSize <= 0) pageSize = 10;

            //مجموع ردیف‌های به دست آمده
            rowsCount = query.Count();

            // اگر شماره صفحه کوچکتر از 0 بود صفحه اول نشان داده شود
            if (rowsCount <= pageSize || pageNum <= 0 || pageNum == null) pageNum = 1;

            // محاسبه ردیف هایی که نسبت به سایز صفحه باید از آنها گذشت
            int excludedRows = (pageNum.Value - 1) * pageSize;

            return query.Skip(() => excludedRows).Take(() => pageSize).Select(selector).ToList();
        }

        public virtual int Count(
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }

        public virtual int Max<TResult>(
            Func<TEntity, TResult> selector,
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!query.Any())
                return 0;
            else
                return Convert.ToInt32(query.Max(selector));
        }


        public virtual int Min<TResult>(
            Func<TEntity, TResult> selector,
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!query.Any())
                return 0;
            else
                return Convert.ToInt32(query.Min(selector));
        }

        public virtual int Sum(
            Func<TEntity, int> selector,
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!query.Any())
                return 0;
            else
                return query.Sum(selector);
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual bool Any(object id)
        {
            if (dbSet.Find(id) == null)
                return false;
            else
                return true;
        }

        public virtual bool Any<TResult>(
            Func<TEntity, TResult> selector,
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Any();
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }
        public virtual void InsertList(List<TEntity> entities)
        {
            dbSet.AddRange(entities);
        }

        public virtual bool Delete(object id)
        {
            try
            {
                TEntity entityToDelete = dbSet.Find(id);
                Delete(entityToDelete);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool Delete(TEntity entityToDelete)
        {
            try
            {
                if (context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                dbSet.Remove(entityToDelete);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public virtual bool Delete(ICollection<TEntity> entityToDelete)
        {
            //try
            //{
            //    foreach (var item in entityToDelete)
            //    {
            //        if (context.Entry(item).State == EntityState.Detached)
            //        {
            //                dbSet.Attach(item);
            //        }
            //        dbSet.Remove(item);
            //    }

            //    return true;
            //}
            //catch(Exception x)
            //{
            //    return false;
            //}

            try
            {
                dbSet.RemoveRange(entityToDelete);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public virtual bool DLTAll(string tablename)
        {
            try
            {
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE [" + tablename + "]");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }


        public virtual void Load(TEntity entityToLoad, string collections)
        {

            context.Entry(entityToLoad).Collection(collections).Load();
        }
    }
}
