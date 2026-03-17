using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using Application.Features.Payrolls.Payroll.Handlers;
using Application.Features.Payrolls.Payroll.Commands;
using Application.Features.Payrolls.Payroll.DTOs;
using Application.Common.Interfaces;    
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Enums;
using Domain.Features.Payrolls.Entities;

// Alias para evitar conflicto de namespaces
using PayrollAggregate = global::Domain.Features.Payrolls.Aggregates.Payroll;

namespace HumanResource.UnitTests.Application.Features.Payrolls.Payroll
{
    public class ApprovedPayrollHandlerTests
    {
        private readonly Mock<IPayrollRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly ApprovedPayrollHandler _handler;

        public ApprovedPayrollHandlerTests()
        {
            _repositoryMock = new Mock<IPayrollRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<ICacheService>();
            _handler = new ApprovedPayrollHandler(_repositoryMock.Object, _unitOfWorkMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task Handle_WhenPayrollExistsAndIsCalculated_ShouldApproveSuccessfully()
        {
            // Arrange
            var payrollId = Guid.NewGuid();
            var command = new ApprovedPayrollCommand { Id = payrollId };
            
            var payroll = new PayrollAggregate(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-1));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                3000m,
                Guid.NewGuid()));
            
            payroll.MarkAsCalculated();
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(payrollId))
                .ReturnsAsync(payroll);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(payroll.Id);
            result.EmployeeId.Should().Be(payroll.EmployeeId);
            result.Status.Should().Be("Approved");
            payroll.Status.Should().Be(PayrollStatus.Approved);
            
            _repositoryMock.Verify(x => x.GetByIdAsync(payrollId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenPayrollDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var payrollId = Guid.NewGuid();
            var command = new ApprovedPayrollCommand { Id = payrollId };
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(payrollId))
                .ReturnsAsync((PayrollAggregate?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Payroll not found.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenPayrollIsAlreadyApproved_ShouldThrowException()
        {
            // Arrange
            var payrollId = Guid.NewGuid();
            var command = new ApprovedPayrollCommand { Id = payrollId };
            
            var payroll = new PayrollAggregate(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-1));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                3000m,
                Guid.NewGuid()));
            
            payroll.MarkAsCalculated();
            payroll.Approve();
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(payrollId))
                .ReturnsAsync(payroll);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Payroll is already approved.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenPayrollIsAlreadyPaid_ShouldThrowException()
        {
            // Arrange
            var payrollId = Guid.NewGuid();
            var command = new ApprovedPayrollCommand { Id = payrollId };
            
            var payroll = new PayrollAggregate(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-1));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                3000m,
                Guid.NewGuid()));
            
            payroll.MarkAsCalculated();
            payroll.Approve();
            payroll.MarkAsPaid(); 
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(payrollId))
                .ReturnsAsync(payroll);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Payroll is already been paid.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenPayrollIsInDraftState_ShouldThrowException()
        {
            // Arrange
            var payrollId = Guid.NewGuid();
            var command = new ApprovedPayrollCommand { Id = payrollId };
            
            var payroll = new PayrollAggregate(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-1));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                3000m,
                Guid.NewGuid()));
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(payrollId))
                .ReturnsAsync(payroll);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
            
            exception.Message.Should().Be("Payroll must be calculated before approval.");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenPayrollIsApproved_ShouldReturnCorrectResponseWithComponents()
        {
            // Arrange
            var payrollId = Guid.NewGuid();
            var command = new ApprovedPayrollCommand { Id = payrollId };
            
            var payroll = new PayrollAggregate(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-1));
            
            var component1 = new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                3000m,
                Guid.NewGuid());
            
            var component2 = new PayrollComponent(
                PayrollComponentType.Overtime,
                PayrollComponentCategory.Earning,
                "Overtime",
                500m,
                Guid.NewGuid());
            
            payroll.AddComponent(component1);
            payroll.AddComponent(component2);
            payroll.MarkAsCalculated();
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(payrollId))
                .ReturnsAsync(payroll);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Components.Should().HaveCount(2);
            
            result.Components.Should().Contain(c => 
                c.Type == "BaseSalary" && 
                c.Category == "Earning" && 
                c.Description == "Base Salary" && 
                c.Amount == 3000m);
            
            result.Components.Should().Contain(c => 
                c.Type == "Overtime" && 
                c.Category == "Earning" && 
                c.Description == "Overtime" && 
                c.Amount == 500m);
            
            result.GrossAmount.Should().Be(3500m);
            result.TotalDeductions.Should().Be(0m);
            result.NetAmount.Should().Be(3500m);
        }

        [Fact]
        public async Task Handle_WhenApprovalSuccessful_ShouldCallRepositoryAndUnitOfWork()
        {
            // Arrange
            var payrollId = Guid.NewGuid();
            var command = new ApprovedPayrollCommand { Id = payrollId };
            
            var payroll = new PayrollAggregate(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-30),
                DateTime.Now.AddDays(-1));
            
            payroll.AddComponent(new PayrollComponent(
                PayrollComponentType.BaseSalary,
                PayrollComponentCategory.Earning,
                "Base Salary",
                3000m,
                Guid.NewGuid()));
            
            payroll.MarkAsCalculated();
            
            _repositoryMock
                .Setup(x => x.GetByIdAsync(payrollId))
                .ReturnsAsync(payroll);

            // Act
            await _handler.Handle(command, CancellationToken.None); 

            // Assert
            _repositoryMock.Verify(x => x.GetByIdAsync(payrollId), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}