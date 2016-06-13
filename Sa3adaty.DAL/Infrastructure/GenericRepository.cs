using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sa3adaty.DAL.EntityModel;

namespace Sa3adaty.DAL.Infrastructure
{
   public  class GenericRepository<TEntity> where TEntity : class 
     {
        internal Sa3adatyEntities context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(Sa3adatyEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Get(System.Linq.Expressions.Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
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
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        public TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }

        public void Insert(TEntity entity)
        {
            dbSet.Add((TEntity)entity);
        }

        public void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach((TEntity)entityToDelete);
            }
            dbSet.Remove((TEntity)entityToDelete);
        }

        public void Update(TEntity entityToUpdate)
        {
            dbSet.Attach((TEntity)entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

    }
}
