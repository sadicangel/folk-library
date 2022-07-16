using FolkLibrary.Models;
using MediatR;
using FolkLibrary.Services;
using FolkLibrary.Dtos;
using AutoMapper;

namespace FolkLibrary.Commands.Artists;

public sealed class ArtistRequestHandler :
    IRequestHandler<ArtistGetSingleRequest, ArtistReadDto>,
    IRequestHandler<ArtistGetManyRequest, List<ArtistReadDto>>

{
    private readonly IMapper _mapper;
    private readonly IRepository<Artist> _artistRepository;

    public ArtistRequestHandler(IMapper mapper, IRepository<Artist> artistRepository)
    {
        _mapper = mapper;
        _artistRepository = artistRepository;
    }

    public async Task<ArtistReadDto> Handle(ArtistGetSingleRequest request, CancellationToken cancellationToken)
    {
        var response = await _artistRepository.GetAsync(request.Specification, cancellationToken);
        return _mapper.Map<ArtistReadDto>(response);
    }

    public async Task<List<ArtistReadDto>> Handle(ArtistGetManyRequest request, CancellationToken cancellationToken)
    {
        var response = await _artistRepository.GetAllAsync(request.Specification, cancellationToken);
        return _mapper.Map<List<ArtistReadDto>>(response);
    }
}
