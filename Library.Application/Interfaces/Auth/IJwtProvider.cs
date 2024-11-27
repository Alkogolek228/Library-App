using Library.Core.Entities;

namespace Library.Application.Interfaces.Auth
{
    public interface IJwtProvider
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}