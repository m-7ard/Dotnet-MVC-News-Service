namespace MVC_News.MVC.DTOs.Contracts.Articles.Preview;

public class PreviewArticleRequestDTO
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string HeaderImage { get; set; } = null!;
    public string[] Tags { get; set; } = null!;
    public bool IsPremium { get; set; }
}