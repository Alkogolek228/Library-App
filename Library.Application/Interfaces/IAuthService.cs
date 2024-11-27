using Library.Application.Contracts.Auth;
using Library.Core.Entities;

namespace Library.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task RegisterUserAsync(RegisterUserRequest request);
        Task<TokensResponse> LoginUserAsync(LoginUserRequest request);
        Task<TokensResponse> RefreshTokensAsync(string refreshToken);
        Task<User> GetUserByTokenAsync(string refreshToken);
        Task<User> GetUserByUsernameAsync(string username);
    }
}