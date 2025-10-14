using CalculatorServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var ports = new[] { 5001, 5002, 5003 };
        var tasks = new List<Task>();

        foreach (var port in ports)
        {
            tasks.Add(Task.Run(() =>
            {
                var builder = WebApplication.CreateBuilder();

                builder.Services.AddGrpc();
                builder.WebHost.ConfigureKestrel(options =>
                {
                    options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
                });

                var app = builder.Build();
                app.MapGrpcService<CalculatorService>();
                app.MapGet("/", () => $"✅ Calculator gRPC Server running on http://localhost:{port}");

                Console.WriteLine($"✅ Calculator gRPC Server running on http://localhost:{port}");
                app.Run();
            }));
        }

        Console.WriteLine("🚀 All 3 Calculator Servers started!");
        Console.WriteLine("Press ENTER to stop all servers...");
        Console.ReadLine();
    }
}
