using FolkLibrary.Interfaces;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddFolkDataLoader(this IServiceCollection services)
    {
        services.AddTransient<FolkDataLoader>();
        return services;
    }
    public static IServiceCollection AddFolkDataExporter(this IServiceCollection services)
    {
        services.AddTransient<FolkDataExporter>();
        return services;
    }

    public static IServiceCollection AddMp3Converter(this IServiceCollection services)
    {
        services.AddTransient<IMp3Converter, Mp3Converter>();
        return services;
    }

    public static IServiceCollection AddFolkDbContext(this IServiceCollection services, string connectionString)
    {
        //services.AddEntityFrameworkNamingConventions();
        services.AddDbContextFactory<FolkDbContext>(opts => opts.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }

    public static IServiceCollection AddFolkHttpClient(this IServiceCollection services, Action<HttpClient> configure)
    {
        services.AddHttpClient<IFolkHttpClient, FolkHttpClient>(configure);
        return services;
    }

    public static THost LoadDatabaseData<THost>(this THost host, bool overwrite = false) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var loader = scope.ServiceProvider.GetRequiredService<FolkDataLoader>();
        loader.LoadData(overwrite);
        loader.ValidateData();
        return host;
    }

    public static THost ExportDatabaseData<THost>(this THost host, string fileName) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var exporter = scope.ServiceProvider.GetRequiredService<FolkDataExporter>();
        exporter.WriteXlsx(fileName, overwrite: true);
        return host;
    }
}