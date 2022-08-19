namespace FolkLibrary.App.Interfaces;
public interface INavigationService
{
    public Task GoToAsync<T>() where T : BaseViewModel;
    public Task GoToAsync<T>(NavigationParameters parameters) where T : BaseViewModel;
    public Task ReturnAsync();
    public Task ReturnAsync(NavigationParameters parameters);
}
