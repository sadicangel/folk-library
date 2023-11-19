using FluentValidation;
using Marten;
using Marten.Events.Projections;
using Marten.Services.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace FolkLibrary.Infrastructure;

public static class DomainServices
{
    public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddNpgsqlDataSource(configuration.GetConnectionString("folk_library")!);
        services.AddMarten(provider =>
        {
            var options = new StoreOptions
            {
                DatabaseSchemaName = "public",
                AutoCreateSchemaObjects = AutoCreate.All,
            };

            options.Connection(configuration.GetConnectionString("folk_library")!);

            options.UseDefaultSerialization(
                serializerType: SerializerType.SystemTextJson,
                enumStorage: EnumStorage.AsString);

            options.Projections.Add<ArtistProjection>(ProjectionLifecycle.Inline);
            options.Schema.For<Artist>().Identity(a => a.ArtistId).UseOptimisticConcurrency(true);

            options.Projections.Add<AlbumProjection>(ProjectionLifecycle.Inline);
            options.Schema.For<Album>().Identity(a => a.AlbumId).UseOptimisticConcurrency(true);

            options.Projections.Add<TrackProjection>(ProjectionLifecycle.Inline);
            options.Schema.For<Track>().Identity(a => a.TrackId).UseOptimisticConcurrency(true);

            return options;
        })
            .UseLightweightSessions();
        services.AddValidatorsFromAssembly(typeof(DomainServices).Assembly, ServiceLifetime.Singleton);

        return services;
    }
}
