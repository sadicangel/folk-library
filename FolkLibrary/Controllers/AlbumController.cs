using Ardalis.Specification;
using FolkLibrary.Models;
using FolkLibrary.Commands.Albums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FolkLibrary.Commands;
using FolkLibrary.Specifications;

namespace FolkLibrary.Controllers;
[ApiController]
[Route("api/[controller]")]
public sealed class AlbumController : ControllerBase
{
    private readonly ISender _mediator;

    public AlbumController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid? artistId)
    {
        var specification = new GenericSpecification<Album>(builder => builder.GetAll(artistId));
        var response = await _mediator.Send(new AlbumGetManyRequest { Specification = specification });
        return Ok(response);
    }

    [HttpGet("{albumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid albumId)
    {
        var specification = new GenericSingleResultSpecification<Album>(albumId, builder => builder.GetAll());
        var response = await _mediator.Send(new AlbumGetSingleRequest { Specification = specification });
        if (response is null)
            return NotFound(albumId);
        return Ok(response);
    }
}
