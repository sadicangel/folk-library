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
    public static string ConfigurationSectionKey { get => "Postgres"; }

    private string? _connectionString;

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


}
