using FolkLibrary.Dtos;
using FolkLibrary.Exceptions;
using FolkLibrary.Models;
using FolkLibrary.Queries.Artists;
using HotChocolate;
using MediatR;

namespace FolkLibrary.Queries.GraphQL;

public sealed class Query
{
    public async ValueTask<Page<ArtistDto>> GetArtists(
        [Service] ISender mediator,
        string? country = null,
        string? district = null,
        string? municipality = null,
        string? parish = null,
        int? afterYear = null,
        int? beforeYear = null,
        string? continuationToken = null)
    {
        var response = await mediator.Send(new GetAllArtistsQuery
        {
            QueryParams = new GetAllArtistsQueryParams
            {
                Country = country,
                District = district,
                Municipality = municipality,
                Parish = parish,
                AfterYear = afterYear,
                BeforeYear = beforeYear,
            },
            ContinuationToken = continuationToken

        });
        return response;
    }

    public async ValueTask<ArtistDto?> GetArtist([Service] ISender mediator, ArtistId artistId)
    {
        try
        {
            return await mediator.Send(new GetArtistByIdQuery { ArtistId = artistId });
        }
        catch (NotFoundException)
        {
            return null;
        }
    }
}
