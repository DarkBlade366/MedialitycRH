
# Estructura Critica de Tests - Human Resource

## Estructura Basica que tiene

```
test/
├── HumanResource.UnitTests/
│   ├── Domain/
│   │   └── Features/
│   │       ├── Payrolls/
│   │       │   └── Calculators/
│   │       │       ├── AguinaldoCalculatorTests.cs
│   │       │       ├── BaseSalaryCalculatorTests.cs
│   │       │       ├── DeductionCalculatorTests.cs
│   │       │       ├── MilestoneCalculatorTests.cs
│   │       │       ├── OvertimeCalculatorTests.cs
│   │       │       ├── ProductivityCalculatorTests.cs
│   │       │       ├── VacationCalculatorTests.cs
│   │       │       └── ProjectCalculatorTests.cs
│   │       │   └── PayrollTest.cs
│   │       └── Employees/
│   │           └── EmployeeTests.cs          (reglas de negocio del empleado)
│   ├── Application/
│   │   ├── Features/
│   │   │   ├── Redmine/
│   │   │   │   ├── SyncRedmineProjectsHandlerTests.cs
│   │   │   │   ├── SyncRedmineUsersHandlerTests.cs
│   │   │   │   ├── SyncRedmineTimeEntriesHandlerTests.cs
│   │   │   │   └── SyncRedmineMilestonesHandlerTests.cs
│   │   │   ├── Payrolls/
│   │   │   │   ├── Payroll/
│   │   │   │   │   ├── CreatePayrollHandlerTests.cs
│   │   │   │   │   ├── ApprovedPayrollHandlerTests.cs
│   │   │   │   │   └── PaidPayrollHandlerTests.cs
│   │   │   │   ├── Rules/
│   │   │   │   │   ├── CreateBaseSalaryRuleHandlerTests.cs
│   │   │   │   │   ├── ChangeBaseSalaryRuleStatusHandlerTests.cs
│   │   │   │   │   ├── CreateOvertimeRuleHandlerTests.cs
│   │   │   │   │   ├── ChangeOvertimeRuleStatusHandlerTests.cs
│   │   │   │   │   ├── CreateDeductionRuleHandlerTests.cs
│   │   │   │   │   ├── ChangeDeductionRuleStatusHandlerTests.cs
│   │   │   │   │   ├── CreateProductivityRuleHandlerTests.cs
│   │   │   │   │   ├── ChangeProductivityRuleStatusHandlerTests.cs
│   │   │   │   │   ├── CreateMilestoneRuleHandlerTests.cs
│   │   │   │   │   ├── ChangeMilestoneRuleStatusHandlerTests.cs
│   │   │   │   │   ├── CreateAguinaldoRuleHandlerTests.cs
│   │   │   │   │   ├── ChangeAguinaldoRuleStatusHandlerTests.cs
│   │   │   │   │   ├── CreateVacationRuleHandlerTests.cs
│   │   │   │   │   ├── ChangeVacationRuleStatusHandlerTests.cs
│   │   │   │   │   ├── CreateProjectRuleHandlerTests.cs
│   │   │   │   │   └── ChangeProjectRuleStatusHandlerTests.cs
│   │   │   ├── TimeEntries/
│   │   │   │   ├── ApproveTimeEntryHandlerTests.cs
│   │   │   │   └── ApproveTimeEntriesBatchHandlerTests.cs
│   │   │   ├── Employees/
│   │   │   │   ├── CreateEmployeeHandlerTests.cs
│   │   │   │   ├── ChangeEmployeeStatusHandlerTests.cs
│   │   │   │   └── UseVacationHandlerTests.cs
│   │   │   └── MilestoneParticipations/
│   │   │       ├── CreateMilestoneParticipationHandlerTests.cs
│   │   │       └── ChangeMilestoneParticipationStatusHandlerTests.cs
│   │   └── Services/
│   │       ├── ProductivityServiceTests.cs
│   │       └── VacationAccrualServiceTests.cs
│   └── Infrastructure/
│       └── Redmine/
│           └── RedmineClientTests.cs          (cliente HTTP, crítico para integración)
│
└── HumanResource.IntegrationTests/
    ├── Features/
    │   ├── Redmine/
    │   │   ├── SyncRedmineProjectsIntegrationTests.cs
    │   │   ├── SyncRedmineUsersIntegrationTests.cs
    │   │   ├── SyncRedmineTimeEntriesIntegrationTests.cs
    │   │   ├── SyncRedmineMilestonesIntegrationTests.cs
    │   │   └── FullSyncWorkflowTests.cs        (prueba la sincronización completa)
    │   ├── Payrolls/
    │   │   ├── CreatePayrollIntegrationTests.cs
    │   │   └── PayrollLifecycleIntegrationTests.cs (crear, aprobar, pagar)
    │   ├── TimeEntries/
    │   │   └── ApproveTimeEntriesIntegrationTests.cs
    │   ├── Projects/
    │   │   └── MilestoneParticipationsIntegrationTests.cs (asignación y pago)
    │   └── Analytics/
    │       ├── ProjectCostsIntegrationTests.cs   (reporte crítico)
    │       └── ProductivityTrendIntegrationTests.cs
    ├── BackgroundServices/
    │   └── RedmineSyncBackgroundServiceIntegrationTests.cs
    └── Repositories/
        ├── EmployeeRepositoryTests.cs
        ├── TimeEntryRepositoryTests.cs
        ├── PayrollRepositoryTests.cs
        ├── ProjectRepositoryTests.cs
        ├── ProjectMilestoneRepositoryTests.cs
        └── MilestoneParticipationRepositoryTests.cs
```
## Total de Archivos de Tests

- **Unit Tests**: ~55 archivos
- **Integration Tests**: ~20 archivos
- **Total**: ~75 archivos de tests

## Tecnologías

```xml
<!-- HumanResource.UnitTests.csproj -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.2" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />

<!-- HumanResource.IntegrationTests.csproj -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.2" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />
<PackageReference Include="Testcontainers.PostgreSql" Version="3.7.0" />
<PackageReference Include="WireMock.Net" Version="1.5.41" />
```

# Estructura Completa de Tests - Human Resource

## Estructura Final

```
test/
├── HumanResource.UnitTests/
│   ├── Application/
│   │   ├── Features/
│   │   │   ├── Redmine/
│   │   │   │   ├── SyncRedmineProjectsHandlerTests.cs
│   │   │   │   ├── SyncRedmineUsersHandlerTests.cs
│   │   │   │   ├── SyncRedmineTimeEntriesHandlerTests.cs
│   │   │   │   └── SyncRedmineMilestonesHandlerTests.cs
│   │   │   ├── Payrolls/
│   │   │   │   ├── Payroll/
│   │   │   │   │   ├── CreatePayrollHandlerTests.cs
│   │   │   │   │   ├── ApprovedPayrollHandlerTests.cs
│   │   │   │   │   └── PaidPayrollHandlerTests.cs
│   │   │   │   ├── Rules/
│   │   │   │   │   ├── Aguinaldo/
│   │   │   │   │   │   ├── CreateAguinaldoRuleHandlerTests.cs
│   │   │   │   │   │   ├── ChangeAguinaldoRuleStatusHandlerTests.cs
│   │   │   │   │   │   ├── GetAguinaldoRuleByIdHandlerTests.cs
│   │   │   │   │   │   └── GetAguinaldoRulesPagedHandlerTests.cs
│   │   │   │   │   ├── BaseSalary/
│   │   │   │   │   │   ├── CreateBaseSalaryRuleHandlerTests.cs
│   │   │   │   │   │   ├── ChangeBaseSalaryRuleStatusHandlerTests.cs
│   │   │   │   │   │   ├── GetBaseSalaryRuleByIdHandlerTests.cs
│   │   │   │   │   │   └── GetBaseSalaryRulesPagedHandlerTests.cs
│   │   │   │   │   ├── Deduction/
│   │   │   │   │   │   ├── CreateDeductionRuleHandlerTests.cs
│   │   │   │   │   │   ├── ChangeDeductionRuleStatusHandlerTests.cs
│   │   │   │   │   │   ├── GetDeductionRuleByIdHandlerTests.cs
│   │   │   │   │   │   └── GetDeductionRulesPagedHandlerTests.cs
│   │   │   │   │   ├── Milestone/
│   │   │   │   │   │   ├── CreateMilestoneRuleHandlerTests.cs
│   │   │   │   │   │   ├── ChangeMilestoneRuleStatusHandlerTests.cs
│   │   │   │   │   │   ├── GetMilestoneRuleByIdHandlerTests.cs
│   │   │   │   │   │   └── GetMilestoneRulesPagedHandlerTests.cs
│   │   │   │   │   ├── Overtime/
│   │   │   │   │   │   ├── CreateOvertimeRuleHandlerTests.cs
│   │   │   │   │   │   ├── ChangeOvertimeRuleStatusHandlerTests.cs
│   │   │   │   │   │   ├── GetOvertimeRuleByIdHandlerTests.cs
│   │   │   │   │   │   └── GetOvertimeRulesPagedHandlerTests.cs
│   │   │   │   │   ├── Productivity/
│   │   │   │   │   │   ├── CreateProductivityRuleHandlerTests.cs
│   │   │   │   │   │   ├── ChangeProductivityRuleStatusHandlerTests.cs
│   │   │   │   │   │   ├── GetProductivityRuleByIdHandlerTests.cs
│   │   │   │   │   │   └── GetProductivityRulesPagedHandlerTests.cs
│   │   │   │   │   ├── Project/
│   │   │   │   │   │   ├── CreateProjectRuleHandlerTests.cs
│   │   │   │   │   │   ├── ChangeProjectRuleStatusHandlerTests.cs
│   │   │   │   │   │   ├── GetProjectRuleByIdHandlerTests.cs
│   │   │   │   │   │   └── GetProjectRulesPagedHandlerTests.cs
│   │   │   │   │   └── Vacation/
│   │   │   │   │       ├── CreateVacationRuleHandlerTests.cs
│   │   │   │   │       ├── ChangeVacationRuleStatusHandlerTests.cs
│   │   │   │   │       ├── GetVacationRuleByIdHandlerTests.cs
│   │   │   │   │       └── GetVacationRulesPagedHandlerTests.cs
│   │   │   │   └── Payments/
│   │   │   │       ├── AguinaldoPaymentHandlerTests.cs
│   │   │   │       ├── DeductionPaymentHandlerTests.cs
│   │   │   │       ├── MilestonePaymentHandlerTests.cs
│   │   │   │       ├── OvertimePaymentHandlerTests.cs
│   │   │   │       ├── ProductivityPaymentHandlerTests.cs
│   │   │   │       └── VacationPaymentHandlerTests.cs
│   │   │   ├── TimeEntries/
│   │   │   │   ├── ApproveTimeEntryHandlerTests.cs
│   │   │   │   └── ApproveTimeEntriesBatchHandlerTests.cs
│   │   │   ├── Employees/
│   │   │   │   ├── CreateEmployeeHandlerTests.cs
│   │   │   │   ├── ChangeEmployeeStatusHandlerTests.cs
│   │   │   │   └── UseVacationHandlerTests.cs
│   │   │   ├── MilestoneParticipations/
│   │   │   │   ├── CreateMilestoneParticipationHandlerTests.cs
│   │   │   │   ├── ChangeMilestoneParticipationStatusHandlerTests.cs
│   │   │   │   ├── GetMilestoneParticipationByIdHandlerTests.cs
│   │   │   │   └── GetMilestoneParticipationsPagedHandlerTests.cs
│   │   │   ├── Analytics/
│   │   │   │   ├── GetProjectCostsHandlerTests.cs
│   │   │   │   ├── GetHoursComparisonHandlerTests.cs
│   │   │   │   └── GetProductivityTrendHandlerTests.cs
│   │   │   └── Validations/
│   │   │       ├── GetProductivityTrendValidatorTests.cs
│   │   │       ├── CreateEmployeeValidatorTests.cs
│   │   │       ├── CreatePayrollValidatorTests.cs
│   │   │       └── (Otras validaciones importantes)
│   │   └── Services/
│   │       ├── ProductivityServiceTests.cs
│   │       └── VacationAccrualServiceTests.cs
│   ├── Domain/
│   │   └── Features/
│   │       ├── Employees/
│   │       │   └── EmployeeTests.cs
│   │       ├── Payrolls/
│   │       │   ├── PayrollTests.cs
│   │       │   └── Calculators/
│   │       │       ├── AguinaldoCalculatorTests.cs
│   │       │       ├── BaseSalaryCalculatorTests.cs
│   │       │       ├── DeductionCalculatorTests.cs
│   │       │       ├── MilestoneCalculatorTests.cs
│   │       │       ├── OvertimeCalculatorTests.cs
│   │       │       ├── ProductivityCalculatorTests.cs
│   │       │       ├── VacationCalculatorTests.cs
│   │       │       └── ProjectCalculatorTests.cs
│   │       ├── Projects/
│   │       │   ├── ProjectTests.cs
│   │       │   ├── ProjectMilestoneTests.cs
│   │       │   └── MilestoneParticipationTests.cs
│   │       └── TimeEntries/
│   │           └── TimeEntryTests.cs
│   ├── Infrastructure/
│   │   ├── Redmine/
│   │   │   └── RedmineClientTests.cs
│   │   ├── Persistence/
│   │   │   ├── DbContextTests.cs
│   │   │   └── MigrationsTests.cs
│   │   ├── Security/
│   │   │   ├── JwtServiceTests.cs
│   │   │   └── PasswordHashingTests.cs
│   │   └── Services/
│   │       └── (Otros servicios de infraestructura)
│   ├── Web.API/
│   │   ├── Endpoints/
│   │   │   ├── Redmine/
│   │   │   │   ├── SyncRedmineProjectsEndpointTests.cs
│   │   │   │   ├── SyncRedmineUsersEndpointTests.cs
│   │   │   │   ├── SyncRedmineTimeEntriesEndpointTests.cs
│   │   │   │   └── SyncRedmineMilestonesEndpointTests.cs
│   │   │   ├── Payrolls/
│   │   │   │   ├── PayrollEndpointsTests.cs
│   │   │   │   ├── RuleEndpointsTests.cs
│   │   │   │   └── PaymentEndpointsTests.cs
│   │   │   ├── Employees/
│   │   │   │   └── EmployeeEndpointsTests.cs
│   │   │   └── Analytics/
│   │   │       └── AnalyticsEndpointsTests.cs
│   │   └── BackgroundServices/
│   │       └── RedmineSyncBackgroundServiceTests.cs
│   └── Configuration/
│       ├── DependencyInjectionTests.cs
│       └── ConfigurationValidationTests.cs
│
└── HumanResource.IntegrationTests/
    ├── Features/
    │   ├── Redmine/
    │   │   ├── SyncRedmineProjectsIntegrationTests.cs
    │   │   ├── SyncRedmineUsersIntegrationTests.cs
    │   │   ├── SyncRedmineTimeEntriesIntegrationTests.cs
    │   │   ├── SyncRedmineMilestonesIntegrationTests.cs
    │   │   ├── FullSyncWorkflowTests.cs
    │   │   ├── ConcurrentSyncTests.cs
    │   │   ├── ErrorHandlingTests.cs
    │   │   └── DataConsistencyTests.cs
    │   ├── Payrolls/
    │   │   ├── CreatePayrollIntegrationTests.cs
    │   │   ├── PayrollLifecycleIntegrationTests.cs
    │   │   ├── PayrollCalculationIntegrationTests.cs
    │   │   ├── PaymentProcessingTests.cs
    │   │   └── PayrollApprovalWorkflowTests.cs
    │   ├── TimeEntries/
    │   │   ├── ApproveTimeEntriesIntegrationTests.cs
    │   │   └── TimeEntryWorkflowTests.cs
    │   ├── Projects/
    │   │   ├── ProjectRulesIntegrationTests.cs
    │   │   ├── ProjectMilestonesIntegrationTests.cs
    │   │   └── MilestoneParticipationsIntegrationTests.cs
    │   └── Analytics/
    │       ├── ProjectCostsIntegrationTests.cs
    │       └── ProductivityTrendIntegrationTests.cs
    ├── BackgroundServices/
    │   ├── RedmineSyncBackgroundServiceIntegrationTests.cs
    │   └── FullSynchronizationCycleTests.cs
    ├── Repositories/
    │   ├── EmployeeRepositoryTests.cs
    │   ├── TimeEntryRepositoryTests.cs
    │   ├── PayrollRepositoryTests.cs
    │   ├── ProjectRepositoryTests.cs
    │   ├── ProjectMilestoneRepositoryTests.cs
    │   └── MilestoneParticipationRepositoryTests.cs
    ├── Endpoints/
    │   ├── RedmineEndpointsIntegrationTests.cs
    │   ├── PayrollEndpointsIntegrationTests.cs
    │   ├── EmployeeEndpointsIntegrationTests.cs
    │   └── AnalyticsEndpointsIntegrationTests.cs
    ├── Infrastructure/
    │   ├── Persistence/
    │   │   ├── DatabaseIntegrationTests.cs
    │   │   └── TransactionManagementTests.cs
    │   ├── Security/
    │   │   └── AuthenticationIntegrationTests.cs
    │   └── ExternalServices/
    │       └── RedmineApiIntegrationTests.cs
    └── Configuration/
        ├── ApplicationConfigurationTests.cs
        └── DatabaseConfigurationTests.cs
```

## Total de Archivos de Tests

- **Unit Tests**: ~85 archivos
- **Integration Tests**: ~45 archivos
- **Total**: ~130 archivos de tests

## Tecnologías

```xml
<!-- HumanResource.UnitTests.csproj -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.0" />
<PackageReference Include="coverlet.collector" Version="6.0.0" />

<!-- HumanResource.IntegrationTests.csproj -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />
<PackageReference Include="Testcontainers.PostgreSql" Version="3.7.0" />
<PackageReference Include="WireMock.Net" Version="1.5.41" />
<PackageReference Include="coverlet.collector" Version="6.0.0" />
```
