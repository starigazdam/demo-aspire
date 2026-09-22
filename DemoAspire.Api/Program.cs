using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureCosmosContainer("todos");
builder.ConfigureFunctionsWebApplication();

builder.Build().Run();

public sealed class Todos(Container todos)
{
    [Function("GetTodos")]
    public async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/todos")] HttpRequest request)
    {
        var result = new List<Todo>();
        using var iterator = todos.GetItemQueryIterator<Todo>(
            new QueryDefinition("SELECT * FROM c ORDER BY c.createdAt"));

        while (iterator.HasMoreResults)
        {
            result.AddRange(await iterator.ReadNextAsync());
        }

        return new OkObjectResult(result);
    }

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
        await todos.CreateItemAsync(todo, new PartitionKey(todo.id));
        return new CreatedResult($"/api/todos/{todo.id}", todo);
    }
}

public sealed record Todo(string id, string title, string createdAt);

public sealed record CreateTodo(string Title);
