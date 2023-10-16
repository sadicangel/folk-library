using FastEndpoints;

namespace FolkLibrary.Artists.GetArtists;

public sealed class GetArtistsRequest
{
    [FromHeader(HeaderName = "X-Page-Index", IsRequired = false)]
    public int? PageIndex { get; init; }

    [FromHeader(HeaderName = "X-Page-Size", IsRequired = false)]
    public int? PageSize { get; init; }

    [FromQueryParams]
    public ArtistFilterDto? Filter { get; init; }
}
