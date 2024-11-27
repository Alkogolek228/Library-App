namespace Library.Application.Contracts.Auth
{
    public record TokensResponse(
        string AccessToken,
        string RefreshToken,
        Guid UserId);
}