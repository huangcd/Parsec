﻿using System;
using System.Globalization;
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

        public static Parser<Char, JValue> String()
        {
            var tokens = Combinators.Any(
                "\\\"".NoneOf(),
                QuotationMark,
                SlashMark,
                ReverseSlashMark,
                BackSpaceMark,
                FormfeedMark,
                NewLineMark,
                CarriageReturnMark,
                TabMark,
                Unicode);
            return from start in Quote
                   from chars in tokens.Repeat()
                   from end in Quote
                   select (JValue)(new String(chars));
        }
    }
}
