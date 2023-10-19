using Generator.Equals;

namespace FolkLibrary;

[Equatable]
public sealed partial record class Track(
    Guid TrackId,
    string Name,
    int Number,
    string? Description,
    int? Year,
    bool IsYearUncertain,
    TimeSpan Duration,
    [property: UnorderedEquality] List<string> Genres
);
