using Library.Core.Common;
using Library.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Library.Infrastructure.Data;

namespace Library.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly LibraryDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(LibraryDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<PagedResult<T>> GetPagedAsync(int page, int pageSize)
        {
            var totalItems = await _dbSet.CountAsync();
            var items = await _dbSet.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<T>(items, totalItems, page, pageSize);
        }
    }
}