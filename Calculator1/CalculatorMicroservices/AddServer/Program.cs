// AddServer/Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "AddServer is up");
app.MapGet("/add", (double a, double b) =>
{
    var result = a + b;
    return Results.Ok(new { result });
});

app.Run();
