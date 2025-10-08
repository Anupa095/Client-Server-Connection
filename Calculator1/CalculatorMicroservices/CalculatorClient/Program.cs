// CalculatorClient/Program.cs
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using var http = new HttpClient();

        Console.WriteLine("Calculator client (type +, -, * or exit)");
        while (true)
        {
            Console.Write("Operation (+ - * or exit): ");
            var op = Console.ReadLine()?.Trim();
            if (string.Equals(op, "exit", StringComparison.OrdinalIgnoreCase))
                break;

            Console.Write("Enter first number: ");
            if (!double.TryParse(Console.ReadLine(), out var a))
            {
                Console.WriteLine("Invalid number");
                continue;
            }

            Console.Write("Enter second number: ");
            if (!double.TryParse(Console.ReadLine(), out var b))
            {
                Console.WriteLine("Invalid number");
                continue;
            }

            // ✅ fixed nullable type
            string? url = op switch
            {
                "+" => $"http://localhost:5001/add?a={a}&b={b}",
                "-" => $"http://localhost:5002/subtract?a={a}&b={b}",
                "*" => $"http://localhost:5003/multiply?a={a}&b={b}",
                _ => null
            };

            if (url == null)
            {
                Console.WriteLine("Unknown operation");
                continue;
            }

            try
            {
                var res = await http.GetAsync(url);
                res.EnsureSuccessStatusCode();
                var json = await res.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(json);
                var value = doc.RootElement.GetProperty("result").GetDouble();

                Console.WriteLine($"Result: {value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling service: {ex.Message}");
            }
        }
    }
}
