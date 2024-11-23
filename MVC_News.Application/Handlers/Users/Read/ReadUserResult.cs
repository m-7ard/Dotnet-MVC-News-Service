using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Users.Read;

public class ReadUserResult
{
    public ReadUserResult(User user)
    {
        User = user;
    }

    public User User { get; set; }
}