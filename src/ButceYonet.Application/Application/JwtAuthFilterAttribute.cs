using ButceYonet.Application.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ButceYonet.Application.Application;

public class JwtAuthFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (user?.Identity is null || !user.Identity.IsAuthenticated)
            throw new JwtAuthorizationFailException();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}