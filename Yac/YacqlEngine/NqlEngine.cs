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
        public static void Run(string input, List<PlayerStats> playerStats)
        {
            var inputStream = CharStreams.fromString(input);
            var lexer = new NqlLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new NqlParser(tokens);
            var tree = parser.query();

            var visitor = new NqlQueryVisitor(playerStats);
            visitor.Visit(tree);
        }
    }
}
