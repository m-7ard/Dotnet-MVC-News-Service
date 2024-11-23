namespace MVC_News.MVC.Models.Users;

public class CheckoutSubscriptionPageModel : BaseViewModel
{
    public CheckoutSubscriptionPageModel(Dictionary<string, List<string>> errors, int duration)
    {
        Errors = errors;
        Duration = duration;
    }

    public int Duration { get; }
    public Dictionary<string, List<string>> Errors { get; }
}