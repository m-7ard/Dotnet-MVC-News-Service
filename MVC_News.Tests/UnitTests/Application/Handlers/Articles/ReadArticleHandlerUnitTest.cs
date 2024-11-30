using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Create;
using MVC_News.Application.Handlers.Articles.Delete;
using MVC_News.Application.Handlers.Articles.Read;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;
using MVC_News.Domain.Errors;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class ReadArticleHandlerUnitTest
{
    private readonly User _admin_001; 
    private readonly User _subbedUser_001; 
    private readonly User _freeUser_001; 
    private readonly Article _freeArticle_001; 
    private readonly Article _premiumArticle_001; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly ReadArticleHandler _handler;

    public ReadArticleHandlerUnitTest()
    {
        // Users
        _admin_001 = Mixins.CreateUser(seed: 1, isAdmin: true, subscriptions: []);
        
        _subbedUser_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        _subbedUser_001.Subscriptions.Add(
            Mixins.CreatedValidSubscription(_subbedUser_001.Id)
        );

        _freeUser_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        _freeUser_001.Subscriptions.Add(
            Mixins.CreatedExpiredSubscription(_subbedUser_001.Id)
        );

        // Articles
        _freeArticle_001 = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);
        _premiumArticle_001 = Mixins.CreateArticle(seed: 2, authorId: _admin_001.Id, isPremium: true);

        // Dependencies
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new ReadArticleHandler(
            articleRepository: _mockArticleRepository.Object,
            userRespository: _mockUserRepository.Object
        );
    }

    [Fact]
    public async Task ReadArticle_FreeArticleAsFreeUser_Success()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: _freeArticle_001.Id,
            userId: _freeUser_001.Id
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(query.Id))
            .ReturnsAsync(_freeArticle_001);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(query.UserId))
            .ReturnsAsync(_freeUser_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ReadArticle_PremiumArticleAsSubbedUser_Success()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: _freeArticle_001.Id,
            userId: _subbedUser_001.Id
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(query.Id))
            .ReturnsAsync(_freeArticle_001);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(query.UserId))
            .ReturnsAsync(_subbedUser_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ReadArticle_PremiumArticleAsFreeUser_Failure()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: _premiumArticle_001.Id,
            userId: _freeUser_001.Id
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(query.Id))
            .ReturnsAsync(_premiumArticle_001);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(query.UserId))
            .ReturnsAsync(_freeUser_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.DomainError, result.AsT1[0].Code);
        var metaData = (ApplicationDomainErrorMetadata)result.AsT1[0].Metadata;
        Assert.Equal(ArticleDomainErrorsCodes.UserNotAllowed, metaData.OriginalError.Code);
    }

    [Fact]
    public async Task ReadArticle_ArticleDoesNotExist_Failure()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: Guid.Empty,
            userId: _admin_001.Id
        );

        _mockUserRepository
            .Setup(repo => repo.GetUserById(query.UserId))
            .ReturnsAsync(_freeUser_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelDoesNotExist, result.AsT1[0].Code);
    }

    
    [Fact]
    public async Task ReadArticle_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: _freeArticle_001.Id,
            userId: Guid.Empty
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(query.Id))
            .ReturnsAsync(_freeArticle_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelDoesNotExist, result.AsT1[0].Code);
    }
}