using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Users.Login;

public class LoginUserResult
{
    public LoginUserResult(User user)
    {
        User = user;
    }

    public User User { get; }
}