using System.Linq.Expressions;
using Library.Core.Common;

namespace Library.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<PagedResult<T>> GetPagedAsync(int page, int pageSize);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate); // Добавьте этот метод
    }
}