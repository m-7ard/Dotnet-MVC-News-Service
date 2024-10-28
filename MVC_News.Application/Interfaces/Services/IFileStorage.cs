using Microsoft.AspNetCore.Http;

namespace MVC_News.Application.Interfaces.Services;

public interface IFileStorage
{
    public Task SaveFile(IFormFile file, string path, CancellationToken cancellationToken);
}