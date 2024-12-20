using System.Net;
using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Create;
using MVC_News.Application.Handlers.Articles.Delete;
using MVC_News.Application.Handlers.Articles.List;
using MVC_News.Application.Handlers.Articles.Read;
using MVC_News.Application.Handlers.Articles.Update;
using MVC_News.Application.Handlers.Users.Read;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Errors;
using MVC_News.MVC.DTOs.Contracts.Articles.Create;
using MVC_News.MVC.DTOs.Contracts.Articles.List;
using MVC_News.MVC.DTOs.Contracts.Articles.Manage;
using MVC_News.MVC.DTOs.Contracts.Articles.Preview;
using MVC_News.MVC.DTOs.Contracts.Articles.Search;
using MVC_News.MVC.DTOs.Contracts.Articles.Update;
using MVC_News.MVC.DTOs.Models;
using MVC_News.MVC.Errors;
using MVC_News.MVC.Exceptions;
using MVC_News.MVC.Models.Articles;
using MVC_News.MVC.Services;
using Sprache;

namespace MVC_News.MVC.Controllers;

public class ArticlesController : BaseController
{
    private readonly ISender _mediator;
    private readonly IValidator<CreateArticleRequestDTO> _createArticleValidator;
    private readonly IValidator<UpdateArticleRequestDTO> _updateArticleValidator;

    private List<string> ProcessRequestTags(List<string> tags)
    {
        return tags.Where(item => !string.IsNullOrEmpty(item)).ToList();
    }

    private Guid TryReadArticleId(string? input)
    {
        if (!Guid.TryParse(input, out var parsedArticleId))
        {
            throw new NotFoundException("Article Id is invalid.");        
        }

        return parsedArticleId;
    }

    public ArticlesController(ISender mediator, IValidator<CreateArticleRequestDTO> createArticleValidator, IValidator<UpdateArticleRequestDTO> updateArticleValidator)
    {
        _mediator = mediator;
        _createArticleValidator = createArticleValidator;
        _updateArticleValidator = updateArticleValidator;
    }

    // ***************
    // FRONTPAGE
    //
    //

    [HttpGet("/")]
    public async Task<IActionResult> Frontpage()
    {
        var query = new ListArticlesQuery(
            title: null,
            authorId: null, 
            createdAfter: null, 
            createdBefore: null, 
            orderBy: "newest", 
            limitBy: 24,
            tags: null
        );
        var result = await _mediator.Send(query);
        if (result.TryPickT1(out var errors, out var value))
        {
            throw new InternalServerErrorException($"Something went wrong trying to list the frontpage articles.");
        }

        var articleDTOs = new List<ArticleDTO>();
        foreach (var article in value.Articles)
        {
            var readUserQuery = new ReadUserQuery(id: article.AuthorId);
            var readUserResult = await _mediator.Send(readUserQuery);
            var author = readUserResult.IsT0 ? readUserResult.AsT0.User : null;
            articleDTOs.Add(DtoModelService.CreateArticleDTO(article: article, author: author));
        }

        List<ArticleDTO> mainArticles = articleDTOs.Take(3).ToList();

        return View(new FrontpagePageModel(
            mainArticles: mainArticles,
            newestArticles: articleDTOs.Skip(3).ToList()
        ));
    }

    // ***************
    // CREATE ARTICLE
    //
    //

    [Authorize(Roles = "Admin")]
    [HttpGet("/articles/create")]
    public IActionResult CreateArticlePage(
        [FromQuery] string? headerImage, 
        [FromQuery] string? title, 
        [FromQuery] string? content,
        [FromQuery] List<string>? tags,
        [FromQuery] bool? isPremium)
    {
        return View(new CreateArticlePageModel(
            title: title ?? "",
            content: content ?? "",
            headerImage: headerImage ?? "",
            tags: tags is null ? new List<string>() : ProcessRequestTags(tags),
            errors: new Dictionary<string, List<string>>(),
            isPremium: isPremium ?? false
        ));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/articles/create")]
    public async Task<IActionResult> CreateArticlePage([FromForm] CreateArticleRequestDTO request)
    {
        request.Tags = ProcessRequestTags(request.Tags);

        var userId = TryReadUserIdFromClaims();

        var validation = _createArticleValidator.Validate(request);
        if (!validation.IsValid)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            
            return View(new CreateArticlePageModel(
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                tags: request.Tags,
                errors: PlainMvcErrorFactory.FluentToApiErrors(validation.Errors),
                isPremium: request.IsPremium
            ));
        }

        var command = new CreateArticleCommand(
            id: Guid.NewGuid(),
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: userId,
            tags: request.Tags.ToList(),
            isPremium: request.IsPremium
        );
        var result = await _mediator.Send(command);

        if (result.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View(new CreateArticlePageModel(
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                tags: request.Tags,
                errors: PlainMvcErrorFactory.TranslateServiceErrors(errors),
                isPremium: request.IsPremium
            ));
        }

        Response.StatusCode = (int)HttpStatusCode.Created;
        return Redirect($"/articles/{value.Article.Id}");
    }

    // ***************
    // UPDATE ARTICLE
    //
    //

    [Authorize(Roles = "Admin")]
    [HttpGet("/articles/{id}/update")]
    public async Task<IActionResult> UpdateArticlePage(
        string id,
        [FromQuery] string? headerImage, 
        [FromQuery] string? title, 
        [FromQuery] string? content,
        [FromQuery] List<string>? tags,
        [FromQuery] bool? isPremium)
    {
        tags = tags is null ? null : ProcessRequestTags(tags);

        var parsedArticleId = TryReadArticleId(id);
        var parsedUserId = TryReadUserIdFromClaims();

        var query = new ReadArticleQuery(id: parsedArticleId, userId: parsedUserId);
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            if (errors.First().Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(errors.First().Message);
            }
        }

        Response.StatusCode = (int)HttpStatusCode.OK;
        return View(new UpdateArticlePageModel(
            id: value.Article.Id,
            title: title ?? value.Article.Title,
            content: content ?? value.Article.Content,
            headerImage: headerImage ?? value.Article.HeaderImage,
            tags: tags.IsNullOrEmpty() ? value.Article.Tags : tags!.ToList(),
            errors: new Dictionary<string, List<string>>(),
            isPremium: isPremium ?? value.Article.IsPremium
        ));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/articles/{id}/update")]
    public async Task<IActionResult> UpdateArticlePage([FromForm] UpdateArticleRequestDTO request, string id)
    {
        var tags = ProcessRequestTags(request.Tags.ToList());

        var parsedArticleId = TryReadArticleId(id);
        var parsedUserId = TryReadUserIdFromClaims();

        var validation = _updateArticleValidator.Validate(request);
        if (!validation.IsValid)
        {
            return View(new UpdateArticlePageModel(
                id: parsedArticleId,
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                tags: tags,
                errors: PlainMvcErrorFactory.FluentToApiErrors(validation.Errors),
                isPremium: request.IsPremium
            ));
        }

        var command = new UpdateArticleCommand(
            id: parsedArticleId,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: parsedUserId,
            tags: tags,
            isPremium: request.IsPremium
        );
        var result = await _mediator.Send(command);

        if (result.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return View(new UpdateArticlePageModel(
                id: parsedArticleId,
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                tags: tags,
                errors: PlainMvcErrorFactory.TranslateServiceErrors(errors),
                isPremium: request.IsPremium
            ));
        }

        Response.StatusCode = (int)HttpStatusCode.OK;
        return Redirect($"/articles/{value.Article.Id}");
    }

    // ***************
    // MANAGE ARTICLES
    //
    //

    [Authorize(Roles = "Admin")]
    [HttpGet("/articles/manage")]
    public async Task<IActionResult> ManageArticlesPage(
        [FromQuery] string? title, 
        [FromQuery] Guid? authorId, 
        [FromQuery] DateTime? createdAfter, 
        [FromQuery] DateTime? createdBefore,
        [FromQuery] string? orderBy,
        [FromQuery] int? limitBy,
        [FromQuery] List<string>? tags)
    {
        var request = new ManageArticlesRequestDTO(
            title: title,
            authorId: authorId,
            createdAfter: createdAfter,
            createdBefore: createdBefore,
            orderBy: orderBy,
            limitBy: limitBy,
            tags: tags
        );

        var listArticlesQuery = new ListArticlesQuery(
            title: request.Title,
            authorId: request.AuthorId,
            createdAfter: request.CreatedAfter,
            createdBefore: request.CreatedBefore,
            orderBy: request.OrderBy,
            limitBy: request.LimitBy,
            tags: request.Tags
        );
        var listArticlesResult = await _mediator.Send(listArticlesQuery);
        if (listArticlesResult.TryPickT1(out var errors, out var value))
        {
            throw new InternalServerErrorException($"Something went wrong trying to list articles.");
        }

        var articleDTOs = new List<ArticleDTO>();
        foreach (var article in value.Articles)
        {
            var readUserQuery = new ReadUserQuery(id: article.AuthorId);
            var readUserResult = await _mediator.Send(readUserQuery);
            var author = readUserResult.IsT0 ? readUserResult.AsT0.User : null;
            articleDTOs.Add(DtoModelService.CreateArticleDTO(article: article, author: author));
        }

        return View(new ManageArticlesPageModel(
            articles: articleDTOs
        ));
    }

    
    // ***************
    // LIST ALL ARTICLES BY TAG
    //
    //

    [HttpGet("/articles/tag/{tag}")]
    public async Task<IActionResult> ListArticlesByTagPage(string tag)
    {
        var request = new ListArticlesByTagRequestDTO(
            tags: [tag]
        );

        var listArticlesQuery = new ListArticlesQuery(
            title: null,
            authorId: null,
            createdAfter: null,
            createdBefore: null,
            orderBy: null,
            limitBy: null,
            tags: request.Tags
        );
        var listArticlesResult = await _mediator.Send(listArticlesQuery);
        if (listArticlesResult.TryPickT1(out var errors, out var value))
        {
            throw new InternalServerErrorException($"Something went wrong trying to list articles.");
        }

        var articleDTOs = new List<ArticleDTO>();
        foreach (var article in value.Articles)
        {
            var readUserQuery = new ReadUserQuery(id: article.AuthorId);
            var readUserResult = await _mediator.Send(readUserQuery);
            var author = readUserResult.IsT0 ? readUserResult.AsT0.User : null;
            articleDTOs.Add(DtoModelService.CreateArticleDTO(article: article, author: author));
        }

        return View(new ListArticlesPageModel(
            articles: articleDTOs,
            tag: tag
        ));
    }

    // ***************
    // SEARCH ARTICLES
    //
    //

    [HttpGet("/articles/search")]
    public async Task<IActionResult> SearchArticlesPage(
        [FromQuery] Guid? authorId, 
        [FromQuery] string? title, 
        [FromQuery] DateTime? createdAfter, 
        [FromQuery] DateTime? createdBefore,
        [FromQuery] string? orderBy,
        [FromQuery] int? limitBy,
        [FromQuery] List<string>? tags)
    {   
        var request = new SearchArticlesRequestDTO(
            title: title,
            authorId: authorId,
            createdAfter: createdAfter,
            createdBefore: createdBefore,
            orderBy: orderBy,
            limitBy: limitBy,
            tags: tags is null ? null : ProcessRequestTags(tags)
        );

        var listArticlesQuery = new ListArticlesQuery(
            title: request.Title,
            authorId: request.AuthorId,
            createdAfter: request.CreatedAfter,
            createdBefore: request.CreatedBefore,
            orderBy: request.OrderBy,
            limitBy: request.LimitBy,
            tags: request.Tags
        );
        var listArticlesResult = await _mediator.Send(listArticlesQuery);
        if (listArticlesResult.TryPickT1(out var errors, out var value))
        {
            throw new InternalServerErrorException($"Something went wrong trying to list articles.");
        }

        var articleDTOs = new List<ArticleDTO>();
        foreach (var article in value.Articles)
        {
            var readUserQuery = new ReadUserQuery(id: article.AuthorId);
            var readUserResult = await _mediator.Send(readUserQuery);
            var author = readUserResult.IsT0 ? readUserResult.AsT0.User : null;
            articleDTOs.Add(DtoModelService.CreateArticleDTO(article: article, author: author));
        }

        return View(new SearchArticlesPageModel(
            articles: articleDTOs,
            authorId: request.AuthorId, 
            title: request.Title, 
            createdAfter: request.CreatedAfter, 
            createdBefore: request.CreatedBefore,
            orderBy: request.OrderBy,
            limitBy: request.LimitBy,
            tags: request.Tags
        ));
    }

    // ***************
    // READ ARTICLE
    //
    //

    [HttpGet("/articles/{id}")]
    public async Task<IActionResult> ReadArticlePage(string id)
    {
        var parsedArticleId = TryReadArticleId(id);
        if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId)) {
            return Redirect($"/login?ReturnUrl=/articles/{id}");
        }

        var articleQuery = new ReadArticleQuery(id: parsedArticleId, userId: parsedUserId);
        var result = await _mediator.Send(articleQuery);
        if (result.TryPickT1(out var articleErrors, out var articleValue))
        {
            var expectedError = articleErrors.First();
            
            if (expectedError.Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(expectedError.Message);
            }

            if (expectedError.Code is ApplicationErrorCodes.DomainError)
            {
                var metadata = (ApplicationDomainErrorMetadata)expectedError.Metadata;
                if (metadata.OriginalError.Code is ArticleDomainErrorsCodes.UserNotAllowed)
                {
                    return Redirect("/users/choose-subscription");
                }
            }

            throw new InternalServerErrorException($"Something went wrong trying to read an article.");
        }

        var article = articleValue.Article;

        var userQuery = new ReadUserQuery(id: article.AuthorId);
        var userResult = await _mediator.Send(userQuery);
        if (userResult.TryPickT1(out var userErrors, out var userValue))
        {
            if (userErrors[0].Code == ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(userErrors[0].Message);
            }
            
            throw new InternalServerErrorException($"Something went wrong trying to read the article's author.");
        }

        var author = userValue.User;

        return View(new ReadArticlePageModel(
            article: DtoModelService.CreateArticleDTO(article, author: author),
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }

    // ***************
    // DELETE ARTICLE
    //
    //

    [Authorize(Roles = "Admin")]
    [HttpGet("/articles/{id}/delete")]
    public async Task<IActionResult> DeleteArticlePage(string id)
    {
        var parsedArticleId = TryReadArticleId(id);
        var parsedUserId = TryReadUserIdFromClaims();

        var articleQuery = new ReadArticleQuery(id: parsedArticleId, userId: parsedUserId);
        var result = await _mediator.Send(articleQuery);
        if (result.TryPickT1(out var articleErrors, out var articleValue))
        {
            var expectedError = articleErrors.First();
            
            if (expectedError.Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(expectedError.Message);
            }

            if (expectedError.Code is ApplicationErrorCodes.DomainError)
            {
                var metadata = (ApplicationDomainErrorMetadata)expectedError.Metadata;
                if (metadata.OriginalError.Code is ArticleDomainErrorsCodes.UserNotAllowed)
                {
                    throw new UnauthorizedException($"User is not allowed to read article.");
                }
            }

            throw new InternalServerErrorException($"Something went wrong trying to read an article.");
        }

        var article = articleValue.Article;

        var userQuery = new ReadUserQuery(id: article.AuthorId);
        var userResult = await _mediator.Send(userQuery);
        if (userResult.TryPickT1(out var userErrors, out var userValue))
        {
            if (userErrors[0].Code == ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(userErrors[0].Message);
            }
            
            throw new InternalServerErrorException($"Something went wrong trying to read the article's author.");
        }

        var author = userValue.User;

        return View(new DeleteArticlesPageModel(
            article: DtoModelService.CreateArticleDTO(article, author: author)
        ));
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost("/articles/{id}/delete")]
    public async Task<IActionResult> DeleteArticleAction(string id)
    {
        var parsedArticleId = TryReadArticleId(id);
        var parsedUserId = TryReadUserIdFromClaims();

        var articleQuery = new DeleteArticleCommand(id: parsedArticleId, userId: parsedUserId);
        var result = await _mediator.Send(articleQuery);
        if (result.TryPickT1(out var errors, out var value))
        {
            var expectedError = errors.First();
            
            if (expectedError.Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(expectedError.Message);
            }

            if (expectedError.Code is ApplicationErrorCodes.DomainError)
            {
                var metadata = (ApplicationDomainErrorMetadata)expectedError.Metadata;
                if (metadata.OriginalError.Code is ArticleDomainErrorsCodes.UserNotAllowed)
                {
                    throw new UnauthorizedException($"User is not allowed to delete article.");
                }
            }

            throw new InternalServerErrorException($"Something went wrong trying to read an article.");
        }

        return Redirect("/articles/manage");
    }

    // ***************
    // PREVIEW ARTICLE
    //
    //

    [Authorize(Roles = "Admin")]
    [HttpPost("/articles/{id}/update/preview")]
    public async Task<IActionResult> PreviewUpdateArticlePage(string id, [FromForm] PreviewArticleRequestDTO request)
    {
        var tags = ProcessRequestTags(request.Tags.ToList());

        var parsedArticleId = TryReadArticleId(id);
        var parsedUserId = TryReadUserIdFromClaims();

        var query = new ReadUserQuery(id: parsedUserId);
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            if (errors[0].Code == ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(errors[0].Message);
            }
            
            throw new InternalServerErrorException($"Something went wrong trying to read the article's author.");
        }
        
        var article = ArticleFactory.BuildNew(
            id: parsedArticleId,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: parsedUserId,
            tags: tags,
            isPremium: request.IsPremium
        );

        return View("PreviewArticlePage", new PreviewArticlePageModel(
            next: "Update",
            article: DtoModelService.CreateArticleDTO(
                article: article,
                author: value.User
            ),
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/articles/create/preview")]
    public async Task<IActionResult> PreviewCreateArticlePage([FromForm] PreviewArticleRequestDTO request)
    {
        var tags = ProcessRequestTags(request.Tags.ToList());

        var parsedUserId = TryReadUserIdFromClaims();

        var query = new ReadUserQuery(id: parsedUserId);
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            if (errors[0].Code == ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(errors[0].Message);
            }
            
            throw new InternalServerErrorException($"Something went wrong trying to read the article's author.");
        }

        var article = ArticleFactory.BuildNew(
            id: Guid.Empty,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: parsedUserId,
            tags: tags,
            isPremium: request.IsPremium
        );

        return View("PreviewArticlePage", new PreviewArticlePageModel(
            next: "Create",
            article: DtoModelService.CreateArticleDTO(
                article: article,
                author: value.User
            ),
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }
}
