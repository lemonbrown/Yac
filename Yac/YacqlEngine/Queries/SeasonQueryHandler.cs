using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;
using YacqlEngine.Queries.Common;

namespace YacqlEngine.Queries {

    // Season-specific query handler
    public class SeasonQueryHandler : BaseQueryHandler<(string TeamName, TeamSeason Season)> {
        protected override IEnumerable<(string TeamName, TeamSeason Season)> GetFilteredData(List<FilterCondition> filters) {
            var teams = _db.GetCollection<Team>("teams").FindAll().ToList();

            var flatSeasons = teams
                .SelectMany(team => team.Seasons.Select(season => (team.Name, season)))
                .ToList();

            return FilterSeasons(flatSeasons, filters);
        }

        private IEnumerable<(string TeamName, TeamSeason Season)> FilterSeasons(
            List<(string TeamName, TeamSeason Season)> seasons,
            List<FilterCondition> filters) {
            return seasons.Where(entry => filters.All(filter => ApplyFilter(entry.Season, filter)));
        }

        //private bool ApplyFilter(TeamSeason season, FilterCondition filter) {
        //    var prop = typeof(TeamSeason).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        //    if (prop == null)
        //        return false;

        //    var val = prop.GetValue(season)?.ToString()?.ToLower();
        //    var filterVal = filter.Value.ToLower();

        //    return filter.Operator switch {
        //        "=" => val == filterVal,
        //        ">" => double.TryParse(val, out var v1) && double.TryParse(filter.Value, out var v2) && v1 > v2,
        //        "<" => double.TryParse(val, out var v3) && double.TryParse(filter.Value, out var v4) && v3 < v4,
        //        ">=" => double.TryParse(val, out var v5) && double.TryParse(filter.Value, out var v6) && v5 >= v6,
        //        "<=" => double.TryParse(val, out var v7) && double.TryParse(filter.Value, out var v8) && v7 <= v8,
        //        "!=" => val != filterVal,
        //        _ => false
        //    };
        //}

        // Enhanced filter application method
        private bool ApplyFilter(TeamSeason season, FilterCondition filter) {
            var leftProp = typeof(TeamSeason).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (leftProp == null)
                return false;

            var leftValue = leftProp.GetValue(season);

            if (filter.IsFieldComparison) {
                // Field-to-field comparison (e.g., wins > losses)
                return CompareFields(leftValue, filter.Operator, season, filter.Value);
            } else {
                // Field-to-value comparison (e.g., wins > 10)
                return CompareFieldToValue(leftValue, filter.Operator, filter.Value);
            }
        }


        private bool CompareFields(object? leftValue, string operatorStr, TeamSeason season, string rightFieldName) {
            var rightProp = typeof(TeamSeason).GetProperty(rightFieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (rightProp == null)
                return false;

            var rightValue = rightProp.GetValue(season);

            // Handle numeric comparisons
            if (TryGetNumericValue(leftValue, out double leftNum) && TryGetNumericValue(rightValue, out double rightNum)) {
                return operatorStr switch {
                    "=" => Math.Abs(leftNum - rightNum) < 0.0001, // Use small epsilon for double comparison
                    ">" => leftNum > rightNum,
                    "<" => leftNum < rightNum,
                    ">=" => leftNum >= rightNum,
                    "<=" => leftNum <= rightNum,
                    "!=" => Math.Abs(leftNum - rightNum) >= 0.0001,
                    _ => false
                };
            }

            // Handle string comparisons
            var leftStr = leftValue?.ToString()?.ToLower() ?? "";
            var rightStr = rightValue?.ToString()?.ToLower() ?? "";

            return operatorStr switch {
                "=" => leftStr == rightStr,
                "!=" => leftStr != rightStr,
                // For non-numeric values, only allow equality comparisons
                _ => false
            };
        }


        private bool CompareFieldToValue(object? fieldValue, string operatorStr, string compareValue) {
            // Handle numeric comparisons
            if (TryGetNumericValue(fieldValue, out double fieldNum) && double.TryParse(compareValue, out double compareNum)) {
                return operatorStr switch {
                    "=" => Math.Abs(fieldNum - compareNum) < 0.0001,
                    ">" => fieldNum > compareNum,
                    "<" => fieldNum < compareNum,
                    ">=" => fieldNum >= compareNum,
                    "<=" => fieldNum <= compareNum,
                    "!=" => Math.Abs(fieldNum - compareNum) >= 0.0001,
                    _ => false
                };
            }

            // Handle string comparisons
            var fieldStr = fieldValue?.ToString()?.ToLower() ?? "";
            var compareStr = compareValue.ToLower().Replace("'", "");

            return operatorStr switch {
                "=" => fieldStr == compareStr,
                "!=" => fieldStr != compareStr,
                _ => false
            };
        }



        private bool TryGetNumericValue(object? value, out double numericValue) {
            numericValue = 0.0;

            return value switch {
                int i => SetAndReturn(i, out numericValue),
                double d => SetAndReturn(d, out numericValue),
                float f => SetAndReturn(f, out numericValue),
                decimal dec => SetAndReturn((double)dec, out numericValue),
                long l => SetAndReturn(l, out numericValue),
                short s => SetAndReturn(s, out numericValue),
                string str when double.TryParse(str, out var parsed) => SetAndReturn(parsed, out numericValue),
                _ => false
            };
        }

        private bool SetAndReturn(double value, out double output) {
            output = value;
            return true;
        }

        protected override Dictionary<string, object?> ProjectItem((string TeamName, TeamSeason Season) item, List<FieldSelection> regularFields) {

            var result = new Dictionary<string, object?> { { "team", item.TeamName } };

            foreach (var field in regularFields) {
                var prop = typeof(TeamSeason).GetProperty(field.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop != null) {
                    result[field.Field] = prop.GetValue(item.Season);
                }
            }

            if (!regularFields.Any()) {

                result["YEAR"] = item.Season.Year;

            }

            return result;
        }

        protected override double GetNumericValue((string TeamName, TeamSeason Season) item, string fieldName) {
            // Get the property from the TeamSeason object, not the tuple
            var prop = typeof(TeamSeason).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return 0.0;

            var val = prop.GetValue(item.Season); // Use item.Season instead of item
            return val switch {
                int i => (double)i,
                double d => Math.Round(d, 2),
                float f => (double)f,
                decimal dec => (double)dec,
                long l => (double)l,
                short s => (double)s,
                _ => 0.0
            };
        }

        protected override string GetGroupingKey((string TeamName, TeamSeason Season) item) {
            return item.TeamName;
        }

        protected override string GetGroupingFieldName() => "team";

        protected override object? GetFieldValue((string TeamName, TeamSeason Season) item, string fieldName) {
            // Handle special case for team name
            if (fieldName.ToLower() == "team" || fieldName.ToLower() == "teamname") {
                return item.TeamName;
            }

            // Get field from TeamSeason object
            var prop = typeof(TeamSeason).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return prop?.GetValue(item.Season);
        }
    }
}
