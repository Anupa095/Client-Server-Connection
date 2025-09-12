using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        Console.Write("Enter server IP (or press ENTER for localhost): ");
        string serverIp = Console.ReadLine() ?? ""; // Prevent CS8600 warning
        if (string.IsNullOrWhiteSpace(serverIp))
        {
            serverIp = "127.0.0.1"; // Default to localhost
        }

        Console.WriteLine($"🌐 Connecting to server at {serverIp}:5000...");

        try
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(serverIp, 5000);

            NetworkStream stream = client.GetStream();

            Console.Write("Enter message to send: ");
            string message = Console.ReadLine() ?? ""; // Prevent CS8600 warning
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
            Console.WriteLine($"📤 Sent: {message}");

            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"📥 Server replied: {response}");

            Console.WriteLine("Press ENTER to close connection.");
            Console.ReadLine();

            client.Close();
            Console.WriteLine("🔒 Disconnected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}
