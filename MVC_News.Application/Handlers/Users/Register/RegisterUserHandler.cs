using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Domain.DomainFactories;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, OneOf<RegisterUserResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<OneOf<RegisterUserResult, List<ApplicationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"User with email \"{request.Email}\" already exists.",
                    path: ["Email"],
                    code: ApplicationErrorCodes.ModelAlreadyExists
                )
            };
        }

        var newUser = await _userRepository.CreateAsync(
            UserFactory.BuildNew(
                email: request.Email,
                passwordHash: _passwordHasher.Hash(request.Password),
                displayName: request.DisplayName,
                isAdmin: false,
                subscriptions: []
            )
        );

        return new RegisterUserResult(user: newUser);
    }
}