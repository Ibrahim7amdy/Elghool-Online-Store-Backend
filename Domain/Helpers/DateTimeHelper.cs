namespace Domain.Helpers;

public static class DateTimeHelper
{
    public static readonly TimeZoneInfo EgyptTimeZone =
         TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo");

    public static DateTime EgyptNow =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTimeZone);

    public static DateTime ToEgyptTime(this DateTime utcDateTime) =>
        TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, EgyptTimeZone);

    public static DateTime? ToEgyptTime(this DateTime? utcDateTime) =>
        utcDateTime.HasValue ? utcDateTime.Value.ToEgyptTime() : null;

    public static DateTime ToUtcFromEgyptTime(this DateTime egyptDateTime) =>
        TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(egyptDateTime, DateTimeKind.Unspecified),
            EgyptTimeZone);
}