using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Validators.UserExistsValidator;

public class UserExistsEmailIdValidator : IUserExistsValidator<UserEmail>
{
    private readonly IUserRepository _userRepository;

    public UserExistsEmailIdValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OneOf<User, List<ApplicationError>>> Validate(UserEmail email)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);

        if (user is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User of email \"{email}\" does not exist.",
                code: ApplicationValidatorErrorCodes.USER_EXISTS_ERROR,
                path: []
            );
        }

        return user;
    }
}