using System;
using System.Collections.Generic;
using System.Linq;
using Parc.Models;

namespace Parc
{
    public class ParcEventFactory
    {
        public static ParcEvent Create(DeltavEvent deltavEvent)
        {
            ParcEvent parcEvent = new ParcEvent
            {
                Timestamp = deltavEvent.Date_Time.AddMilliseconds(deltavEvent.FracSec / 10),

                Type = deltavEvent.Event_Type,
                Priority = deltavEvent.Event_Level,
                Status = deltavEvent.State,

                OriginDescription = deltavEvent.Module_Description
            };

            if (!String.IsNullOrEmpty(deltavEvent.Node)) parcEvent.Origin += $"[ { deltavEvent.Node } ] ";
            if (!String.IsNullOrEmpty(deltavEvent.Area)) parcEvent.Origin += $"[ { deltavEvent.Area } ] ";
            if (!String.IsNullOrEmpty(deltavEvent.Unit)) parcEvent.Origin += $"[ { deltavEvent.Unit } ] ";
            if (!String.IsNullOrEmpty(deltavEvent.Module)) parcEvent.Origin += $"[ { deltavEvent.Module } ]";

            if (!String.IsNullOrEmpty(deltavEvent.Attribute)) parcEvent.Message += $"[ { deltavEvent.Attribute } ] ";
            if (!String.IsNullOrEmpty(deltavEvent.Desc1)) parcEvent.Message += $"[ { deltavEvent.Desc1 } ] ";
            if (!String.IsNullOrEmpty(deltavEvent.Desc2)) parcEvent.Message += $"[ { deltavEvent.Desc2 } ] ";

            return parcEvent;
        }

        public static List<ParcEvent> Create(List<DeltavEvent> deltaVEvents) => deltaVEvents.Select(e => Create(e)).ToList();

    }
}
