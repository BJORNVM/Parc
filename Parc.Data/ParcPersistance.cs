using Microsoft.Extensions.Logging;
using Parc.Models;
using System.Collections.Generic;
using System.Linq;

namespace Parc.Data
{
    public class ParcPersistance : IParcPersistance
    {
        private readonly ParcDbContext _ctx;
        private readonly ILogger<ParcPersistance> _logger;


        public ParcPersistance(ParcDbContext ctx, ILogger<ParcPersistance> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public List<ParcEvent> GetEvents() 
        {
            // TODO
            _ctx.Database.EnsureCreated();

            _logger.LogInformation("Fetching events from Parc database...");

            List<ParcEvent> parcEvents = _ctx.Events.ToList();

            _logger.LogInformation("Found { count } events", parcEvents.Count);

            return parcEvents;
        }

        public void SaveEvents(List<ParcEvent> parcEvents)
        {
            _logger.LogInformation("Adding { count } new events to Parc database...", parcEvents.Count);

            _ctx.AddRange(parcEvents);
            _ctx.SaveChanges();

            _logger.LogInformation("Added { count } new events to Parc database", parcEvents.Count);
            _logger.LogInformation("There are now { count } events in the Parc database", _ctx.Events.Count());
        }
    }
}
