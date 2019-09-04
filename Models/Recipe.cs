using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elastic_search_demo.Models
{
    [ElasticsearchType(RelationName = "recipe")]
    public class Recipe
    {
        public string Id { get; set; }
        // Mark the Name property as a Completion field
        // In part 4, when we implement the Autocomplete method, you'll find out why this is needed
        [Completion]
        public string Name { get; set; }
        // Specify the Ingredients as a text field to enable full-text search
        [Text]
        public string Ingredients { get; set; }

        public string Url { get; set; }

        public string Image { get; set; }

        public string CookTime { get; set; }

        public string RecipeYield { get; set; }

        public DateTime? DatePublished { get; set; }

        public string PrepTime { get; set; }
        // Specify the Description as a text field to enable full-text search
        [Text]
        public string Description { get; set; }
    }
}
