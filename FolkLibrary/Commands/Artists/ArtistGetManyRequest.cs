using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Artists;

public sealed class ArtistGetManyRequest : IRequest<List<ArtistReadDto>>
{
    public ISpecification<Artist> Specification { get; init; } = null!;
}
