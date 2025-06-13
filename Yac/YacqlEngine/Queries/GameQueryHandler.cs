using YacData.Models;
using YacqlEngine.Helpers;
using YacqlEngine.Queries.Common;

namespace YacqlEngine.Queries {



    public class GameQueryHandler : BaseQueryHandler<Dictionary<string, object>> {

        protected override IEnumerable<Dictionary<string, object>> GetFilteredData(List<FilterCondition> filters) {

            var gameRecords = _db.GetCollection<GameRecord>("gamerecords").FindAll().ToList();

            var filtered = new List<Dictionary<string, object>>();

            foreach (var game in gameRecords) {

                var flat = GenericFlattener.FlattenFull(game);

                foreach (var filter in filters) {

                    var statName = filter.Field switch {
                        "passing_yards" => ("PassingStats", "Yards"),
                        _ => ("", "")
                    };

                    var resolvedStatKey = ResolvePassingStatField(filter.Field);

                    var fieldValues = flat.Keys.Select(n => n.ToString()).Where(n => n.Contains(resolvedStatKey.Value.ObjectName) && n.EndsWith("." + resolvedStatKey.Value.PropertyName));

                    foreach (var value in fieldValues) {

                        var fieldValue = flat[value];

                        if (Compare(Convert.ToDecimal(fieldValue), Convert.ToDecimal(filter.Value), filter.Operator)) {
                            filtered.Add(flat);
                        }

                    }

                }
            }


            return filtered;

        }

        private static bool Compare(decimal left, decimal right, string op) =>
           op switch {
               "==" => left == right,
               "!=" => left != right,
               ">" => left > right,
               "<" => left < right,
               ">=" => left >= right,
               "<=" => left <= right,
               _ => throw new NotSupportedException($"Unknown operator {op}")
           };

        protected override Dictionary<string, object?> ProjectItem(Dictionary<string, object> item, List<FieldSelection> regularFields) {

            var projected = new Dictionary<string, object?>();

            foreach (var field in regularFields) {

                var fieldName = field.Field.ToLower() switch {
                    "year" => "Game.Year",
                    "week" => "Game.Week",
                    _ => ""
                };

                if (String.IsNullOrEmpty(fieldName)) { 

                    var resolvedStatKey = ResolvePassingStatField(field.Field);

                    var fieldValues = item.Keys.Select(n => n.ToString()).Where(n => n.Contains(resolvedStatKey.Value.ObjectName) && n.EndsWith("." + resolvedStatKey.Value.PropertyName));

                    var homeCount = 0;

                    var awayCount = 0;

                    foreach (var v in fieldValues) {

                        var team = v.ToString().Split("_")[0];

                        var isHomeTeam = NFLTeamConverter.GetTeamAbbreviation(item["Game.HomeTeamName"].ToString()) == NFLTeamConverter.GetTeamAbbreviation(team);

                        if (isHomeTeam) {

                            homeCount++;

                        } else {
                            awayCount++;
                        }

                        var prefix = isHomeTeam ? "HOME_" : "AWAY_";

                        var appendix = isHomeTeam ? homeCount > 1 ? homeCount.ToString() : "" : awayCount > 1 ? awayCount.ToString() : "";

                        projected.Add(prefix + field.Field + appendix, item[v]);


                    }

                } else {

                    projected.Add(field.Field, item[fieldName]);
                }

            }

            return projected;

        }
        protected override string GetGroupingKey(Dictionary<string, object> item) => throw new NotImplementedException();
        protected override object? GetFieldValue(Dictionary<string, object> item, string fieldName) => throw new NotImplementedException();

        //private IEnumerable<(string TeamName, TeamSeason Season)> FilterGames(
        //    List<GameRecord> gameRecords,
        //    List<FilterCondition> filters) {

        //    return gameRecords.Where(entry => filters.All(filter => ApplyFilter(entry, filter)));
        //}

        //private bool ApplyFilter(GameRecord gameRecord, FilterCondition filter) {

        //    var leftProp = typeof(GameRecord).GetProperty(filter.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        //    if (leftProp == null)
        //        return false;

        //    var leftValue = leftProp.GetValue(season);

        //    if (filter.IsFieldComparison) {
        //        // Field-to-field comparison (e.g., wins > losses)
        //        return CompareFields(leftValue, filter.Operator, season, filter.Value);
        //    } else {
        //        // Field-to-value comparison (e.g., wins > 10)
        //        return CompareFieldToValue(leftValue, filter.Operator, filter.Value);
        //    }
        //}

        public static (string ObjectName, string PropertyName)? ResolvePassingStatField(string field) {
            var normalized = field.ToLowerInvariant();

            // Map of normalized aliases to (PropertyName, ObjectName)
            var map = new Dictionary<string, (string PropertyName, string ObjectName)> {
                // Basic stats
                ["completions"] = ("Completions", "PassingStats"),
                ["cmp"] = ("Completions", "PassingStats"),

                ["attempts"] = ("Attempts", "PassingStats"),
                ["att"] = ("Attempts", "PassingStats"),

                ["passing_yards"] = ("Yards", "PassingStats"),
                ["yards"] = ("Yards", "PassingStats"),
                ["yds"] = ("Yards", "PassingStats"),

                ["first_downs"] = ("FirstDowns", "PassingStats"),
                ["1d"] = ("FirstDowns", "PassingStats"),

                ["first_down_rate"] = ("FirstDownRate", "PassingStats"),
                ["1d%"] = ("FirstDownRate", "PassingStats"),

                // Air yards
                ["intended_air_yards"] = ("IntendedAirYards", "PassingStats"),
                ["iay"] = ("IntendedAirYards", "PassingStats"),

                ["iay_per_attempt"] = ("IntendedAirYardsPerAttempt", "PassingStats"),
                ["iay/pa"] = ("IntendedAirYardsPerAttempt", "PassingStats"),

                ["completed_air_yards"] = ("CompletedAirYards", "PassingStats"),
                ["cay"] = ("CompletedAirYards", "PassingStats"),

                ["cay_per_completion"] = ("CompletedAirYardsPerCompletion", "PassingStats"),
                ["cay/cmp"] = ("CompletedAirYardsPerCompletion", "PassingStats"),

                ["cay_per_attempt"] = ("CompletedAirYardsPerAttempt", "PassingStats"),
                ["cay/pa"] = ("CompletedAirYardsPerAttempt", "PassingStats"),

                ["yards_after_catch"] = ("YardsAfterCatch", "PassingStats"),
                ["yac"] = ("YardsAfterCatch", "PassingStats"),

                ["yac_per_completion"] = ("YardsAfterCatchPerCompletion", "PassingStats"),
                ["yac/cmp"] = ("YardsAfterCatchPerCompletion", "PassingStats"),

                // Accuracy and drops
                ["drops"] = ("Drops", "PassingStats"),
                ["drop_rate"] = ("DropRate", "PassingStats"),
                ["drop%"] = ("DropRate", "PassingStats"),

                ["bad_throws"] = ("BadThrows", "PassingStats"),
                ["badth"] = ("BadThrows", "PassingStats"),

                ["bad_throw_rate"] = ("BadThrowRate", "PassingStats"),
                ["bad%"] = ("BadThrowRate", "PassingStats"),

                // Pressure
                ["sacks"] = ("Sacks", "PassingStats"),
                ["sk"] = ("Sacks", "PassingStats"),

                ["blitzes_faced"] = ("BlitzesFaced", "PassingStats"),
                ["bltz"] = ("BlitzesFaced", "PassingStats"),

                ["hurries"] = ("Hurries", "PassingStats"),
                ["hrry"] = ("Hurries", "PassingStats"),

                ["qb_hits"] = ("QBHits", "PassingStats"),
                ["hits"] = ("QBHits", "PassingStats"),

                ["pressures"] = ("Pressures", "PassingStats"),
                ["prss"] = ("Pressures", "PassingStats"),

                ["pressure_rate"] = ("PressureRate", "PassingStats"),
                ["prss%"] = ("PressureRate", "PassingStats"),

                // Scrambles
                ["scrambles"] = ("Scrambles", "PassingStats"),
                ["scrm"] = ("Scrambles", "PassingStats"),

                ["yards_per_scramble"] = ("YardsPerScramble", "PassingStats"),
                ["yds/scr"] = ("YardsPerScramble", "PassingStats")
            };

            return map.TryGetValue(normalized, out var result) ? (result.ObjectName, result.PropertyName) : null;
        }


    }
}
