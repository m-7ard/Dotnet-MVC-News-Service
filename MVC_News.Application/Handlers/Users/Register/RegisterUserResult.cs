using MVC_News.Domain.ValueObjects.User;

namespace MVC_News.Application.Handlers.Users.Register;

public class RegisterUserResult
{
    public UserId UserId { get; }

    public RegisterUserResult(UserId userId)
    {
        UserId = userId;
    }
}