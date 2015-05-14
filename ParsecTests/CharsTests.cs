using System;
using System.Linq;
using NUnit.Framework;
using Parsec;
using Parsec.Core;

namespace ParsecTests
{
    [TestFixture()]
    public class CharsTests
    {
        [Test()]
        public void RepeatTest()
        {
            var repeatI = Chars.One('I').Many();
            repeatI("nt".AsPlainCharStream()).Match(
                success: (restStream, chars) => { Assert.AreEqual("", new String(chars)); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            repeatI("Int".AsPlainCharStream()).Match(
                success: (restStream, chars) => { Assert.AreEqual("I", new String(chars)); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            repeatI("IIIIIInt".AsPlainCharStream()).Match(
                success: (restStream, chars) => { Assert.AreEqual("IIIIII", new String(chars)); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });

            var repeat1I = Chars.One('I').RepeatAtLeast1();
            repeat1I("nt".AsPlainCharStream()).Match(
                success: (restStream, chars) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);
            repeat1I("Int".AsPlainCharStream()).Match(
                success: (restStream, chars) => { Assert.AreEqual("I", new String(chars)); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            repeat1I("IIIIIInt".AsPlainCharStream()).Match(
                success: (restStream, chars) => { Assert.AreEqual("IIIIII", new String(chars)); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
        }

        [Test()]
        public void SequenceTest()
        {
            var letParser = Chars.Sequance("let");
            letParser("let".AsPlainCharStream()).Match(
                success: (restStream, chars) =>
                {
                    Assert.AreEqual("let", new String(chars.ToArray()));
                    Assert.IsTrue(Chars.EndOfInput()(restStream).Success());
                    return 0;
                },
                failure: (restStream, error) =>
                {
                    Assert.Fail();
                    return 0;
                });
            letParser("leta".AsPlainCharStream()).Match(
                success: (restStream, chars) =>
                {
                    Assert.AreEqual("let", new String(chars.ToArray()));
                    return 0;
                },
                failure: (restStream, error) =>
                {
                    Assert.Fail();
                    return 0;
                });
            letParser("le".AsPlainCharStream()).Match(
                success: (restStream, chars) =>
                {
                    Assert.Fail();
                    return 0;
                },
                failure: (restStream, error) => /*OK*/ 0);
            letParser("int".AsPlainCharStream()).Match(
                success: (restStream, chars) =>
                {
                    Assert.Fail();
                    return 0;
                },
                failure: (restStream, error) => /*OK*/ 0);
        }

        [Test()]
        public void AndTest()
        {
            var iParser = Chars.One('I');
            var nParser = Chars.One('n');
            var parser = iParser.And(nParser);
            parser("Int".AsPlainCharStream()).Match(
                success: (restStream, c) =>
                {
                    Assert.AreEqual('n', c);
                    return 0;
                },
                failure: (restStream, error) =>
                {
                    Assert.Fail();
                    return 0;
                });
        }

        [Test()]
        public void CharTest()
        {
            var parser = Chars.One('I');
            parser("Int".AsPlainCharStream()).Match(
                success: (restStream, c) =>
                {
                    Assert.AreEqual('I', c);
                    Assert.IsTrue(restStream.Current.HasValue());
                    Assert.AreEqual('n', restStream.Current.GetValue());
                    restStream = restStream.MoveNext();
                    Assert.IsTrue(restStream.Current.HasValue());
                    Assert.AreEqual('t', restStream.Current.GetValue());
                    restStream = restStream.MoveNext();
                    Assert.IsFalse(restStream.Current.HasValue());
                    Assert.Throws<Exception>(() => restStream.Current.GetValue());
                    return 0;
                },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("nt".AsPlainCharStream()).Match(
                success: (restStream, c) =>
                {
                    Assert.Fail();
                    return 0;
                },
                failure: (restStream, error) => /*OK*/ 0);
        }

        [Test()]
        public void EndOfInputTest()
        {
            var parser = Chars.EndOfInput();
            parser("Int".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);

            parser("".AsPlainCharStream()).Match(
                success: (restStream, c) => /*OK*/ 0,
                failure: (restStream, error) => { Assert.Fail(); return 0; });
        }

        [Test()]
        public void LetterTest()
        {
            var parser = Chars.Letter();
            parser("I".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.AreEqual('I', c); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("0".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);
            parser("黄".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.AreEqual('黄', c); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);
        }

        [Test()]
        public void DigitTest()
        {
            var parser = Chars.Digit();
            parser("0".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.AreEqual('0', c); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("I".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);
            parser("黄".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);
            parser("".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);
        }

        [Test()]
        public void LetterOrDigitTest()
        {
            var parser = Chars.LetterOrDigit();
            parser("I".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.AreEqual('I', c); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("0".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.AreEqual('0', c); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("黄".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.AreEqual('黄', c); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /*OK*/ 0);
        }

        [Test()]
        public void AnyTest()
        {
            var parser = Combinators.Any(Chars.One('I'), Chars.One('J'), Chars.One('K'));
            Assert.IsTrue(parser("I".AsPlainCharStream()).Success());
            Assert.IsTrue(parser("J".AsPlainCharStream()).Success());
            Assert.IsTrue(parser("K".AsPlainCharStream()).Success());
            Assert.IsFalse(parser("L".AsPlainCharStream()).Success());
            Assert.IsFalse(parser("M".AsPlainCharStream()).Success());
            Assert.IsFalse(parser("".AsPlainCharStream()).Success());
        }

        [Test()]
        public void CharAnyTest()
        {
            var parser = "IJK".OneOf();
            Assert.IsTrue(parser("I".AsPlainCharStream()).Success());
            Assert.IsTrue(parser("J".AsPlainCharStream()).Success());
            Assert.IsTrue(parser("K".AsPlainCharStream()).Success());
            Assert.IsFalse(parser("L".AsPlainCharStream()).Success());
            Assert.IsFalse(parser("M".AsPlainCharStream()).Success());
            Assert.IsFalse(parser("".AsPlainCharStream()).Success());
        }

        [Test()]
        public void RepeatNTest()
        {
            var parser = Chars.Letter().RepeatN(3);
            parser("ABCDEF".AsPlainCharStream()).Match(
                success: (restStream, tokens) =>
                {
                    Assert.AreEqual("ABC", new String(tokens));
                    Assert.AreEqual("DEF", new String(parser(restStream).GetOutput()));
                    return 0;
                },
                failure: (restStream, error) =>
                {
                    Assert.Fail();
                    return 0;
                });
            parser("AB".AsPlainCharStream()).Match(
                success: (restStream, tokens) =>
                {
                    Assert.Fail();
                    return 0;
                },
                failure: (restStream, error) => /* OK */ 0);
        }

        [Test()]
        public void HexDigitTest()
        {
            var parser = Chars.HexDigit().RepeatN(22);
            parser("0123456789ABCDEFabcdef".AsPlainCharStream()).Match(
                success: (restStream, tokens) =>
                {
                    Assert.AreEqual("0123456789ABCDEFabcdef", new String(tokens));
                    Assert.IsTrue(Chars.EndOfInput()(restStream).Success());
                    return 0;
                },
                failure: (restStream, error) =>
                {
                    Assert.Fail();
                    return 0;
                });
            Assert.IsFalse(Chars.HexDigit()("g".AsPlainCharStream()).Success());
        }
    }
}
