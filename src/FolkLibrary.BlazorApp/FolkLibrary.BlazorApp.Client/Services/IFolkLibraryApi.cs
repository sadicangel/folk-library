using System.Text.Json.Nodes;
using Refit;

namespace FolkLibrary.BlazorApp.Client.Services;

public interface IFolkLibraryApi
{
    [Get("/api")]
    public Task<string> Hello();


    [Get("/api/artists")]
    public Task<JsonArray> GetArtists();
}
