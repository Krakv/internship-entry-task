using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using TicTacToe.API.Models;
using TicTacToe.API.Repositories;
using TicTacToe.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Tic-Tac-Toe API",
        Version = "v1.1"
    });
});

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<GameFactory>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IMoveRepository, MoveRepository>();
builder.Services.AddScoped<IMoveService, MoveService>();

builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings"));

var redisHost = builder.Configuration["Redis:Host"] ?? "localhost:6379";

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(
        await ConnectionMultiplexer.ConnectAsync(redisHost));
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
}

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

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

public partial class Program { } 