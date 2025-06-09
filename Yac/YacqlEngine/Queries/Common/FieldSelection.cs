using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacqlEngine.Queries.Common {

    public class FieldSelection {

        public string Field { get; set; }

        public string? Aggregation { get; set; }
    }
}
