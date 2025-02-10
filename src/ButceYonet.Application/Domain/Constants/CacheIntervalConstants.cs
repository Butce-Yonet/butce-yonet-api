namespace ButceYonet.Application.Domain.Constants;

public class CacheIntervalConstants
{
    public static TimeSpan DefaultUserPlan => TimeSpan.FromHours(1);
    public static TimeSpan DefaultNotebookLabels => TimeSpan.FromHours(1);
    public static TimeSpan Plans => TimeSpan.FromDays(1);
    public static TimeSpan Currencies => TimeSpan.FromDays(1);
    public static TimeSpan CurrentUserPlan => TimeSpan.FromHours(1);
}