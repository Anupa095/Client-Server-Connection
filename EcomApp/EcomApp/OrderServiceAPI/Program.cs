using Microsoft.EntityFrameworkCore;
using OrderServiceAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core DbContext
builder.Services.AddDbContext<OrderServiceAPIContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("OrderServiceAPIContext")
        ?? throw new InvalidOperationException("Connection string 'OrderServiceAPIContext' not found.")));

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderServiceAPI v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
