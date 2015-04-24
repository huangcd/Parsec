using System;

namespace Parsec.Core
{
    public interface IEither<out TLeft, out TRight>
    {
        T Match<T>(Func<TLeft, T> left, Func<TRight, T> right);
    }

    public abstract class Either
    {

        public static IEither<TLeft, TRight> Left<TLeft, TRight>(TLeft left)
        {
            return new EitherLeft<TLeft, TRight>(left);
        }

        public static IEither<TLeft, TRight> Right<TLeft, TRight>(TRight right)
        {
            return new EitherRight<TLeft, TRight>(right);
        }

        private sealed class EitherLeft<TLeft, TRight> : IEither<TLeft, TRight>
        {
            private readonly TLeft _left;

            internal EitherLeft(TLeft left)
            {
                _left = left;
            }

            public T Match<T>(Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc)
            {
                return leftFunc(_left);
            }
        }

        private sealed class EitherRight<TLeft, TRight> : IEither<TLeft, TRight>
        {
            private readonly TRight _right;

            internal EitherRight(TRight right)
            {
                _right = right;
            }

            public T Match<T>(Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc)
            {
                return rightFunc(_right);
            }
        }
    }
}