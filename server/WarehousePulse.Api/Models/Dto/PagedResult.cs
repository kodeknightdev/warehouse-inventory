using System.Collections.Generic;

namespace WarehousePulse.Api.Models.Dto
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
