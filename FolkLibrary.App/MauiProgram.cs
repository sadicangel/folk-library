namespace FolkLibrary.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        ConfigureServices(builder.Services);
        ConfigureViewModels(builder.Services);
        ConfigureViews(builder.Services);

        return builder.Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        string connectionString = "Host=10.0.2.2;Username=postgres;Password=postgres;Database=folklibrary;";
        services.AddFolkLibraryContext(connectionString);
        services.AddSingleton<INavigationService, NavigationService>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();
        services.AddTransient<ArtistViewModel>();
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddSingleton<MainPage>();
        services.AddTransient<ArtistPage>();
    }
}
