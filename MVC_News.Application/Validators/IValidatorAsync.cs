using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Validators;

public interface IValidatorAsync<Input, Success>
{
    public Task<OneOf<Success, List<ApplicationError>>> Validate(Input input);
}