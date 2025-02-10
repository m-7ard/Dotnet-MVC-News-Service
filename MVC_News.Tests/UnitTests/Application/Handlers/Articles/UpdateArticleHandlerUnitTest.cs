using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Update;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators.ArticleExistsValidator;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class UpdateArticleHandlerUnitTest
{
    private readonly User _user_001; 
    private readonly User _admin_001; 
    private readonly Article _mockArticle; 
    private readonly Article _updateArticle; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<IUserExistsValidator<UserId>> _mockUserExistsValidator;
    private readonly Mock<IArticleExistsValidator<ArticleId>> _mockArticleExistsValidator;
    private readonly UpdateArticleHandler _handler;

    public UpdateArticleHandlerUnitTest()
    {
        // Users
        _user_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        _admin_001 = Mixins.CreateUser(seed: 2, isAdmin: true, subscriptions: []);
        
        // Articles
        _mockArticle = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);
        _updateArticle = ArticleFactory.BuildExisting(
            id: _mockArticle.Id,
            title: _mockArticle.Title + "_updated",
            content: _mockArticle + "_update",
            headerImage: _mockArticle.HeaderImage + "_updated",
            dateCreated: _mockArticle.DateCreated,
            authorId: _mockArticle.AuthorId,
            tags: new List<string>() { "tag_update" },
            isPremium: false
        );
        
        // Dependencies
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserId>>();
        _mockArticleExistsValidator = new Mock<IArticleExistsValidator<ArticleId>>();
        
        _handler = new UpdateArticleHandler(
            articleRepository: _mockArticleRepository.Object,
            userExistsValidatorAsync: _mockUserExistsValidator.Object,
            articleExistsValidatorAsync: _mockArticleExistsValidator.Object
        );
    }

    [Fact]
    public async Task UpdateArticle_AdminUser_Success()
    {
        // ARRANGE
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id.Value,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: _admin_001.Id.Value,
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags,
            isPremium: false
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _mockArticle.Id, _mockArticle);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _admin_001.Id, _admin_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);

        _mockArticleRepository.Verify(repo => repo.UpdateAsync(It.Is<Article>(d => d.Title == _updateArticle.Title)));
        _mockArticleRepository.Verify(repo => repo.UpdateAsync(It.Is<Article>(d => d.Content == _updateArticle.Content)));
        _mockArticleRepository.Verify(repo => repo.UpdateAsync(It.Is<Article>(d => d.HeaderImage == _updateArticle.HeaderImage)));
    }

    [Fact]
    public async Task UpdateArticle_NotAuthorisedUser_Failure()
    {
        // ARRANGE
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id.Value,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: _user_001.Id.Value,
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags,
            isPremium: false
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _mockArticle.Id, _mockArticle);
        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _user_001.Id, _user_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.NotAllowed, result.AsT1[0].Code);
    }

    [Fact]
    public async Task UpdateArticle_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var invalidGuid = Guid.NewGuid();
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id.Value,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: invalidGuid,
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags,
            isPremium: false
        );

        SetupMockServices.SetupArticleExistsValidatorSuccess(_mockArticleExistsValidator, _mockArticle.Id, _mockArticle);
        SetupMockServices.SetupUserExistsValidatorFailure(_mockUserExistsValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }

    [Fact]
    public async Task UpdateArticle_ArticleDoesNotExist_Failure()
    {
        // ARRANGE
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id.Value,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: Guid.NewGuid(),
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags,
            isPremium: false
        );
        SetupMockServices.SetupArticleExistsValidatorFailure(_mockArticleExistsValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);


        // ASSERT
        Assert.True(result.IsT1);
    }
}