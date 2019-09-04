using elastic_search_demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elastic_search_demo.Services
{
    public interface ISearchService
    {
        Task<SearchResult<Recipe>> Search(string query, int page, int pageSize);
        Task<List<AutocompleteResult>> Autocomplete(string query);
    }
}
