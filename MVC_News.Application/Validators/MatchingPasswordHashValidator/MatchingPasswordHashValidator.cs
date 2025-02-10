using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Services;
using OneOf;

namespace MVC_News.Application.Validators.MatchingPasswordHashValidator;

public class MatchingPasswordHashValidator : IMatchingPasswordHashValidator<string>
{
    private readonly IPasswordHasher _passwordHasher;

    public MatchingPasswordHashValidator(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public OneOf<bool, List<ApplicationError>> Validate(string hashedPassword, string plainPassword)
    {
        if (!_passwordHasher.Verify(passwordHash: hashedPassword, inputPassword: plainPassword))
        {
            return ApplicationErrorFactory.CreateSingleListError(
                message: $"Passwords do not match.",
                path: [],
                code: ApplicationValidatorErrorCodes.ARE_MATCHING_PASSWORDS_ERROR
            );
        }

        return true;
    }
}