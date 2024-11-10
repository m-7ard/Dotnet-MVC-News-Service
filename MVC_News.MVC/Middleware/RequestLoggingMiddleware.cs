namespace MVC_News.MVC.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    // Define color codes
    private const string Yellow = "\u001b[33m";
    private const string Green = "\u001b[32m";
    private const string White = "\u001b[37m";
    private const string Blue = "\u001b[34m";
    private const string Reset = "\u001b[0m";

    // Define the log template with color codes
    private const string LogTemplate = Yellow + "[{Method}]" + Reset + " " +
                                        Green + "{Timestamp}" + Reset + " " +
                                        White + "{URL}" + Reset + " " +
                                        Blue + "{Status}" + Reset;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var url = context.Request.Path;
        var timestamp = DateTime.UtcNow.ToString("HH:mm:ss");

        await _next(context);

        var status = context.Response.StatusCode;

        // Log with color
        _logger.LogInformation(LogTemplate, method, timestamp, url, status);
    }
}