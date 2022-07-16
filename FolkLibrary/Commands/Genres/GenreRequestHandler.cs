using FolkLibrary.Models;
using MediatR;
using FolkLibrary.Services;
using FolkLibrary.Dtos;
using AutoMapper;

namespace FolkLibrary.Commands.Genres;

public sealed class GenreRequestHandler :
    IRequestHandler<GenreGetSingleRequest, GenreReadDto>,
    IRequestHandler<GenreGetManyRequest, List<GenreReadDto>>

{
    private readonly IMapper _mapper;
    private readonly IRepository<Genre> _genreRepository;

    public GenreRequestHandler(IMapper mapper, IRepository<Genre> genreRepository)
    {
        _mapper = mapper;
        _genreRepository = genreRepository;
    }

    public async Task<GenreReadDto> Handle(GenreGetSingleRequest request, CancellationToken cancellationToken)
    {
        var response = await _genreRepository.GetAsync(request.Specification, cancellationToken);
        return _mapper.Map<GenreReadDto>(response);
    }

    public async Task<List<GenreReadDto>> Handle(GenreGetManyRequest request, CancellationToken cancellationToken)
    {
        var response = await _genreRepository.GetAllAsync(request.Specification, cancellationToken);
        return _mapper.Map<List<GenreReadDto>>(response);
    }
}
