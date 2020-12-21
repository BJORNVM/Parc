using Parc.Models;
using System.Collections.Generic;

namespace Parc
{
    public interface IProductionPersistance<T>
    {
        List<ParcEvent> GetEvents();
        ParcEvent ProjectToParcEvent(T productionEvent);
    }
}
