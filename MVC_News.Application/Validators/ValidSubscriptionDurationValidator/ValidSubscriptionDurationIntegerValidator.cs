using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Validators.ValidSubscriptionDurationValidator;

public class ValidSubscriptionDurationIntegerValidator : IValidSubscriptionDurationValidator<int>
{
    public OneOf<DateTime, List<ApplicationError>> Validate(int input)
    {
        var expirationDate = DateTime.UtcNow;
        if (input == 1)
        {
            expirationDate = expirationDate.AddMonths(1);
        }
        else if (input == 2)
        {
            expirationDate = expirationDate.AddMonths(6);
        }
        else if (input == 3)
        {
            expirationDate = expirationDate.AddYears(1);
        }
        else
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"Invalid expiration duration.",
                code: ApplicationValidatorErrorCodes.IS_VALID_SUBSCRIPTION_DURATION_ERROR,
                path: []
            );
        }

        return expirationDate;
    }
}