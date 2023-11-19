using FolkLibrary;
using FolkLibrary.Infrastructure;
using FolkLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Text;
using System.Text.Json;

var cancellationTokenSource = new CancellationTokenSource();

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.None);

builder.Services.AddDomain(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddMediatR(opts => opts.RegisterServicesFromAssemblyContaining(typeof(Program)));

builder.Services.AddHttpClient(nameof(IFolkHttpClient), client => client.BaseAddress = new Uri("https://localhost:7001/"));
builder.Services.AddFolkHttpClient(opts => opts.JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web));

builder.Services.AddSingleton(cancellationTokenSource);
builder.Services.AddSingleton<Parser>();
builder.Services.AddSingleton(provider =>
{
    if (Console.OutputEncoding != Encoding.UTF8)
        Console.OutputEncoding = Encoding.UTF8;
    return AnsiConsole.Console;
});
builder.Services.AddHostedService<ParserService>();

var app = builder.Build();

await app.RunAsync(cancellationTokenSource.Token);