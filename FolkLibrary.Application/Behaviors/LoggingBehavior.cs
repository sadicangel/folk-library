using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace FolkLibrary.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Request: {Type} {@Request}", typeof(TRequest).Name, request);
        var response = await next();
        if (response is IOneOf oneOf)
            _logger.LogInformation("Response: {Type} ({Subtype}) {@Response}", typeof(TResponse).Name, oneOf.Value?.GetType().Name ?? "null", oneOf.Value);
        else
            _logger.LogInformation("Response: {Type} {@Response}", typeof(TResponse).Name, response);
        return response;
    }
}
