using Docker.DotNet;
using Docker.DotNet.Models;
using FolkLibrary.Services;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FolkLibrary.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ICountryInfoProvider, CountryInfoProvider>();
        services.AddTransient<IDataImporter, DataImporter>();
        services.AddTransient<IDataExporter, DataExporter>();
        services.AddTransient<IDataValidator, DataValidator>();
        services.AddTransient<IMp3Converter, Mp3Converter>();
        return services;
    }

    public static IServiceCollection AddFolkHttpClient(this IServiceCollection services, Action<HttpClient> configureClient)
    {
        services.AddHttpClient<IFolkHttpClient, FolkHttpClient>(configureClient);
        return services;
    }

    public static IServiceCollection AddFolkHttpClient(this IServiceCollection services, string httpClientName)
    {
        services.AddTransient<IFolkHttpClient>(provider => new FolkHttpClient(provider.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientName)));
        return services;
    }

    public static IConfigurationBuilder AddContainersConfiguration(this IConfigurationBuilder configuration, string hostname)
    {
        var dockerClient = new DockerClientConfiguration().CreateClient();
        var containers = Task.Run(async () => await dockerClient.Containers.ListContainersAsync(new ContainersListParameters { All = true })).Result;
        var connectionStrings = new Dictionary<string, string?>();
        foreach (var container in containers)
        {
            var ports = container.Ports.Where(p => p.PublicPort > 0).ToList();
            if (ports.Count > 0)
            {
                var port = ports.Count == 1
                    ? ports[0]
                    : ports.Find(p => p.PrivatePort == 443 /*https*/) ?? ports[0];
                var containerName = container.Names[0][1..];
                connectionStrings[$"ConnectionStrings:{containerName}"] = $"https://{hostname}:{port.PublicPort}";
            }
        }
        configuration.AddInMemoryCollection(connectionStrings);
        return configuration;
    }

    public static async Task LoadDatabaseData<THost>(this THost host, bool overwrite = false) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var documentStore = scope.ServiceProvider.GetRequiredService<IDocumentStore>();
        var statistics = await documentStore.Advanced.FetchEventStoreStatistics();
        var isEmpty = statistics.StreamCount == 0;
        if (overwrite || isEmpty)
        {
            await documentStore.Advanced.ResetAllData();
            statistics = await documentStore.Advanced.FetchEventStoreStatistics();
            if (statistics.StreamCount != 0)
                throw new InvalidOperationException("Could not reset database data");
            await scope.ServiceProvider.GetRequiredService<IDataImporter>().ImportAsync("D:/Music/Folk");
            await scope.ServiceProvider.GetRequiredService<IDataValidator>().ValidateAsync();
        }
    }
}
