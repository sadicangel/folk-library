using FolkLibrary.DataImporter;
using FolkLibrary.Domain;
using FolkLibrary.ServiceDefaults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDomain();
builder.Services.AddSingleton<ImportService>();

var app = builder.Build();

await app.StartAsync();

var importer = app.Services.GetRequiredService<ImportService>();

await importer.ImportAsync();

await app.StopAsync();
