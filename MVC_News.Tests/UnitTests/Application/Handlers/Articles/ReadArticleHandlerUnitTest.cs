using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Read;
using MVC_News.Application.Validators.ArticleExistsValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class ReadArticleHandlerUnitTest
{
    private readonly User _admin_001; 
    private readonly User _subbedUser_001; 
    private readonly User _freeUser_001; 
    private readonly Article _freeArticle_001; 
    private readonly Article _premiumArticle_001; 
    private readonly Mock<IArticleExistsValidator<ArticleId>> _mockArticleExistsValidator;
    private readonly Mock<IUserExistsValidator<UserId>> _mockUserExistsValidator;
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
        _mockArticleExistsValidator = new Mock<IArticleExistsValidator<ArticleId>>();
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserId>>();

        _handler = new ReadArticleHandler(
            articleExistsValidator: _mockArticleExistsValidator.Object,
            userExistsValidatorAsync: _mockUserExistsValidator.Object
        );
    }

    [Fact]
    public async Task ReadArticle_FreeArticleAsFreeUser_Success()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: _freeArticle_001.Id.Value,
            userId: _freeUser_001.Id.Value
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _freeArticle_001.Id, _freeArticle_001);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _freeUser_001.Id, _freeUser_001);

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
            id: _freeArticle_001.Id.Value,
            userId: _subbedUser_001.Id.Value
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _freeArticle_001.Id, _freeArticle_001);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _subbedUser_001.Id, _subbedUser_001);

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
            id: _premiumArticle_001.Id.Value,
            userId: _freeUser_001.Id.Value
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _premiumArticle_001.Id, _premiumArticle_001);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _freeUser_001.Id, _freeUser_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.NotAllowed, result.AsT1[0].Code);
    }

    [Fact]
    public async Task ReadArticle_ArticleDoesNotExist_Failure()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: Guid.Empty,
            userId: _admin_001.Id.Value
        );

        SetupMockServices.SetupArticleExistsValidatorFailure(_mockArticleExistsValidator);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _freeUser_001.Id, _freeUser_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }

    
    [Fact]
    public async Task ReadArticle_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: _freeArticle_001.Id.Value,
            userId: Guid.Empty
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _freeArticle_001.Id, _freeArticle_001);
        SetupMockServices.SetupUserExistsValidatorFailure(_mockUserExistsValidator);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }
}