using System;
using System.Collections.Generic;
using Parsec.Core;

namespace Parsec
{
    public static class Chars
    {
        private sealed class PlainCharStream : ITokenStream<Char, Int32>
        {
            private readonly IEnumerator<Char> _iter;
            private Boolean _hasNext;
            private int _position;

            internal PlainCharStream(IEnumerator<Char> iter)
            {
                _iter = iter;
                _position = 0;
            }

            public ITokenStream<Char, Int32> MoveNext()
            {
                _hasNext = _iter.MoveNext();
                _position++;
                return this;
            }

            public IOptional<IPair<Char, Int32>> Current
            {
                get
                {
                    return _hasNext
                        ? Optional.Just(Pair.Create(_iter.Current, _position))
                        : Optional.Nothing<IPair<Char, Int32>>();
                }
            }
        }

        public static ITokenStream<Char, Int32> AsPlainCharStream(this IEnumerable<Char> chars)
        {
            return new PlainCharStream(chars.GetEnumerator()).MoveNext();
        }

        public static Parser<Char, Nothing, TPos> EndOfInput<TPos>()
        {
            return stream =>
                stream.Current.Match(
                    exists: pair => Result.Failure<Char, Nothing, TPos>(stream.MoveNext(), Error.Create(pair.Pos, "Unconsumed tokens")),
                    nothing: () => Result.Success(stream, Nothing.Instance));
        }

        public static Parser<Char, Char, TPos> Char<TPos>(Char c)
        {
            return stream =>
                stream.Current.Match(
                    exists: pair => Result.Success(stream.MoveNext(), pair.Token),
                    nothing: () => Result.Failure<Char, Char, TPos>(stream.MoveNext(), Error.EndOfInput("End of input")));
        }
    }
}
