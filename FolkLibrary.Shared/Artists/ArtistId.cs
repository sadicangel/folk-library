using FolkLibrary.Interfaces;
using StronglyTypedIds;

namespace FolkLibrary.Artists;

[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter)]
public readonly partial struct ArtistId : IId<ArtistId>
{
    public static ArtistId New(Guid guid) => new(guid);
}
