using System;
using Domain.Features.Employees.Aggregates;
using Domain.Features.Employees.Enums;
using Domain.Features.Employees.Entities;
using Xunit;
using FluentAssertions;

namespace Domain.Features.Employees
{
    public class EmployeeTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateEmployee()
        {
            // Arrange
            var fullName = "John Doe";
            var email = "john.doe@company.com";
            var role = EmployeeRole.Employee;
            var passwordHash = "hashedPassword123";
            var redmineUserId = 12345;

            // Act
            var employee = new Employee(fullName, email, role, passwordHash, redmineUserId);

            // Assert
            employee.Id.Should().NotBeEmpty();
            employee.FullName.Should().Be(fullName);
            employee.Email.Should().Be(email);
            employee.Role.Should().Be(role);
            employee.PasswordHash.Should().Be(passwordHash);
            employee.RedmineUserId.Should().Be(redmineUserId);
            employee.IsActive.Should().BeTrue();
            employee.AguinaldoBalance.Should().NotBeNull();
            employee.VacationBalance.Should().NotBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyFullName_ShouldThrowArgumentException(string invalidName)
        {
            // Arrange
            var email = "john.doe@company.com";
            var role = EmployeeRole.Employee;
            var passwordHash = "hashedPassword123";
            var redmineUserId = 12345;

            // Act
            Action act = () => new Employee(invalidName, email, role, passwordHash, redmineUserId);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Full name is required*");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Arrange
            var fullName = "John Doe";
            var role = EmployeeRole.Employee;
            var passwordHash = "hashedPassword123";
            var redmineUserId = 12345;

            // Act
            Action act = () => new Employee(fullName, invalidEmail, role, passwordHash, redmineUserId);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Email is required*");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithEmptyPasswordHash_ShouldThrowArgumentException(string invalidPasswordHash)
        {
            // Arrange
            var fullName = "John Doe";
            var email = "john.doe@company.com";
            var role = EmployeeRole.Employee;
            var redmineUserId = 12345;

            // Act
            Action act = () => new Employee(fullName, email, role, invalidPasswordHash, redmineUserId);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Password hash is required*");
        }

        [Fact]
        public void Constructor_ShouldTrimAndNormalizeEmail()
        {
            // Arrange
            var fullName = "John Doe";
            var email = "  JOHN.DOE@COMPANY.COM  ";
            var role = EmployeeRole.Employee;
            var passwordHash = "hashedPassword123";
            var redmineUserId = 12345;

            // Act
            var employee = new Employee(fullName, email, role, passwordHash, redmineUserId);

            // Assert
            employee.Email.Should().Be("john.doe@company.com");
        }

        [Fact]
        public void Constructor_ShouldTrimFullName()
        {
            // Arrange
            var fullName = "  John Doe  ";
            var email = "john.doe@company.com";
            var role = EmployeeRole.Employee;
            var passwordHash = "hashedPassword123";
            var redmineUserId = 12345;

            // Act
            var employee = new Employee(fullName, email, role, passwordHash, redmineUserId);

            // Assert
            employee.FullName.Should().Be("John Doe");
        }

        [Fact]
        public void ChangeStatus_WhenDifferentStatus_ShouldUpdateStatus()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var originalStatus = employee.IsActive;

            // Act
            employee.ChangeStatus(false);

            // Assert
            employee.IsActive.Should().BeFalse();
            employee.IsActive.Should().NotBe(originalStatus);
        }

        [Fact]
        public void ChangeStatus_WhenSameStatus_ShouldNotUpdateStatus()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var originalStatus = employee.IsActive;

            // Act
            employee.ChangeStatus(originalStatus);

            // Assert
            employee.IsActive.Should().Be(originalStatus);
        }

        [Fact]
        public void Update_WithValidParameters_ShouldUpdateEmployee()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var newFullName = "Jane Smith";
            var newEmail = "jane.smith@company.com";
            var newRole = EmployeeRole.ProjectManager;

            // Act
            employee.Update(newFullName, newEmail, newRole);

            // Assert
            employee.FullName.Should().Be(newFullName);
            employee.Email.Should().Be(newEmail);
            employee.Role.Should().Be(newRole);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithEmptyFullName_ShouldThrowArgumentException(string invalidName)
        {
            // Arrange
            var employee = CreateValidEmployee();
            var newEmail = "jane.smith@company.com";
            var newRole = EmployeeRole.ProjectManager;

            // Act
            Action act = () => employee.Update(invalidName, newEmail, newRole);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Full name is required*");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WithEmptyEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Arrange
            var employee = CreateValidEmployee();
            var newFullName = "Jane Smith";
            var newRole = EmployeeRole.ProjectManager;

            // Act
            Action act = () => employee.Update(newFullName, invalidEmail, newRole);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Email is required*");
        }

        [Fact]
        public void Update_ShouldTrimAndNormalizeEmail()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var newFullName = "Jane Smith";
            var newEmail = "  JANE.SMITH@COMPANY.COM  ";
            var newRole = EmployeeRole.ProjectManager;

            // Act
            employee.Update(newFullName, newEmail, newRole);

            // Assert
            employee.Email.Should().Be("jane.smith@company.com");
        }

        [Fact]
        public void Update_ShouldUpdateUpdatedAt()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var beforeUpdate = employee.UpdatedAt;
            System.Threading.Thread.Sleep(10); // Pequeña pausa

            // Act
            employee.Update("New Name", "new@email.com", EmployeeRole.Employee);

            // Assert
            employee.UpdatedAt.Should().NotBe(beforeUpdate);
            employee.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void SetRedmineUserId_WithValidId_ShouldUpdateRedmineUserId()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var newRedmineUserId = 67890;

            // Act
            employee.SetRedmineUserId(newRedmineUserId);

            // Assert
            employee.RedmineUserId.Should().Be(newRedmineUserId);
        }

        [Fact]
        public void SetRedmineUserId_WithZeroId_ShouldUpdateRedmineUserId()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var newRedmineUserId = 0;

            // Act
            employee.SetRedmineUserId(newRedmineUserId);

            // Assert
            employee.RedmineUserId.Should().Be(0);
        }

        [Fact]
        public void AccrueAguinaldo_WithPositiveAmount_ShouldIncreaseBalance()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var initialBalance = employee.AguinaldoBalance.AccruedAmount;
            var amount = 1000m;

            // Act
            employee.AccrueAguinaldo(amount);

            // Assert
            employee.AguinaldoBalance.AccruedAmount.Should().Be(initialBalance + amount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void AccrueAguinaldo_WithZeroOrNegativeAmount_ShouldThrowArgumentException(decimal invalidAmount)
        {
            // Arrange
            var employee = CreateValidEmployee();

            // Act
            Action act = () => employee.AccrueAguinaldo(invalidAmount);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Amount must be positive*");
        }

        [Fact]
        public void PayAguinaldo_WithPositiveBalance_ShouldReturnPaymentAmountAndResetBalance()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueAguinaldo(1000m);
            employee.AccrueAguinaldo(500m);
            var expectedPayment = 1500m;

            // Act
            var payment = employee.PayAguinaldo();

            // Assert
            payment.Should().Be(expectedPayment);
            employee.AguinaldoBalance.AccruedAmount.Should().Be(0);
            employee.AguinaldoBalance.PaidAmount.Should().Be(expectedPayment);
        }

        [Fact]
        public void PayAguinaldo_WithZeroBalance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = CreateValidEmployee();

            // Act
            Action act = () => employee.PayAguinaldo();

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("*No aguinaldo to pay*");
        }

        [Fact]
        public void AccrueVacationDays_WithPositiveDays_ShouldIncreaseBalance()
        {
            // Arrange
            var employee = CreateValidEmployee();
            var initialBalance = employee.VacationBalance.AccruedDays;
            var days = 5;

            // Act
            employee.AccrueVacationDays(days);

            // Assert
            employee.VacationBalance.AccruedDays.Should().Be(initialBalance + days);
            employee.VacationBalance.LastAccrualDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void AccrueVacationDays_WithZeroOrNegativeDays_ShouldThrowArgumentException(decimal invalidDays)
        {
            // Arrange
            var employee = CreateValidEmployee();

            // Act
            Action act = () => employee.AccrueVacationDays(invalidDays);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Days must be positive*");
        }

        [Fact]
        public void UseVacationDays_WithValidDays_ShouldDecreaseAvailableDays()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueVacationDays(10);
            var initialAvailable = employee.VacationBalance.AvailableDays;
            var daysToUse = 3;

            // Act
            employee.UseVacationDays(daysToUse);

            // Assert
            employee.VacationBalance.AvailableDays.Should().Be(initialAvailable - daysToUse);
            employee.VacationBalance.UsedDays.Should().Be(daysToUse);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void UseVacationDays_WithZeroOrNegativeDays_ShouldThrowArgumentException(decimal invalidDays)
        {
            // Arrange
            var employee = CreateValidEmployee();

            // Act
            Action act = () => employee.UseVacationDays(invalidDays);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Days must be positive*");
        }

        [Fact]
        public void UseVacationDays_WithMoreThanAvailable_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueVacationDays(5);

            // Act
            Action act = () => employee.UseVacationDays(10);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("*Not enough vacation balance*");
        }

        [Fact]
        public void HasAccruedThisMonth_WithNoLastAccrualDate_ShouldReturnFalse()
        {
            // Arrange
            var employee = CreateValidEmployee();

            // Act
            var result = employee.VacationBalance.HasAccruedThisMonth();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void HasAccruedThisMonth_WithCurrentMonthAccrual_ShouldReturnTrue()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueVacationDays(1);

            // Act
            var result = employee.VacationBalance.HasAccruedThisMonth();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasAccruedThisMonth_WithPreviousMonthAccrual_ShouldReturnFalse()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueVacationDays(1);
            // Force last accrual to previous month
            var balance = employee.VacationBalance;
            typeof(EmployeeVacationBalance)
                .GetProperty("LastAccrualDate")
                ?.SetValue(balance, DateTime.UtcNow.AddMonths(-1));

            // Act
            var result = employee.VacationBalance.HasAccruedThisMonth();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void VacationBalance_PayUsedDays_WithValidDays_ShouldDecreaseUsedDays()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueVacationDays(10);
            employee.UseVacationDays(5);
            var initialUsed = employee.VacationBalance.UsedDays;
            var daysToPay = 2;

            // Act
            employee.VacationBalance.PayUsedDays(daysToPay);

            // Assert
            employee.VacationBalance.UsedDays.Should().Be(initialUsed - daysToPay);
            employee.VacationBalance.AvailableDays.Should().Be(10 - (initialUsed - daysToPay));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void VacationBalance_PayUsedDays_WithZeroOrNegativeDays_ShouldThrowArgumentException(decimal invalidDays)
        {
            // Arrange
            var employee = CreateValidEmployee();

            // Act
            Action act = () => employee.VacationBalance.PayUsedDays(invalidDays);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("*Days to pay must be greater than zero*");
        }

        [Fact]
        public void VacationBalance_PayUsedDays_WithMoreThanUsedDays_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueVacationDays(5);
            employee.UseVacationDays(2);

            // Act
            Action act = () => employee.VacationBalance.PayUsedDays(5);

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("*Cannot pay more days than used*");
        }

        [Fact]
        public void VacationBalance_AvailableDays_ShouldReturnCorrectCalculation()
        {
            // Arrange
            var employee = CreateValidEmployee();
            employee.AccrueVacationDays(10);
            employee.UseVacationDays(3);

            // Act
            var available = employee.VacationBalance.AvailableDays;

            // Assert
            available.Should().Be(7);
        }

        [Fact]
        public void Constructor_ShouldSetCreatedAtToUtcNow()
        {
            // Arrange & Act
            var employee = CreateValidEmployee();

            // Assert
            employee.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Constructor_ShouldInitializeBalancesWithZero()
        {
            // Arrange & Act
            var employee = CreateValidEmployee();

            // Assert
            employee.AguinaldoBalance.AccruedAmount.Should().Be(0);
            employee.VacationBalance.AccruedDays.Should().Be(0);
        }

        [Fact]
        public void Constructor_ShouldSetEmployeeIdInBalances()
        {
            // Arrange & Act
            var employee = CreateValidEmployee();

            // Assert
            employee.AguinaldoBalance.EmployeeId.Should().Be(employee.Id);
            employee.VacationBalance.EmployeeId.Should().Be(employee.Id);
        }

        private static Employee CreateValidEmployee()
        {
            return new Employee(
                "John Doe",
                "john.doe@company.com",
                EmployeeRole.Employee,
                "hashedPassword123",
                12345);
        }
    }
}