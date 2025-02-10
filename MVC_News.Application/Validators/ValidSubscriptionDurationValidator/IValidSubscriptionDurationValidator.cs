using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Validators.ValidSubscriptionDurationValidator;

public interface IValidSubscriptionDurationValidator<InputType> 
{
    public OneOf<DateTime, List<ApplicationError>> Validate(InputType input);
}