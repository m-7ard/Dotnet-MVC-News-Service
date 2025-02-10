using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Register;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, OneOf<RegisterUserResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserExistsValidator<UserEmail> _userExistsValidator;


    public RegisterUserHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IUserExistsValidator<UserEmail> userExistsValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userExistsValidator = userExistsValidator;
    }

    public async Task<OneOf<RegisterUserResult, List<ApplicationError>>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var tryCreateResult = UserEmail.TryCreate(request.Email);
        if (tryCreateResult.TryPickT1(out var error, out var userEmail))
        {
            return ApplicationErrorFactory.CreateSingleListError(message: error, path: [], code: ApplicationErrorCodes.NotAllowed);
        }

        var userExistsResult = await _userExistsValidator.Validate(userEmail);
        if (userExistsResult.TryPickT0(out var existingUser, out var _))
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User with email \"{existingUser.Email}\" already exists.",
                code: ApplicationErrorCodes.ModelAlreadyExists,
                path: []
            );
        }

        var newUser = UserFactory.BuildNew(
            userId: UserId.NewUserId(),
            email: userEmail,
            passwordHash: _passwordHasher.Hash(request.Password),
            displayName: request.DisplayName,
            isAdmin: false,
            subscriptions: []
        );

        await _userRepository.CreateAsync(newUser);

        return new RegisterUserResult(userId: newUser.Id);
    }
}