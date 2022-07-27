using FolkLibrary;
using FolkLibrary.Commands.Albums;
using FolkLibrary.Commands.Artists;
using FolkLibrary.Commands.GraphQL;
using FolkLibrary.Commands.Tracks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFolkLibraryContext(opts => opts.UseSqlite("DataSource=db.sqlite"));

builder.Services.AddAutoMapper(typeof(Program), typeof(IAssemblyMarker));
builder.Services.AddMediatR(typeof(Program), typeof(IAssemblyMarker));

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddSorting();
builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // The default HSTS value is 30 days. You may want to
    // change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapGraphQL();
app.MapControllers();

app.MapFallbackToFile("index.html"); ;

app.Run();
