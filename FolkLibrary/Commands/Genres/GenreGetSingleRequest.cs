using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Genres;

public sealed class GenreGetSingleRequest : IRequest<GenreReadDto>
{
    public ISingleResultSpecification<Genre> Specification { get; init; } = null!;
}
