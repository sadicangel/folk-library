using FolkLibrary.Filters;
using Microsoft.Extensions.FileProviders;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, opts) => opts.WriteTo.Console().MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning));

builder.Services.AddSingleton<IFileProvider>(services => services.GetRequiredService<IWebHostEnvironment>().ContentRootFileProvider);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers(opts => opts.Filters.Add<HttpExceptionFilter>())
    .AddJsonOptions(opts => opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SupportNonNullableReferenceTypes();
    opts.DescribeAllParametersInCamelCase();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.LoadDatabaseData(app.Configuration, overwrite: false);

app.Run();
