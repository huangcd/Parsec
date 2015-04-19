using System;

namespace Parsec
{
    public interface IMonad<T>
    {
    }

    public interface IStream
    {
        Char ReadChar();
    }

    public interface IParsecT<in TStream, in TUserData, in TMonad, TOut> : IMonad<TOut>
        where TStream : IStream
    {
    }
}