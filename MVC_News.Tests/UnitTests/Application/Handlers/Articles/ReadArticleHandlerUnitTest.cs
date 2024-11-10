using Moq;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Create;
using MVC_News.Application.Handlers.Articles.Read;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class ReadArticleHandlerUnitTest
{
    private readonly User _admin_001; 
    private readonly Article _article_001; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly ReadArticleHandler _handler;

    public ReadArticleHandlerUnitTest()
    {
        _admin_001 = Mixins.CreateUser(seed: 1, isAdmin: true);
        _article_001 = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);

        _mockArticleRepository = new Mock<IArticleRepository>();
        _handler = new ReadArticleHandler(
            articleRepository: _mockArticleRepository.Object
        );
    }

    [Fact]
    public async Task ReadArticle_ArticleExists_Success()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: _article_001.Id
        );

        _mockArticleRepository
            .Setup(repo => repo.GetByIdAsync(query.Id))
            .ReturnsAsync(_article_001);

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ReadArticle_ArticleDoesNotExist_Failure()
    {
        // ARRANGE
        var query = new ReadArticleQuery(
            id: Guid.Empty
        );

        // ACT
        var result = await _handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT1);
        Assert.Equal(ApplicationErrorCodes.ModelDoesNotExist, result.AsT1[0].Code);
    }
}