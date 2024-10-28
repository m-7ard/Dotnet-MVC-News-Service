using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Login;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, OneOf<LoginUserResult, List<PlainApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<OneOf<LoginUserResult, List<PlainApplicationError>>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            return new List<PlainApplicationError>()
            {
                new PlainApplicationError(
                    message: $"Email or Password is incorrect.",
                    fieldName: "_",
                    code: ApplicationErrorCodes.Custom
                )
            };
        }

        if (_passwordHasher.Verify(user.PasswordHash, request.Password) is false)
        {
            return new List<PlainApplicationError>()
            {
                new PlainApplicationError(
                    message: $"Email or Password is incorrect.",
                    fieldName: "_",
                    code: ApplicationErrorCodes.Custom
                )
            };
        }

        return new LoginUserResult(user: user);
    }
}