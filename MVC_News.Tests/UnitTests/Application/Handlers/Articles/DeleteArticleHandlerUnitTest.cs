using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Delete;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators.ArticleExistsValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class DeleteArticleHandlerUnitTest
{
    private readonly User _admin_001; 
    private readonly User _client_001; 
    private readonly Article _article_001; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<IUserExistsValidator<UserId>> _mockUserExistsValidator;
    private readonly Mock<IArticleExistsValidator<ArticleId>> _mockArticleExistsValidator;

    private readonly DeleteArticleHandler _handler;

    public DeleteArticleHandlerUnitTest()
    {
        // Users
        _admin_001 = Mixins.CreateUser(seed: 1, isAdmin: true, subscriptions: []);

        _client_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);

        // Articles
        _article_001 = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);

        // Dependencies
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserId>>();
        _mockArticleExistsValidator = new Mock<IArticleExistsValidator<ArticleId>>();

        _handler = new DeleteArticleHandler(
            articleRepository: _mockArticleRepository.Object,
            userExistsValidatorAsync: _mockUserExistsValidator.Object,
            articleExistsValidatorAsync: _mockArticleExistsValidator.Object
        );
    }

    [Fact]
    public async Task DeleteArticle_AsAdmin_Success()
    {
        // ARRANGE
        var query = new DeleteArticleCommand(
            id: _article_001.Id.Value,
            userId: _admin_001.Id.Value
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _article_001.Id, _article_001);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _admin_001.Id, _admin_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task DeleteArticle_PremiumArticleAsSubbedUser_Success()
    {
        // ARRANGE
        var query = new DeleteArticleCommand(
            id: _article_001.Id.Value,
            userId: _client_001.Id.Value
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _article_001.Id, _article_001);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _client_001.Id, _client_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.NotAllowed, result.AsT1[0].Code);
    }
}