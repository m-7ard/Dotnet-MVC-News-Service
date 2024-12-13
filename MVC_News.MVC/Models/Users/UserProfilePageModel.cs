using MVC_News.Domain.Entities;
using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Models.Users;

public class UserProfilePageModel : BaseViewModel
{
    public UserProfilePageModel(User user, List<ArticleDTO> articles)
    {
        User = user;
        Articles = articles;
    }

    public User User { get; set; }
    public List<ArticleDTO> Articles { get; set; }
}