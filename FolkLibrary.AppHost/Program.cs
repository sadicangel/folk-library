var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgresContainer("postgres")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithEnvironment("POSTGRES_DB", "folk_library")
    .WithVolumeMount("./docker_local/postgresql/data", "/var/lib/postgresql/data")
    .AddDatabase("folk_library");

//builder.AddContainer("pgadmin", "dpage/pgadmin4")
//    .WithReference(postgres)
//    .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "postgres@email.com")
//    .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "postgres")
//    //.WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "False")
//    //.WithEnvironment("PGADMIN_CONFIG_MASTER_PASSWORD_REQUIRED", "False")
//    .WithVolumeMount("./docker_local/pgadmin", "/var/lib/pgadmin")
//    .WithServiceBinding(80);

var webApi = builder.AddProject<Projects.FolkLibrary_WebApi>("web_api")
    .WithReference(postgres);

builder.AddProject<Projects.BlazorApp>("blazor_app")
    .WithReference(webApi);

builder.Build().Run();
