using Ardalis.Specification;
using FastEndpoints;
using FluentValidation;
using FolkLibrary.Artists.Events;
using FolkLibrary.Artists.GetArtistById;
using FolkLibrary.Repositories;
using System.Net;
using System.Net.Mime;

namespace FolkLibrary.Artists.CreateArtist;

public sealed class CreateArtistEndpoint : Endpoint<CreateArtistRequest, ArtistDto, CreateArtistMapper>
{
    private readonly IArtistRepository _artistRepository;

    public CreateArtistEndpoint(IArtistRepository artistRepository)
    {
        _artistRepository = artistRepository;
    }

    public override void Configure()
    {
        Post("/api/artist");
    }

    public override async Task HandleAsync(CreateArtistRequest request, CancellationToken cancellationToken)
    {
        if (await _artistRepository.AnyAsync(new FindByNameSpecification(request.Name), cancellationToken))
        {
            await SendStringAsync($"'{request.Name}' already exists", (int)HttpStatusCode.Conflict, MediaTypeNames.Text.Plain, cancellationToken);
            return;
        }
        var artist = await Map.ToEntityAsync(request, cancellationToken);

        await _artistRepository.AddAsync(artist, cancellationToken);

        await PublishAsync(new ArtistCreatedEvent { ArtistId = artist.Id }, Mode.WaitForNone, cancellationToken);

        var response = await Map.FromEntityAsync(artist, cancellationToken);

        await SendCreatedAtAsync<GetArtistByIdEndpoint>(new { artistId = artist.Id }, response, cancellation: cancellationToken);
    }

    private sealed class FindByNameSpecification : Specification<Artist>
    {
        public FindByNameSpecification(string name)
        {
            Query.Where(e => e.Name == name);
        }
    }
}