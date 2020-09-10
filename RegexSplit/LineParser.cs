using System.Collections.Generic;
using System.Linq;

namespace LineParser
{
    public abstract class LineParser
    {
        public string Delimiter { get; }
        public string BeginQuote { get; }
        public string EndQuote { get; }
        public char Escape { get; }

        public LineParser(string delimiter, string beginQuote, string endQuote, char escape = '\\')
        {
            Delimiter = delimiter;
            BeginQuote = beginQuote;
            EndQuote = endQuote;
            Escape = escape;
        }

        public LineParser(string delimiter, string quote, char escape = '\\')
            : this(delimiter, quote, quote, escape)
        {
        }

        public abstract IEnumerable<string> Parse(string input);

        public virtual string[] Split(string input) => Parse(input).ToArray();
    }
}
