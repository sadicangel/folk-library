using Ardalis.Specification;
using FolkLibrary.Specifications;

namespace FolkLibrary.App.ViewModels;
public partial class MainViewModel : BaseViewModel
{
    private readonly IRepository<Artist> _artistRepository;
    private readonly List<Artist> _artists = new();

    [ObservableProperty]
    private ObservableCollection<Artist> _artistsView = new();

    [ObservableProperty]
    private bool _showLocal = true;

    [ObservableProperty]
    private bool _showAbroad = true;

    [ObservableProperty]
    private bool _showYearCertain = true;

    [ObservableProperty]
    private bool _showYearUncertain = true;

    [ObservableProperty]
    private string? _searchText;

    public MainViewModel(INavigationService navigationService, IRepository<Artist> artistRepository)
        : base(navigationService)
    {
        Title = "Artists";
        _artistRepository = artistRepository;
    }

    [RelayCommand]
    private async Task OnLoadedAsync()
    {
        _artists.Clear();
        _artists.AddRange(await _artistRepository.GetAllAsync(new GenericSpecification<Artist>(builder => builder.OrderBy(a => a.Year))));
        ArtistsView = new(_artists);
    }

    [RelayCommand]
    private async Task OnArtistTappedAsync(Guid artistId)
    {
        if (artistId == default)
            return;

        await NavigationService.GoToAsync<ArtistViewModel>(new NavigationParameters
        {
            ["ArtistId"] = artistId
        });
    }

    [RelayCommand]
    private void OnFiltersChanged()
    {
        ArtistsView = new(_artists.Where(a =>
        {
            var abroad = (ShowLocal && !a.IsAbroad) || (ShowAbroad && a.IsAbroad);
            var year = (ShowYearCertain && !a.IsYearUncertain) || (ShowYearUncertain && a.IsYearUncertain);
            var text = String.IsNullOrWhiteSpace(SearchText)
                || a.Name.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase)
                || a.Country.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase)
                || a.District is not null && a.District.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase)
                || a.Municipality is not null && a.Municipality.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase)
                || a.Parish is not null && a.Parish.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);
            return abroad && year && text;
        }));
    }
}
