using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace LineParser
{
    class Program
    {
        static void DoIt(LineParser parser, string input)
        {
            Console.Write($"{input}: ");
            Console.WriteLine(string.Join(" ", parser.Parse(input).Select(s => $"[{s}]")));
        }


        static void Main(string[] args)
        {
            var parser = new StringLineParser("|", "\"");

            DoIt(parser, "hello|world|28");
            DoIt(parser, "\"hello\"|world|28");
            DoIt(parser, "hello|\"world|28\"");
            DoIt(parser, "");
            DoIt(parser, "|");
            DoIt(parser, "world");
            DoIt(parser, "\"hello\"");
            DoIt(parser, "\"hello\\\"test\"|world|28");
        }
    }
}
