// MultiplyServer/Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "MultiplyServer is up");
app.MapGet("/multiply", (double a, double b) =>
{
    var result = a * b;
    return Results.Ok(new { result });
});

app.Run();
