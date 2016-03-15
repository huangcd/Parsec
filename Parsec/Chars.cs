using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Core;

namespace Parsec
{
    public static class Chars
    {
        private sealed class PlainCharStream : ITokenStream<char>
        {
            private readonly char[] _chars;
            private readonly int _position;

            internal PlainCharStream(IEnumerable<char> chars)
            {
                _chars = chars.ToArray();
                _position = 0;
            }

            private PlainCharStream(char[] chars, int position)
            {
                _chars = chars;
                _position = position;
            }

            public ITokenStream<char> MoveNext()
            {
                return new PlainCharStream(_chars, _position + 1);
            }

            public IOptional<char> Current
            {
                get
                {
                    return _position < _chars.Length
                        ? Optional.Just(_chars[_position])
                        : Optional.Nothing<char>();
                }
            }

            public override string ToString()
            {
                return new string(_chars, _position, _chars.Length - _position);
            }
        }

        /// <summary>
        /// Convert a list of char to ITokenStream
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public static ITokenStream<char> AsPlainCharStream(this IEnumerable<char> chars)
        {
            return new PlainCharStream(chars);
        }

        /// <summary>
        /// Return a parser to check whether the given stream is consumed
        /// </summary>
        /// <returns></returns>
        public static Parser<char, Nothing> EndOfInput()
        {
            return stream =>
                stream.Current.Match(
                    exists: token => Failure<Nothing>(stream, "Unconsumed tokens"),
                    nothing: () => Result.Success(stream.MoveNext(), Nothing.Instance));
        }

        public static Parser<char, IList<char>> Sequance(IEnumerable<char> seq)
        {
            return seq.Select(One).Sequence();
        }

        public static Parser<char, char> One(char c)
        {
            return Satisfy(token => token == c);
        }

        public static Parser<char, char> HexDigit()
        {
            return Satisfy(c => ('0' <= c && c <= '9') || ('A' <= c && c <= 'F') || ('a' <= c && c <= 'f'));
        }

        public static Parser<char, char> Letter()
        {
            return Satisfy(char.IsLetter);
        }

        public static Parser<char, char> Digit()
        {
            return Satisfy(char.IsDigit);
        }

        public static Parser<char, char> LetterOrDigit()
        {
            return Satisfy(char.IsLetterOrDigit);
        }

        public static Parser<char, char> Not(char c)
        {
            return Satisfy(token => token != c);
        }

        public static Parser<char, char> Satisfy(Func<char, bool> pred)
        {
            return stream => stream.Current.Match(
                exists: token => pred(token)
                    ? Result.Success(stream.MoveNext(), token)
                    : Failure<char>(stream, "Not Satisfy"),
                nothing: () => EndOfInput<char>(stream));
        }

        public static Parser<char, char> OneOf(this IEnumerable<char> chars)
        {
            var set = chars.ToLookup(c => c);
            return Satisfy(set.Contains);
        }

        public static Parser<char, char> NoneOf(this IEnumerable<char> chars)
        {
            var set = chars.ToLookup(c => c);
            return Satisfy(token => !set.Contains(token));
        }

        public static Parser<char, char> Space()
        {
            return Satisfy(char.IsWhiteSpace);
        }

        public static Parser<char, Nothing> Spaces()
        {
            return Space().SkipMany();
        }

        public static IResult<char, TOutput> Failure<TOutput>(ITokenStream<char> stream, string reason)
        {
            return Result.Failure<char, TOutput>(stream, Error.Create(reason));
        }

        public static IResult<char, TOutput> EndOfInput<TOutput>(ITokenStream<char> stream)
        {
            return Failure<TOutput>(stream, "End of input");
        }
    }
}
