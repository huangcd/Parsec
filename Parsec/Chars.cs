using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Core;

namespace Parsec
{
    public static class Chars
    {
        private sealed class PlainCharStream : ITokenStream<Char>
        {
            private readonly Char[] _chars;
            private readonly int _position;

            internal PlainCharStream(IEnumerable<Char> chars)
            {
                _chars = chars.ToArray();
                _position = 0;
            }

            private PlainCharStream(Char[] chars, int position)
            {
                _chars = chars;
                _position = position;
            }

            public ITokenStream<Char> MoveNext()
            {
                return new PlainCharStream(_chars, _position + 1);
            }

            public IOptional<Char> Current
            {
                get
                {
                    return _position < _chars.Length
                        ? Optional.Just(_chars[_position])
                        : Optional.Nothing<Char>();
                }
            }

            public override string ToString()
            {
                return new String(_chars, _position, _chars.Length - _position);
            }
        }

        /// <summary>
        /// Convert a list of char to ITokenStream
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static ITokenStream<Char> AsPlainCharStream(this IEnumerable<Char> chars)
        {
            return new PlainCharStream(chars);
        }

        /// <summary>
        /// Return a parser to check whether the given stream is consumed
        /// </summary>
        /// <returns></returns>
        public static Parser<Char, Nothing> EndOfInput()
        {
            return stream =>
                stream.Current.Match(
                    exists: token => Failure<Nothing>(stream, "Unconsumed tokens"),
                    nothing: () => Result.Success(stream.MoveNext(), Nothing.Instance));
        }

        public static Parser<Char, IList<Char>> Sequance(IEnumerable<Char> seq)
        {
            return seq.Select(One).Sequence();
        }

        public static Parser<Char, Char> One(Char c)
        {
            return Satisfy(token => token == c);
        }

        public static Parser<Char, Char> HexDigit()
        {
            return Satisfy(c => ('0' <= c && c <= '9') || ('A' <= c && c <= 'F') || ('a' <= c && c <= 'f'));
        }

        public static Parser<Char, Char> Letter()
        {
            return Satisfy(System.Char.IsLetter);
        }

        public static Parser<Char, Char> Digit()
        {
            return Satisfy(System.Char.IsDigit);
        }

        public static Parser<Char, Char> LetterOrDigit()
        {
            return Satisfy(System.Char.IsLetterOrDigit);
        }

        public static Parser<Char, Char> Not(Char c)
        {
            return Satisfy(token => token != c);
        }

        public static Parser<Char, Char> Satisfy(Func<Char, Boolean> pred)
        {
            return stream => stream.Current.Match(
                exists: token => pred(token)
                    ? Result.Success(stream.MoveNext(), token)
                    : Failure<Char>(stream, "Not Satisfy"),
                nothing: () => EndOfInput<Char>(stream));
        }

        public static Parser<Char, Char> Any(this IEnumerable<Char> chars)
        {
            var set = chars.ToLookup(c => c);
            return Satisfy(set.Contains);
        }

        public static Parser<Char, Char> NoneOf(this IEnumerable<Char> chars)
        {
            var set = chars.ToLookup(c => c);
            return Satisfy(token => !set.Contains(token));
        }

        public static IResult<Char, TOutput> Failure<TOutput>(ITokenStream<Char> stream, String reason)
        {
            return Result.Failure<Char, TOutput>(stream, Error.Create(reason));
        }

        public static IResult<Char, TOutput> EndOfInput<TOutput>(ITokenStream<Char> stream)
        {
            return Failure<TOutput>(stream, "End of input");
        }
    }
}
