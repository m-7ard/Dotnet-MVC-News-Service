using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Application.Validators;

public class UserWithIdExistsValidatorAsync : IValidatorAsync<Guid, User>
{
    private readonly IUserRepository _userRepository;

    public UserWithIdExistsValidatorAsync(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<OneOf<User, List<ApplicationError>>> Validate(Guid input)
    {
        var user = await _userRepository.GetUserById(input);

        if (user is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User of id \"{input}\" does not exist.",
                code: ApplicationValidatorErrorCodes.USER_WITH_ID_EXISTS_ERROR,
                path: []
            );
        }

        return user;
    }
}