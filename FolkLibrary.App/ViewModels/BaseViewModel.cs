namespace FolkLibrary.App.ViewModels;
public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _title;

    protected INavigationService NavigationService { get; }

    protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }
}
