var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddAzureSqlServer("sql")
    .RunAsContainer(container => container.WithDataVolume());
var database = sql.AddDatabase("appdb");

var api = builder.AddProject<Projects.DemoAspire_Api>("api")
    .WithReference(database)
    .WaitFor(database);

builder.AddViteApp("frontend", "../frontend")
    .WithReference(api)
    .WaitFor(api)
    .PublishAsStaticWebsite("/api", api);

builder.Build().Run();
