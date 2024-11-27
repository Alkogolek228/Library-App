using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Infrastructure.Data;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _context;

        public IRepository<Author> Authors { get; }
        public IBookRepository Books { get; }
        public IUserRepository Users { get; }
        public IRepository<BookPick> BookPicks { get; } 

        public UnitOfWork(LibraryDbContext context)
        {
            _context = context;
            Authors = new Repository<Author>(context);
            Books = new BookRepository(context);
            Users = new UserRepository(context);
            BookPicks = new Repository<BookPick>(context); 
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}