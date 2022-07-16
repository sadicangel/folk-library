using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Genres;

public sealed class GenreGetManyRequest : IRequest<List<GenreReadDto>>
{
    public ISpecification<Genre> Specification { get; init; } = null!;
}
