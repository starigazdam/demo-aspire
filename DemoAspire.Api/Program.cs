using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureCosmosClient("todos");
builder.Services.AddSingleton<TodoRepository>();
builder.ConfigureFunctionsWebApplication();

var host = builder.Build();
using (var scope = host.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<TodoRepository>().InitializeAsync();
}

host.Run();

public sealed class Todos(TodoRepository todos)
{
    [Function("GetTodos")]
    public async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/todos")] HttpRequest request) =>
        new OkObjectResult(await todos.GetAllAsync());

    [Function("CreateTodo")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/todos")] HttpRequest request)
    {
        var todoRequest = await request.ReadFromJsonAsync<CreateTodo>();
        if (string.IsNullOrWhiteSpace(todoRequest?.Title))
        {
            return new BadRequestObjectResult(new { title = "Title is required." });
        }

        var todo = new Todo(Guid.NewGuid().ToString("N"), todoRequest.Title.Trim(), DateTimeOffset.UtcNow.ToString("O"));
        await todos.CreateAsync(todo);
        return new CreatedResult($"/api/todos/{todo.id}", todo);
    }
}

public sealed class TodoRepository(CosmosClient client)
{
    private Container? container;

    public async Task InitializeAsync()
    {
        var database = (await client.CreateDatabaseIfNotExistsAsync("appdb")).Database;
        container = (await database.CreateContainerIfNotExistsAsync("todos", "/id")).Container;
    }

    public async Task<IReadOnlyList<Todo>> GetAllAsync()
    {
        var result = new List<Todo>();
        using var iterator = GetContainer().GetItemQueryIterator<Todo>(
            new QueryDefinition("SELECT * FROM c ORDER BY c.createdAt"));

        while (iterator.HasMoreResults)
        {
            result.AddRange(await iterator.ReadNextAsync());
        }

        return result;
    }

    public Task CreateAsync(Todo todo) =>
        GetContainer().CreateItemAsync(todo, new PartitionKey(todo.id));

    private Container GetContainer() => container ?? throw new InvalidOperationException("Cosmos container is not initialized.");
}

public sealed record Todo(string id, string title, string createdAt);

public sealed record CreateTodo(string Title);
