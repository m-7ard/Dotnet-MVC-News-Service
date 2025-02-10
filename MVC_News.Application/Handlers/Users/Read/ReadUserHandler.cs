using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Read;

public class ReadUserHandler : IRequestHandler<ReadUserQuery, OneOf<ReadUserResult, List<ApplicationError>>>
{
    private readonly IUserExistsValidator<UserId> _userExistsValidator;

    public ReadUserHandler(IUserExistsValidator<UserId> userExistsValidator)
    {
        _userExistsValidator = userExistsValidator;
    }

    public async Task<OneOf<ReadUserResult, List<ApplicationError>>> Handle(ReadUserQuery request, CancellationToken cancellationToken)
    {
        // User Exists
        var userIdResult = UserId.TryCreate(request.Id);
        if (userIdResult.TryPickT1(out var error, out var userId))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var userExistsResult = await _userExistsValidator.Validate(userId);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            return errors;
        }

        return new ReadUserResult(user);
    }
}