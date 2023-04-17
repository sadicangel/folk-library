using EasyNetQ;
using FolkLibrary.Artists;
using FolkLibrary.Database;
using FolkLibrary.Repositories;
using FolkLibrary.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using System.Text.Json.Nodes;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
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
                var bus = scope.ServiceProvider.GetRequiredService<IAdvancedBus>();
                foreach (var queue in definitions.Root["queues"]!.AsArray())
                    await bus.QueuePurgeAsync(queue!["name"]!.GetValue<string>());
                bus.Dispose();
            }
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