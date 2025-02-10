using Api.Services;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.MVC.DTOs.Models;
using MVC_News.MVC.Interfaces;

namespace MVC_News.MVC.Services;

public class DtoModelService : IDtoModelService
{
    private readonly Dictionary<UserId, User?> UserCache = new Dictionary<UserId, User?>();
    private readonly IUserRepository _userRepository;

    public DtoModelService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    private async Task<User?> GetUserFromCacheOrDb(UserId id) 
    {
        if (UserCache.TryGetValue(id, out var cachedUser))
        {
            return cachedUser;
        }

        var user = await _userRepository.GetUserById(id);
        UserCache[id] = user;
        return user;
    }

    public async Task<ArticleDTO> CreateArticleDTO(Article article)
    {
        var user = await GetUserFromCacheOrDb(article.AuthorId);
        var author = user is null ? AuthorDTO.UNKOWN_AUTHOR : new AuthorDTO(id: user.Id.Value, displayName: user.DisplayName);
        
        return new ArticleDTO(
            id: article.Id.Value, 
            title: article.Title, 
            content: article.Content, 
            headerImage: article.HeaderImage, 
            dateCreated: TimeZoneService.ConvertUtcToLocalTime(article.DateCreated), 
            author: author, 
            tags: article.Tags, 
            isPremium: article.IsPremium
        );
    }

    public async Task<List<ArticleDTO>> CreateManyArticleDTO(List<Article> articles)
    {
        var results = new List<ArticleDTO>();

        foreach (var article in articles)
        {
            results.Add(await CreateArticleDTO(article));
        }

        return results;
    }
}