using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos")
    .RunAsEmulator(container => 
    {
        // Use 'latest' tag (2.14.28.0) which has valid evaluation period
        // The 'stable' tag has an expired evaluation license
        container.WithImageTag("latest").WithDataVolume();
    });
var database = cosmos.AddCosmosDatabase("appdb");
var todos = database.AddContainer("todos", "/id");

builder.AddAzureContainerAppEnvironment("functions")
    .WithDashboard(false);

var api = builder.AddAzureFunctionsProject<Projects.DemoAspire_Api>("api")
    .WithEnvironment("FUNCTIONS_WORKER_RUNTIME", "dotnet-isolated")
    .WithReference(todos)
    .WaitFor(todos)
    .WithExternalHttpEndpoints();

var frontend = builder.AddViteApp("frontend", "../frontend")
    .WithEnvironment("services__api__http__0", api.GetEndpoint("http"))
    .WaitFor(api);

api.PublishWithContainerFiles(frontend, "/home/site/wwwroot/wwwroot");

builder.Build().Run();
