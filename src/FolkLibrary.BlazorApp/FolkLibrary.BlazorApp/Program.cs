﻿using FolkLibrary.BlazorApp.Client.Services;
using FolkLibrary.BlazorApp.Components;
using FolkLibrary.ServiceDefaults;
using Microsoft.FluentUI.AspNetCore.Components;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpForwarderWithServiceDiscovery();

builder.Services.AddRefitClient<IFolkLibraryApi>()
    .ConfigureHttpClient(http => http.BaseAddress = new Uri(builder.Configuration["FolkLibrary:Url"]!));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddFluentUIComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(FolkLibrary.BlazorApp.Client._Imports).Assembly);

app.MapForwarder("/api/{**catchAll}", builder.Configuration["FolkLibrary:Url"]!, "/api/{**catchAll}");

app.Run();
