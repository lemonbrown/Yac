using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacqlEngine
{
    public class NqlQueryVisitor : NqlBaseVisitor<object>
    {
        private readonly List<PlayerStats> _data;

        public NqlQueryVisitor(List<PlayerStats> data)
        {
            _data = data;
        }

        public override object VisitPlayerQuery(NqlParser.PlayerQueryContext context)
        {
            string playerName = context.NAME().GetText().Trim();
            var filtered = _data
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
    }
}
