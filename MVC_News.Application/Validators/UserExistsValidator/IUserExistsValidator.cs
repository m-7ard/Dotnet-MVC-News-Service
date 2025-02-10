using MVC_News.Application.Errors;
using MVC_News.Domain.Entities;
using OneOf;

namespace MVC_News.Application.Validators.UserExistsValidator;

public interface IUserExistsValidator<InputType> 
{
    public Task<OneOf<User, List<ApplicationError>>> Validate(InputType input);
}