using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace elastic_search_demo.Models
{
    public class IndexResult
    {
        public bool IsValid { get; set; }

        public string ErrorReason { get; set; }

        public Exception Exception { get; set; }
    }
}
