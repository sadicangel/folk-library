﻿using FolkLibrary.Artists;
using FolkLibrary.Database;
using FolkLibrary.Interfaces;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Postgres
        services.AddDbContextFactory<FolkDbContext>(opts => opts.UseNpgsql(configuration.GetConnectionString("Postgres")).UseSnakeCaseNamingConvention());
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();

        // MongoDB
        ConventionRegistry.Register("FolkLibrary", new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreIfDefaultConvention(ignoreIfDefault: true),
            new EnumRepresentationConvention(BsonType.String),
            new GuidAsStringRepresentationConvention()
        },
        type => true);
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase("folklibrary"));
        services.AddSingleton<IArtistViewRepository, ArtistViewRepository>();

        // Other
        services.AddTransient<FolkDataExporter>();
        services.AddTransient<FolkDataLoader>();
        services.AddTransient<FolkDataValidator>();

        services.AddTransient<IMp3Converter, Mp3Converter>();

        return services;
    }

    public static THost ExportDatabaseData<THost>(this THost host, string fileName) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var exporter = scope.ServiceProvider.GetRequiredService<FolkDataExporter>();
        exporter.WriteXlsx(fileName, overwrite: true);
        return host;
    }

    public static async Task LoadDatabaseData<THost>(this THost host, IConfiguration configuration, bool overwrite = false) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FolkDbContext>();
        var isEmpty = true;
        try
        {
            isEmpty = !await dbContext.Database.CanConnectAsync() || !await dbContext.Set<Artist>().AnyAsync();
        }
        catch (Exception)
        {

        }
        if (overwrite || isEmpty)
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            scope.ServiceProvider.GetRequiredService<IMongoDatabase>()
                .DropCollection(scope.ServiceProvider.GetRequiredService<IArtistViewRepository>().CollectionName);
            var loader = scope.ServiceProvider.GetRequiredService<FolkDataLoader>();
            await loader.LoadDataAsync();

            var validator = scope.ServiceProvider.GetRequiredService<FolkDataValidator>();
            await validator.ValidateDataAsync();
        }
    }
}

file sealed class GuidAsStringRepresentationConvention : ConventionBase, IMemberMapConvention
{
    public void Apply(BsonMemberMap memberMap)
    {
        if (memberMap.MemberType == typeof(Guid))
            memberMap.SetSerializer(new GuidSerializer(BsonType.String));
        else if (memberMap.MemberType == typeof(Guid?))
            memberMap.SetSerializer(new NullableSerializer<Guid>(new GuidSerializer(BsonType.String)));
    }
}
