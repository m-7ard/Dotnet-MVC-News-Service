namespace MVC_News.MVC.DTOs.Contracts.Articles.Update;

public class UpdateArticleRequestDTO
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string HeaderImage { get; set; } = null!;
}