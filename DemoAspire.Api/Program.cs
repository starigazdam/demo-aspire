using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDbContext<TodoDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("appdb")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<TodoDb>().Database.EnsureCreatedAsync();
}

app.MapGet("/api/todos", async (TodoDb db) =>
    await db.Todos.OrderBy(todo => todo.Id).ToListAsync());

app.MapPost("/api/todos", async (CreateTodo request, TodoDb db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [nameof(request.Title)] = ["Title is required."]
        });
    }

    var todo = new Todo { Title = request.Title.Trim() };
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

app.MapDefaultEndpoints();
app.Run();

sealed class TodoDb(DbContextOptions<TodoDb> options) : DbContext(options)
{
    public DbSet<Todo> Todos => Set<Todo>();
}

sealed class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
}

sealed record CreateTodo(string Title);
