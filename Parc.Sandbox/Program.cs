using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parc.Data;
using Parc.Services;
using System;
using System.Threading;

namespace Parc.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            // LOGGER FACTORY
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder
                    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning) // Only log EFCore warning (or higher) messages
                    .AddSimpleConsole(options =>
                    {
                        options.TimestampFormat = "[dd/MM/yyyy] [hh:mm:ss:ffff] ";
                        options.SingleLine = true;
                    })
            );




            // PARC PERSISTANCE SETUP

            // DbContext
            var parcDbContextOptions = new DbContextOptionsBuilder<ParcDbContext>()
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(@"Connectionstring parc sql server")
                .Options;
            var parcDbContext = new ParcDbContext(parcDbContextOptions);

            // Logger
            ILogger<ParcPersistance> parcPersistanceLogger = loggerFactory.CreateLogger<ParcPersistance>();

            // Parc persistance
            IParcPersistance parcPersistance = new ParcPersistance(parcDbContext, parcPersistanceLogger);




            // PRODUCTION PERSISTANCE SETUP

            // DbContext
            var productionDbContextOptions = new DbContextOptionsBuilder<DeltavDbContext>()
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(@"Connectionstring deltav sql server")
                .Options;
            var productionDbContext = new DeltavDbContext(productionDbContextOptions);

            // Logger
            ILogger<DeltavPersistance> productionPersistanceLogger = loggerFactory.CreateLogger<DeltavPersistance>();

            // Production persistance
            IProductionPersistance productionPersistance = new DeltavPersistance(productionDbContext, productionPersistanceLogger);




            // IMPORT SERVICE SETUP
            ILogger<ImportService> importServiceLogger = loggerFactory.CreateLogger<ImportService>();
            ImportService importService = new ImportService(parcPersistance, productionPersistance, importServiceLogger);




            // Start import and repeat every 5 minutes
            var timer = new Timer(
                e => importService.ImportEvents(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(2));

            // Keep program alive
            while (true)
            {

            }
        }
    }
}
