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

        public static ITokenStream<Char> AsPlainCharStream(this IEnumerable<Char> chars)
        {
            return new PlainCharStream(chars);
        }

        public static Parser<Char, Nothing> EndOfInput()
        {
            return stream =>
                stream.Current.Match(
                    exists: token => Failure<Nothing>(stream, "Unconsumed tokens"),
                    nothing: () => Result.Success(stream.MoveNext(), Nothing.Instance));
        }

        public static Parser<Char, IList<Char>> Sequance(IEnumerable<Char> seq)
        {
            return seq.Select(Char).Sequence();
        }

        public static Parser<Char, Char> Char(Char c)
        {
            return stream =>
                stream.Current.Match(
                    exists: token => token == c
                        ? Result.Success(stream.MoveNext(), token)
                        : Failure<Char>(stream, "Char not match"),
                    nothing: () => EndOfInput<Char>(stream));
        }

        public static Parser<Char, Char> Any(IEnumerable<Char> chars)
        {
            var set = chars.ToLookup(c => c);
            return stream =>
                stream.Current.Match(
                    exists: token => set.Contains(token)
                        ? Result.Success(stream.MoveNext(), token)
                        : Failure<Char>(stream, "No match"),
                    nothing: () => EndOfInput<Char>(stream));
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
