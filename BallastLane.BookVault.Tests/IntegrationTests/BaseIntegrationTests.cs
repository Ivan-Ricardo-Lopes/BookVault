using AutoFixture;
using BallastLane.BookVault.Api;
using BallastLane.BookVault.Api.Jwt;
using BallastLane.BookVault.Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Text;
using System.Text.Json;
using Testcontainers.PostgreSql;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BallastLane.BookVault.Tests.IntegrationTests
{
    [TestFixture]
    public abstract class BaseIntegrationTests 
    {
        protected readonly Fixture _fixture = new ();
        protected WebApplicationFactory<Program> _factory;
        protected PostgreSqlContainer _container;
        protected string _accessToken;
        protected readonly JsonSerializerOptions _jsonOptions = new ()
        {
            PropertyNameCaseInsensitive = true
        };

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _container = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("Bookvault")
            .WithUsername("admin")
            .WithPassword("BallastLane!123")
            .Build();

            await _container.StartAsync();

            var connectionString = _container.GetConnectionString();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddScoped<ConnectionStringAccessor>(_ => new ConnectionStringAccessor(connectionString));
                    });

                });

            using var scope = _factory.Services.CreateScope();

            var serviceProvider = scope.ServiceProvider;

            var jwtTokenGenerator = serviceProvider.GetRequiredService<JwtTokenGenerator>();

            _accessToken = $"Bearer {jwtTokenGenerator.GenerateToken("testUser")}";

            var scriptsFolder = Path.Combine(AppContext.BaseDirectory, "Database", "Scripts");
            var scripts = GetSqlScriptsFromFolder(scriptsFolder);

            await ExecuteSqlScriptsAsync(connectionString, scripts);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _container.StopAsync();
            await _container.DisposeAsync();
            await _factory.DisposeAsync();
        }

        protected HttpClient CreateClient()
        {
            var client = _factory.CreateClient();

            client.DefaultRequestHeaders.Add("Authorization", _accessToken);

            return client;
        }

        protected static StringContent CreateContent(object data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            return content;
        }

        protected static List<string> GetSqlScriptsFromFolder(string folderPath)
        {
            var scripts = new List<string>();
            var scriptFiles = Directory.GetFiles(folderPath, "*.sql");

            foreach (var file in scriptFiles)
            {
                var script = File.ReadAllText(file);
                scripts.Add(script);
            }

            return scripts;
        }

        protected static async Task ExecuteSqlScriptsAsync(string connectionString, IEnumerable<string> scripts)
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            foreach (var script in scripts)
            {
                await using var command = new NpgsqlCommand(script, connection);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
