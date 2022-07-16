using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Tracks;

public sealed class TrackGetManyRequest : IRequest<List<TrackReadDto>>
{
    public ISpecification<Track> Specification { get; init; } = null!;
}
