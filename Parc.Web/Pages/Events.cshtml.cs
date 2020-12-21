using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parc.Data;
using Parc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Parc.Web.Pages
{
    public class EventsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ParcDbContext _parcDbContext;
        private readonly LogDbContext _logDbContext;



        public enum Sort { Timestamp, Origin, Message, Status }

        public enum SortDirection { Descending, Ascending }

        public List<SelectListItem> PageSizes => new int[] { 10, 15, 20, 25, 50, 100, 500, 1000 }
            .Select(n => new SelectListItem { Value = n.ToString(), Text = n.ToString() })
            .ToList();



        public EventsModel(ILogger<IndexModel> logger, ParcDbContext parcDbContext, LogDbContext logDbContext)
        {
            _logger = logger;
            _parcDbContext = parcDbContext;
            _logDbContext = logDbContext;
        }



        [BindProperty(SupportsGet = true)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddThh:mm}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; } = DateTime.Now.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddThh:mm}", ApplyFormatInEditMode = true)]
        public DateTime To { get; set; } = DateTime.Now;

        [BindProperty(SupportsGet = true)]
        public string OriginFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string MessageFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public Sort SortOption { get; set; }

        [BindProperty(SupportsGet = true)]
        public SortDirection SortDirectionOption { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 15;



        public PaginatedList<ParcEvent> ParcEvents { get; set; }

        public DateTime LastSuccessfullImport { get; set; }



        public async Task OnGetAsync(int? pageIndex)
        {
            LastSuccessfullImport = await _logDbContext.ImportResults
                .Where(x => x.Database == "BG01") // TODO: databases...
                .OrderByDescending(x => x.Timestamp)
                .Select(x => x.Timestamp)
                .FirstOrDefaultAsync();
                

            IQueryable<ParcEvent> query = _parcDbContext.Events
                .AsNoTracking()
                .Where(e => e.Timestamp >= From && e.Timestamp <= To);


            query = SortOption switch
            {
                Sort.Timestamp => SortDirectionOption == SortDirection.Ascending ? query.OrderBy(e => e.Timestamp) : query.OrderByDescending(e => e.Timestamp),
                Sort.Origin => SortDirectionOption == SortDirection.Ascending ? query.OrderBy(e => e.Origin) : query.OrderByDescending(e => e.Origin),
                Sort.Message => SortDirectionOption == SortDirection.Ascending ? query.OrderBy(e => e.Message) : query.OrderByDescending(e => e.Message),
                Sort.Status => SortDirectionOption == SortDirection.Ascending ? query.OrderBy(e => e.Status) : query.OrderByDescending(e => e.Status),
                _ => SortDirectionOption == SortDirection.Ascending ? query.OrderBy(e => e.Timestamp) : query.OrderByDescending(e => e.Timestamp),
            };

            if (!String.IsNullOrEmpty(OriginFilter)) query = query.Where(e => e.Origin.Contains(OriginFilter));
            if (!String.IsNullOrEmpty(MessageFilter)) query = query.Where(e => e.Message.Contains(MessageFilter));
            if (!String.IsNullOrEmpty(StatusFilter)) query = query.Where(e => e.Status.Contains(StatusFilter));

            ParcEvents = await PaginatedList<ParcEvent>.CreateAsync(
                source: query,
                pageIndex: pageIndex ?? 1,
                pageSize: PageSize);
        }
    }
}
