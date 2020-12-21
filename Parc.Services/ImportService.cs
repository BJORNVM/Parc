using Microsoft.Extensions.Logging;
using Parc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parc.Services
{
    public class ImportService<T>
    {
        private readonly IParcPersistance _parcPersistance;
        private readonly IProductionPersistance<T> _productionPersistance;
        private readonly ILogPersistance _logPersistance;
        private readonly ILogger<ImportService<T>> _logger;

        public ImportService(IParcPersistance parcPersistance, IProductionPersistance<T> productionPersistance, ILogPersistance logPersistance, ILogger<ImportService<T>> logger)
        {
            _parcPersistance = parcPersistance;
            _productionPersistance = productionPersistance;
            _logPersistance = logPersistance;
            _logger = logger;
        }

        public void ImportEvents()
        {
            DateTime startTime = DateTime.Now;

            _logger.LogInformation("Start of import");

            List<ParcEvent> productionEvents = _productionPersistance.GetEvents();
            List<ParcEvent> newEvents = _parcPersistance.FindNewEvents(productionEvents);

            _parcPersistance.SaveEvents(newEvents);

            TimeSpan elapsedTime = DateTime.Now.Subtract(startTime);

            _logPersistance.SaveImportResult(new ImportResult 
            { 
                Database = "BG01", // TODO: Databases...
                Success = true,
                Result = $"Successfully imported { newEvents.Count } new events in { elapsedTime.ToString() }" 
            });

            _logger.LogInformation("End of import. Successfully imported { Count } new events in { time } seconds", newEvents.Count, elapsedTime.TotalSeconds);
        }
    }
}
