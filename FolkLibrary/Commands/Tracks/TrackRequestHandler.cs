using FolkLibrary.Models;
using MediatR;
using Ardalis.Specification;
using System.Net;
using FolkLibrary.Dtos;
using AutoMapper;
using FolkLibrary.Interfaces;

namespace FolkLibrary.Commands.Tracks;

public sealed class TrackRequestHandler :
    IRequestHandler<TrackGetSingleRequest, TrackReadDto>,
    IRequestHandler<TrackGetManyRequest, List<TrackReadDto>>

{
    private readonly IMapper _mapper;
    private readonly IRepository<Track> _trackRepository;

    public TrackRequestHandler(IMapper mapper, IRepository<Track> trackRepository)
    {
        _mapper = mapper;
        _trackRepository = trackRepository;
    }

    public async Task<TrackReadDto> Handle(TrackGetSingleRequest request, CancellationToken cancellationToken)
    {
        var response = await _trackRepository.GetAsync(request.Specification, cancellationToken);
        return _mapper.Map<TrackReadDto>(response);
    }

    public async Task<List<TrackReadDto>> Handle(TrackGetManyRequest request, CancellationToken cancellationToken)
    {
        var response = await _trackRepository.GetAllAsync(request.Specification, cancellationToken);
        return _mapper.Map<List<TrackReadDto>>(response);
    }
}
