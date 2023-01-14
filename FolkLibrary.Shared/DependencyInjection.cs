using EasyNetQ;
using FluentValidation;
using FolkLibrary;
using FolkLibrary.Artists;
using FolkLibrary.Behaviors;
using FolkLibrary.Interfaces;
using FolkLibrary.Profiles;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json.Nodes;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddEventPublisher();
        var callingAsm = Assembly.GetCallingAssembly();
        var currentAsm = Assembly.GetExecutingAssembly();
        services.AddAutoMapper(opts => opts.AddProfile(new MappingProfile(currentAsm, callingAsm)));
        services.AddMediatR(callingAsm, currentAsm);
        services.AddValidatorsFromAssembly(callingAsm);
        services.AddValidatorsFromAssembly(currentAsm);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFolkDbContext(configuration);
        services.AddMongoDb(configuration);
        services.AddFolkDataLoader();
        services.AddFolkDataValidator();
        services.AddFolkDataExporter();
        return services;
    }

    public static IServiceCollection AddEventPublisher(this IServiceCollection services)
    {
        return services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
    }

    public static IServiceCollection AddFolkDataLoader(this IServiceCollection services)
    {
        services.AddTransient<FolkDataLoader>();
        return services;
    }

    public static IServiceCollection AddFolkDataValidator(this IServiceCollection services)
    {
        services.AddTransient<FolkDataValidator>();
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

    public static IServiceCollection AddFolkDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddEntityFrameworkNamingConventions();
        services.AddDbContextFactory<FolkDbContext>(opts => opts.UseNpgsql(configuration.GetConnectionString("Postgres")).UseSnakeCaseNamingConvention());
        services.AddScoped<IAlbumEntityRepository, AlbumEntityRepository>();
        services.AddScoped<IArtistEntityRepository, ArtistEntityRepository>();
        return services;
    }

    private sealed class GuidAsStringRepresentationConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            if (memberMap.MemberType == typeof(Guid))
                memberMap.SetSerializer(new GuidSerializer(BsonType.String));
            else if (memberMap.MemberType == typeof(Guid?))
                memberMap.SetSerializer(new NullableSerializer<Guid>(new GuidSerializer(BsonType.String)));
        }
    }


    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        ConventionRegistry.Register("FolkLibrary", new ConventionPack
        {
            new CamelCaseElementNameConvention(),
            new IgnoreIfDefaultConvention(ignoreIfDefault: true),
            new EnumRepresentationConvention(BsonType.String),
            new GuidAsStringRepresentationConvention()
        },
        type => true);
        foreach (var idType in FolkExtensions.GetAllIdTypes())
            BsonSerializer.RegisterSerializer(idType, (IBsonSerializer)Activator.CreateInstance(typeof(IIdBsonSerializer<>).MakeGenericType(idType))!);
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase("folklibrary"));
        services.AddSingleton<IArtistDocumentRepository, ArtistDocumentRepository>();
        return services;
    }

    public static IServiceCollection AddFolkHttpClient(this IServiceCollection services, Action<HttpClient> configure)
    {
        services.AddHttpClient<IFolkHttpClient, FolkHttpClient>(configure);
        return services;
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
            {
                var fileProvider = scope.ServiceProvider.GetRequiredService<IFileProvider>();
                var definitionsJson = fileProvider.GetFileInfo("rabbitmq_definitions.json");
                using var stream = definitionsJson.CreateReadStream();
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();
                var definitions = JsonNode.Parse(json)!;
                using var bus = RabbitHutch.CreateBus(configuration.GetConnectionString("RabbitMq")).Advanced;
                foreach (var queue in definitions.Root["queues"]!.AsArray())
                    await bus.QueuePurgeAsync(queue!["name"]!.GetValue<string>());
                bus.Dispose();
            }
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            scope.ServiceProvider.GetRequiredService<IMongoDatabase>()
                .DropCollection(scope.ServiceProvider.GetRequiredService<IArtistDocumentRepository>().CollectionName);
            var loader = scope.ServiceProvider.GetRequiredService<FolkDataLoader>();
            await loader.LoadDataAsync();

            var validator = scope.ServiceProvider.GetRequiredService<FolkDataValidator>();
            await validator.ValidateDataAsync();
        }
    }

    public static THost ExportDatabaseData<THost>(this THost host, string fileName) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var exporter = scope.ServiceProvider.GetRequiredService<FolkDataExporter>();
        exporter.WriteXlsx(fileName, overwrite: true);
        return host;
    }
}