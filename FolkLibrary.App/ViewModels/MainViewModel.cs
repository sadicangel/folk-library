using Ardalis.Specification;
using FolkLibrary.Artists;

namespace FolkLibrary.App.ViewModels;
public partial class MainViewModel : BaseViewModel
{
    private readonly IFolkHttpClient _folkHttpClient;
    private readonly List<ArtistDocument> _artists = new();

    [ObservableProperty]
    private ObservableCollection<ArtistDocument> _artistsView = new();

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

    public MainViewModel(INavigationService navigationService, IFolkHttpClient folkHttpClient)
        : base(navigationService)
    {
        Title = "Artists";
        _folkHttpClient = folkHttpClient;
    }

    [RelayCommand]
    private async Task OnLoadedAsync()
    {
        _artists.Clear();
        var first = await _folkHttpClient.GetAllArtistsAsync();
        _artists.AddRange(first.Elements);

        if (first.HasMoreResults)
            LoadRemaining(first.ContinuationToken).GetAwaiter();
        _artists.Sort((a, b) => a.Year!.Value.CompareTo(b.Year!.Value));
        ArtistsView = new(_artists);

        async Task LoadRemaining(string continuationToken)
        {
            var page = await _folkHttpClient.GetAllArtistsAsync(continuationToken: continuationToken);
            _artists.AddRange(page.Elements);
            if (page.HasMoreResults)
                LoadRemaining(page.ContinuationToken).GetAwaiter();
        }
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
