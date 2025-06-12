using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;
using YacqlEngine.Queries.Common;

namespace YacqlEngine.Queries {

    public class TeamQueryHandler : BaseQueryHandler<(string TeamName, Team Team)> {


        protected override object? GetFieldValue((string TeamName, Team Team) item, string fieldName) => throw new NotImplementedException();
        protected override IEnumerable<(string TeamName, Team Team)> GetFilteredData(List<FilterCondition> filters) => throw new NotImplementedException();
        protected override string GetGroupingKey((string TeamName, Team Team) item) => throw new NotImplementedException();
        protected override Dictionary<string, object?> ProjectItem((string TeamName, Team Team) item, List<FieldSelection> regularFields) => throw new NotImplementedException();
    }
}
