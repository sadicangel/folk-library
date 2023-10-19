using DotNext;
using FluentValidation;
using MediatR;

namespace FolkLibrary.Infrastructure;

public static class HandleExtensions
{
    public static async Task<IResult> HandleAsync<TRequest>(this TRequest request, IValidator<TRequest> validator, IRequestHandler<TRequest, Result<Unit>> handler, CancellationToken cancellationToken)
        where TRequest : IRequest<Result<Unit>>
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await handler.Handle(request, cancellationToken);
        return result.Map(ok => Results.Ok()).Match(ok => ok, err => Results.Problem(err.Message));
    }

    public static async Task<IResult> HandleAsync<TRequest, TResponse>(this TRequest request, IValidator<TRequest> validator, IRequestHandler<TRequest, Result<TResponse>> handler, CancellationToken cancellationToken)
        where TRequest : IRequest<Result<TResponse>>
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await handler.Handle(request, cancellationToken);
        return result.Match(Results.Ok, err => Results.Problem(err.Message));
    }
}