namespace FolkLibrary.Services;
public interface ICountryInfoProvider
{
    string GetCountryName(string alpha2);

    string? GetCountryName(string alpha2, string language);

    string GetCountryNameOrDefault(string alpha2, string language);
}
