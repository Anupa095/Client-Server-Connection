using CalculatorServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add gRPC
builder.Services.AddGrpc();

// Configure Kestrel to support HTTP/2 (required for gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http2); // default
    options.ListenLocalhost(5002, o => o.Protocols = HttpProtocols.Http2);
    options.ListenLocalhost(5003, o => o.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();
app.MapGrpcService<CalculatorService>();
app.MapGet("/", () => "Use a gRPC client to communicate with CalculatorService.");

// Ask user which port to use
Console.WriteLine("Select server port:");
Console.WriteLine("1. 5001");
Console.WriteLine("2. 5002");
Console.WriteLine("3. 5003");
Console.Write("Enter choice (1-3): ");

int choice;
while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
{
    Console.Write("Invalid choice. Enter 1, 2, or 3: ");
}

// Select actual port
int selectedPort = choice switch
{
    1 => 5001,
    2 => 5002,
    3 => 5003,
    _ => 5001
};

// Add only the selected URL
app.Urls.Clear();
app.Urls.Add($"http://localhost:{selectedPort}");

Console.WriteLine($"✅ Calculator gRPC Server running on http://localhost:{selectedPort}");
app.Run();
