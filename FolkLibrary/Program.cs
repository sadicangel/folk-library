using FolkLibrary;
using FolkLibrary.Commands.GraphQL;
using MediatR;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, opts) => opts.WriteTo.Console().MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning));

builder.Services.AddSingleton<IFileProvider>(services => services.GetRequiredService<IWebHostEnvironment>().ContentRootFileProvider);
builder.Services.AddFolkDataLoader();
builder.Services.AddFolkDataExporter();
builder.Services.AddFolkDbContext("Host=database;Username=postgres;Password=postgres;Database=folklibrary;");
builder.Services.AddAutoMapper(typeof(Program), typeof(IAssemblyMarker));
builder.Services.AddMediatR(typeof(Program), typeof(IAssemblyMarker));
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddSorting();
builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts => opts.SupportNonNullableReferenceTypes());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapGraphQL();
app.MapControllers();

app.LoadDatabaseData(overwrite: false);
app.ExportDatabaseData("artists.xlxs");

app.Run();
