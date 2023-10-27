using MediatR;
using Spectre.Console;
using System.CommandLine;

namespace FolkLibrary;

internal sealed class CopyCommand : Command
{
    public CopyCommand() : base("copy", "Copy albums to a directory.")
    {
        AddOption(new Option<string>(new string[] { "-s", "--source" }, "Source directory with all artists.")
        {
            IsRequired = true,
        });
        AddOption(new Option<string>(new string[] { "-d", "--destination" }, "Destination directory to copy selected albums to.")
        {
            IsRequired = true
        });
    }
}

internal sealed record class Copy(string Source, string Destination) : IRequest<int>;

internal sealed class CopyHandler : IRequestHandler<Copy, int>
{
    private readonly IAnsiConsole _console;

    public CopyHandler(IAnsiConsole console)
    {
        _console = console;
    }

    public Task<int> Handle(Copy request, CancellationToken cancellationToken) => _console.Unwrap(() =>
    {
        var map = new Dictionary<string, string>();

        foreach (var artist in Directory.EnumerateDirectories(request.Source).SkipLast(1))
        {
            var artistName = Path.GetFileName(artist);
            foreach (var album in Directory.EnumerateDirectories(artist))
            {
                var albumName = Path.GetFileName(album);
                _console.WriteLine($"{artistName}");
                _console.WriteLine($"  {albumName}");
                foreach (var file in Directory.EnumerateFiles(album))
                    _console.WriteLine($"    {Path.GetFileNameWithoutExtension(file)}");
                _console.WriteLine("Copy album? ");
                var shouldCopy = _console.Input.ReadKey(false)!.Value.Key is ConsoleKey.Y;
                if (shouldCopy)
                {
                    map[album] = Path.Combine(request.Destination, artistName, albumName);
                    _console.WriteLine($"Added {artistName}/{albumName}");
                }
                _console.Clear();
            }
        }

        foreach (var (s, t) in map)
        {
            CopyDirectory(s, t);
            _console.WriteLine(t);
        }

        return Task.FromResult(0);
    });

    private static void CopyDirectory(string source, string target)
    {
        var dir = new DirectoryInfo(source);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        DirectoryInfo[] dirs = dir.GetDirectories();

        Directory.CreateDirectory(target);

        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(target, file.Name);
            file.CopyTo(targetFilePath);
        }

        foreach (DirectoryInfo subDir in dirs)
        {
            string newDestinationDir = Path.Combine(target, subDir.Name);
            CopyDirectory(subDir.FullName, newDestinationDir);
        }
    }
}