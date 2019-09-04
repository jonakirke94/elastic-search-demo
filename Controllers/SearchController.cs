using elastic_search_demo.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace elastic_search_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("search")]
        public async Task<JsonResult> Search([FromQuery]string query, int page = 1, int pageSize = 10)
        {
            var result = await _searchService.Search(query, page, pageSize);
            return new JsonResult(result);
        }

        [HttpGet("autocomplete")]
        public async Task<JsonResult> Autocomplete([FromQuery]string query)
        {
            var result = await _searchService.Autocomplete(query);
            return new JsonResult(result);
        }
    }
}
