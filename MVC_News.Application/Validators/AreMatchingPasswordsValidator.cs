using MVC_News.Application.Errors;
using MVC_News.Application.Interfaces.Services;
using OneOf;

namespace MVC_News.Application.Validators;

public class AreMatchingPasswordsValidator : IValidator<AreMatchingPasswordsValidator.Input, bool>
{
    public class Input
    {
        public string hashedPassword;
        public string plainPassword;

        public Input(string hashedPassword, string plainPassword)
        {
            this.hashedPassword = hashedPassword;
            this.plainPassword = plainPassword;
        }
    }

    private readonly IPasswordHasher _passwordHasher;

    public AreMatchingPasswordsValidator(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public OneOf<bool, List<ApplicationError>> Validate(Input input)
    {
        if (!_passwordHasher.Verify(passwordHash: input.hashedPassword, inputPassword: input.plainPassword))
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