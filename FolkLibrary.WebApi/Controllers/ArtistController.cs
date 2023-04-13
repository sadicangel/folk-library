using FolkLibrary.Artists;
using FolkLibrary.Artists.Commands;
using FolkLibrary.Artists.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FolkLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistController : ControllerBase
{
    private readonly ISender _mediator;

    public ArtistController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllArtists(
        [FromQuery] ArtistFilterDto? filter,
        [FromHeader(Name = "X-Page-Index")] int? pageIndex,
        [FromHeader(Name = "X-Page-Size")] int? pageSize)
    {
        var response = await _mediator.Send(new GetArtistsQuery
        {
            Filter = filter,
            PageIndex = pageIndex,
            PageSize = pageSize
        });
        return response.ToActionResult(ok => Ok(ok), filter);
    }

    [HttpGet("{artistId}", Name = nameof(GetArtistById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArtistById([FromRoute] string artistId)
    {
        var response = await _mediator.Send(new GetArtistByIdQuery { ArtistId = artistId });
        return response.ToActionResult(Ok, artistId);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateArtist([FromBody] CreateArtistCommand createArtistCommand)
    {
        var response = await _mediator.Send(createArtistCommand);

        return response.ToActionResult(ok => CreatedAtAction(nameof(GetArtistById), new { artistId = ok.Id }), createArtistCommand);
    }

    [HttpPut("{artistId}/album/{albumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddAlbum([FromRoute] string artistId, [FromRoute] string albumId)
    {
        var response = await _mediator.Send(new AddArtistAlbumCommand
        {
            ArtistId = artistId,
            AlbumId = albumId
        });

        return response.ToActionResult(_ => Ok(), new { artistId, albumId });
    }
}
