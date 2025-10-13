using Microsoft.EntityFrameworkCore;
using UserServiceAPI.Data;
using UserServiceAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ Use SQL Server connection
builder.Services.AddDbContext<UserServiceAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserServiceAPIContext")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Swagger setup
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserServiceAPI v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ✅ Optional: Seed sample data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UserServiceAPIContext>();
    context.Database.EnsureCreated();

    if (!context.User.Any())
    {
        context.User.AddRange(
            new User { Name = "Alice", Email = "alice@example.com", Password = "secret123" },
            new User { Name = "Bob", Email = "bob@example.com", Password = "password456" }
        );
        context.SaveChanges();
    }
}

app.Run();
