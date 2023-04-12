using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FolkLibrary.Filters;

public class HttpExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new ObjectResult(new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
#if DEBUG
            Detail = context.Exception.Message,
            Extensions =
            {
                ["StackTrace"] = context.Exception.ToString()
            }
#endif
        });
        context.ExceptionHandled = true;
    }
}
