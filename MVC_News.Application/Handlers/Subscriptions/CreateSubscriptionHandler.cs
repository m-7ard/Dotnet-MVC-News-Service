using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Application.Validators.ValidSubscriptionDurationValidator;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Subscriptions;

public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, OneOf<CreateSubscriptionResult, List<ApplicationError>>>
{
    private readonly IUserExistsValidator<UserId> _userExistsValidatorAsync;
    private readonly IUserRepository _userRepository;
    private readonly IValidSubscriptionDurationValidator<int> _validSubscriptionDurationValidator;

    public CreateSubscriptionHandler(IUserRepository userRepository, IUserExistsValidator<UserId> userExistsValidatorAsync, IValidSubscriptionDurationValidator<int> validSubscriptionDurationValidator)
    {
        _userRepository = userRepository;
        _userExistsValidatorAsync = userExistsValidatorAsync;
        _validSubscriptionDurationValidator = validSubscriptionDurationValidator;
    }

    public async Task<OneOf<CreateSubscriptionResult, List<ApplicationError>>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // User exists
        var userIdResult = UserId.TryCreate(request.UserId);
        if (userIdResult.TryPickT1(out var error, out var userId))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var userExistsResult = await _userExistsValidatorAsync.Validate(userId);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            return errors;
        }

        var validSubscriptionDurationResult = _validSubscriptionDurationValidator.Validate(request.SubscriptionDuration);
        if (validSubscriptionDurationResult.TryPickT1(out errors, out var expirationDate))
        {
            return errors;
        }

        var startDate = DateTime.UtcNow;
        var subscriptionResult = user.CanSubscribe(startDate, expirationDate);
        if (subscriptionResult.TryPickT1(out error, out var _))
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: error,
                code: ApplicationErrorCodes.StateMismatch,
                path: []
            );
        }

        user.ExcuteSubscribe(startDate, expirationDate);
        await _userRepository.UpdateAsync(user);
        return new CreateSubscriptionResult(user: user);
    }
}