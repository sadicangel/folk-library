using FolkLibrary.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FolkLibrary;

public sealed class SharedFixture : IAsyncLifetime
{
    private const string Username = "postgres";
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public SharedFixture()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithUsername(Username)
            .Build();
    }

    public Task DisposeAsync() => _postgreSqlContainer.StopAsync();

    public Task InitializeAsync() => _postgreSqlContainer.StartAsync();

    public async Task<IHost> CreateHostAsync(Action<IServiceCollection>? configure = null, CancellationToken cancellationToken = default)
    {
        var database = Guid.NewGuid().ToString("N");
        var createResult = await _postgreSqlContainer.ExecScriptAsync($"CREATE DATABASE db_{database};", cancellationToken);
        Assert.True(String.IsNullOrEmpty(createResult.Stderr), createResult.Stderr);
        var permissionsResult = await _postgreSqlContainer.ExecScriptAsync($"GRANT ALL PRIVILEGES ON DATABASE db_{database} to {Username};", cancellationToken);
        Assert.True(String.IsNullOrEmpty(permissionsResult.Stderr), permissionsResult.Stderr);

        var host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((host, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:folk_library"] = _postgreSqlContainer.GetConnectionString()
            }))
            .ConfigureServices((host, services) =>
            {
                services.AddDomain(host.Configuration);
                services.AddApplication();
                configure?.Invoke(services);
            }).Build();

        await host.StartAsync(cancellationToken);

        return host;
    }
}
