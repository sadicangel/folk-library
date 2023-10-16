using CommandLine;

namespace FolkLibrary.Cli;

[Verb("copy", HelpText = "Copy albums to a directory.")]
public sealed class CopyArguments
{
    [Option('s', "source", HelpText = "Source directory with all artists.")]
    public required string Source { get; init; }

    [Option('d', "destination", HelpText = "Destination directory to copy selected albums to.")]
    public required string Destination { get; init; }
}

public static class CopyHandler
{
    public static void Handle(CopyArguments arguments)
    {
        var map = new Dictionary<string, string>();

        foreach (var artist in Directory.EnumerateDirectories(arguments.Source).SkipLast(1))
        {
            var artistName = Path.GetFileName(artist);
            foreach (var album in Directory.EnumerateDirectories(artist))
            {
                var albumName = Path.GetFileName(album);
                Console.WriteLine($"{artistName}");
                Console.WriteLine($"  {albumName}");
                foreach (var file in Directory.EnumerateFiles(album))
                    Console.WriteLine($"    {Path.GetFileNameWithoutExtension(file)}");
                Console.WriteLine("Copy album? ");

                var shouldCopy = Console.ReadKey().Key is ConsoleKey.Y;
                if (shouldCopy)
                {
                    map[album] = Path.Combine(arguments.Destination, artistName, albumName);
                    Console.WriteLine($"Added {artistName}/{albumName}");
                }
                Console.Clear();
            }
        }

        foreach (var (s, t) in map)
        {
            CopyDirectory(s, t);
            Console.WriteLine(t);
        }
    }

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
