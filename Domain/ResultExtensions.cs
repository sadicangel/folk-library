using DotNext;
using System.Runtime.ExceptionServices;

namespace FolkLibrary;

public static class ResultExtensions
{
    private static Exception PrepareForRethrow(Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();

        // The code cannot ever get here. We just return a value to work around a badly-designed API (ExceptionDispatchInfo.Throw):
        //  https://connect.microsoft.com/VisualStudio/feedback/details/689516/exceptiondispatchinfo-api-modifications (http://www.webcitation.org/6XQ7RoJmO)
        return exception;
    }

    public static Result<TResult> Map<TSource, TResult>(this Result<TSource> result, Func<TSource, TResult> map) =>
        result.Match(ok => map(ok), static err => new Result<TResult>(err));

    public static void Match<T>(this Result<T> result, Action<T> ok, Action<Exception> err)
    {
        if (result.IsSuccessful)
            ok(result.Value);
        else
            err(result.Error);
    }

    public static TResult Match<T, TResult>(this Result<T> result, Func<T, TResult> ok, Func<Exception, TResult> err)
    {
        return result.IsSuccessful ? ok(result.Value) : err(result.Error);
    }

    public static T Unwrap<T>(this Result<T> result) => result.IsSuccessful ? result.Value : throw PrepareForRethrow(result.Error);

    public static async Task<T> UnwrapAsync<T>(this Task<Result<T>> resultTask) => Unwrap(await resultTask);

    public static void Match<T>(this Optional<T> option, Action<T> some, Action none)
    {
        if (option)
            some(option.Value);
        else
            none();
    }

    public static TResult Match<T, TResult>(this Optional<T> option, Func<T, TResult> some, Func<TResult> none)
    {
        return option ? some(option.Value) : none();
    }
}