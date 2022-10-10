﻿using MediatR;
using Microsoft.Extensions.Logging;

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
        _logger.LogInformation("Request: {Name} {@Request}", typeof(TRequest).Name, request);
        var response = await next();
        _logger.LogInformation("Reseponse: {Name} {@Response}", typeof(TResponse).Name, response);
        return response;
    }
}
