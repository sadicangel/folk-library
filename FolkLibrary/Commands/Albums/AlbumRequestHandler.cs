using FolkLibrary.Models;
using MediatR;
using Ardalis.Specification;
using FolkLibrary.Dtos;
using AutoMapper;
using FolkLibrary.Interfaces;

namespace FolkLibrary.Commands.Albums;

public sealed class AlbumRequestHandler :
    IRequestHandler<AlbumGetSingleRequest, AlbumReadDto>,
    IRequestHandler<AlbumGetManyRequest, List<AlbumReadDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepository<Album> _albumRepository;

    public AlbumRequestHandler(IMapper mapper, IRepository<Album> albumRepository)
    {
        _mapper = mapper;
        _albumRepository = albumRepository;
    }

    public async Task<AlbumReadDto> Handle(AlbumGetSingleRequest request, CancellationToken cancellationToken)
    {
        var response = await _albumRepository.GetAsync(request.Specification, cancellationToken);
        return _mapper.Map<AlbumReadDto>(response);
    }

    public async Task<List<AlbumReadDto>> Handle(AlbumGetManyRequest request, CancellationToken cancellationToken)
    {
        var response = await _albumRepository.GetAllAsync(request.Specification, cancellationToken);
        return _mapper.Map<List<AlbumReadDto>>(response);
    }
}
