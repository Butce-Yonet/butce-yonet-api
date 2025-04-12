using System.Net;
using System.Text.Json;
using ButceYonet.Application.Domain.Exceptions;
using DotBoil.Entities;
using DotBoil.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ButceYonet.Application.Application;

public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ApiExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            await WriteResponse(context,
                BaseResponse.Response(new { }, HttpStatusCode.BadRequest, validationException.Messages.ToArray()));
        }
        catch (UserPlanValidationException validationException)
        {
            //TODO:
            await WriteResponse(context,
                BaseResponse.Response(new { }, HttpStatusCode.BadRequest, validationException.Message));
        }
        catch (NotFoundException notFoundException)
        {
            //TODO:
            await WriteResponse(context,
                BaseResponse.Response(new { }, HttpStatusCode.NotFound, notFoundException.Message));
        }
        catch (AlreadyExistsException alreadyExistsException)
        {
            //TODO:
            await WriteResponse(context,
                BaseResponse.Response(new { }, HttpStatusCode.NotFound, alreadyExistsException.Message));
        }
        catch (BusinessRuleException businessRuleException)
        {
            //TODO:
            await WriteResponse(context,
                BaseResponse.Response(new { }, HttpStatusCode.NotFound, businessRuleException.Message));
        }
        catch (JwtAuthorizationFailException jwtAuthorizationFailException)
        {
            //TODO:
            await WriteResponse(context,
                BaseResponse.Response(new { }, HttpStatusCode.Unauthorized, jwtAuthorizationFailException.Message));
        }
        catch (Exception ex)
        {
            await WriteResponse(context,
                BaseResponse.Response(new {}, HttpStatusCode.InternalServerError, ex.Message));
        }   
    }
    
    private async Task WriteResponse(HttpContext context, BaseResponse baseResponse)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = (int)baseResponse.StatusCode;

        var json = JsonSerializer.Serialize(baseResponse);

        await response.WriteAsync(json);
    }
}