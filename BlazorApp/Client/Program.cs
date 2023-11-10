using FolkLibrary;
using FolkLibrary.Infrastructure;
using FolkLibrary.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddApplication();

builder.Services.AddTransient<AntiforgeryHandler>();
builder.Services.AddSingleton<ArtistStateProvider>();

builder.Services.AddHttpClient(nameof(IFolkHttpClient), client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<AntiforgeryHandler>();
builder.Services.AddFolkHttpClient(opts => opts.JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web));

await builder.Build().RunAsync();
