using Duende.Bff.Yarp;
using FolkLibrary.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddContainersConfiguration("localhost");
var webApiUrl = builder.Configuration.GetConnectionString("WebApi");
if (String.IsNullOrEmpty(webApiUrl))
    throw new InvalidOperationException("Invalid WebApi URL");

builder.Services.AddBff()
    .AddRemoteApis();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseBff();
app.UseAuthorization();

app.MapRemoteBffApiEndpoint("/api", $"{webApiUrl}/api")
     .WithOptionalUserAccessToken()
     .AllowAnonymous();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
