using MVC_News.Application.Contracts.Criteria;
using MVC_News.Domain.Entities;

namespace MVC_News.Application.Interfaces.Repositories;

public interface IArticleRepository
{
    public Task<Article> CreateAsync(Article article);
    public Task UpdateAsync(Article article);
    public Task<Article?> GetByIdAsync(Guid id);
    public Task<List<Article>> FilterAllAsync(FilterAllArticlesCriteria criteria);
}