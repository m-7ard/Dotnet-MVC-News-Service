using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Subscriptions;

public class CreateSubscriptionCommand : IRequest<OneOf<CreateSubscriptionResult, List<ApplicationError>>>
{
    public CreateSubscriptionCommand(Guid userId, int subscriptionDuration)
    {
        UserId = userId;
        SubscriptionDuration = subscriptionDuration;
    }

    public Guid UserId { get; }
    public int SubscriptionDuration { get; }
}