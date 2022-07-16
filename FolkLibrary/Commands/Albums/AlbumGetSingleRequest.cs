using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Albums;

public sealed class AlbumGetSingleRequest : IRequest<AlbumReadDto>
{
    public ISingleResultSpecification<Album> Specification { get; init; } = null!;
}
