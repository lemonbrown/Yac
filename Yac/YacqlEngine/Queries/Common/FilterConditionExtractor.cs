using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace YacqlEngine.Queries.Common {

    public class FilterConditionExtractor {

        // Updated field extraction method to detect field comparisons
        public static List<FilterCondition> ExtractConditions(NqlParser.WhereClauseContext context) {
            var conditions = new List<FilterCondition>();

            if (context == null)
                return conditions;

            foreach (var conditionContext in context.condition()) {
                var field = conditionContext.NAME().GetText();
                var operatorStr = conditionContext.@operator().GetText();
                var value = conditionContext.value().GetText();

                // Determine if this is a field comparison
                bool isFieldComparison = IsFieldName(value);

                conditions.Add(new FilterCondition {
                    Field = field,
                    Operator = operatorStr,
                    Value = value,
                    IsFieldComparison = isFieldComparison
                });
            }

            return conditions;
        }

        // Helper method to determine if a value is likely a field name
        public static bool IsFieldName(string value) {
            // Check if the value looks like a field name (not a number, not quoted string)
            if (double.TryParse(value, out _))
                return false; // It's a number

            if (value.StartsWith("'") && value.EndsWith("'"))
                return false; // It's a quoted string

            // Check if it's a known field name from your TeamSeason class
            var knownFields = new[]
            {
        "wins", "losses", "points_for", "points_against", "year",
        "touchdowns", "field_goals", "turnovers", "penalties",
        "home_wins", "away_wins", "division_wins", "conference_wins"
        // Add more field names as needed
    };

            return knownFields.Contains(value.ToLower()) ||
                   typeof(TeamSeason).GetProperty(value, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

    }
}
