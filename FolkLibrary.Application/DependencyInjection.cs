using Docker.DotNet;
using Docker.DotNet.Models;
using FolkLibrary.Interfaces;
using FolkLibrary.Services;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ICountryInfoProvider, CountryInfoProvider>();
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
}
