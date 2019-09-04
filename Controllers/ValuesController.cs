using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using elastic_search_demo.Elastic;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace elastic_search_demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataIndexer _indexer;

        public ValuesController(DataIndexer indexer)
        {
            _indexer = indexer;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("file")]
        public async Task<IActionResult> IndexDataFromFile([FromQuery]string fileName, string index, bool deleteIndexIfExists)
        {
            var response = await _indexer.IndexRecipesFromFile(fileName, deleteIndexIfExists, index);
            return Ok(response);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
