using System;
using ServiceReference1; // Matches the namespace in Reference.cs

class Program
{
    static void Main()
    {
        var client = new HelloServiceClient(
            HelloServiceClient.EndpointConfiguration.BasicHttpBinding_IHelloService);

        // Use the correct method name as defined in the generated proxy, e.g., HelloAsync
        var result = client.HelloAsync("Anupa Sandeepa").Result;
        Console.WriteLine(result);

        client.Close();
    }
}
