using HotelListing.Data;
using HotelListing.IReposiroty;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HotelListing.Reposiroty
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DatabaseContext context;
        private readonly DbSet<T> db;

        public GenericRepository(DatabaseContext context)
        {
            this.context = context;
            this.db = context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await db.FindAsync(id);
            this.db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            this.db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = this.db;
            if (includes != null)
                foreach (string includeProperty in includes)// for foreign keys 
                    query.Include(includeProperty);

            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = this.db;
            if (expression != null)
                query = query.Where(expression);

            if (includes != null)
                foreach (string includeProperty in includes)// for foreign keys 
                    query.Include(includeProperty);

            if (orderBy != null)
                query = orderBy(query);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await this.db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await this.db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            this.db.Attach(entity);
            this.context.Entry(entity).State = EntityState.Modified;
        }
    }
}
