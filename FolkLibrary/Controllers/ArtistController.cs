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
    public async Task<IActionResult> GetAllArtists([FromHeader(Name = "X-Continuation-Token")] string? continuationToken, [FromQuery] GetAllArtistsQueryParams queryParams)
    {
        var response = await _mediator.Send(new GetAllArtistsQuery
        {
            QueryParams = queryParams,
            ContinuationToken = continuationToken,
        });
        return Ok(response);
    }

    [HttpGet("{artistId}", Name = nameof(GetArtistById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArtistById([FromRoute] Guid artistId)
    {
        var response = await _mediator.Send(new GetArtistByIdQuery { ArtistId = new ArtistId(artistId) });
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateArtist([FromBody] CreateArtistCommand createArtistCommand)
    {
        var response = await _mediator.Send(createArtistCommand);

        return CreatedAtAction(nameof(GetArtistById), new { artistId = response });
    }

    [HttpPut("album")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddAlbum([FromBody] AddAlbumCommand addAlbumCommand)
    {
        var response = await _mediator.Send(addAlbumCommand);

        return Ok(response);
    }
}
