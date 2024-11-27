using System.Collections.Generic;

namespace Library.Core.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; }
        public int TotalItems { get; }
        public int Page { get; }
        public int PageSize { get; }

        public PagedResult(IEnumerable<T> items, int totalItems, int page, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            Page = page;
            PageSize = pageSize;
        }
    }
}