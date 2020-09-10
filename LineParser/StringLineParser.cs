using System;
using System.Collections.Generic;
using System.IO;

namespace LineParser
{
    public class StringLineParser : LineParser
    {

        public StringLineParser(string delimiter, string beginQuote, string endQuote, char escape = '\\')
            : base(delimiter, beginQuote, endQuote, escape)
        {
        }

        public StringLineParser(string delimiter, string quote, char escape = '\\')
            : this(delimiter, quote, quote, escape)
        {
        }

        int IndexOfUnescaped(ReadOnlySpan<char> input, string value)
        {
            var left = 0;
            while (!input.IsEmpty)
            {
                int index = input.IndexOf(value);
                if (index < 0)
                    return index;

                if (index == 0 || input[index - 1] != Escape)
                    return left + index;

                left += index + value.Length;
                input = input.Slice(index + value.Length);
            }
            return -1;
        }

        int GetEndOfQuotedString(ReadOnlySpan<char> input)
        {
            int end = IndexOfUnescaped(input.Slice(BeginQuote.Length), EndQuote);
            if (end < 0)
                throw new InvalidDataException($"Unmatched {BeginQuote}");
            end += BeginQuote.Length + EndQuote.Length;

            if (end < input.Length && !input.Slice(end).StartsWith(Delimiter))
                throw new InvalidDataException($"Expected {Delimiter} after {EndQuote}");

            return end;
        }

        int GetEndOfField(ReadOnlySpan<char> input)
        {
            int end = IndexOfUnescaped(input, Delimiter);
            if (end < 0)
                end = input.Length;

            var field = input[0..end];
            ValidateNoQuote(field, BeginQuote);
            ValidateNoQuote(field, EndQuote);

            return end;

            void ValidateNoQuote(ReadOnlySpan<char> field, string quote)
            {
                if (field.IndexOf(quote) >= 0)
                    throw new InvalidDataException($"Unexpected {quote} in field data.");
            }
        }

        IEnumerable<string> GetTokens(string input)
        {
            var inputMem = input.AsMemory();

            while (!inputMem.IsEmpty)
            {
                int end = Delimiter.Length;
                var span = inputMem.Span;

                if (span.StartsWith(BeginQuote))
                    end = GetEndOfQuotedString(span);
                else if (!span.StartsWith(Delimiter))
                    end = GetEndOfField(span);

                yield return inputMem[0..end].ToString();
                inputMem = inputMem.Slice(end);
            }
        }

        public override IEnumerable<string> Parse(string input)
        {
            if (input == null)
                yield break;

            string lastValue = null;
            foreach(var token in GetTokens(input))
            {
                var value = token;
                if (value == Delimiter)
                {
                    yield return lastValue ?? "";
                    value = null;
                }
                lastValue = value;
            }
            yield return lastValue ?? "";
        }
    }
}
