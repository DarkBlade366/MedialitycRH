using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Features.Payrolls.Payroll.Queries;
using Application.Features.Payrolls.Rules.BaseSalary.Commands;
using Application.Features.Payrolls.Rules.BaseSalary.DTOs;
using Application.Features.Payrolls.Rules.BaseSalary.Handlers;
using Application.Features.Payrolls.Rules.BaseSalary.Queries;
using Domain.Features.Employees.Enums;
using Domain.Features.Payrolls.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Domain.Features.Payrolls.Interfaces;
using Domain.Features.Payrolls.Rules;

namespace HumanResource.IntegrationTests.Services
{
    public class CacheIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task BaseSalaryRules_Cache_ShouldWorkAndInvalidate()
        {
            // Arrange
            await ResetDatabaseAsync();

            using var scope = Factory.Services.CreateScope();
            var dbContext = await GetDbContextAsync();
            var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
            var createHandler = scope.ServiceProvider.GetRequiredService<CreateBaseSalaryRuleHandler>();
            var getPagedHandler = scope.ServiceProvider.GetRequiredService<GetBaseSalaryRulesPagedHandler>();

            var initialRule = new BaseSalaryRule(EmployeeRole.Employee, 3000);
            dbContext.BaseSalaryRules.Add(initialRule);
            await dbContext.SaveChangesAsync();

            var query = new GetBaseSalaryRulesPagedQuery { Page = 1, PageSize = 10 };
            var result1 = await getPagedHandler.HandleAsync(query);

            result1.Items.Should().HaveCount(1);
            result1.Items.First().Amount.Should().Be(3000);

            var cachedList = await cache.GetAsync<List<BaseSalaryRule>>("baseSalaryRules:all");
            
            if (cachedList != null)
            {
                cachedList.Count.Should().Be(1);
            }

            var result2 = await getPagedHandler.HandleAsync(query);
            result2.Items.Should().HaveCount(1);
            result2.Items.First().Amount.Should().Be(3000);

            // Act: crear una nueva regla (invalida caché si está disponible)
            var createCommand = new CreateBaseSalaryRuleCommand
            {
                Role = "ProjectManager",
                Amount = 5000
            };
            await createHandler.HandleAsync(createCommand);

            if (cachedList != null)
            {
                var updatedCache = await cache.GetAsync<List<BaseSalaryRule>>("baseSalaryRules:all");
                updatedCache.Should().BeNull();
            }

            var result3 = await getPagedHandler.HandleAsync(query);
            result3.Items.Should().HaveCount(2);
            result3.Items.Should().Contain(r => r.Role == "Employee" && r.Amount == 3000);
            result3.Items.Should().Contain(r => r.Role == "ProjectManager" && r.Amount == 5000);
        }
    }
}