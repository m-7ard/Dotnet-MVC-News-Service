using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Create;
using MVC_News.Application.Handlers.Articles.Update;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class UpdateArticleHandlerUnitTest
{
    private readonly User _user_001; 
    private readonly User _admin_001; 
    private readonly Article _mockArticle; 
    private readonly Article _updateArticle; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UpdateArticleHandler _handler;

    public UpdateArticleHandlerUnitTest()
    {
        _user_001 = Mixins.CreateUser(seed: 1, isAdmin: false);
        _admin_001 = Mixins.CreateUser(seed: 2, isAdmin: true);
        _mockArticle = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);
        _updateArticle = ArticleFactory.BuildExisting(
            id: _mockArticle.Id,
            title: _mockArticle.Title + "_updated",
            content: _mockArticle + "_update",
            headerImage: _mockArticle.HeaderImage + "_updated",
            dateCreated: _mockArticle.DateCreated,
            authorId: _mockArticle.AuthorId,
            tags: new List<string>() { "tag_update" }
        );

        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new UpdateArticleHandler(
            userRepository: _mockUserRepository.Object,
            articleRepository: _mockArticleRepository.Object
        );
    }

    [Fact]
    public async Task UpdateArticle_AdminUser_Success()
    {
        // ARRANGE
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: _admin_001.Id,
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(_mockArticle.Id))
            .ReturnsAsync(_mockArticle);

        _mockUserRepository
            .Setup(repo => repo.GetUserById(_admin_001.Id))
            .ReturnsAsync(_admin_001);

        _mockArticleRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Article>()))
            .Returns(Task.CompletedTask);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);

        _mockUserRepository.Verify(repo => repo.GetUserById(_admin_001.Id), Times.Once);

        _mockArticleRepository.Verify(repo => repo.UpdateAsync(It.Is<Article>(d => d.Title == _updateArticle.Title)));
        _mockArticleRepository.Verify(repo => repo.UpdateAsync(It.Is<Article>(d => d.Content == _updateArticle.Content)));
        _mockArticleRepository.Verify(repo => repo.UpdateAsync(It.Is<Article>(d => d.HeaderImage == _updateArticle.HeaderImage)));
    }

    [Fact]
    public async Task UpdateArticle_NotAuthorisedUser_Failure()
    {
        // ARRANGE
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: _user_001.Id,
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(_mockArticle.Id))
            .ReturnsAsync(_mockArticle);

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
    public async Task UpdateArticle_UserDoesNotExist_Failure()
    {
        // ARRANGE
        var invalidGuid = Guid.NewGuid();
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: invalidGuid,
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(_mockArticle.Id))
            .ReturnsAsync(_mockArticle);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelDoesNotExist, result.AsT1[0].Code);

        _mockUserRepository.Verify(repo => repo.GetUserById(invalidGuid));
    }

    [Fact]
    public async Task UpdateArticle_ArticleDoesNotExist_Failure()
    {
        // ARRANGE
        var command = new UpdateArticleCommand(
            id: _mockArticle.Id,
            title: _updateArticle.Title,
            content: _updateArticle.Content,
            authorId: Guid.NewGuid(),
            headerImage: _updateArticle.HeaderImage,
            tags: _mockArticle.Tags
        );

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelDoesNotExist, result.AsT1[0].Code);

        _mockArticleRepository.Verify(repo => repo.GetByIdAsync(_mockArticle.Id));
    }
}