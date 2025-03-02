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
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.ValueObjects.Article;
using MVC_News.Domain.ValueObjects.User;
using MVC_News.MVC.DTOs.Contracts.Articles.Create;
using MVC_News.MVC.DTOs.Contracts.Articles.List;
using MVC_News.MVC.DTOs.Contracts.Articles.Manage;
using MVC_News.MVC.DTOs.Contracts.Articles.Preview;
using MVC_News.MVC.DTOs.Contracts.Articles.Search;
using MVC_News.MVC.DTOs.Contracts.Articles.Update;
using MVC_News.MVC.DTOs.Models;
using MVC_News.MVC.Errors;
using MVC_News.MVC.Exceptions;
using MVC_News.MVC.Interfaces;
using MVC_News.MVC.Models.Articles;
using MVC_News.MVC.Services;
using Sprache;

namespace MVC_News.MVC.Controllers;

public class ArticlesController : BaseController
{
    private readonly ISender _mediator;
    private readonly IValidator<CreateArticleRequestDTO> _createArticleValidator;
    private readonly IValidator<UpdateArticleRequestDTO> _updateArticleValidator;
    private readonly IDtoModelService _dtoModelService;

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

    public ArticlesController(ISender mediator, IValidator<CreateArticleRequestDTO> createArticleValidator, IValidator<UpdateArticleRequestDTO> updateArticleValidator, IDtoModelService dtoModelService)
    {
        _mediator = mediator;
        _createArticleValidator = createArticleValidator;
        _updateArticleValidator = updateArticleValidator;
        _dtoModelService = dtoModelService;
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

        var articleDTOs = await _dtoModelService.CreateManyArticleDTO(value.Articles);
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
                errors: PlainMvcErrorFactory.MapApplicationErrors(errors),
                isPremium: request.IsPremium
            ));
        }

        Response.StatusCode = (int)HttpStatusCode.Created;
        return Redirect($"/articles/{value.Id}");
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
            var firstError = errors.First();

            if (firstError.Code is ApplicationValidatorErrorCodes.ARTICLE_EXISTS_ERROR)
            {
                throw new NotFoundException(firstError.Message);
            }
        }

        Response.StatusCode = (int)HttpStatusCode.OK;
        return View(new UpdateArticlePageModel(
            id: value.Article.Id.Value,
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
                errors: PlainMvcErrorFactory.MapApplicationErrors(errors),
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

        var articleDTOs = await _dtoModelService.CreateManyArticleDTO(value.Articles);

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

        var articleDTOs = await _dtoModelService.CreateManyArticleDTO(value.Articles);

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

        var articleDTOs = await _dtoModelService.CreateManyArticleDTO(value.Articles);

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

        var query = new ReadArticleQuery(id: parsedArticleId, userId: parsedUserId);
        var result = await _mediator.Send(query);
        if (result.TryPickT1(out var errors, out var value))
        {
            var expectedError = errors.First();
            
            if (expectedError.Code is ApplicationValidatorErrorCodes.ARTICLE_EXISTS_ERROR)
            {
                throw new NotFoundException(expectedError.Message);
            }

            if (expectedError.Code is ApplicationErrorCodes.NotAllowed)
            {
                return Redirect("/users/choose-subscription");
            }

            throw new InternalServerErrorException(expectedError.Message);
        }

        var articleDTO = await _dtoModelService.CreateArticleDTO(value.Article);

        return View(new ReadArticlePageModel(
            article: articleDTO,
            markup: MarkupParser.ParseToHtml(articleDTO.Content)
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

        var query = new ReadArticleQuery(id: parsedArticleId, userId: parsedUserId);
        var result = await _mediator.Send(query);
        if (result.TryPickT1(out var errors, out var value))
        {
            var expectedError = errors.First();
            
            if (expectedError.Code is ApplicationValidatorErrorCodes.ARTICLE_EXISTS_ERROR)
            {
                throw new NotFoundException(expectedError.Message);
            }

            if (expectedError.Code is ApplicationErrorCodes.NotAllowed)
            {
                throw new UnauthorizedException(expectedError.Message);
            }

            throw new InternalServerErrorException(expectedError.Message);
        }

        var articleDTO = await _dtoModelService.CreateArticleDTO(value.Article);

        return View(new DeleteArticlesPageModel(
            article: articleDTO
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
            
            if (expectedError.Code is ApplicationValidatorErrorCodes.ARTICLE_EXISTS_ERROR)
            {
                throw new NotFoundException(expectedError.Message);
            }

            if (expectedError.Code is ApplicationErrorCodes.NotAllowed)
            {
                throw new UnauthorizedException(expectedError.Message);
            }

            throw new InternalServerErrorException(expectedError.Message);
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
        
        var article = ArticleFactory.BuildNew(
            id: ArticleId.ExecuteCreate(parsedArticleId),
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            dateCreated: DateTime.UtcNow,
            authorId: UserId.ExecuteCreate(parsedUserId),
            tags: tags,
            isPremium: request.IsPremium
        );

        var articleDto = await _dtoModelService.CreateArticleDTO(article);

        return View("PreviewArticlePage", new PreviewArticlePageModel(
            next: "Update",
            article: articleDto,
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("/articles/create/preview")]
    public async Task<IActionResult> PreviewCreateArticlePage([FromForm] PreviewArticleRequestDTO request)
    {
        var tags = ProcessRequestTags(request.Tags.ToList());

        var parsedUserId = TryReadUserIdFromClaims();

        var article = ArticleFactory.BuildNew(
            id: ArticleId.ExecuteCreate(Guid.Empty),
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            dateCreated: DateTime.UtcNow,
            authorId: UserId.ExecuteCreate(parsedUserId),
            tags: tags,
            isPremium: request.IsPremium
        );

        var articleDto = await _dtoModelService.CreateArticleDTO(article);

        return View("PreviewArticlePage", new PreviewArticlePageModel(
            next: "Create",
            article: articleDto,
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }
}
