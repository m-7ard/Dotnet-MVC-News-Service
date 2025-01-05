using System.Globalization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MVC_News.Application.Handlers.Users.Register;
using MVC_News.Application.Interfaces.Repositories;
using MVC_News.Application.Interfaces.Services;
using MVC_News.Domain.DomainFactories;
using MVC_News.Infrastructure;
using MVC_News.Infrastructure.Repositories;
using MVC_News.Infrastructure.Services;
using MVC_News.MVC.Filters;
using MVC_News.MVC.Interfaces;
using MVC_News.MVC.Middleware;
using MVC_News.MVC.Services;
using MVC_News.MVC.Validators;

var builder = WebApplication.CreateBuilder(args);

///
///
/// Authentication / Authorization
/// 

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options  =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/unauthorised";
        options.LoginPath = "/login";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});
builder.Services.AddDirectoryBrowser(); // For media

///
///
/// Controllers
/// 

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ControllerExceptionFilter>();
    options.Filters.Add<GlobalDataInjectorFilter>();
});

///
///
/// Env
/// 

DotNetEnv.Env.Load();
var DefaultConnection = Environment.GetEnvironmentVariable("DefaultConnection");

///
///
/// DB / database / dbcontext
/// 

builder.Services.AddDbContext<NewsDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"), 
    b => b.MigrationsAssembly("MVC_News.MVC")
));

var services = builder.Services;

///
///
/// Localisation
/// 

{
    services.AddLocalization(options => options.ResourcesPath = "Resources");

    services.Configure<RequestLocalizationOptions>(options =>
    {
        var supportedCultures = new[] { new CultureInfo("en-US") };
        options.DefaultRequestCulture = new RequestCulture("en-US");
        options.SupportedCultures = supportedCultures;
        options.SupportedUICultures = supportedCultures;
    });

    services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        })
        .AddViewLocalization()
        .AddDataAnnotationsLocalization();
}

///
///
/// Mediatr
/// 

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserHandler).Assembly));

///
///
/// Dependency Injection / DI / Services
/// 

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IDtoModelService, DtoModelService>();

///
///
/// Fluent Validation DI / Dependency Injection
/// 

builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserValidator).Assembly);

///
///
/// Cookie Authentication
/// 

builder.Services.AddHttpContextAccessor();


var app = builder.Build();

app.UseRequestLocalization();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

///
///
/// Startup behaviour
/// 

using (var scope = app.Services.CreateScope())
{
    var localService = scope.ServiceProvider;
    var context = localService.GetRequiredService<NewsDbContext>();

    try
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = localService.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while resetting the database.");
    }

    var repo = localService.GetRequiredService<IUserRepository>();
    var hasher = localService.GetRequiredService<IPasswordHasher>();
    await repo.CreateAsync(
        UserFactory.BuildNew("admin@mail.com", hasher.Hash("adminword"), "admin", true, [])
    );
}
///
///
/// Media config
/// 

var mediaProvider = new PhysicalFileProvider(DirectoryService.GetMediaDirectory());

app.UseFileServer(new FileServerOptions
{
    FileProvider = mediaProvider,
    RequestPath = "/media",
    EnableDirectoryBrowsing = true
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseMiddleware<RequestLoggingMiddleware>();

app.Run();

public partial class Program {  }
