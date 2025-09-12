using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;

var builder = WebApplication.CreateBuilder(args);

// Register CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// Register your service
builder.Services.AddSingleton<HelloService>();

var app = builder.Build();

// Configure SOAP endpoint
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<HelloService>();
    serviceBuilder.AddServiceEndpoint<HelloService, IHelloService>(
        new BasicHttpBinding(),
        "/HelloService.svc");
});

// Enable WSDL
var smb = app.Services.GetRequiredService<ServiceMetadataBehavior>();
smb.HttpGetEnabled = true;

// Use a fixed port (change if needed)
app.Urls.Add("http://localhost:5170");

app.Run();
