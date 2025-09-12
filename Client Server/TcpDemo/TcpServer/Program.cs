using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Listen on all available network interfaces
        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();

        Console.WriteLine("✅ Server started. Waiting for connections on port 5000...");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine($"🔌 Client connected from {client.Client.RemoteEndPoint}");

            _ = HandleClientAsync(client); // Handle in background
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"📥 Received from client: {received}");

        string response = $"🧠 Server says: You said '{received}'";
        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);

        client.Close();
        Console.WriteLine("🔒 Client connection closed.\n");
    }
}
