using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using FluentAssertions;
using Web.API.BackgroundServices;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Configuration;

namespace HumanResource.IntegrationTests.BackgroundServices
{
    public class RedmineSyncBackgroundServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly PostgreSqlContainer _postgresContainer;
        private WireMockServer? _mockRedmineServer;
        private IServiceScopeFactory? _scopeFactory;
        private string? _redmineBaseUrl;

        public RedmineSyncBackgroundServiceIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _postgresContainer = new PostgreSqlBuilder()
                .WithImage("postgres:15")
                .WithDatabase("testdb")
                .WithUsername("test")
                .WithPassword("test")
                .Build();
        }

        public async Task InitializeAsync()
        {
            // Iniciar contenedor PostgreSQL
            await _postgresContainer.StartAsync();

            // Iniciar servidor WireMock
            _mockRedmineServer = WireMockServer.Start();
            _redmineBaseUrl = _mockRedmineServer.Urls[0];

            // Configurar las respuestas mock de Redmine
            SetupMockRedmineResponses();

            // Crear un factory con configuración personalizada
            var factoryWithMock = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["Redmine:BaseUrl"] = _redmineBaseUrl,
                        ["Redmine:ApiKey"] = "fake-api-key",
                        ["ConnectionStrings:DbMedialitycHR"] = _postgresContainer.GetConnectionString()
                    });
                });
            });

            _scopeFactory = factoryWithMock.Services.GetRequiredService<IServiceScopeFactory>();

            // Aplicar migraciones a la base de datos de prueba
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                await dbContext.Database.MigrateAsync();
            }
        }

        public async Task DisposeAsync()
        {
            _mockRedmineServer?.Stop();
            if (_postgresContainer != null)
            {
                await _postgresContainer.StopAsync();
            }
        }

        [Fact]
        public async Task ExecuteAsync_ShouldSyncData_WhenEnabled()
        {
            // Arrange
            var options = Options.Create(new RedmineSyncScheduleOptions
            {
                Enabled = true,
                IntervalHours = 24,
                TimeEntryLookBackDays = 30
            });

            var logger = Mock.Of<ILogger<RedmineSyncBackgroundService>>();

            var service = new RedmineSyncBackgroundService(
                logger,
                _scopeFactory!,
                options);

            // Cancelamos después de 35 segundos para superar el delay inicial de 30s y ejecutar una iteración
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(35));

            // Act
            try
            {
                await service.StartAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Esperado
            }

            // Assert - Verificar que los datos se hayan insertado
            using (var scope = _scopeFactory!.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

                var projects = await dbContext.Projects.ToListAsync();
                projects.Should().NotBeEmpty();
                projects.Should().Contain(p => p.RedmineProjectId == 1 && p.Name == "Test Project");

                var employees = await dbContext.Employees.ToListAsync();
                employees.Should().Contain(e => e.Email == "test@example.com");

                var milestones = await dbContext.ProjectMilestones.ToListAsync();
                milestones.Should().Contain(m => m.Name == "v1.0");
            }
        }

        [Fact]
        public async Task ExecuteAsync_WhenDisabled_ShouldNotSync()
        {
            // Arrange
            var options = Options.Create(new RedmineSyncScheduleOptions
            {
                Enabled = false,
                IntervalHours = 24,
                TimeEntryLookBackDays = 30
            });

            var logger = Mock.Of<ILogger<RedmineSyncBackgroundService>>();

            var service = new RedmineSyncBackgroundService(
                logger,
                _scopeFactory!,
                options);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(35));

            // Act
            try
            {
                await service.StartAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Esperado
            }

            // Assert - No debería haber datos
            using (var scope = _scopeFactory!.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
                (await dbContext.Projects.CountAsync()).Should().Be(0);
                (await dbContext.Employees.CountAsync()).Should().Be(0);
            }
        }

        private void SetupMockRedmineResponses()
        {
            _mockRedmineServer!
                .Given(Request.Create().WithPath("/projects.json"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""projects"": [
                            {
                                ""id"": 1,
                                ""name"": ""Test Project"",
                                ""status"": 1
                            }
                        ]
                    }"));

            _mockRedmineServer
                .Given(Request.Create().WithPath("/users.json"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""users"": [
                            {
                                ""id"": 1,
                                ""login"": ""testuser"",
                                ""firstname"": ""Test"",
                                ""lastname"": ""User"",
                                ""mail"": ""test@example.com""
                            }
                        ]
                    }"));

            _mockRedmineServer
                .Given(Request.Create().WithPath("/projects/1/versions.json"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""versions"": [
                            {
                                ""id"": 1,
                                ""name"": ""v1.0"",
                                ""status"": ""open""
                            }
                        ]
                    }"));

            _mockRedmineServer
                .Given(Request.Create().WithPath("/time_entries.json"))
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(@"{
                        ""time_entries"": []
                    }"));
        }
    }
}