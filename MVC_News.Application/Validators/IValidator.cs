using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Validators;

public interface IValidator<Input, Success>
{
    public OneOf<Success, List<ApplicationError>> Validate(Input input);
}