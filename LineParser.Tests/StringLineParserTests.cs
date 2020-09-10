using System;
using Xunit;
using LineParser;
using System.IO;

namespace LineParser.Tests
{
    public class StringLineParserTests
    {
        [Fact]
        public void HandlesNull()
        {
            var parser = new StringLineParser("|", "<", ">");
            var results = parser.Split(null);
            Assert.Empty(results);
        }

        [Theory]
        [InlineData("hello<world>")]
        [InlineData("<hello>world")]
        public void HandlesBadQuotedData(string input)
        {
            var parser = new StringLineParser("|", "<", ">");
            Assert.Throws<InvalidDataException>(() => parser.Split(input));
        }

        [Fact]
        public void HandlesEmptyString()
        {
            var parser = new StringLineParser("|", "<", ">");
            var results = parser.Split("");
            Assert.Collection(results, v => Assert.Equal("", v));
        }

        [Fact]
        public void HandlesSingleDelimiter()
        {
            var parser = new StringLineParser("|", "<", ">");
            var results = parser.Split("|");
            Assert.Collection(results,
                              v => Assert.Equal("", v),
                              v => Assert.Equal("", v));
        }

        [Fact]
        public void HandlesTwoDelimiters()
        {
            var parser = new StringLineParser("|", "<", ">");
            var results = parser.Split("||");
            Assert.Collection(results,
                              v => Assert.Equal("", v),
                              v => Assert.Equal("", v),
                              v => Assert.Equal("", v));
        }

        [Theory]
        [InlineData("abc", "abc")]
        [InlineData("<abc>", "<abc>")]
        [InlineData("<a|b>", "<a|b>")]
        [InlineData(@"<a\|b>", @"<a\|b>")]
        [InlineData("<a|b|c>", "<a|b|c>")]
        [InlineData(@"<a\|b\|c>", @"<a\|b\|c>")]
        public void HandlesSingleValue(string input, string a)
        {
            var parser = new StringLineParser("|", "<", ">");
            var results = parser.Split(input);
            Assert.Collection(results, v => Assert.Equal(a, v));
        }

        [Theory]
        [InlineData("abc|123", "abc", "123")]
        [InlineData("<abc>|123", "<abc>", "123")]
        [InlineData("<abc>|<123>", "<abc>", "<123>")]
        [InlineData("abc|<123>", "abc", "<123>")]
        [InlineData("<abc>|", "<abc>", "")]
        [InlineData("|<123>", "", "<123>")]
        public void HandlesTwoValues(string input, string a, string b)
        {
            var parser = new StringLineParser("|", "<", ">");
            var results = parser.Split(input);
            Assert.Collection(results,
                              v => Assert.Equal(a, v),
                              v => Assert.Equal(b, v));
        }

        [Fact]
        public void HandlesThreeValues()
        {
            var parser = new StringLineParser("|", "<", ">");
            var results = parser.Split("abc|123|ghi");
            Assert.Collection(results,
                              v => Assert.Equal("abc", v),
                              v => Assert.Equal("123", v),
                              v => Assert.Equal("ghi", v));
        }
    }
}
