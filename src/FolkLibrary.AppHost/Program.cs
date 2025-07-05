var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithPgWeb();

var database = postgres.AddDatabase("folk-library-db");

builder.AddProject<Projects.FolkLibrary_DataImporter>("folk-library-data-importer")
    .WithReference(database)
    .WaitFor(database)
    .WithExplicitStart()
    .ExcludeFromManifest();

var webApi = builder.AddProject<Projects.FolkLibrary_WebApi>("folk-library-web-api")
    .WithReference(database)
    .WaitFor(database);

var webApp = builder.AddProject<Projects.FolkLibrary_BlazorApp>("folk-library-blazor-app")
    .WithReference(webApi)
    .WithEnvironment("FolkLibrary__Url", webApi.GetEndpoint("https"))
    .WaitFor(webApi);

builder.Build().Run();
