using Shell.Utilities;

namespace ShellTests
{
    public class CommandLineParserCustomPropertyDoubleTests
    {
        private static readonly Random _random = new Random();
        private static readonly double _default = _random.NextDouble();

        /// <summary>
        /// Get a test value that's different from the default value
        /// </summary>
        /// <returns></returns>
        private static double GetTestValue()
        {
            var x = _random.NextDouble();
            while (x == _default) { x = _random.NextDouble(); }
            return x;
        }

        public class CustomPropertyDoubleOptions
        {
            private double _option = _default;
            public double? Option
            {
                get { return _option; }
                set { _option = value ?? _default; }
            }
        }

        [Fact]
        public void TestParseCustomPropertyDoubleWithEmptyParameterList()
        {
            var options = CommandLineParser.Parse<CustomPropertyDoubleOptions>(new string[0]);
            Assert.NotNull(options);
            Assert.Equal(_default, options.Option);
        }

        [Fact]
        public void TestParseCustomPropertyDoubleWithValueProvided()
        {
            var testValue = GetTestValue();
            var options = CommandLineParser.Parse<CustomPropertyDoubleOptions>(new[] { "--option", testValue.ToString() });
            Assert.NotNull(options);
            Assert.Equal(testValue, options.Option);
        }

        [Fact]
        public void TestParseCustomPropertyDoubleWithoutValue()
        {
            Assert.Throws<InvalidOperationException>(() => CommandLineParser.Parse<CustomPropertyDoubleOptions>(new[] { "--option" }));
        }

        [Fact]
        public void TestParseCustomPropertyDoubleWithOnlyDifferentParameter()
        {
            var options = CommandLineParser.Parse<CustomPropertyDoubleOptions>(new[] { "--stuff", GetTestValue().ToString() });
            Assert.NotNull(options);
            Assert.Equal(_default, options.Option);
        }

        [Fact]
        public void TestParseCustomPropertyDoubleWithOtherParameters()
        {
            var testValue = GetTestValue();
            var options = CommandLineParser.Parse<CustomPropertyDoubleOptions>(new[] { "--firstParam", GetTestValue().ToString(), "--option", testValue.ToString(), "--lastParam", GetTestValue().ToString() });
            Assert.NotNull(options);
            Assert.Equal(testValue, options.Option);
        }
    }
}