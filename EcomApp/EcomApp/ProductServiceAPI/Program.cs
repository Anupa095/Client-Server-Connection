using Microsoft.EntityFrameworkCore;
using ProductServiceAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductServiceAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductServiceAPIContext")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductServiceAPI v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
