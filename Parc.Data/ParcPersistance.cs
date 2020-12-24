using Microsoft.Extensions.Logging;
using Parc.Models;
using System;
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
            _logger.LogInformation("Fetching events...");

            List<ParcEvent> parcEvents = _ctx.Events.ToList();

            _logger.LogInformation("Found { count } events", parcEvents.Count);

            return parcEvents;
        }

        public List<ParcEvent> FindNewEvents(List<ParcEvent> events)
        {
            _logger.LogInformation("Creating hashset of unique identifiers from existing events...");

            HashSet<string> uniqueIdentifiers = _ctx.Events.Select(e => e.UniqueIdentifier).ToHashSet();

            _logger.LogInformation("Created hashset of unique identifiers with { count } members", uniqueIdentifiers.Count());

            _logger.LogInformation("Comparing { countGiven } given events with { countExisting } existing events to find new events...", events.Count, uniqueIdentifiers.Count);

            List<ParcEvent> newEvents = events.Where(e => !uniqueIdentifiers.Contains(e.UniqueIdentifier)).ToList();

            _logger.LogInformation("Found { count } new events", newEvents.Count);

            return newEvents;
        }

        public void SaveEvents(List<ParcEvent> events)
        {
            _logger.LogInformation("Adding { count } events...", events.Count);

            _ctx.Events.AddRange(events);
            _ctx.SaveChanges();

            _logger.LogInformation("Added { count } events to database, new total: { total } events", events.Count, _ctx.Events.Count());
        }
    }
}
