using Antlr4.Runtime;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YacData.Models;

namespace YacqlEngine
{
    public class NqlEngine
    {
        public static object Run(string input)
        {
            var inputStream = CharStreams.fromString(input);
            var lexer = new NqlLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            tokens.Fill();

            var test = tokens.GetTokens();

            var parser = new NqlParser(tokens);
            var tree = parser.query();


            var visitor = new NqlQueryVisitor();
            var result = visitor.Visit(tree);

            return result;

        }
    }
}
