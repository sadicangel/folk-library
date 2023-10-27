using MediatR;
using Spectre.Console;
using System.CommandLine;

namespace FolkLibrary;

internal sealed class HelloCommand : Command
{
    public HelloCommand() : base("hello", "Say hello to someone")
    {
        AddOption(new Option<string>("--to", "The person to say hello to")
        {
            IsRequired = true,
        });
    }
}
internal sealed record class Hello(string To) : IRequest<int>;

internal class HelloHandler : IRequestHandler<Hello, int>
{
    private readonly IAnsiConsole _console;

    // Inject anything here, no more hard dependency on System.CommandLine
    public HelloHandler(IAnsiConsole console)
    {
        _console = console;
    }

    public Task<int> Handle(Hello request, CancellationToken cancellationToken) => _console.Unwrap(() =>
    {
        _console.MarkupLine($"Hello [bold green]{request.To}[/]!");
        return Task.FromResult(0);
    });
}