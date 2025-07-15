using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TicTacToe.API.Services;
using TicTacToe.IntegrationTests;

namespace TicTacToe.API.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IGameService> GameServiceMock { get; } = new();
        public Mock<IMoveService> MoveServiceMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.AddSingleton(GameServiceMock.Object);
                services.AddSingleton(MoveServiceMock.Object);
                services.RemoveAll(typeof(ICacheService));
                services.AddSingleton<ICacheService, InMemoryCacheService>();
            });
        }
    }
}
