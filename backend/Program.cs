using backend.Data;
using backend.Services.Repositories.Recipe;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

Env.Load(".env");
var connectionString = Environment.GetEnvironmentVariable("FeastfulDatabase");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FeastfulDbContext>(options =>
    options.UseNpgsql(connectionString)); 

builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();