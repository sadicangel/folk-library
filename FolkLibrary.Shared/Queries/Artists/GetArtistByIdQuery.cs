using AutoMapper;
using FluentValidation;
using FolkLibrary.Dtos;
using FolkLibrary.Exceptions;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using MediatR;

namespace FolkLibrary.Queries.Artists;

public sealed class GetArtistByIdQuery : IRequest<ArtistDto>
{
    public Guid ArtistId { get; init; }

    public sealed class Validator : AbstractValidator<GetArtistByIdQuery>
    {
        public Validator() => RuleFor(e => e.ArtistId).NotEmpty();
    }

    public sealed class Handler : IRequestHandler<GetArtistByIdQuery, ArtistDto>
    {
        private readonly IMongoRepository<ArtistDto> _artistRepository;

        public Handler(IMongoRepository<ArtistDto> artistRepository)
        {
            _artistRepository = artistRepository;
        }

        public async Task<ArtistDto> Handle(GetArtistByIdQuery request, CancellationToken cancellationToken)
        {
            var artist = await _artistRepository.GetByIdAsync(request.ArtistId, cancellationToken);
            if (artist is null)
                throw new NotFoundException($"Artist does not exist ({request.ArtistId})");
            return artist;
        }
    }
}
