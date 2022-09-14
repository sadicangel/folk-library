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
        services.AddFolkHttpClient(opts => opts.BaseAddress = new("https://localhost:52947"));
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
