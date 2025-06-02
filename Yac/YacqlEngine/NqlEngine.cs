using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YacqlEngine
{
    public class NqlEngine
    {
        public static object Run(string input, List<PlayerStats> players,
        List<TeamSchedule> schedules,
        List<TeamRecord> records,
        List<Game> games)
        {
            var inputStream = CharStreams.fromString(input);
            var lexer = new NqlLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new NqlParser(tokens);
            var tree = parser.query();

            var visitor = new NqlQueryVisitor(players, schedules, records, games);
            var result = visitor.Visit(tree);

            return result;

        }
    }
}
