using DotNext;
using FluentValidation;
using MediatR;

namespace FolkLibrary.Infrastructure;

public static class HandleExtensions
{
    private static IResult MatchError(Exception err)
    {
        return err switch
        {
            ValidationException ex => Results.ValidationProblem(ex.Errors.GroupBy(x => x.PropertyName).ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray())),
            _ => Results.Problem(err.Message)
        };
    }

    public static async Task<IResult> ToResultAsync(this Task<Result<Unit>> result)
    {
        var unwrapped = await result;
        return unwrapped.Match(ok => Results.Ok(), MatchError);
    }

    public static async Task<IResult> ToResultAsync<T>(this Task<Result<T>> result)
    {
        var unwrapped = await result;
        return unwrapped.Match(Results.Ok, MatchError);
    }
}