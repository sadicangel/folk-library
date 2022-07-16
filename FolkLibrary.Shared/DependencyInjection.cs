using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddFolkLibraryContext(this IServiceCollection services, Action<DbContextOptionsBuilder>? configure = null)
    {
        services.AddDbContextFactory<FolkLibraryContext>(configure);
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        return services;
    }
}