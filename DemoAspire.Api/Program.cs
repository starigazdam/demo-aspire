using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("appdb")));
builder.ConfigureFunctionsWebApplication();

var host = builder.Build();
using (var scope = host.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<TodoDb>().Database.EnsureCreatedAsync();
}

host.Run();

public sealed class Todos(TodoDb db)
{
    [Function("GetTodos")]
    public async Task<IActionResult> Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todos")] HttpRequest request) =>
        new OkObjectResult(await db.Todos.OrderBy(todo => todo.Id).ToListAsync());

    [Function("CreateTodo")]
    public async Task<IActionResult> Create(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todos")] HttpRequest request)
    {
        var todoRequest = await request.ReadFromJsonAsync<CreateTodo>();
        if (string.IsNullOrWhiteSpace(todoRequest?.Title))
        {
            return new BadRequestObjectResult(new { title = "Title is required." });
        }

        var todo = new Todo { Title = todoRequest.Title.Trim() };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();
        return new CreatedResult($"/api/todos/{todo.Id}", todo);
    }
}

public sealed class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}

public sealed class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
}

public sealed record CreateTodo(string Title);
