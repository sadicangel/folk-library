using FolkLibrary.Interfaces;
using StronglyTypedIds;

namespace FolkLibrary.Tracks;

[StronglyTypedId(StronglyTypedIdBackingType.Guid, StronglyTypedIdConverter.SystemTextJson | StronglyTypedIdConverter.EfCoreValueConverter)]
public readonly partial struct TrackId : IId<TrackId>
{
    public static TrackId New(Guid guid) => new(guid);
}
