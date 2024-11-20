using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Register;

public class RegisterUserCommand : IRequest<OneOf<RegisterUserResult, List<ApplicationError>>>
{
    public RegisterUserCommand(string email, string password, string displayName)
    {
        Email = email;
        Password = password;
        DisplayName = displayName;
    }

    public string Email { get; }
    public string Password { get; }
    public string DisplayName { get; }
}