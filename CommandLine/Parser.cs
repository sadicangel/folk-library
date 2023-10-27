using MediatR;
using Spectre.Console;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace FolkLibrary;

internal sealed class Parser
{
    private readonly IAnsiConsole _console;
    private readonly IMediator _mediator;
    private readonly RootCommand _rootCommand;

    public Parser(IAnsiConsole console, IMediator mediator)
    {
        _console = console;
        _mediator = mediator;
        _rootCommand = new RootCommand
        {
            new HelloCommand
            {
                Handler = CommandHandler.Create<Hello, CancellationToken>(HandleOptions)
            },
            new CopyCommand
            {
                Handler = CommandHandler.Create<Copy, CancellationToken>(HandleOptions)
            },
            new QueryArtistsCommand
            {
                Handler = CommandHandler.Create<QueryArtists, CancellationToken>(HandleOptions)
            }
        };
    }
    private Task<int> HandleOptions<TRequest>(TRequest request, CancellationToken cancellationToken) where TRequest : IRequest<int>
    {
        return _mediator.Send(request, cancellationToken);
    }

    public Task<int> InvokeAsync(string commandLine, IConsole? console = null) => _rootCommand.InvokeAsync(commandLine, console);
}
