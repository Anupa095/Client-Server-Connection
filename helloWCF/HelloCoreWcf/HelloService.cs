using CoreWCF;

public class HelloService : IHelloService
{
    public string Hello(string name) => $"Hello, {name}! (from CoreWCF)";
}
