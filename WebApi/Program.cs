using FolkLibrary.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.FileProviders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, opts) => opts.WriteTo.Console().MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning));

builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddSingleton<IFileProvider>(services => services.GetRequiredService<IWebHostEnvironment>().ContentRootFileProvider);

builder.Services.AddDomain();
builder.Services.AddApplication();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts => opts.SupportNonNullableReferenceTypes());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapArtistEndpoints();
app.MapAlbumEndpoints();
app.MapTrackEndpoints();

await app.LoadDatabaseData(folderName: /*"D:/Music/Folk"*/ null, validate: false, overwrite: false);

app.Run();
