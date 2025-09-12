using CoreWCF;

[ServiceContract]
public interface IHelloService
{
    [OperationContract]
    string Hello(string name);
}
