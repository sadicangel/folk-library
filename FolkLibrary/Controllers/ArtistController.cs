using Ardalis.Specification;
using FolkLibrary.Commands;
using FolkLibrary.Commands.Artists;
using FolkLibrary.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
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

    private static void BuildSpec(ISpecificationBuilder<Artist> builder)
    {
        builder.Include(a => a.Albums).Include(t => t.Tracks).AsNoTracking();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] string? country,
        [FromQuery] string? district,
        [FromQuery] string? municipality,
        [FromQuery] string? parish)
    {
        var specification = new GenericSpecification<Artist>(createSpec);
        var response = await _mediator.Send(new ArtistGetManyRequest { Specification = specification });
        return Ok(response);

        void createSpec(ISpecificationBuilder<Artist> builder)
        {
            BuildSpec(builder);
            if (country is not null)
                builder.Where(a => a.Country == country);
            if (district is not null)
                builder.Where(a => a.District == district);
            if (municipality is not null)
                builder.Where(a => a.Municipality == municipality);
            if (parish is not null)
                builder.Where(a => a.Parish == parish);
        }
    }

    [HttpGet("{albumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid albumId)
    {
        var specification = new GenericSingleResultSpecification<Artist>(albumId, BuildSpec);
        var response = await _mediator.Send(new ArtistGetSingleRequest { Specification = specification });
        if (response is null)
            return NotFound(albumId);
        return Ok(response);
    }
}
