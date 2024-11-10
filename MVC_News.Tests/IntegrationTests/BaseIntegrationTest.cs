namespace MVC_News.Tests.IntegrationTests;

public class BaseIntegrationTest : IAsyncLifetime
{
    protected readonly IntegrationWebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;

    public BaseIntegrationTest()
    {
        _factory = new IntegrationWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        
        // Delete saved test files before every run
        // Environment.SetEnvironmentVariable("IS_TEST", "true");
        // foreach (string file in Directory.GetFiles(DirectoryService.GetMediaDirectory()))
        // {
        //     File.Delete(file);
        // }
    }

    public virtual async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        if (_client != null)
        {
            _client.Dispose();
        }
        if (_factory != null)
        {
            await _factory.DisposeAsync();
        }

        // Delete saved test files after all tests have finished
        // Environment.SetEnvironmentVariable("IS_TEST", "true");
        // foreach (string file in Directory.GetFiles(DirectoryService.GetMediaDirectory()))
        // {
        //     File.Delete(file);
        // }
    }
}