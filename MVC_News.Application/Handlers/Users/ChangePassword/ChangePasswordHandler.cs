using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Application.Validators;
using MVC_News.Domain.DomainFactories;
using OneOf;
using static MVC_News.Application.Validators.AreMatchingPasswordsValidator;

namespace MVC_News.Application.Handlers.Users.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, OneOf<ChangePasswordResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UserWithIdExistsValidatorAsync _userExistsValidatorAsync;
    private readonly AreMatchingPasswordsValidator _areMatchingPasswordsValidator;


    public ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
                
        _userExistsValidatorAsync = new UserWithIdExistsValidatorAsync(userRepository);
        _areMatchingPasswordsValidator = new AreMatchingPasswordsValidator(passwordHasher);
    }

    public async Task<OneOf<ChangePasswordResult, List<ApplicationError>>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userExistsResult = await _userExistsValidatorAsync.Validate(request.Id);
        if (userExistsResult.TryPickT1(out var errors, out var user))
        {
            return errors;
        }

        var areMatchingPasswordsResult = _areMatchingPasswordsValidator.Validate(new Input(hashedPassword: user.PasswordHash, request.CurrentPassword));
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