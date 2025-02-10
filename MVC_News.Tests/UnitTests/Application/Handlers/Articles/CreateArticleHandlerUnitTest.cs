using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Create;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Validators.UserExistsValidator;
using MVC_News.Domain.Entities;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.Tests.UnitTests.Utils;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class CreateArticleHandlerUnitTest
{
    private readonly User _user_001; 
    private readonly User _admin_001; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<IUserExistsValidator<UserId>> _mockUserExistsValidator;
    private readonly CreateArticleHandler _handler;

    public CreateArticleHandlerUnitTest()
    {
        // Users
        _user_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        _admin_001 = Mixins.CreateUser(seed: 2, isAdmin: true, subscriptions: []);

        // Dependencies
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockUserExistsValidator = new Mock<IUserExistsValidator<UserId>>();
        
        _handler = new CreateArticleHandler(
            articleRepository: _mockArticleRepository.Object,
            userExistsValidatorAsync: _mockUserExistsValidator.Object
        );
    }

    [Fact]
    public async Task CreateArticle_AdminUser_Success()
    {
        // ARRANGE
        var mockArticle = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);
        var command = new CreateArticleCommand(
            title: mockArticle.Title,
            content: mockArticle.Content,
            authorId: mockArticle.AuthorId.Value,
            headerImage: mockArticle.HeaderImage,
            tags: mockArticle.Tags,
            isPremium: false
        );

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _admin_001.Id, _admin_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
        
        _mockArticleRepository.Verify(repo => repo.CreateAsync(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task CreateArticle_NotAuthorisedUser_Failure()
    {
        // ARRANGE
        var mockArticle = Mixins.CreateArticle(seed: 1, authorId: _user_001.Id);
        var command = new CreateArticleCommand(
            title: mockArticle.Title,
            content: mockArticle.Content,
            authorId: mockArticle.AuthorId.Value,
            headerImage: mockArticle.HeaderImage,
            tags: mockArticle.Tags,
            isPremium: false
        );

        SetupMockServices.SetupUserExistsValidatorSuccess(_mockUserExistsValidator, _user_001.Id, _user_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.NotAllowed, result.AsT1[0].Code);
    }

    [Fact]
    public async Task CreateArticle_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var mockArticle = Mixins.CreateArticle(seed: 1, authorId: UserId.NewUserId());
        var command = new CreateArticleCommand(
            title: mockArticle.Title,
            content: mockArticle.Content,
            authorId: mockArticle.AuthorId.Value,
            headerImage: mockArticle.HeaderImage,
            tags: mockArticle.Tags,
            isPremium: false
        );
        SetupMockServices.SetupUserExistsValidatorFailure(_mockUserExistsValidator);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
    }
}