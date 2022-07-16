using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Artists;

public sealed class ArtistGetSingleRequest : IRequest<ArtistReadDto>
{
    public ISingleResultSpecification<Artist> Specification { get; init; } = null!;
}
