namespace FolkLibrary.App.ViewModels;

[QueryProperty(nameof(ArtistId), nameof(ArtistId))]
public partial class ArtistViewModel : BaseViewModel
{
    private readonly IRepository<Artist> _artistRepository;

    [ObservableProperty]
    private Guid _artistId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Location))]
    private Artist? _artist;

    public string? Location { get => String.Join(" - ", EnumerateLocations(_artist)); }

    public ArtistViewModel(INavigationService navigationService, IRepository<Artist> artistRepository)
        : base(navigationService)
    {
        _artistRepository = artistRepository;
    }

    private static IEnumerable<string> EnumerateLocations(Artist? artist)
    {
        if (artist is not null)
        {
            if (artist.Country is not null)
                yield return artist.Country;
            if (artist.District is not null)
                yield return artist.District;
            if (artist.Municipality is not null)
                yield return artist.Municipality;
            if (artist.Parish is not null)
                yield return artist.Parish;
        }
    }

    [RelayCommand]
    private async Task OnLoadedAsync()
    {
        if(_artistId == default)
        {
            await NavigationService.ReturnAsync();
            return;
        }

        Artist = await _artistRepository.GetAsync(new GenericSingleResultSpecification<Artist>(_artistId, builder => builder.Configure()));

        if (Artist is null)
        {
            await NavigationService.ReturnAsync();
            return;
        }

        Title = Artist.ShortName;
    }
}
