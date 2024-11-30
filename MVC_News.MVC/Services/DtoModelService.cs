using MVC_News.Domain.Entities;
using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Services;

public class DtoModelService
{
    static public ArticleDTO CreateArticleDTO(Article article, User? author)
    {
        return new ArticleDTO(
            id: article.Id,
            title: article.Title,
            content: article.Content,
            headerImage: article.HeaderImage,
            dateCreated: article.DateCreated,
            author: author is null ? CreateUnkownAuthorDTO() : CreateAuthorDTO(author),
            tags: article.Tags,
            isPremium: article.IsPremium
        );
    }

    static public AuthorDTO CreateAuthorDTO(User user)
    {
        return new AuthorDTO(
            id: user.Id,
            displayName: user.DisplayName
        );
    }

    static public AuthorDTO CreateUnkownAuthorDTO()
    {
        return new AuthorDTO(
            id: Guid.Empty,
            displayName: "Unkown Author"
        );
    }
}