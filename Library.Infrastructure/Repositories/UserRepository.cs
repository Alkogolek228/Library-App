using Library.Core.Entities;
using Library.Core.Interfaces;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null)
            {
                throw new Exception("No such user.");
            }
            
            return user;
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if( user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User> GetByRefreshTokenAsync(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
            if(user == null)
            {
                return null;
            }

            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}