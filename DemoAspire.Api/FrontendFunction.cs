using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace DemoAspire.Api;

public sealed class FrontendFunction
{
    private static readonly string WebRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");

    [Function("Frontend")]
    public IActionResult Get(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{*path}")] HttpRequest request,
        string? path)
    {
        if (path?.StartsWith("api/", StringComparison.OrdinalIgnoreCase) == true)
        {
            return new NotFoundResult();
        }

        var root = Path.GetFullPath(WebRoot) + Path.DirectorySeparatorChar;
        var file = Path.GetFullPath(Path.Combine(WebRoot, string.IsNullOrEmpty(path) ? "index.html" : path));
        if (!file.StartsWith(root, StringComparison.Ordinal) || !File.Exists(file))
        {
            file = Path.Combine(WebRoot, "index.html");
        }

        return File.Exists(file)
            ? new FileStreamResult(File.OpenRead(file), ContentType(file))
            : new NotFoundResult();
    }

    private static string ContentType(string path) => Path.GetExtension(path) switch
    {
        ".css" => "text/css",
        ".js" => "text/javascript",
        ".svg" => "image/svg+xml",
        ".json" => "application/json",
        _ => "text/html"
    };
}
