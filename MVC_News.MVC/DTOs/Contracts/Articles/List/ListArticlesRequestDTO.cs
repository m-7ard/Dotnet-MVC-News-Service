namespace MVC_News.MVC.DTOs.Contracts.Articles.List;

public class ListArticlesByTagRequestDTO
{
    public ListArticlesByTagRequestDTO(List<string>? tags)
    {
        Tags = tags;
    }

    public List<string>? Tags { get; }
}