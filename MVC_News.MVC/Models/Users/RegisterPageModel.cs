using MVC_News.MVC.Models;

namespace MVC_Classifieds.Api.Models.Users;

public class RegisterPageModel : BaseViewModel
{
    public RegisterPageModel(string email, string password, Dictionary<string, List<string>> errors, string displayName)
    {
        Email = email;
        Password = password;
        Errors = errors;
        DisplayName = displayName;
    }

    public string Email { get; }
    public string Password { get; }
    public string DisplayName { get; }
    public Dictionary<string, List<string>> Errors { get; }
}