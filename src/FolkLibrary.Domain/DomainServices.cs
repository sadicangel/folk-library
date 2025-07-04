using FolkLibrary.Domain.Albums;
using FolkLibrary.Domain.Artists;
using JasperFx;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Weasel.Core;

namespace FolkLibrary.Domain;

public static class DomainServices
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddSingleton(provider => NpgsqlDataSource.Create(provider.GetRequiredService<IConfiguration>().GetConnectionString("folk-library-db")!));
        services.AddMarten(provider =>
        {
            var options = new StoreOptions
            {
                DatabaseSchemaName = "public",
                AutoCreateSchemaObjects = AutoCreate.All,
            };

            options.Connection(provider.GetRequiredService<NpgsqlDataSource>());

            options.UseSystemTextJsonForSerialization(enumStorage: EnumStorage.AsString);

            options.Events.StreamIdentity = StreamIdentity.AsGuid;

            options.Projections.Add<ArtistProjection>(ProjectionLifecycle.Inline);
            options.Schema.For<Artist>().Identity(a => a.Id)
                .UseOptimisticConcurrency(true)
                .UseNumericRevisions(true);

            options.Projections.Add<AlbumProjection>(ProjectionLifecycle.Inline);
            options.Schema.For<Album>().Identity(a => a.Id)
                .UseOptimisticConcurrency(true)
                .UseNumericRevisions(true);

            options.Projections.Add(new ArtistViewProjection(), ProjectionLifecycle.Inline);

            return options;
        })
            .UseLightweightSessions()
            .ApplyAllDatabaseChangesOnStartup();

        return services;
    }
}
