using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Parc.Web
{
    public class PaginatedList<T> : List<T>
    {
        public int SourceCount { get; private set; }
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }

        public PaginatedList(List<T> items, int sourceCount, int pageIndex, int pageSize)
        {
            SourceCount = sourceCount;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(sourceCount / (double)pageSize);
            PageSize = pageSize;

            this.AddRange(items);
        }

        public bool HasPreviousPage => (PageIndex > 1);

        public bool HasNextPage => (PageIndex < TotalPages);

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            List<T> items = await source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            int sourceCount = await source.CountAsync();

            return new PaginatedList<T>(items, sourceCount, pageIndex, pageSize);
        }
    }
}