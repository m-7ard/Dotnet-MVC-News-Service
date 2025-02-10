using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Validators.MatchingPasswordHashValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Login;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, OneOf<LoginUserResult, List<ApplicationError>>>
{
    private readonly IUserExistsValidator<UserEmail> _userExistsValidator;
    private readonly IMatchingPasswordHashValidator<string> _areMatchingPasswordsValidator;

    public LoginUserHandler(IUserExistsValidator<UserEmail> userExistsValidator, IMatchingPasswordHashValidator<string> areMatchingPasswordsValidator)
    {
        _userExistsValidator = userExistsValidator;
        _areMatchingPasswordsValidator = areMatchingPasswordsValidator;
    }

    public async Task<OneOf<LoginUserResult, List<ApplicationError>>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var userEmailResult = UserEmail.TryCreate(request.Email);
        if (userEmailResult.TryPickT1(out var error, out var userEmail))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var userExistsResult = await _userExistsValidator.Validate(userEmail);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            // Different error for security reasons
            return ApplicationErrorFactory.CreateSingleListError(
                message: "Email or password is incorrect",
                code: ApplicationErrorCodes.Custom,
                path: []
            );
        }
        
        var areMatchingPasswordsResult = _areMatchingPasswordsValidator.Validate(hashedPassword: user.PasswordHash, plainPassword: request.Password);
        if (areMatchingPasswordsResult.TryPickT1(out errors, out var _))
        {
            // Different error for security reasons
            return ApplicationErrorFactory.CreateSingleListError(
                message: "Email or password is incorrect",
                code: ApplicationErrorCodes.Custom,
                path: []
            );
        }

        return new LoginUserResult(user: user);
    }
}