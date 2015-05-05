using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonParser;
using NUnit.Framework;
using Parsec;

namespace JsonParserTests
{
    [TestFixture()]
    public class DeserializerTests
    {
        [Test()]
        public void StringTest()
        {
            var parser = Deserializer.String();
            parser("\"Hello\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("Hello", token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("\"\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("", token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("\"\\t\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("\t", token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("Hello\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => { return 0; });
            parser("\"黄\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("黄", token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("\"黄\\u0a3e\":123".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("黄\u0a3e", token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
        }
    }
}
