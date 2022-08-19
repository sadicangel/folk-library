namespace FolkLibrary.App.Services;
public sealed class NavigationService : INavigationService
{
    private static readonly Dictionary<Type, string> _typeRoutes;

    static NavigationService()
    {
        _typeRoutes = new();
        var asm = typeof(NavigationService).Assembly;
        var ns = typeof(MainViewModel).Namespace;
        var contentPageTypes = asm.GetTypes().Where(type => type.IsAssignableTo(typeof(ContentPage)));
        foreach (var contentPageType in contentPageTypes)
        {
            Routing.RegisterRoute(contentPageType.Name, contentPageType);
            var viewModelTypeName = $"{ns}.{contentPageType.Name[..^4]}ViewModel";
            _typeRoutes[asm.GetType(viewModelTypeName, throwOnError: true)!] = contentPageType.Name;
        }
    }

    public Task GoToAsync<T>() where T : BaseViewModel => GoToUriAsync(_typeRoutes[typeof(T)]);
    public Task GoToAsync<T>(NavigationParameters parameters) where T : BaseViewModel => GoToUriAsync(_typeRoutes[typeof(T)], parameters);
    public Task ReturnAsync() => GoToUriAsync("..");
    public Task ReturnAsync(NavigationParameters parameters) => GoToUriAsync("..", parameters);

    private async Task GoToUriAsync(string uri) => await Shell.Current.GoToAsync(new ShellNavigationState(uri));
    private Task GoToUriAsync(string uri, NavigationParameters parameters) => Shell.Current.GoToAsync(new ShellNavigationState(uri), parameters);
}
