// SubtractServer/Program.cs
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "SubtractServer is up");
app.MapGet("/subtract", (double a, double b) =>
{
    var result = a - b;
    return Results.Ok(new { result });
});

app.Run();
