using Marten;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Weasel.Core;

namespace FolkLibrary.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddValidatedOptions<PostgresOptions>();
        services.AddSingleton(provider => new NpgsqlDataSourceBuilder(provider.GetRequiredService<PostgresOptions>().ConnectionString).Build());
        services.AddMarten(provider =>
        {
            var options = new StoreOptions
            {
                DatabaseSchemaName = "public",
                AutoCreateSchemaObjects = AutoCreate.All
            };

            options.Connection(provider.GetRequiredService<PostgresOptions>().ConnectionString);

            return options;
        });

        return services;
    }

    public static IServiceCollection AddValidatedOptions<T>(this IServiceCollection services) where T : class, IHasConfigurationKey
    {
        services.AddOptions<T>()
            .BindConfiguration(T.ConfigurationSectionKey)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
