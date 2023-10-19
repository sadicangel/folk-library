using System.Reflection;
using System.Text.Json;

namespace FolkLibrary.Services;
public interface ICountryInfoProvider
{
    string GetCountryName(string alpha2, string language = "en");
}

internal sealed class CountryInfoProvider : ICountryInfoProvider
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string?>> _countries;

    public CountryInfoProvider()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileName = assembly.GetManifestResourceNames().Single(r => r.EndsWith("Countries.json"));
        using var stream = assembly.GetManifestResourceStream(fileName)!;
        var nodes = JsonSerializer.Deserialize<List<JsonElement>>(stream)!;

        _countries = nodes.ToDictionary(GetId, FilterObject, StringComparer.InvariantCultureIgnoreCase);

        static string GetId(JsonElement element) => element.GetProperty("alpha2").GetString()!;

        static IReadOnlyDictionary<string, string?> FilterObject(JsonElement element) =>
            element.EnumerateObject().Where(p => p.Value.ValueKind == JsonValueKind.String).ToDictionary(p => p.Name, p => p.Value.GetString());
    }

    public string GetCountryName(string alpha2, string language = "en") => _countries[alpha2][language] ?? throw new InvalidOperationException($"'{language}' name for country {alpha2} does not exist");
}
