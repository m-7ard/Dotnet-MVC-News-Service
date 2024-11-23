namespace MVC_News.MVC.Helpers;

public static class UtilityHelpers
{
    public static List<string>? GetErrorsOrNull(Dictionary<string, List<string>> dictionary, string key)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return null;
    }
}
