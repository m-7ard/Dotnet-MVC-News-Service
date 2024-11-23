using MVC_News.Domain.Entities;

namespace MVC_News.MVC.Models.Users;

public class UserAccountPageModel : BaseViewModel
{
    public UserAccountPageModel(Dictionary<string, List<string>> errors, User user, Subscription? subscription)
    {
        Errors = errors;
        User = user;
        Subscription = subscription;
    }

    public User User { get; set; }
    public Subscription? Subscription { get; set; }
    public Dictionary<string, List<string>> Errors { get; }
}