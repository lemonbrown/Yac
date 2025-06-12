using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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

                    var fieldValues = flat.Keys.Select(n => n.ToString()).Where(n => n.Contains(statName.Item1) && n.EndsWith("."+statName.Item2));

                    foreach(var value in fieldValues) {

                        var fieldValue = flat[value];

                        if(Compare(Convert.ToDecimal(fieldValue), Convert.ToDecimal(filter.Value), filter.Operator)) {
                            filtered.Add(flat);
                        }

                    }

                    //var statListProp = game.GetType().GetProperty(statName);
                    //if (statListProp == null)
                    //    continue;

                    //var statList = (System.Collections.IEnumerable)statListProp.GetValue(game);
                    //if (statList == null)
                    //    continue;

                    //bool matchFound = false;
                    //foreach (var stat in statList) {
                    //    var fieldProp = stat.GetType().GetProperty(filter.Field == "passing_yards" ? "Yards" : "");
                    //    if (fieldProp == null)
                    //        continue;

                    //    var fieldValue = fieldProp.GetValue(stat);
                    //    if (fieldValue == null)
                    //        continue;

                    //    var fieldDecimal = Convert.ToDecimal(fieldValue);
                    //    var clauseDecimal = Convert.ToDecimal(filter.Value);

                    //    if (Compare(fieldDecimal, clauseDecimal, filter.Operator)) {
                    //        matchFound = true;

                    //        filtered.Add(game);
                    //        break;
                    //    }
                    //}

                    //if (!matchFound)
                    //    continue;
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

        //protected override Dictionary<string, object?> ProjectItem(GameRecord item, List<FieldSelection> regularFields) {


        //    var result = new Dictionary<string, object?>();

        //    if (!regularFields.Any()) {

        //        result.Add("Year", item.Year);
        //        result.Add("Week", item.Week);
        //        result.Add("Home Team", item.HomeTeamName);
        //        result.Add("Home Score", item.HomeTeamScore);
        //        result.Add("Away Team", item.AwayTeamName);
        //        result.Add("Away Score", item.AwayTeamScore);
        //    }


        //    //foreach (var field in regularFields) {

        //    //    var fp = item.GetType().GetProperty(field.Field);

        //    //    if (fp != null) {

        //    //        var fieldValue = fp.GetValue(item);

        //    //        if (fieldValue == null)
        //    //            break;

        //    //        result.Add(field.Field, fieldValue);

        //    //        continue;
        //    //    }


        //    //}

        //    foreach (var field in regularFields) {

        //        var fieldName = field.Field switch {
        //            "passing_yards" => "Yards",
        //            "qb" => "PlayerName",
        //            _ => field.Field
        //        };

        //        var statName = field.Field switch {
        //            "passing_yards" => "PassingStats",
        //            _ => field.Field
        //        };

        //        var fp = item.GetType().GetProperty(fieldName);

        //        if(fp != null) {

        //            var fieldValue = fp.GetValue(item);

        //            if (fieldValue == null)
        //                break;

        //            result.Add(field.Field, fieldValue);

        //            continue;
        //        }

        //        var statListProp = item.GetType().GetProperty(statName);

        //        if (statListProp == null)
        //            continue;

        //        var statList = (System.Collections.IEnumerable)statListProp.GetValue(item);
        //        if (statList == null)
        //            continue;

        //        bool matchFound = false;

        //        var count = 0;

        //        var homeCount = 0;

        //        var awayCount = 0;

        //        foreach (var stat in statList) {

        //            homeCount++;

        //            awayCount++;

        //            var teamProp = stat.GetType().GetProperty("TeamName");

        //            var teamName = teamProp.GetValue(stat).ToString();

        //            var isHomeTeam = NFLTeamConverter.GetTeamAbbreviation(item.HomeTeamName) == NFLTeamConverter.GetTeamAbbreviation(teamName);

        //            var fieldProp = stat.GetType().GetProperty(fieldName);
        //            if (fieldProp == null)
        //                break;

        //            var fieldValue = fieldProp.GetValue(stat);
        //            if (fieldValue == null)
        //                break;

        //            var fieldDecimal = Convert.ToDecimal(fieldValue);                    

        //            result.Add((isHomeTeam ? (homeCount > 1 ? $"{homeCount}" : "") + "HOME_" : (awayCount > 1 ? $"{awayCount}" : "") + "AWAY_") + field.Field, fieldDecimal);
        //        }                
        //    }

        //    return result;

        //}
        //protected override string GetGroupingKey(GameRecord item) => throw new NotImplementedException();
        //protected override object? GetFieldValue(GameRecord item, string fieldName) => throw new NotImplementedException();
        protected override Dictionary<string, object?> ProjectItem(Dictionary<string, object> item, List<FieldSelection> regularFields) {

            var projected = new Dictionary<string, object?>();

            foreach (var field in regularFields) {

                var fieldName = field.Field.ToLower() switch {
                    "year" => "Game.Year",
                    "week" => "Game.Week",
                    _ => ""
                };

                if (String.IsNullOrEmpty(fieldName)) {

                    var combinedKeyName = field.Field.ToLower() switch {
                        "passing_yards" => ("PassingStats", "Yards"),
                        _ => ("", "")
                    };

                    var fieldValues = item.Keys.Select(n => n.ToString()).Where(n => n.Contains(combinedKeyName.Item1) && n.EndsWith("." + combinedKeyName.Item2));

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

    }
}
