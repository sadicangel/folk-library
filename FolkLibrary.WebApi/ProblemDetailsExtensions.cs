using FolkLibrary.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace FolkLibrary;

public static class ProblemDetailsExtensions
{
    public static IActionResult ToActionResult<TSuccess>(this Response<TSuccess> response, Func<TSuccess, IActionResult> success, object? value, [CallerArgumentExpression(nameof(value))] string expression = "")
    {
        return response.Match(
            success,
            notFound => notFound.ToActionResult(value, expression),
            alreadyExists => alreadyExists.ToActionResult(value, expression),
            invalid => invalid.ToActionResult(value, expression));
    }

    public static TSuccess EnsureSuccess<TSuccess>(this Response<TSuccess> response)
    {
        if (response.TryPickT0(out var success, out var errors))
            return success;

        errors.Switch(
            notFound => throw new HubException("Not Found"),
            alreadyExists => throw new HubException("Conflict"),
            invalid => throw new HubException("Bad Request"));

        throw new HubException("Internal Server Error");
    }

    public static IActionResult ToActionResult(this OneOf.Types.NotFound _, object? value, [CallerArgumentExpression(nameof(value))] string expression = "") => new NotFoundObjectResult(new ProblemDetails
    {
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
        Title = "Not Found",
        Status = StatusCodes.Status404NotFound,
        Detail = $"{expression} '{value}' not found"
    });

    public static IActionResult ToActionResult(this AlreadyExists _, object? value, [CallerArgumentExpression(nameof(value))] string expression = "") => new NotFoundObjectResult(new ProblemDetails
    {
        Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
        Title = "Conflict",
        Status = StatusCodes.Status409Conflict,
        Detail = $"{expression} '{value}' already exists"
    });

    public static IActionResult ToActionResult(this Invalid _, object? value, [CallerArgumentExpression(nameof(value))] string expression = "")
    {
        var details = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = StatusCodes.Status400BadRequest,
            Detail = $"{expression} is not valid"
        };

        foreach (var (key, errors) in _.Errors)
            details.Extensions[key] = errors;

        return new BadRequestObjectResult(details);
    }
}
