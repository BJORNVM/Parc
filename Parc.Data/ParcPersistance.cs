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
            _logger.LogInformation("Fetching events from database...");

            List<ParcEvent> parcEvents = _ctx.Events.ToList();

            _logger.LogInformation("Found { count } events", parcEvents.Count);

            return parcEvents;
        }

        public List<ParcEvent> FindNewEvents(List<ParcEvent> events)
        {
            _logger.LogInformation("Looking for new events in list of { count } events...", events.Count);

            List<DateTime> existingTimestamps = _ctx.Events.Select(e => e.Timestamp).ToList();
            List<ParcEvent> newEvents = events
                .Where(e => !existingTimestamps.Contains(e.Timestamp))
                .ToList();

            _logger.LogInformation("Found { count } new events", newEvents.Count);

            return newEvents;
        }

        public void SaveEvents(List<ParcEvent> events)
        {
            _logger.LogInformation("Adding { count } events to database...", events.Count);

            _ctx.Events.AddRange(events);
            _ctx.SaveChanges();

            _logger.LogInformation("Added { count } events to database, new total: { total } events", events.Count, _ctx.Events.Count());
        }
    }
}
