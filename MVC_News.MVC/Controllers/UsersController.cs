using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MVC_News.Domain.Entities;

namespace MVC_Classifieds.Api.Controllers;

public class UsersController : Controller
{
    private readonly ISender _mediator;
    private readonly IValidator<RegisterUserRequestDTO> _registerUserValidator;
    private readonly IValidator<LoginUserRequestDTO> _loginUserValidator;

    public UsersController(ISender mediator, IValidator<RegisterUserRequestDTO> registerUserValidator, IValidator<LoginUserRequestDTO> loginUserValidator)
    {
        _mediator = mediator;
        _registerUserValidator = registerUserValidator;
        _loginUserValidator = loginUserValidator;
    }

    private async Task SetCookieUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("DisplayName", user.DisplayName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

    [HttpGet("/login")]
    public IActionResult LoginPage()
    {
        return View(new LoginPageModel(
            email: "",
            password: "",
            errors: new Dictionary<string, List<string>>() {}
        ));
    }

    [HttpPost("/login")]
    public async Task<IActionResult> LoginPage([FromForm] LoginUserRequestDTO request)
    { 
        var validation = _loginUserValidator.Validate(request);
        if (!validation.IsValid)
        {
            return View(new LoginPageModel(
                email: request.Email,
                password: request.Password,
                errors: _errorHandlingService.FluentToApiErrors(validation.Errors)
            ));
        }

        var query = new LoginUserQuery(
            email: request.Email,
            password: request.Password
        );
        var result = await _mediator.Send(query);
        
        if (result.TryPickT1(out var errors, out var value))
        {
            return View(new LoginPageModel(
                email: request.Email,
                password: request.Password,
                errors: _errorHandlingService.TranslateServiceErrors(errors)
            ));
        }

        var user = value.User;
        await SetCookieUser(user);

        return Redirect("/");
    }

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
            return View(new RegisterPageModel(
                email: request.Email,
                password: request.Password,
                displayName: request.DisplayName,
                errors: _errorHandlingService.FluentToApiErrors(validation.Errors)
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
            return View(new RegisterPageModel(
                email: request.Email,
                password: request.Password,
                displayName: request.DisplayName,
                errors: _errorHandlingService.TranslateServiceErrors(errors)
            ));
        }

        var user = value.User;
        await SetCookieUser(user);

        return Redirect("/");
    }

    [HttpPost("/logout")]
    public async Task<IActionResult> Logout()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToAction("Frontpage", "Products");
        }
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
