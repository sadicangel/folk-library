using FolkLibrary.Domain;
using FolkLibrary.Domain.Artists;
using FolkLibrary.ServiceDefaults;
using Marten;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDomain();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/api/", () => "Hello, world!");
app.MapGet("/api/artists", (IDocumentSession documentSession) => documentSession.Query<ArtistView>().ToList());

app.Run();
