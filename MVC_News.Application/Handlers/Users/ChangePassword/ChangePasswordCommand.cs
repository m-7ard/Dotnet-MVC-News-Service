using MediatR;
using MVC_News.Application.Errors;
using OneOf;

namespace MVC_News.Application.Handlers.Users.ChangePassword;

public class ChangePasswordCommand : IRequest<OneOf<ChangePasswordResult, List<ApplicationError>>>
{
    public ChangePasswordCommand(Guid id, string currentPassword, string newPassword)
    {
        Id = id;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }

    public Guid Id { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    
}