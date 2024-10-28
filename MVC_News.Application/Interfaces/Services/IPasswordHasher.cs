namespace MVC_News.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string inputPassword);
}