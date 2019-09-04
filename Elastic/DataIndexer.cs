using elastic_search_demo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace elastic_search_demo.Elastic
{
    public class DataIndexer
    {
        private readonly ElasticClient client;
        private readonly string contentRootPath;
        private readonly string defaultIndex;

        public DataIndexer(ElasticClientProvider clientProvider, IHostingEnvironment env, IOptions<ElasticConnectionSettings> settings)
        {
            client = clientProvider.Client;
            contentRootPath = Path.Combine(env.ContentRootPath, "data"); 
            defaultIndex = settings.Value.DefaultIndex;
        }

        public async Task<IndexResult> IndexRecipesFromFile(string fileName, bool deleteIndexIfExists, string index = null)
        {
            SanitizeIndexName(ref index);
            Recipe[] mappedCollection = await ParseJsonFile(fileName);
            await DeleteIndexIfExists(index, deleteIndexIfExists);
            await CreateIndexIfItDoesntExist(index);
            return await IndexDocuments(mappedCollection, index);
        }

        private async Task CreateIndexIfItDoesntExist(string index)
        {
            if (!client.Indices.Exists(index).Exists)
            {
                var indexDescriptor = new CreateIndexDescriptor(index).Map<Recipe>(m => m.AutoMap());
                await client.Indices.CreateAsync(index, i => indexDescriptor);
            }
        }

        private async Task DeleteIndexIfExists(string index, bool shouldDeleteIndex)
        {
            if (client.Indices.Exists(index).Exists && shouldDeleteIndex)
            {
                await client.Indices.DeleteAsync(index);
            }
        }

        private void SanitizeIndexName(ref string index)
        {
            // The index must be lowercase, this is a requirement from Elastic
            if (index == null)
            {
                index = defaultIndex;
            }
            else
            {
                index = index.ToLower();
            }
        }

        private async Task<IndexResult> IndexDocuments(Recipe[] mappedCollection, string index)
        {
            int batchSize = 10000; // magic
            int totalBatches = (int)Math.Ceiling((double)mappedCollection.Length / batchSize);

            for (int i = 0; i < totalBatches; i++)
            {
                var response = await this.client.IndexManyAsync(mappedCollection.Skip(i * batchSize).Take(batchSize), index);

                if (!response.IsValid)
                {
                    return new IndexResult
                    {
                        IsValid = false,
                        ErrorReason = response.ServerError?.Error?.Reason,
                        Exception = response.OriginalException
                    };
                }
                else
                {
                    Debug.WriteLine($"Successfully indexed batch {i + 1}");
                }
            }

            return new IndexResult
            {
                IsValid = true
            };
        }

        private async Task<Recipe[]> ParseJsonFile(string fileName)
        {
            using (FileStream fs = new FileStream(Path.Combine(contentRootPath, fileName), FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    // Won't be efficient with large files
                    string rawJsonCollection = await reader.ReadToEndAsync();

                    Recipe[] mappedCollection = JsonConvert.DeserializeObject<Recipe[]>(rawJsonCollection, new JsonSerializerSettings
                    {
                        Error = HandleDeserializationError
                    });

                    return mappedCollection;
                }
            }
        }

        // https://stackoverflow.com/questions/26107656/ignore-parsing-errors-during-json-net-data-parsing
        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }

}

