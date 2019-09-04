using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elastic_search_demo.Models
{
    public class SearchResult<T>
    {
        public long Total { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<T> Results { get; set; }

        public long ElapsedMilliseconds { get; set; }
    }
}
