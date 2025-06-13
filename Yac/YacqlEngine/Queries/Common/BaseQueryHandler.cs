using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace YacqlEngine.Queries.Common {
    public abstract class BaseQueryHandler<T> : IDisposable{

        protected readonly LiteDatabase _db;
        protected readonly string _dbPath = "C:\\Users\\cameron\\source\\repos\\Yac\\Yac\\DataScraper\\bin\\Debug\\net8.0\\yac.db";

        protected BaseQueryHandler() {
            _db = new LiteDatabase(_dbPath);
        }

        protected abstract IEnumerable<T> GetFilteredData(List<FilterCondition> filters);
        protected abstract Dictionary<string, object?> ProjectItem(T item, List<FieldSelection> regularFields);
        protected abstract string GetGroupingKey(T item);

        public List<Dictionary<string, object?>> ExecuteQuery(List<FieldSelection> fields, List<FilterCondition> filters) {
            var filteredData = GetFilteredData(filters);

            var regularFields = fields.Where(f => f.Aggregation == null).ToList();
            var aggregatedFields = fields.Where(f => f.Aggregation != null).ToList();

            if (aggregatedFields.Any()) {
                return ExecuteAggregatedQuery(filteredData, regularFields, aggregatedFields);
            } else {
                return ExecuteSimpleQuery(filteredData, regularFields);
            }
        }

        private List<Dictionary<string, object?>> ExecuteSimpleQuery(IEnumerable<T> data, List<FieldSelection> regularFields) {
            return data.Select(item => ProjectItem(item, regularFields)).ToList();
        }

        private List<Dictionary<string, object?>> ExecuteAggregatedQuery(
            IEnumerable<T> data,
            List<FieldSelection> regularFields,
            List<FieldSelection> aggregatedFields) {
            var grouped = data.GroupBy(GetGroupingKey);

            return grouped.Select(group => {
                var result = new Dictionary<string, object?>();

                // Add grouping key
                AddGroupingInfo(result, group.Key);

                // Add regular fields (from first item)
                if (group.Any()) {
                    var firstItem = group.First();
                    var regularProjection = ProjectItem(firstItem, regularFields);
                    foreach (var kvp in regularProjection.Where(x => x.Key != GetGroupingFieldName())) {
                        result[kvp.Key] = kvp.Value;
                    }
                }

                // Add aggregated fields
                foreach (var field in aggregatedFields) {
                    var aggregatedValue = CalculateAggregation(group, field);
                    result[$"{field.Aggregation}_{field.Field}"] = aggregatedValue;

                    // For "most" and "least" aggregations, also include associated fields from the same record
                    if (field.Aggregation?.ToLower() == "most" || field.Aggregation?.ToLower() == "least") {
                        var targetItem = field.Aggregation.ToLower() == "most"
                            ? GetItemWithMax(group, field.Field)
                            : GetItemWithMin(group, field.Field);

                        // Add all regular fields from the record that had the max/min value
                        foreach (var regularField in regularFields) {
                            var fieldValue = GetFieldValue(targetItem, regularField.Field);
                            result[regularField.Field] = fieldValue;
                        }
                    }
                }

                // If no "most"/"least" aggregations, add regular fields from first item (existing behavior)
                if (!aggregatedFields.Any(f => f.Aggregation?.ToLower() == "most" || f.Aggregation?.ToLower() == "least")) {
                    if (group.Any()) {
                        var firstItem = group.First();
                        foreach (var field in regularFields) {
                            result[field.Field] = GetFieldValue(firstItem, field.Field);
                        }
                    }
                }

                return result;
            }).ToList();
        }
        protected abstract object? GetFieldValue(T item, string fieldName);


        protected virtual void AddGroupingInfo(Dictionary<string, object?> result, string groupKey) {
            result[GetGroupingFieldName()] = groupKey;
        }

        protected virtual string GetGroupingFieldName() => "group";

        private object? CalculateAggregation(IGrouping<string, T> group, FieldSelection field) {
            return field.Aggregation?.ToLower() switch {
                "total" => CalculateSum(group, field.Field),
                "avg" => CalculateAverage(group, field.Field),
                "most" => GetNumericValue(GetItemWithMax(group, field.Field), field.Field), // Get the max value
                "least" => GetNumericValue(GetItemWithMin(group, field.Field), field.Field), // Get the min value
                "count" => group.Count(),
                _ => null
            };
        }

        protected double CalculateSum(IGrouping<string, T> group, string fieldName) {
            return group.Select(item => GetNumericValue(item, fieldName)).Sum();
        }

        protected double CalculateAverage(IGrouping<string, T> group, string fieldName) {
            var values = group.Select(item => GetNumericValue(item, fieldName)).ToList();
            return values.Any() ? Math.Round(values.Average(), 2) : 0.0;
        }

        protected double CalculateMax(IGrouping<string, T> group, string fieldName) {
            return group.Select(item => GetNumericValue(item, fieldName)).DefaultIfEmpty(0.0).Max();
        }

        protected double CalculateMin(IGrouping<string, T> group, string fieldName) {
            return group.Select(item => GetNumericValue(item, fieldName)).DefaultIfEmpty(0.0).Min();
        }

        protected T GetItemWithMax(IGrouping<string, T> group, string fieldName) {
            return group.OrderByDescending(item => GetNumericValue(item, fieldName)).First();
        }

        // New method to get the item with minimum value for a field
        protected T GetItemWithMin(IGrouping<string, T> group, string fieldName) {
            return group.OrderBy(item => GetNumericValue(item, fieldName)).First();
        }


        // Make this virtual so derived classes can override it
        protected virtual double GetNumericValue(T item, string fieldName) {
            var prop = typeof(T).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
                return 0.0;

            var val = prop.GetValue(item);
            return val switch {
                int i => (double)i,
                double d => d,
                float f => (double)f,
                decimal dec => (double)dec,
                long l => (double)l,
                short s => (double)s,
                _ => 0.0
            };
        }

        public void Dispose() {
            _db?.Dispose();
        }


    }

}
