using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Create;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class CreateArticleHandlerUnitTest
{
    private readonly User _user_001; 
    private readonly User _admin_001; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateArticleHandler _handler;

    public CreateArticleHandlerUnitTest()
    {
        // Users
        _user_001 = Mixins.CreateUser(seed: 1, isAdmin: false, subscriptions: []);
        _admin_001 = Mixins.CreateUser(seed: 2, isAdmin: true, subscriptions: []);

        // Dependencies
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new CreateArticleHandler(
            userRepository: _mockUserRepository.Object,
            articleRepository: _mockArticleRepository.Object
        );
    }

    [Fact]
    public async Task CreateArticle_AdminUser_Success()
    {
        // ARRANGE
        var mockArticle = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);
        var command = new CreateArticleCommand(
            id: mockArticle.Id,
            title: mockArticle.Title,
            content: mockArticle.Content,
            authorId: mockArticle.AuthorId,
            headerImage: mockArticle.HeaderImage,
            tags: mockArticle.Tags,
            isPremium: false
        );

        _mockUserRepository
            .Setup(repo => repo.GetUserById(_admin_001.Id))
            .ReturnsAsync(_admin_001);

        _mockArticleRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Article>()))
            .ReturnsAsync(mockArticle);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
        
        _mockUserRepository.Verify(repo => repo.GetUserById(_admin_001.Id), Times.Once);
    }

    [Fact]
    public async Task CreateArticle_NotAuthorisedUser_Failure()
    {
        // ARRANGE
        var mockArticle = Mixins.CreateArticle(seed: 1, authorId: _user_001.Id);
        var command = new CreateArticleCommand(
            id: mockArticle.Id,
            title: mockArticle.Title,
            content: mockArticle.Content,
            authorId: mockArticle.AuthorId,
            headerImage: mockArticle.HeaderImage,
            tags: mockArticle.Tags,
            isPremium: false
        );

        _mockUserRepository
            .Setup(repo => repo.GetUserById(_user_001.Id))
            .ReturnsAsync(_user_001);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.NotAllowed, result.AsT1[0].Code);

        _mockUserRepository.Verify(repo => repo.GetUserById(_user_001.Id), Times.Once);
    }

    [Fact]
    public async Task CreateArticle_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var mockArticle = Mixins.CreateArticle(seed: 1, authorId: Guid.NewGuid());
        var command = new CreateArticleCommand(
            id: mockArticle.Id,
            title: mockArticle.Title,
            content: mockArticle.Content,
            authorId: mockArticle.AuthorId,
            headerImage: mockArticle.HeaderImage,
            tags: mockArticle.Tags,
            isPremium: false
        );

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationValidatorErrorCodes.USER_WITH_ID_EXISTS_ERROR, result.AsT1[0].Code);
    }
}