using Shell.Utilities;
using System.Globalization;

namespace ShellTests
{
    public class CommandLineParserSimpleDoubleTests
    {
        public static readonly Random _random = new Random();

        public class SimpleDoubleOptions
        {
            public double? Option { get; set; }
        }

        [Fact]
        public void TestParseSimpleDoubleWithEmptyParameterList()
        {
            var options = CommandLineParser.Parse<SimpleDoubleOptions>(new string[0]);
            Assert.NotNull(options);
            Assert.Null(options.Option);
        }

        [Fact]
        public void TestParseSimpleDoubleWithValueProvided()
        {
            var testValue = _random.NextDouble();
            var options = CommandLineParser.Parse<SimpleDoubleOptions>(new[] { "--option", testValue.ToString() });
            Assert.NotNull(options);
            Assert.Equal(testValue, options.Option);
        }

        [Fact]
        public void TestParseSimpleDoubleWithoutValue()
        {
            Assert.Throws<InvalidOperationException>(() => CommandLineParser.Parse<SimpleDoubleOptions>(new[] { "--option" }));
        }

        [Fact]
        public void TestParseSimpleDoubleWithOnlyDifferentParameter()
        {
            var options = CommandLineParser.Parse<SimpleDoubleOptions>(new[] { "--stuff", _random.NextDouble().ToString() });
            Assert.NotNull(options);
            Assert.Null(options.Option);
        }

        [Fact]
        public void TestParseSimpleDoubleWithOtherParameters()
        {
            var testValue = _random.NextDouble();
            var options = CommandLineParser.Parse<SimpleDoubleOptions>(new[] { "--firstParam", _random.NextDouble().ToString(), "--option", testValue.ToString(), "--lastParam", _random.NextDouble().ToString() });
            Assert.NotNull(options);
            Assert.Equal(testValue, options.Option);
        }
    }
}