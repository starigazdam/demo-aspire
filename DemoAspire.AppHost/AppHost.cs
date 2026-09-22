var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();
var database = postgres.AddDatabase("appdb");

var api = builder.AddProject<Projects.DemoAspire_Api>("api")
    .WithReference(database)
    .WaitFor(database);

builder.AddViteApp("frontend", "../frontend")
    .WithReference(api)
    .WaitFor(api)
    .PublishAsStaticWebsite("/api", api);

builder.Build().Run();
