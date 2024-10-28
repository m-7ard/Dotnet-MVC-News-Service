using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Domain.DomainFactories;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, OneOf<RegisterUserResult, List<PlainApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<OneOf<RegisterUserResult, List<PlainApplicationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new List<PlainApplicationError>()
            {
                new PlainApplicationError(
                    message: $"User with email \"{request.Email}\" already exists.",
                    fieldName: "Email",
                    code: ApplicationErrorCodes.ModelAlreadyExists
                )
            };
        }

        var newUser = await _userRepository.CreateAsync(
            UserFactory.BuildNewUser(
                email: request.Email,
                passwordHash: _passwordHasher.Hash(request.Password),
                displayName: request.DisplayName,
                isAdmin: false
            )
        );

        return new RegisterUserResult(user: newUser);
    }
}