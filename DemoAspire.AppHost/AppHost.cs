using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddAzureSqlServer("sql")
    .RunAsContainer(container => container.WithDataVolume());
var database = sql.AddDatabase("appdb");

builder.AddAzureAppServiceEnvironment("functions");

var api = builder.AddAzureFunctionsProject<Projects.DemoAspire_Api>("api")
    .WithEnvironment("FUNCTIONS_WORKER_RUNTIME", "dotnet-isolated")
    .WithReference(database)
    .WaitFor(database)
    .WithExternalHttpEndpoints()
    .PublishAsAzureAppServiceWebsite((_, app) => app.Kind = "functionapp,linux");

var frontend = builder.AddViteApp("frontend", "../frontend")
    .WaitFor(api);

api.PublishWithContainerFiles(frontend, "/home/site/wwwroot/wwwroot");

builder.Build().Run();
