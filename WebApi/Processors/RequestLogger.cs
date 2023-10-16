using FastEndpoints;
using FluentValidation.Results;

namespace FolkLibrary.Processors;

internal sealed class RequestLogger : IGlobalPreProcessor
{
    public Task PreProcessAsync(object req, HttpContext ctx, List<ValidationFailure> failures, CancellationToken ct)
    {
        var logger = ctx.Resolve<ILogger<RequestLogger>>();
        logger.LogInformation("Request: {Type} {@Request}", req.GetType().Name, req);
        return Task.CompletedTask;
    }
}