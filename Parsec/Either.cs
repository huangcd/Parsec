using System;

namespace Parsec
{
    public abstract class Either<TLeft, TRight>
    {
        protected Either()
        {
        }

        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left)
        {
            return new EitherLeft<TLeft, TRight>(left);
        }

        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right)
        {
            return new EitherRight<TLeft, TRight>(right);
        }

        public abstract T Match<T>(Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc);

        public sealed class EitherLeft<TLeft, TRight> : Either<TLeft, TRight>
        {
            private readonly TLeft _left;

            internal EitherLeft(TLeft left)
            {
                _left = left;
            }

            public override T Match<T>(Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc)
            {
                return leftFunc(_left);
            }
        }

        public sealed class EitherRight<TLeft, TRight> : Either<TLeft, TRight>
        {
            private readonly TRight _right;

            internal EitherRight(TRight right)
            {
                _right = right;
            }

            public override T Match<T>(Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc)
            {
                return rightFunc(_right);
            }
        }
    }
}