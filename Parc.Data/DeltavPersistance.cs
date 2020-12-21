using Microsoft.Extensions.Logging;
using Parc.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parc.Data
{
    public class DeltavPersistance : IProductionPersistance<DeltavEvent>
    {
        public readonly DeltavDbContext _ctx;
        private readonly ILogger<DeltavPersistance> _logger;

        public DeltavPersistance(DeltavDbContext ctx, ILogger<DeltavPersistance> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public List<ParcEvent> GetEvents()
        {
            _logger.LogInformation("Fetching events from DeltaV database...");

            List<DeltavEvent> productionEvents = _ctx.Events.ToList();

            _logger.LogInformation("Found { count } events", productionEvents.Count);

            _logger.LogInformation("Projecting DeltaV events to Parc Events...");

            List<ParcEvent> projectedEvents = productionEvents
                .Select(e => ProjectToParcEvent(e))
                .ToList();

            _logger.LogInformation("Projected { count } events", projectedEvents.Count);

            return projectedEvents;
        }

        public ParcEvent ProjectToParcEvent(DeltavEvent productionEvent) => new ParcEvent
        {
            // Date_Time is stored as UTC time -> convert to local (= +1h)
            // FracSec is stored as 1/1000th of one second -> add to timestamp
            Timestamp = productionEvent.Date_Time
                .ToLocalTime()
                .AddMilliseconds(productionEvent.FracSec / 10),

            Type = productionEvent.Event_Type,
            Category = productionEvent.Category,
            Priority = productionEvent.Event_Level,
            Status = productionEvent.State,

            // Combine Node, Area, Unit, Module, Module_Description into Origin, separated by " > " (empty strings are ignored)
            Origin = String.Join(
                separator: " > ",
                values: new string[] {
                    productionEvent.Node,
                    productionEvent.Area,
                    productionEvent.Unit,
                    productionEvent.Module,
                    productionEvent.Module_Description
                }.Where(s => !String.IsNullOrEmpty(s))),

            // Combine Attribute, Desc1, Desc2 into Message, separated by " > " (empty strings are ignored)
            Message = String.Join(
                separator: " > ",
                values: new string[] {
                    productionEvent.Attribute,
                    productionEvent.Desc1,
                    productionEvent.Desc2
                }.Where(s => !String.IsNullOrEmpty(s)))
        };
    }
}
