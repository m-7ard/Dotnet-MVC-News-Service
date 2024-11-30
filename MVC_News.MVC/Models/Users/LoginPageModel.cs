using MVC_News.MVC.Models;

namespace MVC_Classifieds.Api.Models.Users;

public class LoginPageModel : BaseViewModel
{
    public LoginPageModel(string email, string password, Dictionary<string, List<string>> errors, string? returnUrl)
    {
        Email = email;
        Password = password;
        Errors = errors;
        ReturnUrl = returnUrl;
    }

    public string Email { get; }
    public string Password { get; }
    public string? ReturnUrl { get; }
    public Dictionary<string, List<string>> Errors { get; }
}