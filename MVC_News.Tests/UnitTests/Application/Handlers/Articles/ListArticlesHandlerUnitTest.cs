using Moq;
using MVC_News.Application.Contracts.Criteria;
using MVC_News.Application.Handlers.Articles.List;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Domain.Entities;

namespace MVC_News.Tests.UnitTests.Application.Handlers.Articles;

public class ListArticlesHandlerUnitTest
{
    private readonly User _admin_001; 
    private readonly User _admin_002; 
    private readonly Article _article_001; 
    private readonly Article _article_002; 
    private readonly Article _article_003; 
    private readonly List<Article> _admin_001_articles; 
    private readonly List<Article> _admin_002_articles; 
    private readonly List<Article> _articles_all; 
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly ListArticlesHandler _handler;

    public ListArticlesHandlerUnitTest()
    {
        // Users
        _admin_001 = Mixins.CreateUser(seed: 1, isAdmin: true, subscriptions: []);
        _admin_002 = Mixins.CreateUser(seed: 2, isAdmin: true, subscriptions: []);
        
        // Articles
        _article_001 = Mixins.CreateArticle(seed: 1, authorId: _admin_001.Id);
        _article_002 = Mixins.CreateArticle(seed: 2, authorId: _admin_001.Id);
        _article_003 = Mixins.CreateArticle(seed: 3, authorId: _admin_002.Id);
        _articles_all = new List<Article>() { _article_001, _article_002, _article_003 };

        _admin_001_articles = new List<Article>() { _article_001, _article_002 };
        _admin_002_articles = new List<Article>() { _article_003 };

        // Dependencies
        _mockArticleRepository = new Mock<IArticleRepository>();
        _handler = new ListArticlesHandler(
            articleRepository: _mockArticleRepository.Object
        );
    }

    [Fact]
    public async Task ListArticles_NoArguments_Success()
    {
        // ARRANGE
        var command = new ListArticlesQuery(
            authorId: null,
            createdAfter: null,
            createdBefore: null,
            orderBy: null,
            limitBy: null,
            tags: null,
            title: null
        );

        var criteria = new FilterAllArticlesCriteria(
            createdAfter: command.CreatedAfter, 
            createdBefore: command.CreatedBefore,
            authorId: command.AuthorId,
            orderBy: new Tuple<string, bool>("DateCreated", false),
            limitBy: command.LimitBy,
            tags: command.Tags,
            title: command.Title
        );

        _mockArticleRepository
            .Setup(repo => repo.FilterAllAsync(criteria))
            .ReturnsAsync(_articles_all);

        // ACT
        var result = await _handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsT0);
        Assert.Equal(result.AsT0.Articles.Count, _articles_all.Count);
    }
}