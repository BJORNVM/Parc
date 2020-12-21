using Microsoft.Extensions.Logging;
using Parc.Models;
using System.Collections.Generic;
using System.Linq;

namespace Parc.Data
{
    public class LogPersistance : ILogPersistance
    {
        private readonly LogDbContext _ctx;
        private readonly ILogger<LogPersistance> _logger;

        public LogPersistance(LogDbContext ctx, ILogger<LogPersistance> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public void SaveImportResult(ImportResult importResult)
        {
            _logger.LogInformation("Adding new import result to database...");

            _ctx.ImportResults.Add(importResult);
            _ctx.SaveChanges();

            _logger.LogInformation("Added import result to database, new total: { total } import results", _ctx.ImportResults.Count());
        }
    }
}
