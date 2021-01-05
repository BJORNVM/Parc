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
        private readonly ILogger<ImportService<T>> _logger;

        public ImportService(IParcPersistance parcPersistance, IProductionPersistance<T> productionPersistance, ILogger<ImportService<T>> logger)
        {
            _parcPersistance = parcPersistance;
            _productionPersistance = productionPersistance;
            _logger = logger;
        }

        public void ImportEvents()
        {
            DateTime startTime = DateTime.Now;

            try
            {
                _logger.LogInformation("--- Start of import ---");

                List<ParcEvent> productionEvents = _productionPersistance.GetEvents();

                _logger.LogInformation("Removing possible duplicates from { count } production events...", productionEvents.Count);

                List<ParcEvent> distinctProductionEvents = productionEvents.Distinct().ToList();

                _logger.LogInformation("Removed { removedCount } duplicates, resulting in { distinctCount} distinct production events", productionEvents.Count - distinctProductionEvents.Count, distinctProductionEvents.Count);

                List<ParcEvent> newEvents = _parcPersistance.FindNewEvents(distinctProductionEvents);

                _parcPersistance.SaveEvents(newEvents);

                _logger.LogInformation("Successfully imported { Count } new events in { seconds } seconds", newEvents.Count, Math.Round(DateTime.Now.Subtract(startTime).TotalSeconds));
            } 
            catch(Exception e)
            {
                _logger.LogError("Error importing events: { message }", e.Message);
            }
            finally
            {
                _logger.LogInformation("--- End of import ---");
            }
        }
    }
}
