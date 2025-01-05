using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators;
using OneOf;
using static MVC_News.Application.Validators.AreMatchingPasswordsValidator;

namespace MVC_News.Application.Handlers.Users.Login;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, OneOf<LoginUserResult, List<ApplicationError>>>
{
    private readonly UserWithEmailExistsValidatorAsync _userExistsValidatorAsync;
    private readonly AreMatchingPasswordsValidator _areMatchingPasswordsValidator;

    public LoginUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userExistsValidatorAsync = new UserWithEmailExistsValidatorAsync(userRepository);
        _areMatchingPasswordsValidator = new AreMatchingPasswordsValidator(passwordHasher);
    }

    public async Task<OneOf<LoginUserResult, List<ApplicationError>>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var userExistsResult = await _userExistsValidatorAsync.Validate(request.Email);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            // Different error for security reasons
            return ApplicationErrorFactory.CreateSingleListError(
                message: "Email or password is incorrect",
                code: ApplicationErrorCodes.Custom,
                path: []
            );
        }
        
        var areMatchingPasswordsResult = _areMatchingPasswordsValidator.Validate(new Input(hashedPassword: user.PasswordHash, request.Password));
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