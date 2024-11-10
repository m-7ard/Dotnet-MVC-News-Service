using MVC_News.Domain.Entities;
using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Services;

public class DtoModelService
{
    static public ArticleDTO CreateArticleDTO(Article article, Author author)
    {
        return new ArticleDTO(
            id: article.Id,
            title: article.Title,
            content: article.Content,
            headerImage: article.HeaderImage,
            dateCreated: article.DateCreated,
            author: CreateAuthorDTO(author)
        );
    }

    static public AuthorDTO CreateAuthorDTO(Author author)
    {
        return new AuthorDTO(
            id: author.Id,
            displayName: author.DisplayName
        );
    }
}