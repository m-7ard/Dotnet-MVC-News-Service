using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Subscriptions;

public class CreateSubscriptionResult
{
    public CreateSubscriptionResult(User user)
    {
        User = user;
    }

    public User User { get; set; }
}