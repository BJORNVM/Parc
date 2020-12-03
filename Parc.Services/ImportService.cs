using Microsoft.Extensions.Logging;
using Parc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parc.Services
{
    public class ImportService
    {
        private readonly IParcPersistance _parcPersistance;
        private readonly IProductionPersistance _productionPersistance;
        private readonly ILogger<ImportService> _logger;

        public ImportService(IParcPersistance parcPersistance, IProductionPersistance productionPersistance, ILogger<ImportService> logger)
        {
            _parcPersistance = parcPersistance;
            _productionPersistance = productionPersistance;
            _logger = logger;
        }

        public void ImportEvents()
        {
                _logger.LogInformation("Start of import");

                List<ParcEvent> productionEvents = _productionPersistance.GetEvents();
                List<ParcEvent> parcEvents = _parcPersistance.GetEvents();

                _logger.LogInformation("Comparing events...");

                List<DateTime> parcEventTimestamps = parcEvents.Select(e => e.Timestamp).ToList();
                List<ParcEvent> newEvents = productionEvents
                    .Where(de => !parcEventTimestamps.Contains(de.Timestamp))
                    .ToList();

                _logger.LogInformation("Found { count } new events", newEvents.Count);

                _parcPersistance.SaveEvents(newEvents);

                _logger.LogInformation("End of import");
        }
    }
}
