using FolkLibrary.Interfaces;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MediatR;
using System.Reflection;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using FolkLibrary.Profiles;
using FluentValidation;
using FolkLibrary.Behaviors;
using FolkLibrary.Dtos;

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
        services.AddScoped(typeof(IPostgresRepository<>), typeof(PostgresRepository<>));
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
        services.AddSingleton<IMongoClient>(new MongoClient(configuration.GetConnectionString("MongoDB")));
        services.AddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase("folklibrary"));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
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
        var mongodb = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
        mongodb.DropCollection(scope.ServiceProvider.GetRequiredService<IMongoRepository<ArtistDto>>().CollectionName);
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