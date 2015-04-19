using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsec
{
    internal class Parsec
    {
        public const String AllLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const String AllDigits = "0123456789";

        public static IParsecT<TStream, TU, TM, Char> Digit<TStream, TU, TM>()
            where TStream : IStream
        {
            return OneOf<TStream, TU, TM>(AllDigits);
        }

        public static IParsecT<TStream, TU, TM, Char> Letter<TStream, TU, TM>()
            where TStream : IStream
        {
            return OneOf<TStream, TU, TM>(AllLetters);
        }

        public static IParsecT<TStream, TU, TM, Char> Satisfy<TStream, TU, TM>(
            Func<Char, Boolean> func)
            where TStream : IStream
        {
            return null;
        }

        public static IParsecT<TStream, TU, TM, Char> One<TStream, TU, TM>(
            Char c)
            where TStream : IStream
        {
            return null;
        }

        public static IParsecT<TStream, TU, TM, Char> OneOf<TStream, TU, TM>(
            IEnumerable<Char> chars)
            where TStream : IStream
        {
            return Satisfy<TStream, TU, TM>(chars.Contains);
        }

        public static IParsecT<TStream, TU, TM, Char> NoneOf<TStream, TU, TM>(
            IEnumerable<Char> chars)
            where TStream : IStream
        {
            return Satisfy<TStream, TU, TM>(c => !chars.Contains(c));
        }

        public static IParsecT<TStream, TU, TM, TType> Choice<TStream, TU, TM, TType>(
            IParsecT<TStream, TU, TM, TType> first,
            IParsecT<TStream, TU, TM, TType> second)
            where TStream : IStream
        {
            return null;
        }

        public static IParsecT<TStream, TU, TM, TType> Choice<TStream, TU, TM, TType>(
            params IParsecT<TStream, TU, TM, TType>[] first)
            where TStream : IStream
        {
            return null;
        }

        public static IParsecT<TStream, TU, TM, Null> SkipMany1<TStream, TU, TM, TType>(
            IParsecT<TStream, TU, TM, TType> input)
            where TStream : IStream
        {
            return null;
        }

        public static IParsecT<TStream, TU, TM, IEnumerable<TType>> Many<TStream, TU, TM, TType>(
            IParsecT<TStream, TU, TM, TType> input)
            where TStream : IStream
        {
            return null;
        }

        public static Func<String, TStream, Either<Exception, TResult>> Parse<TStream, TU, TM, TResult>(
            IParsecT<TStream, TU, TM, TResult> parsec)
            where TStream : IStream
        {
            return (name, stream) => Parse(parsec, name, stream);
        }

        public static Func<TStream, Either<Exception, TResult>> Parse<TStream, TU, TM, TResult>(
            IParsecT<TStream, TU, TM, TResult> parsec,
            String name)
            where TStream : IStream
        {
            return stream => Parse(parsec, name, stream);
        }

        public static Either<Exception, TResult> Parse<TStream, TU, TM, TResult>(
            IParsecT<TStream, TU, TM, TResult> parsec,
            String name,
            TStream input)
            where TStream : IStream
        {
            return null;
        }
    }
}