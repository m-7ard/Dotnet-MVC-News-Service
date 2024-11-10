using Microsoft.EntityFrameworkCore;
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
        var oldDbEntity = _dbContext.Article.SingleAsync(d => d.Id == article.Id);
        var newDbEntity = ArticleMapper.FromDomainToDbEntity(article);
        _dbContext.Entry(oldDbEntity).CurrentValues.SetValues(newDbEntity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Article?> GetByIdAsync(Guid id)
    {
        var entity = await _dbContext.Article.Include(d => d.Author).SingleOrDefaultAsync(d => d.Id == id);
        return entity is null ? null : ArticleMapper.FromDbEntityToDomain(entity);
    }

    public async Task<List<Article>> FilterAllAsync(DateTime? createdAfter, DateTime? createdBefore, Guid? authorId)
    {
        IQueryable<ArticleDbEntity> query = _dbContext.Article.Include(d => d.Author);
        
        if (createdAfter is not null && createdBefore is not null && createdAfter > createdBefore)
        {
            createdBefore = null;
        }

        if (createdAfter is not null)
        {
            query = query.Where(article => article.DateCreated > createdAfter);
        }

        if (createdBefore is not null)
        {
            query = query.Where(article => article.DateCreated < createdBefore);
        }

        if (authorId is not null)
        {
            query = query.Where(article => article.AuthorId == authorId);
        }

        var articles = await query.ToListAsync();
        return articles.Select(ArticleMapper.FromDbEntityToDomain).ToList();
    }
}
