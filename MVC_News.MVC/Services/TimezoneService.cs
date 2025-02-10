namespace Api.Services;

public static class TimeZoneService
{
    private readonly static TimeZoneInfo _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");

    public static DateTime ConvertUtcToLocalTime(DateTime utcDate)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, _localTimeZone);
    }

    public static DateTime ConvertLocalTimeToUtc(DateTime madridDate)
    {
        return TimeZoneInfo.ConvertTimeToUtc(madridDate, _localTimeZone);
    }
}
