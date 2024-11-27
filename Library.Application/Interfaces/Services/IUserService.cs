using Library.Application.DTOs;
using Library.Core.Entities;
using System.Threading.Tasks;

namespace Library.Application.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string userName, string password);
        Task RegisterAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<IEnumerable<BookDTO>> GetUserBooksAsync(Guid userId);
    }
}