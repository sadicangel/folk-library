using FluentValidation;
using FolkLibrary.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FolkLibrary.Filters;

public class HttpExceptionFilter : ExceptionFilterAttribute
{
    private static readonly IReadOnlyDictionary<Type, Action<ExceptionContext>> _handlers = new Dictionary<Type, Action<ExceptionContext>>
    {
        [typeof(ValidationException)] = OnBadRequest,
        [typeof(UnauthorizedException)] = OnUnauthorized,
        [typeof(ForbiddenException)] = OnForbidden,
        [typeof(NotFoundException)] = OnNotFound,
        [typeof(ConflictException)] = OnConflict,
        [typeof(UnknownException)] = OnUnknown
    };

    public override void OnException(ExceptionContext context)
    {
        if (_handlers.TryGetValue(context.Exception.GetType(), out var handler))
            handler.Invoke(context);
        else
            base.OnException(context);
    }

    private static void OnBadRequest(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;
        context.Result = new BadRequestObjectResult(new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = String.Join(",", exception.Errors.Select(e => $"{e.ErrorMessage}")),
        });
        context.ExceptionHandled = true;
    }

    private static void OnUnauthorized(ExceptionContext context)
    {
        context.Result = new UnauthorizedObjectResult(new ProblemDetails
        {
            //Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = context.Exception.Message,
        });
        context.ExceptionHandled = true;
    }

    private static void OnForbidden(ExceptionContext context)
    {
        context.Result = new ObjectResult(new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
            Title = "Forbidden",
            Status = StatusCodes.Status403Forbidden,
            Detail = context.Exception.Message,
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
        context.ExceptionHandled = true;
    }

    private static void OnNotFound(ExceptionContext context)
    {
        context.Result = new NotFoundObjectResult(new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
            Title = "Not Found",
            Status = StatusCodes.Status404NotFound,
            Detail = context.Exception.Message,
        });
        context.ExceptionHandled = true;
    }

    private static void OnConflict(ExceptionContext context)
    {
        context.Result = new ConflictObjectResult(new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
            Title = "Conflict",
            Status = StatusCodes.Status409Conflict,
            Detail = context.Exception.Message,
        });
        context.ExceptionHandled = true;
    }

    private static void OnUnknown(ExceptionContext context)
    {
        context.Result = new ObjectResult(new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            Detail = context.Exception.Message,
        })
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}
