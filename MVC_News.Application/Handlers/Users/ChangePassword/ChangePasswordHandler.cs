using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators.MatchingPasswordHashValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Handlers.Users.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, OneOf<ChangePasswordResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserExistsValidator<UserId> _userExistsValidator;
    private readonly IMatchingPasswordHashValidator<string> _areMatchingPasswordsValidator;


    public ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IUserExistsValidator<UserId> userExistsValidator, IMatchingPasswordHashValidator<string> areMatchingPasswordsValidator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _userExistsValidator = userExistsValidator;
        _areMatchingPasswordsValidator = areMatchingPasswordsValidator;
    }

    public async Task<OneOf<ChangePasswordResult, List<ApplicationError>>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
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

        // Passwords match
        var areMatchingPasswordsResult = _areMatchingPasswordsValidator.Validate(hashedPassword: user.PasswordHash, request.CurrentPassword);
        if (areMatchingPasswordsResult.TryPickT1(out errors, out var _))
        {
            return errors;
        }

        var updatedUser = UserFactory.BuildExisting(
            id: user.Id,
            email: user.Email,
            passwordHash: _passwordHasher.Hash(request.NewPassword),
            displayName: user.DisplayName,
            isAdmin: user.IsAdmin,
            subscriptions: user.Subscriptions
        );

        await _userRepository.UpdateAsync(updatedUser);

        return new ChangePasswordResult(user: updatedUser);
    }
}