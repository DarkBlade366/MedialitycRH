using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Infrastructure.Persistence
{
    public class ApiDbContextFactory : IDesignTimeDbContextFactory<ApiDbContext>
    {
        public ApiDbContext CreateDbContext(string[] args)
        {
            // Lee appsettings.json desde Web.API
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Web.API"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
            optionsBuilder.UseNpgsql(
                configuration.GetConnectionString("DbMedialitycHR"));

            ICurrentUserService currentUser = new DesignTimeCurrentUserService();

            return new ApiDbContext(optionsBuilder.Options, currentUser);
        }

        // Clase dummy que implementa ICurrentUserService solo para migraciones
        public class DesignTimeCurrentUserService : ICurrentUserService
        {
            public string UserName => "SYSTEM";
        }
    }
}