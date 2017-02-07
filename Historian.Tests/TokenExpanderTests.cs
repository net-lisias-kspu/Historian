using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using KSEA.Historian;

namespace Historian.Tests
{
    [TestFixture(Category = "TokenExpander")]
    class TokenExpanderTests
    {
        static Dictionary<string, Action<StringBuilder, CommonInfo, string[]>> Parsers
            = new Dictionary<string, Action<StringBuilder, CommonInfo, string[]>>
            {
                { "Echo", Echo },
                { "Swap", Swap },
                { "NOP", NOP },
                { "Custom", Custom }
            };

        [Test]
        public void SimpleLiteral()
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = true, Key = "literal"}
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, true).ToString();

            Assert.That(result, Is.EqualTo("literal"));
        }

        [Test]
        public void SingleTag()
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = false, Key = "Echo"}
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, true).ToString();

            Assert.That(result, Is.EqualTo(EchoString));
        }

        [Test]
        public void SingleParameterizedTag()
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = false, Key = "Swap", Args = new []{ "one", "two" } }
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, true).ToString();

            Assert.That(result, Is.EqualTo("two, one"));
        }

        [Test]
        public void UnrecognizedTag()
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = false, Key = "UNKNOWN", Args = null }
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, true).ToString();

            Assert.That(result, Is.EqualTo("<UNKNOWN>"));
        }

        [Test]
        public void UnrecognizedTagWithParams()
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = false, Key = "UNKNOWN", Args = new []{ "one", "two" }  }
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, true).ToString();

            Assert.That(result, Is.EqualTo("<UNKNOWN(one,two)>"));
        }

        [Test]
        public void AllowCustomTag()
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = false, Key = "Custom"}
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, true).ToString();

            Assert.That(result, Is.EqualTo(CustomString));
        }

        [Test]
        public void DontAllowCustomTag()
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = false, Key = "Custom"}
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, false).ToString();

            Assert.That(result, Is.EqualTo("<Custom>"));
        }

        [TestCase(true, "Hello : Echo, echo, echo...<UNKNOWN(one,two)> World! Wibble!two, one")]
        [TestCase(false, "Hello : Echo, echo, echo...<UNKNOWN(one,two)> World! <Custom>two, one")]
        public void MixedTagsAndLiterals(bool allowCustom, string expected)
        {
            var input = new List<Token>
            {
                new Token { IsLiteral = true, Key = "Hello : " },
                new Token { IsLiteral = false, Key = "Echo"},
                new Token { IsLiteral = false, Key = "UNKNOWN", Args = new []{ "one", "two" }  },
                new Token { IsLiteral = true, Key = " World! " },
                new Token { IsLiteral = false, Key = "Custom"},
                new Token { IsLiteral = false, Key = "Swap", Args = new []{ "one", "two" }  }
            };

            var sb = new StringBuilder();
            var info = new CommonInfo();

            var result = sb.ExpandTokenizedText(input, info, Parsers, allowCustom).ToString();

            Assert.That(result, Is.EqualTo(expected));

        }


        private static string EchoString = "Echo, echo, echo...";
        private static string CustomString = "Wibble!";

        private static void Echo(StringBuilder sb, CommonInfo info, string[] args)
            => sb.Append(EchoString);

        private static void Custom(StringBuilder sb, CommonInfo info, string[] args)
            => sb.Append(CustomString);

        private static void Swap(StringBuilder sb, CommonInfo info, string[] args)
        {
            Assert.That(args.Count, Is.EqualTo(2));
            sb.Append(args[1]).Append(", ").Append(args[0]);
        }

        private static void NOP(StringBuilder sb, CommonInfo info, string[] args)
        {
            // do nothing
        }
    }
}
