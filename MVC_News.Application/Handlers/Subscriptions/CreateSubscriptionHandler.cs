using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Application.Handlers.Subscriptions;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, OneOf<CreateSubscriptionResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;

    public CreateSubscriptionHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OneOf<CreateSubscriptionResult, List<ApplicationError>>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.UserId);
        if (user is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User of id \"{request.UserId}\" does not exist.",
                path: ["_"],
                code: ApplicationErrorCodes.ModelDoesNotExist
            );
        }

        if (user.HasActiveSubscription())
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User is already subscribed.",
                path: ["_"],
                code: ApplicationErrorCodes.StateMismatch
            );
        }
    
        var expirationDate = DateTime.Now;
        if (request.SubscriptionDuration == 1)
        {
            expirationDate = expirationDate.AddMonths(1);
        }
        else if (request.SubscriptionDuration == 2)
        {
            expirationDate = expirationDate.AddMonths(6);
        }
        else if (request.SubscriptionDuration == 3)
        {
            expirationDate = expirationDate.AddYears(1);
        }
        else
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"expirationDate is invalid.",
                path: ["expirationDate"],
                code: ApplicationErrorCodes.Custom
            );
        }

        var subscription = new Subscription(
            id: Guid.NewGuid(), 
            userId: request.UserId, 
            startDate: DateTime.Now, 
            expirationDate: expirationDate
        );

        user.Subscriptions.Add(subscription);
        await _userRepository.UpdateAsync(user);

        return new CreateSubscriptionResult(user: user);
    }
}