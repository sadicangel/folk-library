using FolkLibrary.Infrastructure;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddContainersConfiguration("localhost");
var webApiUrl = builder.Configuration.GetConnectionString("WebApi");
if (String.IsNullOrEmpty(webApiUrl))
    throw new InvalidOperationException("Invalid WebApi URL");

var cluster = new ClusterConfig
{
    ClusterId = $"cluster-{Guid.NewGuid():N}",
    Destinations = new Dictionary<string, DestinationConfig>
    {
        [$"destination-{Guid.NewGuid():N}"] = new DestinationConfig { Address = "https://localhost:7001" }
    }
};
var route = new RouteConfig
{
    RouteId = $"route-{Guid.NewGuid():N}",
    ClusterId = cluster.ClusterId,
    Match = new RouteMatch { Path = "/api/{**catch-all}" }
};

builder.Services.AddReverseProxy()
    .LoadFromMemory(new List<RouteConfig> { route }, new List<ClusterConfig> { cluster });
//builder.Services.AddBff()
//    .AddRemoteApis();

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
app.UseAuthorization();

//app.MapRemoteBffApiEndpoint("/api", $"{webApiUrl}/api")
//     .WithOptionalUserAccessToken()
//     .AllowAnonymous();
app.MapReverseProxy();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
