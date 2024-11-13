using System.Net;
using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.Create;
using MVC_News.Application.Handlers.Articles.List;
using MVC_News.Application.Handlers.Articles.Read;
using MVC_News.Application.Handlers.Articles.Update;
using MVC_News.Application.Handlers.Authors.Read;
using MVC_News.Domain.DomainFactories;
using MVC_News.Domain.Entities;
using MVC_News.MVC.DTOs.Contracts.Articles.Create;
using MVC_News.MVC.DTOs.Contracts.Articles.List;
using MVC_News.MVC.DTOs.Contracts.Articles.Preview;
using MVC_News.MVC.DTOs.Contracts.Articles.Update;
using MVC_News.MVC.DTOs.Models;
using MVC_News.MVC.Errors;
using MVC_News.MVC.Models.Articles;
using MVC_News.MVC.Services;

namespace MVC_News.MVC.Controllers;

public class ArticlesController : Controller
{
    private readonly ISender _mediator;
    private readonly IValidator<CreateArticleRequestDTO> _createArticleValidator;
    private readonly IValidator<UpdateArticleRequestDTO> _updateArticleValidator;

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
            authorId: null, 
            createdAfter: null, 
            createdBefore: null, 
            orderBy: "newest", 
            limitBy: 24
        );
        var result = await _mediator.Send(query);
        if (result.TryPickT1(out var errors, out var value))
        {
            return View("~/Views/500BadRequest.cshtml", $"Something went wrong trying to list the frontpage articles.");
        }

        var articleDTOs = new List<ArticleDTO>();
        foreach (var article in value.Articles)
        {
            var readAuthorQuery = new ReadAuthorQuery(id: article.AuthorId);
            var readAuthorResult = await _mediator.Send(readAuthorQuery);
            var author = readAuthorResult.IsT0 ? readAuthorResult.AsT0.Author : new Author(
                id: Guid.Empty,
                displayName: "Unkown Author"
            );

            articleDTOs.Add(DtoModelService.CreateArticleDTO(article: article, author: author));
        }

        List<ArticleDTO?> mainArticles = [];

        var i = 0;
        while (i < 3) {
            if (articleDTOs.Count > i) 
            {
                mainArticles.Add(articleDTOs[i]);
            }
            else
            {
                mainArticles.Add(null);
            }

            i++;
        } 

        return View(new FrontpagePageModel(
            mainArticles: mainArticles,
            newestArticles: articleDTOs.Skip(3).ToList()
        ));
    }

    // ***************
    // CREATE ARTICLE
    //
    //

    [HttpGet("/articles/create")]
    public IActionResult CreateArticlePage(
        [FromQuery] string? headerImage, 
        [FromQuery] string? title, 
        [FromQuery] string? content)
    {
        return View(new CreateArticlePageModel(
            title: title ?? "",
            content: content ?? "",
            headerImage: headerImage ?? "",
            errors: new Dictionary<string, List<string>>()
        ));
    }

    [HttpPost("/articles/create")]
    public async Task<IActionResult> CreateArticlePage([FromForm] CreateArticleRequestDTO request)
    {
        if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)) {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View("~/Views/401Unauthorized.cshtml", $"User ID is missing from claims.");
        }

        var validation = _createArticleValidator.Validate(request);
        if (!validation.IsValid)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            
            return View(new CreateArticlePageModel(
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                errors: PlainMvcErrorHandlingService.FluentToApiErrors(validation.Errors)
            ));
        }

        var command = new CreateArticleCommand(
            id: Guid.NewGuid(),
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: userId
        );
        var result = await _mediator.Send(command);

        if (result.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View(new CreateArticlePageModel(
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                errors: PlainMvcErrorHandlingService.TranslateServiceErrors(errors)
            ));
        }

        Response.StatusCode = (int)HttpStatusCode.Created;
        return Redirect($"/articles/{value.Article.Id}");
    }

    // ***************
    // UPDATE ARTICLE
    //
    //

    [HttpGet("/articles/{id}/update")]
    public async Task<IActionResult> UpdateArticlePage(
        string id,
        [FromQuery] string? headerImage, 
        [FromQuery] string? title, 
        [FromQuery] string? content)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("~/Views/404NotFound.cshtml");
        }

        var query = new ReadArticleQuery(id: guid);
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            if (errors.First().Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return View("~/Views/404NotFound.cshtml");
            }
        }

        Response.StatusCode = (int)HttpStatusCode.OK;
        return View(new UpdateArticlePageModel(
            id: value.Article.Id,
            title: title ?? value.Article.Title,
            content: content ?? value.Article.Content,
            headerImage: headerImage ?? value.Article.HeaderImage,
            errors: new Dictionary<string, List<string>>()
        ));
    }

    [HttpPost("/articles/{id}/update")]
    public async Task<IActionResult> UpdateArticlePage([FromForm] UpdateArticleRequestDTO request, string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("~/Views/404NotFound.cshtml");
        }

        if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId)) {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View("~/Views/401Unauthorized.cshtml", $"User ID is missing from claims.");
        }

        var validation = _updateArticleValidator.Validate(request);
        if (!validation.IsValid)
        {
            return View(new UpdateArticlePageModel(
                id: guid,
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                errors: PlainMvcErrorHandlingService.FluentToApiErrors(validation.Errors)
            ));
        }

        var command = new UpdateArticleCommand(
            id: guid,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: userId
        );
        var result = await _mediator.Send(command);

        if (result.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return View(new UpdateArticlePageModel(
                id: guid,
                title: request.Title,
                content: request.Content,
                headerImage: request.HeaderImage,
                errors: PlainMvcErrorHandlingService.TranslateServiceErrors(errors)
            ));
        }

        Response.StatusCode = (int)HttpStatusCode.OK;
        return Redirect($"/articles/{value.Article.Id}");
    }

    // ***************
    // LIST ALL ARTICLES / FILTER ALL ARTICLES
    //
    //

    [HttpGet("/articles")]
    public async Task<IActionResult> ListArticlesPage(
        [FromQuery] Guid? authorId, 
        [FromQuery] DateTime? createdAfter, 
        [FromQuery] DateTime? createdBefore,
        [FromQuery] string? orderBy,
        [FromQuery] int? limitBy)
    {
        var request = new ListArticlesRequestDTO(
            authorId: authorId,
            createdAfter: createdAfter,
            createdBefore: createdBefore,
            orderBy: orderBy,
            limitBy: limitBy
        );

        var listArticlesQuery = new ListArticlesQuery(
            authorId: request.AuthorId,
            createdAfter: request.CreatedAfter,
            createdBefore: request.CreatedBefore,
            orderBy: orderBy,
            limitBy: limitBy
        );
        var listArticlesResult = await _mediator.Send(listArticlesQuery);
        if (listArticlesResult.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return View("~/Views/500BadRequest.cshtml", $"Something went wrong trying to list articles.");
        }

        var articleDTOs = new List<ArticleDTO>();
        foreach (var article in value.Articles)
        {
            var readAuthorQuery = new ReadAuthorQuery(id: article.AuthorId);
            var readAuthorResult = await _mediator.Send(readAuthorQuery);
            var author = readAuthorResult.IsT0 ? readAuthorResult.AsT0.Author : new Author(
                id: Guid.Empty,
                displayName: "Unkown Author"
            );

            articleDTOs.Add(DtoModelService.CreateArticleDTO(article: article, author: author));
        }

        return View(new ListArticlesPageModel(
            articles: articleDTOs
        ));
    }

    // ***************
    // READ ARTICLE
    //
    //

    [HttpGet("/articles/{id}")]
    public async Task<IActionResult> ReadArticlePage(string id)
    {
        if (!Guid.TryParse(id, out var guid))
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("~/Views/404NotFound.cshtml");
        }

        var query = new ReadArticleQuery(id: guid);
        var result = await _mediator.Send(query);
        if (result.TryPickT1(out var errors, out var value))
        {
            if (errors.First().Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return View("~/Views/404NotFound.cshtml");
            }

            return View("~/Views/500BadRequest.cshtml", $"Something went wrong trying to read an article.");
        }

        var article = value.Article;
        return View(new ReadArticlePageModel(
            article: article,
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }

    // ***************
    // PREVIEW ARTICLE
    //
    //

    [HttpPost("/articles/{id}/update/preview")]
    public async Task<IActionResult> PreviewUpdateArticlePage(string id, [FromForm] PreviewArticleRequestDTO request)
    {
        if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId)) {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View("~/Views/401Unauthorized.cshtml", $"User ID is missing from claims.");
        }

        if (!Guid.TryParse(id, out var parsedArticleId)) 
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View("~/Views/404NotFound.cshtml");
        }

        var query = new ReadAuthorQuery(id: parsedUserId);
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            if (errors[0].Code == ApplicationErrorCodes.ModelDoesNotExist)
            {
                return View("~/Views/404NotFound.cshtml");
            }
            
            return View("~/Views/500BadRequest.cshtml", $"Something went wrong trying to read the article's author.");
        }
        
        var article = ArticleFactory.BuildNew(
            id: parsedArticleId,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: parsedUserId
        );

        return View("PreviewArticlePage", new PreviewArticlePageModel(
            next: "Update",
            article: DtoModelService.CreateArticleDTO(
                article: article,
                author: value.Author
            ),
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }

    [HttpPost("/articles/create/preview")]
    public async Task<IActionResult> PreviewArticlePage([FromForm] PreviewArticleRequestDTO request)
    {
        if (!Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var parsedUserId)) {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return View("~/Views/401Unauthorized.cshtml", $"User ID is missing from claims.");
        }

        var query = new ReadAuthorQuery(id: parsedUserId);
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            if (errors[0].Code == ApplicationErrorCodes.ModelDoesNotExist)
            {
                return View("~/Views/404NotFound.cshtml");
            }
            
            return View("~/Views/500BadRequest.cshtml", $"Something went wrong trying to read the article's author.");
        }

        var article = ArticleFactory.BuildNew(
            id: Guid.Empty,
            title: request.Title,
            content: request.Content,
            headerImage: request.HeaderImage,
            authorId: parsedUserId
        );

        return View("PreviewArticlePage", new PreviewArticlePageModel(
            next: "Create",
            article: DtoModelService.CreateArticleDTO(
                article: article,
                author: value.Author
            ),
            markup: MarkupParser.ParseToHtml(article.Content)
        ));
    }
}
