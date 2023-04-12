using FolkLibrary.Artists.Events;

namespace FolkLibrary.Messaging;

public interface IArtistUpdatedEventPublisher : IEventPublisher<ArtistUpdatedEvent> { }