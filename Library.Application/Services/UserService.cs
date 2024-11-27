using AutoMapper;
using Library.Application.DTOs;
using Library.Application.Interfaces;
using Library.Core.Entities;
using Library.Core.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<User?> AuthenticateAsync(string userName, string password)
        {
            var user = await _unitOfWork.Users.GetByUserNameAsync(userName);
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash))
            {
                return null;
            }

            return user;
        }

        public async Task RegisterAsync(User user)
        {
            user.PasswordHash = CreatePasswordHash(user.PasswordHash);
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }


        private bool VerifyPasswordHash(string password, string storedHash)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(storedHash));
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(computedHash) == storedHash;
        }

        public async Task<IEnumerable<BookDTO>> GetUserBooksAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("User not found");

            var bookPicks = await _unitOfWork.BookPicks.FindAsync(bp => bp.UserId == userId);
            var bookIds = bookPicks.Select(bp => bp.BookId).ToList();
            var books = await _unitOfWork.Books.FindAsync(b => bookIds.Contains(b.Id));

            return _mapper.Map<IEnumerable<BookDTO>>(books);
        }

        private string CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}