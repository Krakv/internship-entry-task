using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TicTacToe.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tic-Tac-Toe API",
        Version = "v0.1",
        Description = "API для игры в крестики-нолики"
    });
});

var boardSize = int.Parse(builder.Configuration["GameSettings:BoardSize"] ?? "3");
builder.Services.AddSingleton(new GameFactory(boardSize));

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");
app.MapGet("/health", () => Results.Ok("Healthy"));
app.MapControllers();

app.UseHttpsRedirection();
await app.RunAsync();