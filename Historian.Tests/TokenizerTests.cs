using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Historian.Tests
{
    [TestFixture(Category = "Tokenizer")]
    public class TokenizerTests
    {
        [Test]
        public void TokenizeRawTextTest()
        {
            var input = "Raw text";
            
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(1));

            var token = result[0];
            Assert.That(token.IsLiteral, Is.True);
            Assert.That(token.Key, Is.EqualTo(input));
            Assert.That(token.Args, Is.Null);
        }

        [Test]
        public void SingleTokenTest()
        {
            var input = "<TestToken>";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(1));

            var token = result[0];
            Assert.That(token.IsLiteral, Is.False);
            Assert.That(token.Key, Is.EqualTo("TestToken"));
            Assert.That(token.Args, Is.Null);
        }

        [Test]
        public void SingleTokenWithOneParam()
        {
            var input = "<TestToken(Param)>";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(1));

            var token = result[0];
            Assert.That(token.IsLiteral, Is.False);
            Assert.That(token.Key, Is.EqualTo("TestToken"));
            Assert.That(token.Args, Is.Not.Null);
            Assert.That(token.Args.Length, Is.EqualTo(1));
            Assert.That(token.Args[0], Is.EqualTo("Param"));
        }

        [Test]
        public void SingleTokenWithTwoParameters()
        {
            var input = "<token(p1,p2)>";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(1));

            var token = result[0];
            Assert.That(token.IsLiteral, Is.False);
            Assert.That(token.Key, Is.EqualTo("token"));
            Assert.That(token.Args, Is.Not.Null);
            Assert.That(token.Args.Length, Is.EqualTo(2));
            Assert.That(token.Args[0], Is.EqualTo("p1"));
            Assert.That(token.Args[1], Is.EqualTo("p2"));
        }

        [Test]
        public void TwoTokens()
        {
            var input = "<One><Two>";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(2));

            var token = result[0];
            Assert.That(token.IsLiteral, Is.False);
            Assert.That(token.Key, Is.EqualTo("One"));
            Assert.That(token.Args, Is.Null);

            token = result[1];
            Assert.That(token.IsLiteral, Is.False);
            Assert.That(token.Key, Is.EqualTo("Two"));
            Assert.That(token.Args, Is.Null);
        }

        [Test]
        public void TokenAfterLiteral()
        {
            var input = "literal <token>";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Key, Is.EqualTo("literal "));
            Assert.That(result[1].Key, Is.EqualTo("token"));
        }

        [Test]
        public void TokenBeforeLiteral()
        {
            var input = "<token> literal";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Key, Is.EqualTo("token"));
            Assert.That(result[1].Key, Is.EqualTo(" literal"));
        }

        [Test]
        public void TokenBetweenLiterals()
        {
            var input = "start <token> end";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Key, Is.EqualTo("start "));
            Assert.That(result[1].Key, Is.EqualTo("token"));
            Assert.That(result[2].Key, Is.EqualTo(" end"));
        }

        [Test]
        public void ParameterizedTokenBetweenLiterals()
        {
            var input = "start <token(p1,p2)> end";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].Key, Is.EqualTo("start "));
            Assert.That(result[1].Key, Is.EqualTo("token"));
            Assert.That(result[1].Args.Count, Is.EqualTo(2));
            Assert.That(result[2].Key, Is.EqualTo(" end"));
        }

        [Test]
        public void TestMixed()
        {
            var input = "<color = #ffff>Raw test code <N>start <token(p1, p2, stop now)><two> gap <three> end";
            var result = KSEA.Historian.Parser.GetTokens(input);

            Assert.That(result.Count, Is.EqualTo(9));

            var paramToken = result[4];
            Assert.That(paramToken.Key, Is.EqualTo("token"));
            Assert.That(paramToken.Args.Count, Is.EqualTo(3));
            Assert.That(paramToken.Args[1], Is.EqualTo("p2"));
            Assert.That(paramToken.Args[2], Is.EqualTo("stop now"));

            Assert.That(result[8].Key, Is.EqualTo(" end"));
            Assert.That(result[8].IsLiteral, Is.True);
        }
    }
}
