using System.Net;
using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_Classifieds.Api.Models.Users;
using MVC_News.Application.Errors;
using MVC_News.Application.Handlers.Articles.List;
using MVC_News.Application.Handlers.Subscriptions;
using MVC_News.Application.Handlers.Users.ChangePassword;
using MVC_News.Application.Handlers.Users.Login;
using MVC_News.Application.Handlers.Users.Read;
using MVC_News.Application.Handlers.Users.Register;
using MVC_News.Domain.Entities;
using MVC_News.MVC.DTOs.Contracts.Users.ChangePassword;
using MVC_News.MVC.DTOs.Contracts.Users.CheckoutSubscription;
using MVC_News.MVC.DTOs.Contracts.Users.Login;
using MVC_News.MVC.DTOs.Contracts.Users.Register;
using MVC_News.MVC.Errors;
using MVC_News.MVC.Exceptions;
using MVC_News.MVC.Interfaces;
using MVC_News.MVC.Models;
using MVC_News.MVC.Models.Users;
using MVC_News.MVC.Services;

namespace MVC_News.MVC.Controllers;

public class UsersController : BaseController
{
    private readonly ISender _mediator;
    private readonly IValidator<RegisterUserRequestDTO> _registerUserValidator;
    private readonly IValidator<LoginUserRequestDTO> _loginUserValidator;
    private readonly IValidator<ChangePasswordRequestDTO> _changePasswordValidator;
    private readonly IDtoModelService _dtoModelService;

    public UsersController(ISender mediator, IValidator<RegisterUserRequestDTO> registerUserValidator, IValidator<LoginUserRequestDTO> loginUserValidator, IValidator<ChangePasswordRequestDTO> changePasswordValidator, IDtoModelService dtoModelService)
    {
        _mediator = mediator;
        _registerUserValidator = registerUserValidator;
        _loginUserValidator = loginUserValidator;
        _changePasswordValidator = changePasswordValidator;
        _dtoModelService = dtoModelService;
    }

    private async Task<ReadUserResult> MakeReadUserQuery(Guid userId)
    {
        var query = new ReadUserQuery(userId);
        var queryResult = await _mediator.Send(query);

        if (queryResult.TryPickT1(out var queryErrors, out var queryValue))
        {
            var firstError = queryErrors.First();
            if (firstError.Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(firstError.Message);
            }

            throw new InternalServerErrorException(firstError.Message);
        }

        return queryValue;
    }

    private async Task SetCookieUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("DisplayName", user.DisplayName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Reader")

        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }

    // ***************
    //  LOGIN USER
    //
    //

    [HttpGet("/login")]
    public IActionResult LoginPage(string? returnUrl)
    {
        return View(new LoginPageModel(
            email: "",
            password: "",
            errors: new Dictionary<string, List<string>>(),
            returnUrl: returnUrl
        ));
    }

    [HttpPost("/login")]
    public async Task<IActionResult> LoginPage([FromForm] LoginUserRequestDTO request, string? returnUrl)
    {
        var validation = _loginUserValidator.Validate(request);
        if (!validation.IsValid)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View(new LoginPageModel(
                email: request.Email,
                password: request.Password,
                errors: PlainMvcErrorFactory.FluentToApiErrors(validation.Errors),
                returnUrl: returnUrl
            ));
        }

        var query = new LoginUserQuery(
            email: request.Email,
            password: request.Password
        );
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View(new LoginPageModel(
                email: request.Email,
                password: request.Password,
                errors: PlainMvcErrorFactory.TranslateServiceErrors(errors),
                returnUrl: returnUrl
            ));
        }

        var user = value.User;
        await SetCookieUser(user);

        Console.WriteLine("returnUrlreturnUrlreturnUrlreturnUrlreturnUrl");
        Console.WriteLine("returnUrlreturnUrlreturnUrlreturnUrlreturnUrl");
        Console.WriteLine("returnUrlreturnUrlreturnUrlreturnUrlreturnUrl");
        Console.WriteLine("returnUrlreturnUrlreturnUrlreturnUrlreturnUrl");
        Console.WriteLine(returnUrl);
        Console.WriteLine(returnUrl);
        Console.WriteLine(returnUrl);
        Console.WriteLine(returnUrl);

        return Redirect(returnUrl ?? "/");
    }

    // ***************
    //  REGISTER USER
    //
    //

    [HttpGet("/register")]
    public IActionResult RegisterPage()
    {
        return View(new RegisterPageModel(
            email: "",
            password: "",
            displayName: "",
            errors: new Dictionary<string, List<string>>() {}
        ));
    }

    [HttpPost("/register")]
    public async Task<IActionResult> RegisterPage([FromForm] RegisterUserRequestDTO request)
    {
        var validation = _registerUserValidator.Validate(request);
        if (!validation.IsValid)
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View(new RegisterPageModel(
                email: request.Email,
                password: request.Password,
                displayName: request.DisplayName,
                errors: PlainMvcErrorFactory.FluentToApiErrors(validation.Errors)
            ));
        }

        var command = new RegisterUserCommand(
            email: request.Email,
            password: request.Password,
            displayName: request.DisplayName
        );
        var result = await _mediator.Send(command);

        if (result.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View(new RegisterPageModel(
                email: request.Email,
                password: request.Password,
                displayName: request.DisplayName,
                errors: PlainMvcErrorFactory.TranslateServiceErrors(errors)
            ));
        }

        var user = value.User;
        await SetCookieUser(user);

        Response.StatusCode = (int)HttpStatusCode.Created;
        return Redirect("/");
    }

    // ***************
    //  LOGOUT USER
    //
    //

    [HttpGet("/logout")]
    public async Task<IActionResult> Logout()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/");
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }

    // ***************
    //  CHOOSE SUBSCRIPTION
    //
    //

    [HttpGet("/users/choose-subscription")]
    public IActionResult ChooseSubscriptionPage()
    {
        return View(new BaseViewModel());
    }

    // ***************
    //  CHECKOUT SUBSCRIPTION
    //
    //

    [Authorize]
    [HttpGet("/users/checkout-subscription")]
    public IActionResult CheckoutSubscriptionPage([FromQuery] int? duration)
    {
        if (duration is null || duration.Value < 0 || duration.Value > 3)
        {
            throw new InternalServerErrorException("Subscription duration is invalid.");
        }

        return View(new CheckoutSubscriptionPageModel(duration: duration.Value, errors: new Dictionary<string, List<string>>()));
    }

    [HttpPost("/users/checkout-subscription")]
    public async Task<IActionResult> CheckoutSubscriptionPage([FromForm] CheckoutSubscriptionRequestDTO request)
    {
        if (request.Duration < 1 || request.Duration > 3)
        {
            throw new InternalServerErrorException("Subscription duration is invalid.");
        }

        var parsedUserId = TryReadUserIdFromClaims();
        var command = new CreateSubscriptionCommand(userId: parsedUserId, subscriptionDuration: request.Duration);
        var result = await _mediator.Send(command);

        if (result.TryPickT1(out var errors, out var value))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View(new CheckoutSubscriptionPageModel(
                duration: request.Duration,
                errors: PlainMvcErrorFactory.TranslateServiceErrors(errors)
            ));
        }

        return Redirect("/");
    }

    // ***************
    //  USER ACCOUNT 
    //
    //

    [Authorize]
    [HttpGet("/users/account/")]
    public async Task<IActionResult> UserAccountPage()
    {
        var parsedUserId = TryReadUserIdFromClaims();
        var query = new ReadUserQuery(parsedUserId);
        var result = await _mediator.Send(query);

        if (result.TryPickT1(out var errors, out var value))
        {
            var firstError = errors.First();
            if (firstError.Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(firstError.Message);
            }

            throw new InternalServerErrorException(firstError.Message);
        }

        return View(new UserAccountPageModel(
            errors: new Dictionary<string, List<string>>(), 
            user: value.User, 
            subscription: value.User.GetActiveSubscription()
        ));
    }

    // ***************
    //  CHANGE PASSWORD (USER ACCOUNT)
    //
    //

    [Authorize]
    [HttpPost("/users/account/change-password")]
    public async Task<IActionResult> UserAccountPage([FromForm] ChangePasswordRequestDTO request)
    {
        var parsedUserId = TryReadUserIdFromClaims();
        var queryResult = await MakeReadUserQuery(parsedUserId);
        
        var validation = _changePasswordValidator.Validate(request);
        if (!validation.IsValid)
        {
            return View(new UserAccountPageModel(
                errors: PlainMvcErrorFactory.FluentToApiErrors(validation.Errors),
                user: queryResult.User, 
                subscription: queryResult.User.GetActiveSubscription()
            ));
        }
        
        var command = new ChangePasswordCommand(
            id: parsedUserId,
            currentPassword: request.CurrentPassword,
            newPassword: request.NewPassword
        );
        var result = await _mediator.Send(command);

        if (result.TryPickT1(out var errors, out var value))
        {
            var firstError = errors.First();
            if (firstError.Code is ApplicationErrorCodes.ModelDoesNotExist)
            {
                throw new NotFoundException(firstError.Message);
            }

            return View(new UserAccountPageModel(
                errors: PlainMvcErrorFactory.TranslateServiceErrors(errors),
                user: queryResult.User,
                subscription: queryResult.User.GetActiveSubscription()
            ));
        }

        var pageModel = new UserAccountPageModel(
            errors: new Dictionary<string, List<string>>(), 
            user: value.User, 
            subscription: value.User.GetActiveSubscription()
        );
        pageModel.SystemMessage = "Successfully Changed Password.";

        return View(pageModel);
    }

    // ***************
    //  USER PROFILE
    //
    //

    [Authorize]
    [HttpGet("/users/{id}")]
    public async Task<IActionResult> UserProfilePage(string id)
    {
        if (!Guid.TryParse(id, out var parsedUserId)) {
            throw new InternalServerErrorException($"Invalid user id.");
        }

        var userQueryValue = await MakeReadUserQuery(parsedUserId);
        
        var articlesQuery = new ListArticlesQuery(
            authorId: userQueryValue.User.Id,
            createdAfter: null,
            createdBefore: null,
            orderBy: null,
            limitBy: null,
            tags: null,
            title: null
        );

        var articlesResult = await _mediator.Send(articlesQuery);
        if (articlesResult.TryPickT1(out var articlesErrors, out var articlesValue))
        {
            var firstError = articlesErrors.First();
            throw new InternalServerErrorException(firstError.Message);
        }

        var articleDTOs = await _dtoModelService.CreateManyArticleDTO(articlesValue.Articles);

        var pageModel = new UserProfilePageModel(
            user: userQueryValue.User,
            articles: articleDTOs
        );

        return View(pageModel);
    }    
}
