using System.Linq.Expressions;
using UniTrack.Application.Abstraction.Repositories.Pagenation;

namespace UniTrack.Application.Abstraction.Repositories
{
    public interface BaseEntityRepository<T> where T : class
    {
        // Yeni kayıt ekle
        Task<T> AddAsync(T entity);

        // Mevcut kaydı güncelle
        Task UpdateAsync(T entity);

        // Kayıt sil
        Task DeleteAsync(T entity);

        Task<List<T>> GetAllAsync();

        Task<T> GetAsync(Expression<Func<T, bool>> predicate);

        // Paggenation
        Task<IPagingExecutionResult<T>> GetPagedResult<T>(IEnumerable<T> query, int? pageSize = 10, int? pageIndex = 1,
       Func<IQueryable<T>, IOrderedQueryable<T>> ordering = null,
       CancellationToken cancellationToken = default);

    }
}
