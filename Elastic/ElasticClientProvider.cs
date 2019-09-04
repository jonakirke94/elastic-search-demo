using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elastic_search_demo.Elastic
{
    public class ElasticClientProvider
    {
        public ElasticClientProvider(IOptions<ElasticConnectionSettings> settings)
        {
            var connectionSettings = new ConnectionSettings(new System.Uri(settings.Value.ClusterUrl));

            connectionSettings.EnableDebugMode();

            if (settings.Value.DefaultIndex != null)
            {
                connectionSettings.DefaultIndex(settings.Value.DefaultIndex);
            }

            this.Client = new ElasticClient(connectionSettings);
        }

        public ElasticClient Client { get; }
    }
}
