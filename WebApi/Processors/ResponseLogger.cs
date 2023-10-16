using FastEndpoints;
using FluentValidation.Results;
using OneOf;

namespace FolkLibrary.Processors;

internal sealed class ResponseLogger : IGlobalPostProcessor
{
    public Task PostProcessAsync(object req, object? res, HttpContext ctx, IReadOnlyCollection<ValidationFailure> failures, CancellationToken ct)
    {
        var logger = ctx.Resolve<ILogger<RequestLogger>>();
        if (res is IOneOf oneOf)
            logger.LogInformation("Response: {Type} {@Response}", oneOf.Value?.GetType().Name ?? "null", oneOf.Value);
        else
            logger.LogInformation("Response: {Type} {@Response}", res?.GetType().Name ?? "null", res);
        return Task.CompletedTask;
    }
}
