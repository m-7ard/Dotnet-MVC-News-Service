using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Users.ChangePassword;

public class ChangePasswordResult
{
    public ChangePasswordResult(User user)
    {
        User = user;
    }

    public User User { get; }
}