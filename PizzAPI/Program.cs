using Microsoft.EntityFrameworkCore;
using PizzAPI.Models;
using PizzAPI.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<PizzaStoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PizzaStoreConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddScoped<IRedisService, RedisService>();


// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (example setup allowing all — adjust for production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();




// Enable Swagger only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAll");

// Optional: Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Authorization middleware
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

app.Run();
