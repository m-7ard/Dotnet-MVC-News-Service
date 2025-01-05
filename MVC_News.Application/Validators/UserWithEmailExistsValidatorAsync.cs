using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Application.Validators;

public class UserWithEmailExistsValidatorAsync : IValidatorAsync<string, User>
{
    private readonly IUserRepository _userRepository;

    public UserWithEmailExistsValidatorAsync(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OneOf<User, List<ApplicationError>>> Validate(string input)
    {
        var user = await _userRepository.GetUserByEmailAsync(input);

        if (user is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User of email \"{input}\" does not exist.",
                code: ApplicationValidatorErrorCodes.USER_WITH_EMAIL_EXISTS_ERROR,
                path: []
            );
        }

        return user;
    }
}