using Parc.Models;
using System.Collections.Generic;

namespace Parc
{
    public interface IProductionPersistance
    {
        List<ParcEvent> GetEvents();
    }
}
