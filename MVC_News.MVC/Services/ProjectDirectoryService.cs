using System.Reflection;

namespace MVC_News.MVC.Services;

public class DirectoryService
{
    private static readonly List<string> _projects = ["MVC_News.Application", "MVC_News.MVC", "MVC_News.Domain", "MVC_News.Files", "MVC_News.Infrastructure", "MVC_News.Tests"];
    public static string GetProjectRoot()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var dir = new DirectoryInfo(assembly.Location);
        while (!_projects.Contains(dir!.Name))
        {
            dir = Directory.GetParent(dir.FullName);
        }

        dir = Directory.GetParent(dir.FullName);
        return dir!.FullName;
    }

    public static string GetMediaDirectory()
    {
        var projectRoot = GetProjectRoot();
        var appRoot = Path.Join(projectRoot, "MVC_News.Files");
        if (Environment.GetEnvironmentVariable("IS_TEST") == "true")
        {
            return Path.Join(appRoot, "Media/Tests");
        }

        Console.WriteLine(Path.Join(appRoot, "Media"));

        return Path.Join(appRoot, "Media");
    }
}