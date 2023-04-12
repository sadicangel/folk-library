using FolkLibrary.Artists.Events;

namespace FolkLibrary.Messaging;

public interface IArtistAlbumAddedEventPublisher : IEventPublisher<ArtistAlbumAddedEvent> { }