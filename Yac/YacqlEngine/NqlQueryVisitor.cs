using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacqlEngine {
    public class NqlQueryVisitor : NqlBaseVisitor<object> {
        private readonly List<PlayerStats> _players;
        private readonly List<TeamSchedule> _schedules;
        private readonly List<TeamRecord> _records;
        private readonly List<Game> _games;

        public NqlQueryVisitor(
            List<PlayerStats> players,
            List<TeamSchedule> schedules,
            List<TeamRecord> records,
            List<Game> games) {
            _players = players;
            _schedules = schedules;
            _records = records;
            _games = games;
        }

        public override object VisitPlayerQuery(NqlParser.PlayerQueryContext context) {
            string playerName = context.NAME().GetText().Trim();
            var filtered = _players
                .Where(p => p.Name.Contains(playerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (context.whereClause() is { } where) {
                foreach (var cond in where.condition()) {
                    string field = cond.NAME().GetText().Trim().ToLower();
                    string op = cond.@operator().GetText().Trim();
                    string val = cond.value().GetText().Trim();

                    filtered = ApplyCondition(filtered, field, op, val);
                }
            }

            if (filtered.Count == 0) {
                Console.WriteLine("⚠️ No matching results.");
                return null;
            }

            foreach (var p in filtered) {
                Console.WriteLine($"\n📊 {p.Name} — {p.Season} Season");
                Console.WriteLine($"  Yards: {p.PassingYards}");
                Console.WriteLine($"  TDs:   {p.Touchdowns}");
                Console.WriteLine($"  INTs:  {p.Interceptions}");
            }

            return filtered;
        }

        private List<PlayerStats> ApplyCondition(List<PlayerStats> players, string field, string op, string val) {
            return field switch {
                "season" => FilterNumeric(players, p => p.Season, op, val),
                "touchdowns" => FilterNumeric(players, p => p.Touchdowns, op, val),
                "interceptions" => FilterNumeric(players, p => p.Interceptions, op, val),
                "passingyards" or "yards" => FilterNumeric(players, p => p.PassingYards, op, val),
                _ => players
            };
        }

        private List<string> ExtractFields(NqlParser.FieldSelectionContext context) {
            var fields = new List<string>();

            if (context == null)
                return fields;

            foreach (var expr in context.fieldExpr()) {
                var allFields = expr.field();

                if (allFields.Count() == 2) {
                    var left = allFields[0].GetText().Trim();
                    var right = allFields[1].GetText().Trim();
                    fields.Add($"{left}/{right}"); // handle computed fields like td/int
                } else if (allFields.Count() == 1) {
                    fields.Add(allFields[0].GetText().Trim());
                }
            }

            return fields;
        }

        private List<QueryCondition> ExtractConditions(NqlParser.WhereClauseContext context) {
            var conditions = new List<QueryCondition>();

            if (context == null)
                return conditions;

            foreach (var cond in context.condition()) {
                var field = cond.NAME().GetText().Trim();
                var op = cond.@operator().GetText().Trim();
                var val = cond.value().GetText().Trim();

                conditions.Add(new QueryCondition {
                    Field = field,
                    Operator = op,
                    Value = val
                });
            }

            return conditions;
        }

        private List<string> ExtractNames(ParserRuleContext listContext) {
            var names = new List<string>();
            if (listContext == null)
                return names;

            // We assume the context has multiple NAME tokens as children
            // so we find all tokens of type NAME in the subtree

            // Use GetTokens or find children of type TerminalNode for NAME tokens
            var nameTokens = listContext.GetTokens(NqlParser.NAME);

            foreach (var token in nameTokens) {
                var name = token.GetText().Trim();
                if (!string.IsNullOrEmpty(name))
                    names.Add(name);
            }

            return names;
        }


        private List<PlayerStats> FilterNumeric(List<PlayerStats> players, Func<PlayerStats, int> selector, string op, string val) {
            if (!int.TryParse(val, out int num))
                return players;

            return op switch {
                "=" => players.Where(p => selector(p) == num).ToList(),
                ">" => players.Where(p => selector(p) > num).ToList(),
                "<" => players.Where(p => selector(p) < num).ToList(),
                ">=" => players.Where(p => selector(p) >= num).ToList(),
                "<=" => players.Where(p => selector(p) <= num).ToList(),
                _ => players
            };
        }

        private List<string> ExtractGames(ParserRuleContext gameListContext) {
            var games = new List<string>();
            if (gameListContext == null)
                return games;

            var gameTokens = gameListContext.GetTokens(NqlParser.NAME);
            foreach (var token in gameTokens) {
                var game = token.GetText().Trim();
                if (!string.IsNullOrEmpty(game))
                    games.Add(game);
            }

            return games;
        }


        public override object VisitCompareQuery(NqlParser.CompareQueryContext context) {
            var fields = ExtractFields(context.fieldSelection());

            var filters = ExtractConditions(context.whereClause());

            var compareResults = new object();

            if (context.compareTarget().playerList() is not null) {
                var players = ExtractNames(context.compareTarget().playerList());
                var playerResult = ComparePlayers(players, fields, filters, _players);

                compareResults = playerResult;
            }
            //else if (context.compareTarget().teamList() is not null) {
            //    var teams = ExtractNames(context.compareTarget().teamList());
            //    CompareTeams(teams, fields, filters);
            //} else if (context.compareTarget().gameList() is not null) {
            //    var games = ExtractGames(context.compareTarget().gameList());
            //    CompareGames(games, fields, filters);
            //}

            return compareResults;
        }

        public void CompareGames(List<string> gameIds, List<string> fields, List<QueryCondition> filters) {
            var allStats = _games;

            // Filter by game IDs
            var filteredData = allStats
                .Where(g => gameIds.Any(id => g.GameId.Equals(id, StringComparison.OrdinalIgnoreCase)
                                             || g.Opponent.Equals(id, StringComparison.OrdinalIgnoreCase)))
                .Where(g => ApplyGameFilters(g, filters))
                .ToList();

            if (!filteredData.Any()) {
                Console.WriteLine("⚠️ No data found for the specified games with given filters.");
                return;
            }

            Console.WriteLine($"\n🎲 Comparing Games: {string.Join(", ", gameIds)}");

            foreach (var field in fields) {
                if (field.Contains('/')) // computed ratio like td/int
                {
                    var parts = field.Split('/');
                    var numerator = parts[0].ToLower();
                    var denominator = parts[1].ToLower();

                    double numeratorTotal = GetGameFieldTotal(filteredData, numerator);
                    double denominatorTotal = GetGameFieldTotal(filteredData, denominator);

                    if (denominatorTotal == 0)
                        Console.WriteLine($"  {field}: undefined (division by zero)");
                    else {
                        double ratio = numeratorTotal / denominatorTotal;
                        Console.WriteLine($"  {field}: {ratio:F2}");
                    }
                } else // simple totals
                  {
                    double total = GetGameFieldTotal(filteredData, field.ToLower());
                    Console.WriteLine($"  {field}: {total}");
                }
            }
        }

        private double GetGameFieldTotal(List<Game> data, string fieldName) {
            return fieldName switch {
                //"touchdowns" or "td" => data.Sum(g => g.Touchdowns),
                //"interceptions" or "int" => data.Sum(g => g.Interceptions),
                //"yards" or "passingyards" => data.Sum(g => g.PassingYards),
                //"points" or "score" => data.Sum(g => g.Points),
                "games" => data.Count,
                _ => 0
            };
        }

        private bool ApplyGameFilters(Game stat, List<QueryCondition> filters) {
            foreach (var cond in filters) {
                var field = cond.Field.ToLower();
                var op = cond.Operator;
                var val = cond.Value;

                bool result = field switch {
                    "season" => CompareInt(stat.Season, op, val),
                    //"player" => CompareString(stat.PlayerName, op, val),
                    //"team" => CompareString(stat.TeamName, op, val),
                    _ => true
                };

                if (!result)
                    return false;
            }

            return true;
        }

        // Reuse CompareInt and CompareString helpers as before



        // Helper to apply filters to a PlayerStats entry
        private bool ApplyFilters(PlayerStats stat, List<QueryCondition> filters) {
            foreach (var cond in filters) {
                var field = cond.Field.ToLower();
                var op = cond.Operator;
                var val = cond.Value;

                bool result = field switch {
                    "season" => CompareInt(stat.Season, op, val),
                    //"opponent" => CompareString(stat.Opponent, op, val),
                    _ => true // ignore unknown filters for now
                };

                if (!result)
                    return false;
            }

            return true;
        }

        public void CompareTeams(List<string> teamNames, List<string> fields, List<QueryCondition> filters) {
            var allStats = _records;

            foreach (var teamName in teamNames) {
                var filteredData = allStats
                    .Where(t => t.Team.Equals(teamName, StringComparison.OrdinalIgnoreCase))
                    .Where(t => ApplyTeamFilters(t, filters))
                    .ToList();

                if (!filteredData.Any()) {
                    Console.WriteLine($"⚠️ No data found for team '{teamName}' with given filters.");
                    continue;
                }

                Console.WriteLine($"\n🏟️ {teamName} Totals:");

                foreach (var field in fields) {
                    if (field.Contains('/')) // computed fields like wins/losses ratio
                    {
                        var parts = field.Split('/');
                        var numerator = parts[0].ToLower();
                        var denominator = parts[1].ToLower();

                        double numeratorTotal = GetTeamFieldTotal(filteredData, numerator);
                        double denominatorTotal = GetTeamFieldTotal(filteredData, denominator);

                        if (denominatorTotal == 0)
                            Console.WriteLine($"  {field}: undefined (division by zero)");
                        else {
                            double ratio = numeratorTotal / denominatorTotal;
                            Console.WriteLine($"  {field}: {ratio:F2}");
                        }
                    } else // simple fields
                      {
                        double total = GetTeamFieldTotal(filteredData, field.ToLower());
                        Console.WriteLine($"  {field}: {total}");
                    }
                }
            }
        }

        private double GetTeamFieldTotal(List<TeamRecord> data, string fieldName) {
            return fieldName switch {
                "wins" => data.Sum(t => t.Wins),
                "losses" => data.Sum(t => t.Losses),
                //"pointsfor" or "points_scored" or "points" => data.Sum(t => t.PointsFor),
                //"pointsagainst" or "points_allowed" => data.Sum(t => t.PointsAgainst),
                "games" => data.Count,
                _ => 0
            };
        }

        private bool ApplyTeamFilters(TeamRecord stat, List<QueryCondition> filters) {
            foreach (var cond in filters) {
                var field = cond.Field.ToLower();
                var op = cond.Operator;
                var val = cond.Value;

                bool result = field switch {
                    "season" => CompareInt(stat.Season, op, val),
                    //"opponent" => CompareString(stat.Opponent, op, val),
                    _ => true
                };

                if (!result)
                    return false;
            }

            return true;
        }

        // You can reuse the CompareInt and CompareString helpers from before



        // Compare numeric with operator
        private bool CompareInt(int fieldVal, string op, string val) {
            if (!int.TryParse(val, out int cmp))
                return true;

            return op switch {
                "=" => fieldVal == cmp,
                ">" => fieldVal > cmp,
                "<" => fieldVal < cmp,
                ">=" => fieldVal >= cmp,
                "<=" => fieldVal <= cmp,
                _ => true
            };
        }

        // Compare string with operator (only '=' supported here)
        private bool CompareString(string fieldVal, string op, string val) {
            if (op != "=")
                return true; // only = supported for strings now
            return string.Equals(fieldVal, val, StringComparison.OrdinalIgnoreCase);
        }
        public List<PlayerComparisonResult> ComparePlayers(List<string> playerNames, List<string> fields, List<QueryCondition> filters, List<PlayerStats> playerStats) {
            var results = new List<PlayerComparisonResult>();

            foreach (var playerName in playerNames) {
                var filteredData = playerStats
                    .Where(p => p.Name.Contains(playerName, StringComparison.OrdinalIgnoreCase))
                    .Where(p => ApplyFilters(p, filters))
                    .ToList();

                if (!filteredData.Any()) {
                    // Optional: You can either skip or add an empty result
                    Console.WriteLine($"⚠️ No data found for player '{playerName}' with given filters.");
                    continue;
                }

                var statResults = new Dictionary<string, double?>();

                foreach (var field in fields) {
                    if (field.Contains('/')) {
                        var parts = field.Split('/');
                        var numerator = parts[0].ToLower();
                        var denominator = parts[1].ToLower();

                        double numeratorTotal = GetFieldTotal(filteredData, numerator);
                        double denominatorTotal = GetFieldTotal(filteredData, denominator);

                        if (denominatorTotal == 0) {
                            statResults[field] = null; // null to indicate undefined
                        } else {
                            statResults[field] = numeratorTotal / denominatorTotal;
                        }
                    } else {

                        if (field.ToLower() == "totals") {

                            var touchdowns = filteredData.Sum(p => p.Touchdowns);
                            var interceptions = filteredData.Sum(p => p.Interceptions);
                            var yards = filteredData.Sum(p => p.PassingYards);

                            statResults["touchdowns"] = touchdowns;
                            statResults["interceptions"] = interceptions;
                            statResults["yards"] = yards; 

                        } else {

                            double total = GetFieldTotal(filteredData, field.ToLower());

                            statResults[field] = total;
                        }
                    }
                }

                results.Add(new PlayerComparisonResult {
                    PlayerName = playerName,
                    Stats = statResults
                });
            }

            return results;
        }


        // Helper to sum numeric field by name
        private double GetFieldTotal(List<PlayerStats> data, string fieldName) {
            return fieldName switch {
                "touchdowns" or "td" => data.Sum(p => p.Touchdowns),
                "interceptions" or "int" => data.Sum(p => p.Interceptions),
                "yards" or "passingyards" => data.Sum(p => p.PassingYards),
                "games" => data.Count,  // useful if you want counts
                _ => 0
            };
        }



        public override object VisitTeamQuery(NqlParser.TeamQueryContext context) {
            string team = context.NAME().GetText().Trim();
            string mode = context.GetText().Contains("schedule") ? "schedule" : "record";

            if (mode == "schedule") {
                var filtered = _schedules.Where(s => s.Team.Contains(team, StringComparison.OrdinalIgnoreCase)).ToList();
                if (context.whereClause() is { } where)
                    filtered = ApplyScheduleConditions(filtered, where);
                foreach (var s in filtered)
                    Console.WriteLine($"Week {s.Week}: {s.Team} vs {s.Opponent} — {s.Result}");
            } else if (mode == "record") {
                var filtered = _records.Where(r => r.Team.Contains(team, StringComparison.OrdinalIgnoreCase)).ToList();
                if (context.whereClause() is { } where)
                    filtered = ApplyRecordConditions(filtered, where);
                foreach (var r in filtered)
                    Console.WriteLine($"{r.Season} {r.Team}: {r.Wins}-{r.Losses}");
            }

            return null;
        }

        public override object VisitGameQuery(NqlParser.GameQueryContext context) {
            var filtered = _games;
            if (context.whereClause() is { } where)
                filtered = ApplyGameConditions(filtered, where);

            foreach (var g in filtered)
                Console.WriteLine($"{g.Season} Wk {g.Week}: {g.Team} vs {g.Opponent} — {g.Result}");

            return null;
        }

        // condition logic
        private List<TeamSchedule> ApplyScheduleConditions(List<TeamSchedule> data, NqlParser.WhereClauseContext where) {
            foreach (var cond in where.condition()) {
                var field = cond.NAME().GetText().Trim().ToLower();
                var op = cond.@operator().GetText();
                var val = cond.value().GetText();
                data = field switch {
                    "week" => FilterNumeric(data, s => s.Week, op, val),
                    "result" => data.Where(s => s.Result.Equals(val, StringComparison.OrdinalIgnoreCase)).ToList(),
                    _ => data
                };
            }
            return data;
        }

        private List<TeamRecord> ApplyRecordConditions(List<TeamRecord> data, NqlParser.WhereClauseContext where) {
            foreach (var cond in where.condition()) {
                var field = cond.NAME().GetText().Trim().ToLower();
                var op = cond.@operator().GetText();
                var val = cond.value().GetText();
                data = field switch {
                    "season" => FilterNumeric(data, r => r.Season, op, val),
                    "wins" => FilterNumeric(data, r => r.Wins, op, val),
                    _ => data
                };
            }
            return data;
        }

        private List<Game> ApplyGameConditions(List<Game> data, NqlParser.WhereClauseContext where) {
            foreach (var cond in where.condition()) {
                var field = cond.NAME().GetText().Trim().ToLower();
                var op = cond.@operator().GetText();
                var val = cond.value().GetText();
                data = field switch {
                    "season" => FilterNumeric(data, g => g.Season, op, val),
                    "week" => FilterNumeric(data, g => g.Week, op, val),
                    "result" => data.Where(g => g.Result.Equals(val, StringComparison.OrdinalIgnoreCase)).ToList(),
                    "team" => data.Where(g => g.Team.Contains(val, StringComparison.OrdinalIgnoreCase)).ToList(),
                    _ => data
                };
            }
            return data;
        }

        // generic numeric filter
        private List<T> FilterNumeric<T>(List<T> list, Func<T, int> selector, string op, string val) {
            if (!int.TryParse(val, out int num))
                return list;
            return op switch {
                "=" => list.Where(x => selector(x) == num).ToList(),
                ">" => list.Where(x => selector(x) > num).ToList(),
                "<" => list.Where(x => selector(x) < num).ToList(),
                ">=" => list.Where(x => selector(x) >= num).ToList(),
                "<=" => list.Where(x => selector(x) <= num).ToList(),
                _ => list
            };
        }

        // Reuse your existing ExtractNames method to convert NAME tokens to strings
        private List<string> ExtractNames(NqlParser.PlayerListContext context) {
            return context?.NAME()
                .Select(nameToken => nameToken.GetText().Trim())
                .ToList() ?? new List<string>();
        }

        // Overloads for other list types
        private List<string> ExtractNames(NqlParser.TeamListContext context) {
            return context?.NAME()
                .Select(nameToken => nameToken.GetText().Trim())
                .ToList() ?? new List<string>();
        }

        private List<string> ExtractNames(NqlParser.GameListContext context) {
            return context?.NAME()
                .Select(nameToken => nameToken.GetText().Trim())
                .ToList() ?? new List<string>();
        }

    }
}
