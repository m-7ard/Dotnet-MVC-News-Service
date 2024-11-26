using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MVC_News.Application.Contracts.Criteria;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using MVC_News.Infrastructure.DbEntities;
using MVC_News.Infrastructure.Mappers;

namespace MVC_News.Infrastructure.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly NewsDbContext _dbContext;

    public ArticleRepository(NewsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Article> CreateAsync(Article article)
    {
        var dbEntity = ArticleMapper.FromDomainToDbEntity(article);
        _dbContext.Add(dbEntity);
        await _dbContext.SaveChangesAsync();
        return ArticleMapper.FromDbEntityToDomain(dbEntity);
    }

    public async Task UpdateAsync(Article article)
    {
        var oldDbEntity = await _dbContext.Article.SingleAsync(d => d.Id == article.Id);
        var newDbEntity = ArticleMapper.FromDomainToDbEntity(article);
        _dbContext.Entry(oldDbEntity).CurrentValues.SetValues(newDbEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Article?> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Article.Include(d => d.Author).SingleOrDefaultAsync(d => d.Id == id);
        return entity is null ? null : ArticleMapper.FromDbEntityToDomain(entity);
    }

    public async Task<List<Article>> FilterAllAsync(FilterAllArticlesCriteria criteria)
    {
        IQueryable<ArticleDbEntity> query = _dbContext.Article.Include(d => d.Author);
        
        if (criteria.Title is not null)
        {
            query = query.Where(article => article.Title.Contains(criteria.Title));
        }

        if (criteria.CreatedAfter is not null && criteria.CreatedBefore is not null && criteria.CreatedAfter > criteria.CreatedBefore)
        {
            criteria.CreatedBefore = null;
        }

        if (criteria.CreatedAfter is not null)
        {
            query = query.Where(article => article.DateCreated > criteria.CreatedAfter);
        }

        if (criteria.CreatedBefore is not null)
        {
            query = query.Where(article => article.DateCreated < criteria.CreatedBefore);
        }

        if (criteria.AuthorId is not null)
        {
            query = query.Where(article => article.AuthorId == criteria.AuthorId);
        }

        if (criteria.OrderBy is not null)
        {
            Dictionary<string, Expression<Func<ArticleDbEntity, object>>> fieldMappings = new()
            {
                { "DateCreated", p => p.DateCreated },
            };

            if (fieldMappings.TryGetValue(criteria.OrderBy.Item1, out var orderByExpression))
            {
                query = criteria.OrderBy.Item2 ? query.OrderBy(orderByExpression) : query.OrderByDescending(orderByExpression);
            }
        }

        if (criteria.LimitBy is not null)
        {
            query = query.Take(criteria.LimitBy.Value);
        }

        var articles = await query.ToListAsync();

        // Done on the server due to JSON field usage
        if (criteria.Tags is not null)
        {
            articles = articles.Where(article => criteria.Tags.All(tag => article.Tags.Contains(tag))).ToList();
        }

        return articles.Select(ArticleMapper.FromDbEntityToDomain).ToList();
    }
}
