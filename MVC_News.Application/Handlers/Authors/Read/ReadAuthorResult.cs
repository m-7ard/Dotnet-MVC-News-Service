using MVC_News.Domain.Entities;

namespace MVC_News.Application.Handlers.Authors.Read;

public class ReadAuthorResult
{
    public ReadAuthorResult(Author author)
    {
        Author = author;
    }

    public Author Author { get; }
}