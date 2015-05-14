using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonParser;
using NUnit.Framework;
using Parsec;
using Parsec.Core;

namespace JsonParserTests
{
    [TestFixture()]
    public class DeserializerTests
    {
        [Test()]
        public void StringTest()
        {
            var parser = Deserializer.String;
            parser("\"Hello\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("Hello", (String)token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("\"\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("", (String)token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("\"\\t\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("\t", (String)token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("Hello\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.Fail(); return 0; },
                failure: (restStream, error) => /* OK */ 0);
            parser("\"黄\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("黄", (String)token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
            parser("\"黄\\u0a3e\"".AsPlainCharStream()).Match(
                success: (restStream, token) => { Assert.AreEqual("黄\u0a3e", (String)token); return 0; },
                failure: (restStream, error) => { Assert.Fail(); return 0; });
        }

        [Test()]
        public void NumberTest()
        {
            var parser = Deserializer.Number();
            Assert.AreEqual(123, (Double)parser("123abc".AsPlainCharStream()).GetOutput());
            Assert.AreEqual(123.012, (Double)parser("123.012abd".AsPlainCharStream()).GetOutput());
            Assert.AreEqual(123.012e2, (Double)parser("123.012e2adb".AsPlainCharStream()).GetOutput());
            Assert.AreEqual(123e+1, (Double)parser("123e+1".AsPlainCharStream()).GetOutput());
            Assert.AreEqual(123, (Double)parser("123eabc".AsPlainCharStream()).GetOutput());
            Assert.AreEqual(123, (Double)parser("123.e+1".AsPlainCharStream()).GetOutput());
        }
    }
}
