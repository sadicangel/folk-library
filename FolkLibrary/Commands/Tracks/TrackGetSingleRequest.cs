using Ardalis.Specification;
using FolkLibrary.Dtos;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Commands.Tracks;

public sealed class TrackGetSingleRequest : IRequest<TrackReadDto>
{
    public ISingleResultSpecification<Track> Specification { get; init; } = null!;
}
