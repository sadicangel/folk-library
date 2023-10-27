using Spectre.Console;

namespace FolkLibrary;

internal static class AnsiConsoleExtensions
{
    public static async Task<int> Unwrap(this IAnsiConsole console, Func<Task<int>> execute)
    {
        try
        {
            return await execute();
        }
        catch (Exception ex)
        {
            console.WriteException(ex, ExceptionFormats.ShortenEverything);
            return -1;
        }
    }
}
