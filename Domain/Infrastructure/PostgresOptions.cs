using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace FolkLibrary.Infrastructure;

public sealed record class PostgresOptions(
    [property: Required] string Host,
    [property: Required] string Database,
    [property: Required] string Username,
    [property: Required] string Password,
    [property: Required, Range(1, 65535)] int Port)
    : IHasConfigurationKey
{
    private string? _connectionString;

    public PostgresOptions() : this("127.0.0.1", "postgres", "postgres", "postgres", 5432) { }

    public static string ConfigurationSectionKey { get => "Postgres"; }

    public string ConnectionString
    {
        get => _connectionString ??= new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Database = Database,
            Username = Username,
            Password = Password,
            Port = Port
        }.ConnectionString;
    }

    public static PostgresOptions FromConnectionString(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        return new PostgresOptions(
            builder.Host ?? throw new ArgumentException($"Invalid {nameof(Host)}", nameof(connectionString)),
            builder.Database ?? throw new ArgumentException($"Invalid {nameof(Database)}", nameof(connectionString)),
            builder.Username ?? throw new ArgumentException($"Invalid {nameof(Username)}", nameof(connectionString)),
            builder.Password ?? throw new ArgumentException($"Invalid {nameof(Password)}", nameof(connectionString)),
            builder.Port);
    }
}
