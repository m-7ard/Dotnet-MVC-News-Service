using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Validators.MatchingPasswordHashValidator;

public interface IMatchingPasswordHashValidator<InputType> 
{
    public OneOf<bool, List<ApplicationError>> Validate(InputType hashedPassword, InputType plainPassword);
}