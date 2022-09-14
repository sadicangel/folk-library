using FolkLibrary.Commands.Artists;
using FolkLibrary.Models;
using FolkLibrary.Specifications;
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
    public async Task<IActionResult> Get([FromQuery] string? country, [FromQuery] string? district, [FromQuery] string? municipality, [FromQuery] string? parish)
    {
        var specification = new GenericSpecification<Artist>(builder => builder.GetAll(country, district, municipality, parish));
        var response = await _mediator.Send(new ArtistGetManyRequest { Specification = specification });
        return Ok(response);
    }

    [HttpGet("{albumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid albumId)
    {
        var specification = new GenericSingleResultSpecification<Artist>(albumId, builder => builder.Configure());
        var response = await _mediator.Send(new ArtistGetSingleRequest { Specification = specification });
        if (response is null)
            return NotFound(albumId);
        return Ok(response);
    }
}
