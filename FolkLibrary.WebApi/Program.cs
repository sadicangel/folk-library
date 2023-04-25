using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, opts) => opts.WriteTo.Console().MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning));

builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddSingleton<IFileProvider>(services => services.GetRequiredService<IWebHostEnvironment>().ContentRootFileProvider);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddFastEndpoints();
builder.Services.AddSwaggerDoc(opts =>
{
    opts.Title = "Folk Library API";
    opts.Version = "v1";
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseFastEndpoints(opts =>
{
    opts.Endpoints.Configurator = e => e.AllowAnonymous();
    opts.Serializer.Options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
});
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

await app.LoadDatabaseData(app.Configuration, overwrite: false);

app.Run();
