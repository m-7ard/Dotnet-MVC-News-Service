using Microsoft.AspNetCore.Identity;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;
using MVC_News.Infrastructure;
using MVC_News.Infrastructure.Repositories;
using MVC_News.Infrastructure.Services;

namespace MVC_News.Tests.IntegrationTests;

public class Mixins
{
    private readonly NewsDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public Mixins(NewsDbContext dbContexts)
    {
        _dbContext = dbContexts;
        _userRepository = new UserRepository(_dbContext);
        _articleRepository = new ArticleRepository(_dbContext);
        _passwordHasher = new PasswordHasher();
    }

    public async Task<User> CreateUser(int seed, bool isAdmin)
    {
        var user = await _userRepository.CreateAsync(
            UserFactory.BuildNew(
                email: $"user_{seed}@mail.com",
                passwordHash: _passwordHasher.Hash("userword"),
                displayName: $"user_{seed}",
                isAdmin: isAdmin,
                subscriptions: []
            )
        );

        return user;
    }
}