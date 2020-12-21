using Parc.Models;
using System.Collections.Generic;

namespace Parc
{
    public interface ILogPersistance
    {
        void SaveImportResult(ImportResult importResult);
    }
}
