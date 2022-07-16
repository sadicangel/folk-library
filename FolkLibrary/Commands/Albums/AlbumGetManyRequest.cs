using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Albums;

public sealed class AlbumGetManyRequest : IRequest<List<AlbumReadDto>>
{
    public ISpecification<Album> Specification { get; init; } = null!;
}
