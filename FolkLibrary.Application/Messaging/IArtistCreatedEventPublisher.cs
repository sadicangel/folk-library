using FolkLibrary.Artists.Events;

namespace FolkLibrary.Messaging;

public interface IArtistCreatedEventPublisher : IEventPublisher<ArtistCreatedEvent> { }