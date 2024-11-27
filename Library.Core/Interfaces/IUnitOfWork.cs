using Library.Core.Entities;
using System.Threading.Tasks;

namespace Library.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Author> Authors { get; }
        IBookRepository Books { get; }
        IUserRepository Users { get; }
        IRepository<BookPick> BookPicks { get; }
        Task SaveChangesAsync();
    }
}