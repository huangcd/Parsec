using System;

namespace Parsec
{
    public abstract class Maybe<TValue> : Either<TValue, Null>
    {
        public static JustMaybe<TValue> Just<TValue>(TValue value)
        {
            return new JustMaybe<TValue>(value);
        }

        public static NothingMaybe Noting()
        {
            return new NothingMaybe();
        }

        public sealed class NothingMaybe : Maybe<Null>
        {
            internal NothingMaybe()
            {
            }

            public override T Match<T>(Func<Null, T> leftFunc, Func<Null, T> rightFunc)
            {
                return rightFunc(Null.Instance);
            }
        }

        public sealed class JustMaybe<TValue> : Maybe<TValue>
        {
            private readonly TValue _val;

            internal JustMaybe(TValue value)
            {
                _val = value;
            }

            public override T Match<T>(Func<TValue, T> leftFunc, Func<Null, T> rightFunc)
            {
                return leftFunc(_val);
            }
        }
    }
}