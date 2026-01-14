namespace ButceYonet.Application.Domain.Constants;

public class CacheKeyConstants
{
    public static string DefaultUserPlan => "DotBoil:ButceYonet:Plans:Default";
    public static string DefaultNotebookLabels => "DotBoil:ButceYonet:NotebookLabels:Default";
    public static string Plans => "DotBoil:ButceYonet:Plans";
    public static string Currencies => "DotBoil:ButceYonet:Currencies";
    public static string CurrentUserPlan => "DotBoil:ButceYonet:User:{UserId}:Plan";
    public static string NotebookLabels => "DotBoil:ButceYonet:Notebook:{NotebookId}:Labels";
    public static string NotebookUsers => "DotBoil:ButceYonet:Notebook:{NotebookId}:Users";
}