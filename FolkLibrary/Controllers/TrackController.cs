using FolkLibrary.Commands.Tracks;
using FolkLibrary.Models;
using FolkLibrary.Specifications;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FolkLibrary.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrackController : ControllerBase
{
    private readonly ISender _mediator;

    public TrackController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid? albumId, [FromQuery] Guid? artistId)
    {
        var specification = new GenericSpecification<Track>(builder => builder.GetAll(albumId, artistId));
        var response = await _mediator.Send(new TrackGetManyRequest { Specification = specification });
        return Ok(response);
    }

    [HttpGet("{albumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid albumId)
    {
        var specification = new GenericSingleResultSpecification<Track>(albumId, builder => builder.GetAll());
        var response = await _mediator.Send(new TrackGetSingleRequest { Specification = specification });
        if (response is null)
            return NotFound(albumId);
        return Ok(response);
    }
}
