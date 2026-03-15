using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Services;
using Domain.Features.TimeEntries.Interfaces;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Entities;

namespace HumanResource.UnitTests.Application.Services
{
    public class ProductivityServiceTests
    {
        private readonly Mock<ITimeEntryRepository> _timeEntryRepositoryMock;
        private readonly Mock<IActivityProductivityWeightRepository> _activityWeightRepositoryMock;
        private readonly ProductivityService _service;

        public ProductivityServiceTests()
        {
            _timeEntryRepositoryMock = new Mock<ITimeEntryRepository>();
            _activityWeightRepositoryMock = new Mock<IActivityProductivityWeightRepository>();
            _service = new ProductivityService(
                _timeEntryRepositoryMock.Object,
                _activityWeightRepositoryMock.Object);
        }

        [Fact]
        public async Task CalculateProductivityMetric_WhenNoTimeEntries_ShouldReturnZero()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now;

            _timeEntryRepositoryMock
                .Setup(x => x.GetHoursByActivityAsync(employeeId, periodStart, periodEnd))
                .ReturnsAsync(new Dictionary<int, decimal>());

            // Act
            var result = await _service.CalculateProductivityMetric(employeeId, periodStart, periodEnd);

            // Assert
            result.Should().Be(0m);
            _activityWeightRepositoryMock.Verify(x => x.GetAllAsync(), Times.Never);
        }

        [Fact]
        public async Task CalculateProductivityMetric_WithTimeEntriesAndNoWeights_ShouldUseDefaultWeight()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now;

            var hoursByActivity = new Dictionary<int, decimal>
            {
                { 10, 40.5m },
                { 11, 20.0m },
                { 12, 15.5m }
            };

            _timeEntryRepositoryMock
                .Setup(x => x.GetHoursByActivityAsync(employeeId, periodStart, periodEnd))
                .ReturnsAsync(hoursByActivity);

            _activityWeightRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ActivityProductivityWeight>());

            // Act
            var result = await _service.CalculateProductivityMetric(employeeId, periodStart, periodEnd);

            // Assert
            result.Should().Be(76.0m); 
        }

        [Fact]
        public async Task CalculateProductivityMetric_WithTimeEntriesAndWeights_ShouldApplyWeights()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now;

            var hoursByActivity = new Dictionary<int, decimal>
            {
                { 10, 40.5m },
                { 11, 20.0m },
                { 12, 15.5m }
            };

            var weights = new List<ActivityProductivityWeight>
            {
                new ActivityProductivityWeight(10, "Development", 0.8m),
                new ActivityProductivityWeight(11, "Testing", 0.6m),
                new ActivityProductivityWeight(13, "Documentation", 0.7m)
            };

            _timeEntryRepositoryMock
                .Setup(x => x.GetHoursByActivityAsync(employeeId, periodStart, periodEnd))
                .ReturnsAsync(hoursByActivity);

            _activityWeightRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(weights);

            // Act
            var result = await _service.CalculateProductivityMetric(employeeId, periodStart, periodEnd);

            // Assert
            result.Should().Be(59.9m);
        }

        [Fact]
        public async Task CalculateProductivityMetric_WithInactiveWeights_ShouldIgnoreInactiveWeights()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now;

            var hoursByActivity = new Dictionary<int, decimal>
            {
                { 10, 40.5m },
                { 11, 20.0m }
            };

            var weights = new List<ActivityProductivityWeight>
            {
                new ActivityProductivityWeight(10, "Development", 0.8m),
                new ActivityProductivityWeight(11, "Testing", 0.6m)
            };

            weights[0].Deactivate();

            _timeEntryRepositoryMock
                .Setup(x => x.GetHoursByActivityAsync(employeeId, periodStart, periodEnd))
                .ReturnsAsync(hoursByActivity);

            _activityWeightRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(weights);

            // Act
            var result = await _service.CalculateProductivityMetric(employeeId, periodStart, periodEnd);

            // Assert
            result.Should().Be(52.5m);
        }

        [Fact]
        public async Task CalculateProductivityMetric_WithActivityIdZero_ShouldUseDefaultWeight()
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now;

            var hoursByActivity = new Dictionary<int, decimal>
            {
                { 0, 30.0m }, 
                { 10, 20.0m }
            };

            var weights = new List<ActivityProductivityWeight>
            {
                new ActivityProductivityWeight(10, "Development", 0.9m)
            };

            _timeEntryRepositoryMock
                .Setup(x => x.GetHoursByActivityAsync(employeeId, periodStart, periodEnd))
                .ReturnsAsync(hoursByActivity);

            _activityWeightRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(weights);

            // Act
            var result = await _service.CalculateProductivityMetric(employeeId, periodStart, periodEnd);

            // Assert
            result.Should().Be(48.0m); 
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task CalculateProductivityMetric_WithMultipleActivities_ShouldCalculateCorrectly(int activityCount)
        {
            // Arrange
            var employeeId = Guid.NewGuid();
            var periodStart = DateTime.Now.AddDays(-30);
            var periodEnd = DateTime.Now;

            var hoursByActivity = new Dictionary<int, decimal>();
            var weights = new List<ActivityProductivityWeight>();

            for (int i = 1; i <= activityCount; i++)
            {
                hoursByActivity[i] = 10.0m * i;
                weights.Add(new ActivityProductivityWeight(i, $"Activity{i}", 0.5m + (0.05m * i)));
            }

            _timeEntryRepositoryMock
                .Setup(x => x.GetHoursByActivityAsync(employeeId, periodStart, periodEnd))
                .ReturnsAsync(hoursByActivity);

            _activityWeightRepositoryMock
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(weights);

            // Act
            var result = await _service.CalculateProductivityMetric(employeeId, periodStart, periodEnd);

            // Assert
            decimal expected = 0m;
            for (int i = 1; i <= activityCount; i++)
            {
                expected += (10.0m * i) * (0.5m + (0.05m * i));
            }

            result.Should().Be(expected);
        }
    }
}
