using MVC_News.Application.Contracts.Criteria;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;

namespace MVC_News.Application.Interfaces.Repositories;

public interface IArticleRepository
{
    public Task CreateAsync(Article article);
    public Task UpdateAsync(Article article);
    public Task<Article?> GetByIdAsync(ArticleId id);
    public Task DeleteAsync(Article article);
    public Task<List<Article>> FilterAllAsync(FilterAllArticlesCriteria criteria);
}