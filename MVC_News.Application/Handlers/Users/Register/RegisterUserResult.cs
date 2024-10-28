using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Users.Register;

public class RegisterUserResult
{
    public RegisterUserResult(User user)
    {
        User = user;
    }

    public User User { get; }
}