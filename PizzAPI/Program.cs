using Microsoft.EntityFrameworkCore;
using PizzAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<PizzaStoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PizzaStoreConnection")));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Add Documentation

//Iam missing some middleware like cors

app.UseAuthorization();
app.MapControllers();
app.Run();
