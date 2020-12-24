using Parc.Models;
using System.Collections.Generic;

namespace Parc
{
    public interface IProductionPersistance<T>
    {
        string Name { get; set; }

        List<ParcEvent> GetEvents();
        ParcEvent ProjectToParcEvent(T productionEvent);
    }
}
