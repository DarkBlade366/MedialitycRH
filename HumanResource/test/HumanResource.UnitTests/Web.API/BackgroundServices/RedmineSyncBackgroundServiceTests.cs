using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FluentAssertions;
using Application.Common.Interfaces;
using Web.API.BackgroundServices;

namespace HumanResource.UnitTests.Web.API.BackgroundServices
{
    public class RedmineSyncBackgroundServiceTests
    {
        private readonly Mock<IRedmineSyncJob> _jobMock;
        private readonly Mock<ILogger<RedmineSyncBackgroundService>> _loggerMock;
        private readonly IOptions<RedmineSyncScheduleOptions> _options;

        public RedmineSyncBackgroundServiceTests()
        {
            _jobMock = new Mock<IRedmineSyncJob>();
            _loggerMock = new Mock<ILogger<RedmineSyncBackgroundService>>();
            _options = Options.Create(new RedmineSyncScheduleOptions
            {
                Enabled = true,
                IntervalHours = 24,
                InitialDelaySeconds = 1
            });
        }

        [Fact]
        public async Task ExecuteAsync_WhenEnabled_ShouldCallJobAndRespectInterval()
        {
            // Arrange
            var service = new RedmineSyncBackgroundService(
                _loggerMock.Object,
                _jobMock.Object,
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
                _jobMock.Object,
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
