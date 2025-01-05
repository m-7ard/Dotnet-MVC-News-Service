using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators;
using MVC_News.Domain.DomainFactories;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, OneOf<RegisterUserResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserWithEmailExistsValidatorAsync _userExistsValidatorAsync;


    public RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userExistsValidatorAsync = new UserWithEmailExistsValidatorAsync(userRepository);

    }

    public async Task<OneOf<RegisterUserResult, List<ApplicationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userExistsResult = await _userExistsValidatorAsync.Validate(request.Email);
        if (userExistsResult.TryPickT0(out var existingUser, out var _))
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User with email \"{existingUser.Email}\" already exists.",
                code: ApplicationErrorCodes.ModelAlreadyExists,
                path: []
            );
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