using Ardalis.Specification;
using FolkLibrary.Commands;
using FolkLibrary.Commands.Tracks;
using FolkLibrary.Models;
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

    private static void BuildSpec(ISpecificationBuilder<Track> builder)
    {
        builder.Include(a => a.Album).Include(a => a.Artists).Include(a => a.Genres).AsNoTracking();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(
        [FromQuery] Guid? albumId,
        [FromQuery] Guid? artistId,
        [FromQuery] Guid? genreId)
    {
        var specification = new GenericSpecification<Track>(createSpec);
        var response = await _mediator.Send(new TrackGetManyRequest { Specification = specification });
        return Ok(response);

        void createSpec(ISpecificationBuilder<Track> builder)
        {
            BuildSpec(builder);
            if (albumId is not null)
                builder.Where(a => a.Album!.Id == albumId).OrderBy(t => t.Number);
            if (artistId is not null)
                builder.Where(a => a.Artists.Any(g => g.Id == artistId));
            if (genreId is not null)
                builder.Where(a => a.Genres.Any(g => g.Id == genreId));
        }
    }

    [HttpGet("{albumId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid albumId)
    {
        var specification = new GenericSingleResultSpecification<Track>(albumId, BuildSpec);
        var response = await _mediator.Send(new TrackGetSingleRequest { Specification = specification });
        if (response is null)
            return NotFound(albumId);
        return Ok(response);
    }
}
