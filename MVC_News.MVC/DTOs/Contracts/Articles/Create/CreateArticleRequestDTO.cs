namespace MVC_News.MVC.DTOs.Contracts.Articles.Create;

public class CreateArticleRequestDTO
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string HeaderImage { get; set; } = null!;
    public List<string> Tags { get; set; } = null!;
}