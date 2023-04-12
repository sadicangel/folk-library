using FolkLibrary.Artists.Events;

namespace FolkLibrary.Messaging;

public interface IArtistDeletedEventPublisher : IEventPublisher<ArtistDeletedEvent> { }