using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace FolkLibrary;

internal sealed class ParserService : BackgroundService
{
    private readonly IAnsiConsole _console;
    private readonly Parser _parser;
    private readonly TextPrompt<string> _prompt;

    public ParserService(IAnsiConsole console, Parser parser)
    {
        _console = console;
        _parser = parser;
        _prompt = new TextPrompt<string>("[green]%[/]")
            .PromptStyle("grey70");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _console
            .Status()
            .StartAsync("Loading", ctx => _parser.InvokeAsync("--help"));

        while (!stoppingToken.IsCancellationRequested)
        {
            WriteDivider();
            try
            {
                //var line = await _prompt.ShowAsync(_console, stoppingToken);
                var line = await Console.In.ReadLineAsync(stoppingToken);

                if (!String.IsNullOrEmpty(line))
                {
                    await _console
                        .Status()
                        .StartAsync("Processing", ctx => _parser.InvokeAsync(line));
                }
            }
            catch (TaskCanceledException)
            {
                // Expected.
            }
        }
    }

    private void WriteDivider()
    {
        _console.WriteLine();
        _console.Write(new Rule().RuleStyle("grey").LeftJustified());
    }
}