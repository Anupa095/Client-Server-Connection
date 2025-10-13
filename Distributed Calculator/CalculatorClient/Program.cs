using Grpc.Net.Client;
using CalculatorServer;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Hardcode port here (must match the server choice)
        // If server chooses 5002, client will connect to 5002
        string port = "5002"; // Change this if server chooses another port
        string address = $"http://localhost:{port}";

        using var channel = GrpcChannel.ForAddress(address);
        var client = new Calculator.CalculatorClient(channel);

        Console.WriteLine($"✅ Connected to server at {address}");
        Console.WriteLine("----------------------------------------");

        while (true)
        {
            Console.WriteLine("\n1. Square\n2. Cube\n3. Multiply\n4. Exit");
            Console.Write("Choose operation (1-4): ");
            string? op = Console.ReadLine();

            if (op == "4")
            {
                Console.WriteLine("👋 Exiting...");
                break;
            }

            Console.Write("Enter number: ");
            if (!int.TryParse(Console.ReadLine(), out int num1))
            {
                Console.WriteLine("❌ Invalid number, try again.");
                continue;
            }

            if (op == "3")
            {
                Console.Write("Enter number 2: ");
                if (!int.TryParse(Console.ReadLine(), out int num2))
                {
                    Console.WriteLine("❌ Invalid number, try again.");
                    continue;
                }

                var response = await client.SlowMultiplyAsync(new MultiplyRequest
                {
                    Number1 = num1,
                    Number2 = num2
                });

                Console.WriteLine($"🧩 Multiply Result: {response.Result}");
            }
            else if (op == "1")
            {
                var response = await client.SquareAsync(new CalculationRequest { Number = num1 });
                Console.WriteLine($"🧩 Square Result: {response.Result}");
            }
            else if (op == "2")
            {
                var response = await client.CubeAsync(new CalculationRequest { Number = num1 });
                Console.WriteLine($"🧩 Cube Result: {response.Result}");
            }
            else
            {
                Console.WriteLine("❌ Invalid operation. Try again.");
            }
        }
    }
}
