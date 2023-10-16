﻿using FastEndpoints;
using FolkLibrary.Repositories;

namespace FolkLibrary.Albums.GetAlbumById;

public sealed class GetAlbumByIdMapper : Mapper<GetAlbumByIdRequest, AlbumDto, Album?>
{
    public override Task<Album?> ToEntityAsync(GetAlbumByIdRequest request, CancellationToken cancellationToken = default)
    {
        return Resolve<IAlbumRepository>().GetByIdAsync(request.AlbumId, cancellationToken);
    }

    public override Task<AlbumDto> FromEntityAsync(Album? entity, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(entity?.ToAlbumDto() ?? throw new ArgumentNullException(nameof(entity)));
    }
}