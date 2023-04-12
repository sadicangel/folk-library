using FolkLibrary.Albums.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FolkLibrary.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AlbumController : ControllerBase
{
    private readonly ISender _mediator;

    public AlbumController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAlbum([FromBody] CreateAlbumCommand createAlbumCommand)
    {
        var response = await _mediator.Send(createAlbumCommand);

        return response.ToActionResult(Ok, createAlbumCommand.Name);
    }
}
