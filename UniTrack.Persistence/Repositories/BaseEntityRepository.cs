using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories.Pagenation;
using UniTrack.Domain.Entities;
using UniTrack.Persistence.Context;
using UniTrack.Persistence.Repositories.Pagenation;


namespace UniTrack.Persistence.Repositories
{
    public class BaseEntityRepository<T> : Application.Abstraction.Repositories.IBaseEntityRepository<T> where T : class
    {
        protected readonly UniTrackDbContext context;
        protected DbSet<T> dbSet;
        public BaseEntityRepository(UniTrackDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public async Task<T> AddAsync(T entity)
        {
            dbSet.Add(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            dbSet.Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return dbSet.FirstOrDefaultAsync(predicate);

        }

        public async Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public Task<IPagingExecutionResult<T>> GetPagedResult<T>(
            IEnumerable<T> query,
            int? pageSize = 50,
            int? pageIndex = 1,
            Func<IQueryable<T>, IOrderedQueryable<T>> ordering = null,
            CancellationToken cancellationToken = default)
        {
            pageIndex ??= 1;
            pageSize ??= 50;

            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = 1;

            var hasPaging = pageSize.HasValue && pageIndex.HasValue;
            int totalCount;
            List<T> data;

            if (hasPaging)
            {
                var queryable = query.AsQueryable();

                if (ordering != null)
                    queryable = ordering(queryable);

                totalCount = queryable.Count();

                data = queryable
                    .Skip(pageSize.Value * (pageIndex.Value - 1))
                    .Take(pageSize.Value)
                    .ToList();
            }
            else
            {
                data = query.ToList();
                totalCount = data.Count;
                pageIndex = 1;
                pageSize = data.Count;
            }

            var result = new PagingExecutionResult<T>(data, hasPaging, pageIndex.Value, pageSize.Value, totalCount);
            return Task.FromResult<IPagingExecutionResult<T>>(result);

        }

       

    }
}
