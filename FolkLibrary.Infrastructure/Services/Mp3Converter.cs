using FolkLibrary.Interfaces;
using Microsoft.Extensions.Logging;
using NAudio.Lame;
using NAudio.Wave;

namespace FolkLibrary.Services;

internal sealed class Mp3Converter : IMp3Converter
{
    private readonly ILogger<Mp3Converter> _logger;
    private static readonly LameConfig LameConfig = new();

    public Mp3Converter(ILogger<Mp3Converter> logger)
    {
        _logger = logger;
    }

    public async Task ConvertToMp3Async(IEnumerable<string> folders, string extension, bool deleteOldFiles = true)
    {
        await Task.WhenAll(folders.Select(folder => ConvertToMp3Async(folder, extension, deleteOldFiles)));
    }

    public async Task ConvertToMp3Async(string folder, string extension, bool deleteOldFiles = true)
    {
        var filter = extension[0] == '.' ? $"*{extension}" : $"*.{extension}";
        if (Directory.EnumerateFiles(folder, filter).Any())
        {
            _logger.LogInformation("{folder}", folder);
            await Task.WhenAll(Directory.EnumerateFiles(folder, filter).Select(trackFile => ConvertToMp3Async(trackFile, deleteOldFiles)));
        }

        static async Task ConvertToMp3Async(string trackFile, bool deleteOld = true)
        {
            var mp3 = Path.ChangeExtension(trackFile, "mp3");
            {
                using var reader = new AudioFileReader(trackFile);
                using var writer = new LameMP3FileWriter(mp3, reader.WaveFormat, LameConfig);
                await reader.CopyToAsync(writer).ConfigureAwait(false);
            }

            var source = TagLib.File.Create(trackFile);
            var target = TagLib.File.Create(mp3);
            source.Tag.CopyTo(target.Tag, overwrite: true);
            target.Save();
            if (deleteOld)
                File.Delete(trackFile);
        }
    }
}