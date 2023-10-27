using MediatR;
using Spectre.Console;
using System.CommandLine;

namespace FolkLibrary;

internal sealed class ExitCommand : Command
{
    public ExitCommand() : base("exit") { }
}

internal sealed record class Exit : IRequest<int>;

internal sealed class ExitHandler : IRequestHandler<Exit, int>
{
    private readonly IAnsiConsole _console;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public ExitHandler(IAnsiConsole console, CancellationTokenSource cancellationTokenSource)
    {
        _console = console;
        _cancellationTokenSource = cancellationTokenSource;
    }

    public Task<int> Handle(Exit request, CancellationToken cancellationToken) => _console.Unwrap(() =>
    {
        _cancellationTokenSource.Cancel();
        return Task.FromResult(0);
    });
}