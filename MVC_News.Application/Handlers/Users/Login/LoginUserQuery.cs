using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Users.Login;

public class LoginUserQuery : IRequest<OneOf<LoginUserResult, List<PlainApplicationError>>>
{
    public LoginUserQuery(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}