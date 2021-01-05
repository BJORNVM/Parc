using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parc.Data;
using Parc.Models;
using Parc.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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
            // Setup Parc persistance
            var parcDbContextOptions = new DbContextOptionsBuilder<ParcDbContext>()
                .UseLoggerFactory(_loggerFactory)
                .UseSqlServer(@"Server=localhost; Database=Parc; Trusted_Connection=True")
                .Options;
            var parcDbContext = new ParcDbContext(parcDbContextOptions);

            IParcPersistance parcPersistance = new ParcPersistance(
                ctx: parcDbContext,
                logger: _loggerFactory.CreateLogger<ParcPersistance>());


            // Setup Production persistance
            var productionName = "AgfaProductionExample";
            var productionDbContextOptions = new DbContextOptionsBuilder<DeltavDbContext>()
                .UseLoggerFactory(_loggerFactory)
                .UseSqlServer(@"Server=localhost; Database=AgfaProductionExample; Trusted_Connection=True")
                .Options;
            var productionDbContext = new DeltavDbContext(productionDbContextOptions);

            IProductionPersistance<DeltaVEvent> productionPersistance = new DeltavPersistance(
                ctx: productionDbContext, 
                logger: _loggerFactory.CreateLogger<DeltavPersistance>(), 
                name: productionName);


            // Setup ImportService
            ImportService<DeltaVEvent> importService = new ImportService<DeltaVEvent>(
                parcPersistance: parcPersistance, 
                productionPersistance: productionPersistance, 
                logger: _loggerFactory.CreateLogger<ImportService<DeltaVEvent>>());



            // Ensure existance of databases
            // TODO: Remove creation of databases...
            parcDbContext.Database.EnsureCreated();
            productionDbContext.Database.EnsureCreated();


            // Check if Production database needs to be seeded
            if (productionDbContext.Events.Count() == 0) SeedProductionDatabase(productionDbContext);


            // Start import of production events to Parc persistance
            importService.ImportEvents();
            importService.ImportEvents();
        }

        private static void SeedProductionDatabase(DeltavDbContext ctx)
        {
            List<DeltaVEvent> events = Enumerable
                .Range(1, 1000)
                .Select(n => new DeltaVEvent
                {
                    Date_Time = DateTime.Now.AddMinutes(-n),
                    FracSec = (short)n,
                })
                .ToList();

            ctx.Events.AddRange(events);

            // Add 50 duplicates on purpose, these should not be imported
            ctx.Events.AddRange(events
                .Take(50)
                .Select(e => new DeltaVEvent
                {
                    Date_Time = e.Date_Time,
                    FracSec = e.FracSec,
                })
                .ToList());

            ctx.SaveChanges();
        }
    }
}
