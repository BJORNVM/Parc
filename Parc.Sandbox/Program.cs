using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parc.Data;
using Parc.Models;
using Parc.Services;
using System;
using System.Threading;

namespace Parc.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            // SETUP
            string parcSQLConnectionString = @"Server=MORLM752\PARC; Database=BG01; Trusted_Connection=True";
            string productionSQLConnectionString = @"Server=secret";
            string logSQLConnectionString = @"Server=MORLM752\PARC; Database=LOGS; Trusted_Connection=True";

            ILoggerFactory loggerFactory = CreateLoggerFactory();
            IParcPersistance parcPersistance = CreateParcPersistance(loggerFactory, parcSQLConnectionString);
            IProductionPersistance<DeltavEvent> productionPersistance = CreateProductionPersistance<DeltavEvent>(loggerFactory, productionSQLConnectionString);
            ILogPersistance logPersistance = CreateLogPersistance(loggerFactory, logSQLConnectionString);
            ImportService<DeltavEvent> importService = CreateImportService(loggerFactory, parcPersistance, productionPersistance, logPersistance);


            // START IMPORT
            importService.ImportEvents();
        }





        private static ILoggerFactory CreateLoggerFactory()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                builder
                    .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning) // Only log EFCore warning (or higher) messages
                    .AddSimpleConsole(options =>
                    {
                        options.TimestampFormat = "[dd/MM/yyyy] [hh:mm:ss:ffff] ";
                        options.SingleLine = true;
                    })
            );

            return loggerFactory;
        }


        private static IParcPersistance CreateParcPersistance(ILoggerFactory loggerFactory, string sqlConnectionString)
        {
            // DbContext
            var parcDbContextOptions = new DbContextOptionsBuilder<ParcDbContext>()
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(sqlConnectionString)
                .Options;
            var parcDbContext = new ParcDbContext(parcDbContextOptions);



            // TODO: Move creation of databases
            parcDbContext.Database.EnsureCreated();



            // Logger
            ILogger<ParcPersistance> parcPersistanceLogger = loggerFactory.CreateLogger<ParcPersistance>();

            // Parc persistance
            return new ParcPersistance(parcDbContext, parcPersistanceLogger);
        }


        private static IProductionPersistance<T> CreateProductionPersistance<T>(ILoggerFactory loggerFactory, string sqlConnectionString)
        {
            // DbContext
            var productionDbContextOptions = new DbContextOptionsBuilder<DeltavDbContext>()
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(sqlConnectionString)
                .Options;
            var productionDbContext = new DeltavDbContext(productionDbContextOptions);

            // Logger
            ILogger<DeltavPersistance> productionPersistanceLogger = loggerFactory.CreateLogger<DeltavPersistance>();

            // Production persistance
            return (IProductionPersistance<T>) new DeltavPersistance(productionDbContext, productionPersistanceLogger);
        }


        private static ILogPersistance CreateLogPersistance(ILoggerFactory loggerFactory, string sqlConnectionString)
        {
            // DbContext
            var logDbContextOptions = new DbContextOptionsBuilder<LogDbContext>()
                .UseLoggerFactory(loggerFactory)
                .UseSqlServer(sqlConnectionString)
                .Options;
            var logDbContext = new LogDbContext(logDbContextOptions);



            // TODO: Move creation of databases
            logDbContext.Database.EnsureCreated();



            // Logger
            ILogger<LogPersistance> logPersistanceLogger = loggerFactory.CreateLogger<LogPersistance>();

            // Log persistance
            return new LogPersistance(logDbContext, logPersistanceLogger);
        }


        private static ImportService<T> CreateImportService<T>(ILoggerFactory loggerFactory, IParcPersistance parcPersistance, IProductionPersistance<T> productionPersistance, ILogPersistance logPersistance)
        {
            // Logger
            ILogger<ImportService<T>> importServiceLogger = loggerFactory.CreateLogger<ImportService<T>>();

            return new ImportService<T>(parcPersistance, productionPersistance, logPersistance, importServiceLogger);
        }
    }
}
