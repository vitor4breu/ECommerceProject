using Application.UseCases.Order.CreateOrder;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Testcontainers.MySql;
using Xunit;

namespace IntegrationTests.Controllers;

public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly MySqlContainer _mysqlContainer;

    public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;

        _mysqlContainer = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithUsername("root")
            .WithPassword("senha123")
            .WithDatabase("desafio")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _mysqlContainer.StartAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings:DesafioDb", _mysqlContainer.GetConnectionString());
    }

    public async Task DisposeAsync()
    {
        await _mysqlContainer.StopAsync();
        await _mysqlContainer.DisposeAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        var seedFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Controllers", "seed.sql");
        var seed = await File.ReadAllTextAsync(seedFilePath);
        var result = await _mysqlContainer.ExecScriptAsync(seed);
    }

    [Fact]
    public async Task CreateOrder_Should_Return_Created_Status()
    {
        var client = _factory.CreateClient();
        await SeedDatabaseAsync();

        var input = new CreateOrderInput(new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), [
                new (new Guid("6d73a1f6-2d8e-4f77-97e4-f7d6190e174c"), 70),
                new (new Guid("3e5c5a3f-c7b6-41a7-a0a3-b8e1f39d0ae5"), 2)
            ]);

        var response = await client.PostAsJsonAsync("/Orders/CreateOrder", input);
        var output = await response.Content.ReadFromJsonAsync<CreateOrderOutput>();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        output!.TotalPrice.Should().Be(7895m);
    }
}
