using DotNext;

namespace FolkLibrary;

public static class ResultExtensions
{
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
}
