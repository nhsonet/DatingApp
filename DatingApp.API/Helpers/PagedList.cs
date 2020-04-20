using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalItem { get; set; }
        public int TotalPage { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            TotalItem = count;
            TotalPage = (int)Math.Ceiling(count/ (double)pageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageSize, int pageNumber)
        {
            var count = await source.CountAsync();

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}