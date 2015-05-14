using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using Parsec;
using Parsec.Core;

namespace JsonParser
{
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

        public static Parser<Char, JValue> String = 
                from start in Quote
                from chars in Combinators.Any("\\\"".NoneOf(),
                                              QuotationMark,
                                              SlashMark,
                                              ReverseSlashMark,
                                              BackSpaceMark,
                                              FormfeedMark,
                                              NewLineMark,
                                              CarriageReturnMark,
                                              TabMark,
                                              Unicode)
                                         .Many()
                from end in Quote
                select (JValue)(new String(chars));

        public static Parser<TToken, TOutput[]> Concat<TToken, TOutput>(
            this Parser<TToken, TOutput> singleParser,
            Parser<TToken, TOutput[]> multiParser)
        {
            return singleParser.And(multiParser, (single, multi) => new[] { single }.Concat(multi).ToArray());
        }

        public static Parser<Char, JValue> Number()
        {
            var integralPart = Combinators.Sequence(Chars.One('0')).Or("123456789".OneOf().Concat(Chars.Digit().Many()));
            var decimalPart =
                Chars.One('.').And(Chars.Digit().RepeatAtLeast1(), (left, right) => new[] { left }.Concat(right).ToArray()).Optional();
            var expPart =
                "eE".OneOf()
                    .And("+-".OneOf().Optional(), (left, right) => right.Match(val => new[] { left, val }, () => new[] { left }))
                    .And(Chars.Digit().RepeatAtLeast1(), (left, right) => left.Concat(right).ToArray())
                    .Optional();
            return from negative in Chars.One('-').Optional()
                   from number1 in integralPart
                   from number2 in decimalPart
                   from exp in expPart
                   select (JValue)Double.Parse(new String(
                       negative.Match(val => new[] { val }, () => new char[0])
                           .Concat(number1)
                           .Concat(number2.GetOrDefault(new char[0]))
                           .Concat(exp.GetOrDefault(new char[0]))
                           .ToArray()));
        }
    }
}
