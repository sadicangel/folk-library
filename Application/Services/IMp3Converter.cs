namespace FolkLibrary.Interfaces;

public interface IMp3Converter
{
    Task ConvertToMp3Async(IEnumerable<string> folders, string extension, bool deleteOldFiles = true);
    Task ConvertToMp3Async(string folder, string extension, bool deleteOldFiles = true);
}