using Microsoft.Extensions.Logging;
using Parc.Models;
using System.Collections.Generic;
using System.Linq;

namespace Parc.Data
{
    public class DeltavPersistance : IProductionPersistance
    {
        private readonly DeltavDbContext _ctx;
        private readonly ILogger<DeltavPersistance> _logger;

        public DeltavPersistance(DeltavDbContext ctx, ILogger<DeltavPersistance> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public List<ParcEvent> GetEvents()
        {
            _logger.LogInformation("Fetching events from DeltaV database...");

            List<DeltavEvent> deltavEvents = _ctx.Events.ToList();

            _logger.LogInformation("Found { count } events", deltavEvents.Count);

            _logger.LogInformation("Projecting DeltaV events to Parc Events...");

            List<ParcEvent> projectedEvents = ParcEventFactory.Create(deltavEvents);

            _logger.LogInformation("Projected { count } events", projectedEvents.Count);

            return projectedEvents;
        }
    }
}
