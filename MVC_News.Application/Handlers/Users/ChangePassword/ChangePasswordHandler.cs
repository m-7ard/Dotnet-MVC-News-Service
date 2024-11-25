using MediatR;
using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Domain.DomainFactories;
using OneOf;

namespace MVC_News.Application.Handlers.Users.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, OneOf<ChangePasswordResult, List<ApplicationError>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<OneOf<ChangePasswordResult, List<ApplicationError>>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserById(request.Id);
        if (user is null)
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"User of Id \"{request.Id}\" does not exist.",
                    path: ["_"],
                    code: ApplicationErrorCodes.ModelDoesNotExist
                )
            };
        }

        if (_passwordHasher.Verify(user.PasswordHash, request.CurrentPassword) is false)
        {
            return new List<ApplicationError>()
            {
                new ApplicationError(
                    message: $"Password is incorrect.",
                    path: ["CurrentPassword"],
                    code: ApplicationErrorCodes.ValidationFailure
                )
            };
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