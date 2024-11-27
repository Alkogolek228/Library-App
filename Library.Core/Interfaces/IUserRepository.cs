using Library.Core.Entities;
using System.Threading.Tasks;

namespace Library.Core.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUserNameAsync(string userName);
        Task<User> GetByRefreshTokenAsync(string token);
        Task UpdateAsync(User user);
    }
}