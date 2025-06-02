using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacqlEngine
{
    public class NqlQueryVisitor : NqlBaseVisitor<object>
    {
        private readonly List<PlayerStats> _players;
        private readonly List<TeamSchedule> _schedules;
        private readonly List<TeamRecord> _records;
        private readonly List<Game> _games;

        public NqlQueryVisitor(
            List<PlayerStats> players,
            List<TeamSchedule> schedules,
            List<TeamRecord> records,
            List<Game> games)
        {
            _players = players;
            _schedules = schedules;
            _records = records;
            _games = games;
        }

        public override object VisitPlayerQuery(NqlParser.PlayerQueryContext context)
        {
            string playerName = context.NAME().GetText().Trim();
            var filtered = _players
                .Where(p => p.Name.Contains(playerName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (context.whereClause() is { } where)
            {
                foreach (var cond in where.condition())
                {
                    string field = cond.NAME().GetText().Trim().ToLower();
                    string op = cond.@operator().GetText().Trim();
                    string val = cond.value().GetText().Trim();

                    filtered = ApplyCondition(filtered, field, op, val);
                }
            }

            if (filtered.Count == 0)
            {
                Console.WriteLine("⚠️ No matching results.");
                return null;
            }

            foreach (var p in filtered)
            {
                Console.WriteLine($"\n📊 {p.Name} — {p.Season} Season");
                Console.WriteLine($"  Yards: {p.PassingYards}");
                Console.WriteLine($"  TDs:   {p.Touchdowns}");
                Console.WriteLine($"  INTs:  {p.Interceptions}");
            }

            return null;
        }

        private List<PlayerStats> ApplyCondition(List<PlayerStats> players, string field, string op, string val)
        {
            return field switch
            {
                "season" => FilterNumeric(players, p => p.Season, op, val),
                "touchdowns" => FilterNumeric(players, p => p.Touchdowns, op, val),
                "interceptions" => FilterNumeric(players, p => p.Interceptions, op, val),
                "passingyards" or "yards" => FilterNumeric(players, p => p.PassingYards, op, val),
                _ => players
            };
        }

        private List<PlayerStats> FilterNumeric(List<PlayerStats> players, Func<PlayerStats, int> selector, string op, string val)
        {
            if (!int.TryParse(val, out int num))
                return players;

            return op switch
            {
                "=" => players.Where(p => selector(p) == num).ToList(),
                ">" => players.Where(p => selector(p) > num).ToList(),
                "<" => players.Where(p => selector(p) < num).ToList(),
                ">=" => players.Where(p => selector(p) >= num).ToList(),
                "<=" => players.Where(p => selector(p) <= num).ToList(),
                _ => players
            };
        }

        public override object VisitTeamQuery(NqlParser.TeamQueryContext context)
        {
            string team = context.NAME().GetText().Trim();
            string mode = context.GetText().Contains("schedule") ? "schedule" : "record";

            if (mode == "schedule")
            {
                var filtered = _schedules.Where(s => s.Team.Contains(team, StringComparison.OrdinalIgnoreCase)).ToList();
                if (context.whereClause() is { } where)
                    filtered = ApplyScheduleConditions(filtered, where);
                foreach (var s in filtered)
                    Console.WriteLine($"Week {s.Week}: {s.Team} vs {s.Opponent} — {s.Result}");
            }
            else if (mode == "record")
            {
                var filtered = _records.Where(r => r.Team.Contains(team, StringComparison.OrdinalIgnoreCase)).ToList();
                if (context.whereClause() is { } where)
                    filtered = ApplyRecordConditions(filtered, where);
                foreach (var r in filtered)
                    Console.WriteLine($"{r.Season} {r.Team}: {r.Wins}-{r.Losses}");
            }

            return null;
        }

        public override object VisitGameQuery(NqlParser.GameQueryContext context)
        {
            var filtered = _games;
            if (context.whereClause() is { } where)
                filtered = ApplyGameConditions(filtered, where);

            foreach (var g in filtered)
                Console.WriteLine($"{g.Season} Wk {g.Week}: {g.Team} vs {g.Opponent} — {g.Result}");

            return null;
        }

        // condition logic
        private List<TeamSchedule> ApplyScheduleConditions(List<TeamSchedule> data, NqlParser.WhereClauseContext where)
        {
            foreach (var cond in where.condition())
            {
                var field = cond.NAME().GetText().Trim().ToLower();
                var op = cond.@operator().GetText();
                var val = cond.value().GetText();
                data = field switch
                {
                    "week" => FilterNumeric(data, s => s.Week, op, val),
                    "result" => data.Where(s => s.Result.Equals(val, StringComparison.OrdinalIgnoreCase)).ToList(),
                    _ => data
                };
            }
            return data;
        }

        private List<TeamRecord> ApplyRecordConditions(List<TeamRecord> data, NqlParser.WhereClauseContext where)
        {
            foreach (var cond in where.condition())
            {
                var field = cond.NAME().GetText().Trim().ToLower();
                var op = cond.@operator().GetText();
                var val = cond.value().GetText();
                data = field switch
                {
                    "season" => FilterNumeric(data, r => r.Season, op, val),
                    "wins" => FilterNumeric(data, r => r.Wins, op, val),
                    _ => data
                };
            }
            return data;
        }

        private List<Game> ApplyGameConditions(List<Game> data, NqlParser.WhereClauseContext where)
        {
            foreach (var cond in where.condition())
            {
                var field = cond.NAME().GetText().Trim().ToLower();
                var op = cond.@operator().GetText();
                var val = cond.value().GetText();
                data = field switch
                {
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
        private List<T> FilterNumeric<T>(List<T> list, Func<T, int> selector, string op, string val)
        {
            if (!int.TryParse(val, out int num))
                return list;
            return op switch
            {
                "=" => list.Where(x => selector(x) == num).ToList(),
                ">" => list.Where(x => selector(x) > num).ToList(),
                "<" => list.Where(x => selector(x) < num).ToList(),
                ">=" => list.Where(x => selector(x) >= num).ToList(),
                "<=" => list.Where(x => selector(x) <= num).ToList(),
                _ => list
            };
        }
    }
}
