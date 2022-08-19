using FolkLibrary.Interfaces;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddFolkLibraryContext(this IServiceCollection services, string connectionString)
    {
        services.AddEntityFrameworkNamingConventions();
        services.AddDbContextFactory<FolkLibraryContext>(opts => opts.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
    public static IServiceCollection AddFolkLibraryClient(this IServiceCollection services, Action<HttpClient> configure)
    {
        services.AddHttpClient<IFolkLibraryClient, FolkLibraryClient>(configure);
        return services;
    }
}