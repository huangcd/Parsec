using System;
using NUnit.Framework;
using Parsec.Core;

namespace Parsec.Tests
{
    [TestFixture()]
    public class CharsTests
    {
        [Test()]
        public void CharTest()
        {
            var parser = Chars.Char<Int32>('I');
            parser("Int".AsPlainCharStream()).Match(
                success: (restStream, c) =>
                {
                    Assert.AreEqual('I', c);
                    Assert.IsTrue(restStream.Current.HasValue());
                    Assert.AreEqual('n', restStream.Current.GetValue().Token);
                    restStream = restStream.MoveNext();
                    Assert.IsTrue(restStream.Current.HasValue());
                    Assert.AreEqual('t', restStream.Current.GetValue().Token);
                    restStream = restStream.MoveNext();
                    Assert.IsFalse(restStream.Current.HasValue());
                    Assert.Throws<Exception>(() => restStream.Current.GetValue());
                    return Nothing.Instance;
                },
                failure: (restStream, error) => { Assert.Fail(); return Nothing.Instance; });
        }

        [Test()]
        public void EndOfInputTest()
        {
            var parser = Chars.EndOfInput<Int32>();
            parser("Int".AsPlainCharStream()).Match(
                success: (restStream, c) => { Assert.Fail(); return Nothing.Instance; },
                failure: (restStream, error) => /*OK*/ Nothing.Instance);

            parser("".AsPlainCharStream()).Match(
                success: (restStream, c) => /*OK*/ Nothing.Instance,
                failure: (restStream, error) => { Assert.Fail(); return Nothing.Instance; });
        }
    }
}
