using Library.Application.Contracts.Auth;
using Library.Application.Interfaces.Auth;
using Library.Application.Interfaces.Services;
using Library.Core.Entities;
using Library.Core.Interfaces;

namespace Library.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
        }

        public async Task<TokensResponse> LoginUserAsync(LoginUserRequest request)
        {
            var user = await _unitOfWork.Users.GetByUserNameAsync(request.UserName);

            var result = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if(result == false)
            {
                throw new Exception("Failed to login");
            }

            var accessToken = _jwtProvider.GenerateAccessToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new TokensResponse(accessToken, refreshToken, user.Id);
        }

        public async Task RegisterUserAsync(RegisterUserRequest request)
        {
            if(request.Password != request.PasswordConfirm)
            {
                throw new ArgumentException("Пароли не совпадают.");
            }

            var existingUser = await _unitOfWork.Users.GetByUserNameAsync(request.UserName);
            if (existingUser != null)
            {
                throw new ArgumentException("Пользователь с таким именем уже существует.");
            }

            var passwordHash = _passwordHasher.GenerateHash(request.Password);

            var user = User.Create(Guid.NewGuid(), request.UserName, passwordHash, request.Email);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TokensResponse> RefreshTokensAsync(string refreshToken)
        {
            var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var newAccessToken = _jwtProvider.GenerateAccessToken(user);
            var newRefreshToken = _jwtProvider.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new TokensResponse(newAccessToken, newRefreshToken, user.Id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            var user = await _unitOfWork.Users.GetByUserNameAsync(username);

            if (user == null)
            {
                return null;
            }

            return user;
        }

        public async Task<User> GetUserByTokenAsync(string refreshToken)
        {
            var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                throw new Exception("No such user.");
            }

            return user;
        }
    }
}