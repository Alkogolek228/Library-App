namespace Library.Application.Interfaces.Auth
{
    public interface IPasswordHasher
    {
        string GenerateHash(string password);
        bool Verify(string password, string hashedPassword);
    }
}