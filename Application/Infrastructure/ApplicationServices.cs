using Dapper;
using FluentValidation;
using FolkLibrary.Services;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Polly;
using Polly.Retry;

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

    public static async Task LoadDatabaseData<THost>(this THost host, string? folderName = null, bool validate = false, bool overwrite = false) where THost : IHost
    {
        using var scope = host.Services.CreateScope();
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 5,
            })
            .Build();

        await pipeline.ExecuteAsync(async cancellationToken =>
        {
            var dataSource = new NpgsqlDataSourceBuilder(
                new NpgsqlConnectionStringBuilder(scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>().ConnectionString)
                {
                    Database = "postgres"
                }.ConnectionString)
            .Build();

            await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
            try
            {
                await connection.ExecuteAsync("CREATE DATABASE folk_library;");

            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.DuplicateDatabase)
            {
                // Already exists.
            }
        });

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
