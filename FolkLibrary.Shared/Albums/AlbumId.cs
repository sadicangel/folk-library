using FolkLibrary.Interfaces;
using StronglyTypedIds;

namespace FolkLibrary.Albums;

[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter)]
public readonly partial struct AlbumId : IId<AlbumId>
{
    public static AlbumId New(Guid guid) => new(guid);
}
