using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacqlEngine.Queries.Common {

    public class FilterCondition {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool IsFieldComparison { get; set; } // New property to indicate field-to-field comparison

    }
}
