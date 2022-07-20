using FolkLibrary;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFolkLibraryContext(opts => opts.UseSqlite("DataSource=db.sqlite"));

builder.Services.AddAutoMapper(typeof(Program), typeof(IAssemblyMarker));
builder.Services.AddMediatR(typeof(Program), typeof(IAssemblyMarker));

builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
