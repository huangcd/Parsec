using System;

namespace Parsec.Core
{
    public interface IOptional<out TValue>
    {
        T Match<T>(Func<TValue, T> exists, Func<T> nothing);
    }

    public static class Optional
    {
        public static TValue GetOrDefault<TValue>(this IOptional<TValue> optional, TValue defaultValue)
        {
            return optional.Match(
                exists: value => value,
                nothing: () => defaultValue);
        }

        public static Boolean HasValue<TValue>(this IOptional<TValue> optional)
        {
            return optional.Match(
                exists: val => true,
                nothing: () => false);
        }

        public static TValue GetValue<TValue>(this IOptional<TValue> optional)
        {
            return optional.Match(
                exists: val => val,
                nothing: () => { throw new Exception("Cannot get value on Nothing"); });
        }

        public static IOptional<TValue> Just<TValue>(TValue value)
        {
            return new JustImpl<TValue>(value);
        }

        public static IOptional<TValue> Nothing<TValue>()
        {
            return new NothingImpl<TValue>();
        }

        private sealed class NothingImpl<TValue> : IOptional<TValue>
        {
            public T Match<T>(Func<TValue, T> exists, Func<T> nothing)
            {
                return nothing();
            }

            public override string ToString()
            {
                return String.Format("[Optional Nothing]");
            }
        }

        private sealed class JustImpl<TValue> : IOptional<TValue>
        {
            private readonly TValue _val;

            internal JustImpl(TValue value)
            {
                _val = value;
            }

            public T Match<T>(Func<TValue, T> exists, Func<T> nothing)
            {
                return exists(_val);
            }

            public override string ToString()
            {
                return String.Format("[Optional {0}]", _val);
            }
        }
    }
}