using FluentValidation;
using Marten;
using Marten.Events.Projections;
using Marten.Services.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Weasel.Core;

namespace FolkLibrary.Infrastructure;

public static class DomainServices
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddValidatedOptions<PostgresOptions>();
        services.AddSingleton(provider => new NpgsqlDataSourceBuilder(provider.GetRequiredService<IOptions<PostgresOptions>>().Value.ConnectionString).Build());
        services.AddMarten(provider =>
        {
            var options = new StoreOptions
            {
                DatabaseSchemaName = "public",
                AutoCreateSchemaObjects = AutoCreate.All,
            };

            options.Connection(provider.GetRequiredService<IOptions<PostgresOptions>>().Value.ConnectionString);

            options.UseDefaultSerialization(
                serializerType: SerializerType.SystemTextJson,
                enumStorage: EnumStorage.AsString);

            options.Projections.Add<ArtistProjection>(ProjectionLifecycle.Inline);
            options.Schema.For<Artist>().Identity(a => a.ArtistId).UseOptimisticConcurrency(true);

            options.Projections.Add<AlbumProjection>(ProjectionLifecycle.Inline);
            options.Schema.For<Album>().Identity(a => a.AlbumId).UseOptimisticConcurrency(true);

            return options;
        })
            .UseLightweightSessions();
        services.AddValidatorsFromAssembly(typeof(DomainServices).Assembly, ServiceLifetime.Singleton);

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

    public static IConfigurationBuilder AddJsonObject<T>(this IConfigurationBuilder configuration, T options) where T : class, IHasConfigurationKey
    {
        var json = $$"""{"{{T.ConfigurationSectionKey}}":{{JsonNode.Parse(JsonSerializer.Serialize(options))}}}""";
        configuration.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)) { Position = 0 });
        return configuration;
    }
}
