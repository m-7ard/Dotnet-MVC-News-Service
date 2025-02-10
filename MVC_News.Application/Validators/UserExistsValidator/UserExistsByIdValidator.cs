using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using OneOf;

namespace MVC_News.Application.Validators.UserExistsValidator;

public class UserExistsByIdValidator : IUserExistsValidator<UserId>
{
    private readonly IUserRepository _userRepository;

    public UserExistsByIdValidator(IUserRepository orderRepository)
    {
        _userRepository = orderRepository;
    }

    public async Task<OneOf<User, List<ApplicationError>>> Validate(UserId id)
    {
        var user = await _userRepository.GetUserById(id);

        if (user is null)
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"User of Id \"{id}\" does not exist.",
                code: ApplicationValidatorErrorCodes.USER_EXISTS_ERROR,
                path: []
            );
        }

        return user;
    }
}