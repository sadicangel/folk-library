using FolkLibrary.Queries.Artists;
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
        [FromQuery] string? country,
        [FromQuery] string? district,
        [FromQuery] string? municipality,
        [FromQuery] string? parish,
        [FromQuery] int? afterYear,
        [FromQuery] int? beforeYear,
        [FromQuery] string? continuationToken)
    {
        var response = await _mediator.Send(new GetAllArtistsQuery
        {
            Country = country,
            District = district,
            Municipality = municipality,
            Parish = parish,
            AfterYear = afterYear,
            BeforeYear = beforeYear,
            ContinuationToken = continuationToken,

        });
        return Ok(response);
    }

    [HttpGet("{albumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArtistById([FromRoute] Guid artistId)
    {
        var response = await _mediator.Send(new GetArtistByIdQuery { ArtistId = artistId });
        return Ok(response);
    }
}
