using Parc.Models;
using System.Collections.Generic;

namespace Parc
{
    public interface IParcPersistance
    {
        List<ParcEvent> GetEvents();
        void SaveEvents(List<ParcEvent> events);
    }
}
