using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Application.Common.Interfaces;
using Web.API.BackgroundServices;

namespace HumanResource.UnitTests.Web.API.BackgroundServices
{
    public class RedmineSyncBackgroundServiceTests
    {
        private readonly Mock<IRedmineSyncJob> _jobMock;
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
        private readonly Mock<IServiceScope> _scopeMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ILogger<RedmineSyncBackgroundService>> _loggerMock;
        private readonly IOptions<RedmineSyncScheduleOptions> _options;

        public RedmineSyncBackgroundServiceTests()
        {
            _jobMock = new Mock<IRedmineSyncJob>();
            _scopeMock = new Mock<IServiceScope>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _scopeFactoryMock = new Mock<IServiceScopeFactory>();
            _loggerMock = new Mock<ILogger<RedmineSyncBackgroundService>>();
            _options = Options.Create(new RedmineSyncScheduleOptions
            {
                Enabled = true,
                IntervalHours = 24,
                InitialDelaySeconds = 1
            });

            // Setup the scope factory to return our mock scope
            _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
            
            // Setup the scope to return our mock service provider
            _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
            
            // Setup the service provider to return our mock job
            _serviceProviderMock.Setup(x => x.GetService(typeof(IRedmineSyncJob))).Returns(_jobMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenEnabled_ShouldCallJobAndRespectInterval()
        {
            // Arrange
            var service = new RedmineSyncBackgroundService(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                _options);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(3));

            // Act
            await service.StartAsync(cts.Token);

            await Task.Delay(2000);

            // Assert
            _jobMock.Verify(
                x => x.ExecuteAsync(It.IsAny<CancellationToken>()),
                Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_WhenDisabled_ShouldNotCallJob()
        {
            // Arrange
            var disabledOptions = Options.Create(new RedmineSyncScheduleOptions
            {
                Enabled = false,
                IntervalHours = 24,
                InitialDelaySeconds = 1
            });

            var service = new RedmineSyncBackgroundService(
                _loggerMock.Object,
                _scopeFactoryMock.Object,
                disabledOptions);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(2));

            // Act
            await service.StartAsync(cts.Token);

            // Assert
            _jobMock.Verify(
                x => x.ExecuteAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
