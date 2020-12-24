using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parc.Data;
using Parc.Models;
using Parc.Services;

namespace Parc.Sandbox
{
    class Program
    {
        private static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.None) // Exclude EFCore
                .AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "[dd/MM/yyyy] [hh:mm:ss:fff] ";
                    options.SingleLine = true;
                });
        });


        static void Main(string[] args)
        {
            // Setup
            string parcPersistanceSQLConnectionString = @"Server=MORLM752\PARC; Database=PARC; Trusted_Connection=True";
            string productionPersistanceName = "BG01 (DV20PLUS.AGFA.BE)";
            string productionPersistanceSQLConnectionString = @"Server=";

            IParcPersistance parcPersistance = CreateParcPersistance(parcPersistanceSQLConnectionString);
            IProductionPersistance<DeltaVEvent> productionPersistance = CreateProductionPersistance<DeltaVEvent>(productionPersistanceSQLConnectionString, productionPersistanceName);
            ImportService<DeltaVEvent> importService = CreateImportService(parcPersistance, productionPersistance);

            // Ensure existance of Parc database
            // TODO: Remove creation of databases...
            EnsureExistanceOfParcDatabase(parcPersistanceSQLConnectionString);

            // Start import of production events to Parc persistance
            importService.ImportEvents();
        }


        private static IParcPersistance CreateParcPersistance(string sqlConnectionString)
        {
            // DbContext
            var parcDbContextOptions = new DbContextOptionsBuilder<ParcDbContext>()
                .UseLoggerFactory(_loggerFactory)
                .UseSqlServer(sqlConnectionString)
                .Options;
            var parcDbContext = new ParcDbContext(parcDbContextOptions);

            // Logger
            ILogger<ParcPersistance> parcPersistanceLogger = _loggerFactory.CreateLogger<ParcPersistance>();

            // Parc persistance
            return new ParcPersistance(parcDbContext, parcPersistanceLogger);
        }


        private static IProductionPersistance<T> CreateProductionPersistance<T>(string sqlConnectionString, string name)
        {
            // DbContext
            var productionDbContextOptions = new DbContextOptionsBuilder<DeltavDbContext>()
                .UseLoggerFactory(_loggerFactory)
                .UseSqlServer(sqlConnectionString)
                .Options;
            var productionDbContext = new DeltavDbContext(productionDbContextOptions);

            // Logger
            ILogger<DeltavPersistance> productionPersistanceLogger = _loggerFactory.CreateLogger<DeltavPersistance>();

            // Production persistance
            return (IProductionPersistance<T>) new DeltavPersistance(productionDbContext, productionPersistanceLogger, name);
        }


        private static ImportService<T> CreateImportService<T>(IParcPersistance parcPersistance, IProductionPersistance<T> productionPersistance)
        {
            // Logger
            ILogger<ImportService<T>> importServiceLogger = _loggerFactory.CreateLogger<ImportService<T>>();

            return new ImportService<T>(parcPersistance, productionPersistance, importServiceLogger);
        }


        private static void EnsureExistanceOfParcDatabase(string sqlConnectionString)
        {
            // DbContext
            var parcDbContextOptions = new DbContextOptionsBuilder<ParcDbContext>()
                .UseLoggerFactory(_loggerFactory)
                .UseSqlServer(sqlConnectionString)
                .Options;
            var parcDbContext = new ParcDbContext(parcDbContextOptions);

            parcDbContext.Database.EnsureCreated();
        }
    }
}
