using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using OneOf;

namespace MVC_News.Application.Handlers.Subscriptions;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, OneOf<CreateSubscriptionResult, List<ApplicationError>>>
{
    private readonly UserWithIdExistsValidatorAsync _userExistsValidatorAsync;
    private readonly IUserRepository _userRepository;

    public CreateSubscriptionHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _userExistsValidatorAsync = new UserWithIdExistsValidatorAsync(userRepository);
    }

    public async Task<OneOf<CreateSubscriptionResult, List<ApplicationError>>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userExistsResult = await _userExistsValidatorAsync.Validate(request.UserId);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            return errors;
        }

        var isValidSubscriptionDurationValidator = new IsValidSubscriptionDurationValidator();
        var isValidSubscriptionDurationResult = isValidSubscriptionDurationValidator.Validate(request.SubscriptionDuration);
        if (isValidSubscriptionDurationResult.TryPickT1(out errors, out var expirationDate))
        {
            return errors;
        }

        var subscriptionResult = user.CanSubscribe(expirationDate);
        if (subscriptionResult.TryPickT1(out var error, out var _))
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: error,
                code: ApplicationErrorCodes.StateMismatch,
                path: []
            );
        }
        
        await _userRepository.UpdateAsync(user);
        return new CreateSubscriptionResult(user: user);
    }
}