using MVC_News.Domain.Entities;
using MVC_News.MVC.DTOs.Models;

namespace MVC_News.MVC.Interfaces;

public interface IDtoModelService {
    public Task<ArticleDTO> CreateArticleDTO(Article article);
    public Task<List<ArticleDTO>> CreateManyArticleDTO(List<Article> articles);
}