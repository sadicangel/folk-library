using FolkLibrary.Filters;
using FolkLibrary.GraphQL;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, opts) => opts.WriteTo.Console().MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning));

builder.Services.AddSingleton<IFileProvider>(services => services.GetRequiredService<IWebHostEnvironment>().ContentRootFileProvider);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddSorting();
builder.Services.AddControllers(opts =>
{
    opts.Filters.Add<HttpExceptionFilter>();
    //opts.ValueProviderFactories.Add(new CamelCaseQueryValueProviderFactory());
})
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

await app.LoadDatabaseData(app.Configuration, overwrite: false);
//app.ExportDatabaseData("artists.xlxs");

app.Run();
