﻿using Docker.DotNet;
using Docker.DotNet.Models;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FolkLibrary.Infrastructure;

public static class ApplicationServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ICountryInfoProvider, CountryInfoProvider>();
        services.AddSingleton<IUuidProvider, UuidProvider>();
        services.AddTransient<IDataImporter, DataImporter>();
        services.AddTransient<IDataExporter, DataExporter>();
        services.AddTransient<IDataValidator, DataValidator>();
        services.AddTransient<IMp3Converter, Mp3Converter>();
        services.AddMediatR(opts => opts.RegisterServicesFromAssemblyContaining(typeof(ApplicationServices)));
        services.AddValidatorsFromAssembly(typeof(ApplicationServices).Assembly, ServiceLifetime.Singleton);
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

    public static async Task LoadDatabaseData<THost>(this THost host, string? folderName = null, bool validate = false, bool overwrite = false) where THost : IHost
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
            await scope.ServiceProvider.GetRequiredService<IDataImporter>().ImportAsync(folderName);
        }
        if (validate || overwrite)
        {
            var validationResult = await scope.ServiceProvider.GetRequiredService<IDataValidator>().ValidateAsync();
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }
    }
}
