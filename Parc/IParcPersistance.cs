using Parc.Models;
using System.Collections.Generic;

namespace Parc
{
    public interface IParcPersistance
    {
        List<ParcEvent> GetEvents();
        List<ParcEvent> FindNewEvents(List<ParcEvent> events);
        void SaveEvents(List<ParcEvent> events);
    }
}
