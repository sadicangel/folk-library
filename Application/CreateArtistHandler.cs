using DotNext;
using MediatR;

namespace FolkLibrary;

public sealed record class CreateArtistRequest(
    Guid Id,
    string Name,
    string ShortName,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    bool IsAbroad,
    string Country,
    string? District,
    string? Municipality,
    string? Parish,
    List<string> Genres) : IRequest<Result<Unit>>;

public sealed class CreateArtistHandler : IRequestHandler<CreateArtistRequest, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(CreateArtistRequest request, CancellationToken cancellationToken)
    {
        return Random.Shared.Next(2) == 1 ? await Unit.Task : new Result<Unit>(new Exception("Error"));
    }
}