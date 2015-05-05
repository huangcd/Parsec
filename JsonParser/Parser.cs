using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsec;
using Parsec.Core;

namespace JsonParser
{
    public class JObject
    {
    }

    public class JString : JObject
    {
    }

    public static class Deserializer
    {
        public static Parser<Char, Char> Quote = Chars.One('"');
        public static Parser<Char, Char> Slash = Chars.One('\\');
        public static Parser<Char, Char> QuotationMark = Slash.And(Quote, (left, right) => right);
        public static Parser<Char, Char> SlashMark = Slash.And(Slash, (left, right) => right);
        public static Parser<Char, Char> ReverseSlashMark = Slash.And(Chars.One('/'), (left, right) => right);
        public static Parser<Char, Char> BackSpaceMark = Slash.And(Chars.One('b'), (left, right) => '\b');
        public static Parser<Char, Char> FormfeedMark = Slash.And(Chars.One('f'), (left, right) => '\f');
        public static Parser<Char, Char> NewLineMark = Slash.And(Chars.One('n'), (left, right) => '\n');
        public static Parser<Char, Char> CarriageReturnMark = Slash.And(Chars.One('r'), (left, right) => '\r');
        public static Parser<Char, Char> TabMark = Slash.And(Chars.One('t'), (left, right) => '\t');
        public static Parser<Char, Char> Unicode = Slash.And(
            Chars.One('u').And(Chars.HexDigit().RepeatN(4)),
            (_, digits) => (Char)Int32.Parse(new String(digits), NumberStyles.HexNumber));
    }
}
