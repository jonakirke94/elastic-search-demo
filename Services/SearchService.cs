using elastic_search_demo.Elastic;
using elastic_search_demo.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elastic_search_demo.Services
{
    public class SearchService : ISearchService
    {
        private readonly ElasticClient _client;

        public SearchService(ElasticClientProvider clientProvider)
        {
            _client = clientProvider.Client;
        }
        
        public async Task<SearchResult<Recipe>> Search(string query, int page, int pageSize)
        {
            var response = await _client.SearchAsync<Recipe>(searchDescriptor => searchDescriptor
                    .Query(queryContainerDescriptor => queryContainerDescriptor
                        .Bool(queryDescriptor => queryDescriptor
                            .Must(queryStringQuery => queryStringQuery
                                .QueryString(queryString => queryString
                                    .Query(query)))))
                                        .From((page - 1) * pageSize)
                                        .Size(pageSize));

            return new SearchResult<Recipe>
            {
                Total = response.Total,
                ElapsedMilliseconds = response.Took,
                Page = page,
                PageSize = pageSize,
                Results = response.Documents
            };
        }

        public async Task<List<AutocompleteResult>> Autocomplete(string query)
        {
            var response = await _client.SearchAsync<Recipe>(sr => sr
                .Suggest(scd => scd
                    .Completion("recipe-name-completion", cs => cs
                        .Prefix(query)
                        .Fuzzy(fsd => fsd
                            .Fuzziness(Fuzziness.Auto))
                        .Field(r => r.Name))));

            return ExtractAutocompleteSuggestions(response);
        }

        private List<AutocompleteResult> ExtractAutocompleteSuggestions(ISearchResponse<Recipe> response)
        {
            // same key as we used in the completion query
            var suggestions = response.Suggest["recipe-name-completion"].Select(s => s.Options);

            return suggestions
                .SelectMany(option => option
                .Select(opt => new AutocompleteResult() { Id = opt.Source.Id, Name = opt.Source.Name }))
                .ToList();
        }
    }
}
