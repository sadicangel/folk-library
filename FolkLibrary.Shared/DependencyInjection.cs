using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddFolkLibraryContext(this IServiceCollection services, Action<DbContextOptionsBuilder>? configure = null)
    {
        services.AddDbContextFactory<FolkLibraryContext>(configure);
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
    public static IServiceCollection AddFolkLibraryClient(this IServiceCollection services, Action<HttpClient> configure)
    {
        services.AddHttpClient<FolkLibraryClient>(configure);
        return services;
    }
}