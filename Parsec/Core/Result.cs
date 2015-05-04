using System;

namespace Parsec.Core
{
    public interface IResult<out TToken, out TOutput> : IEither<IError, TOutput>
    {
        T Match<T>(
            Func<ITokenStream<TToken>, IError, T> failure,
            Func<ITokenStream<TToken>, TOutput, T> success);
    }

    public static class Result
    {
        public static IResult<TToken, TOutput> Success<TToken, TOutput>(
            ITokenStream<TToken> restStream,
            TOutput output)
        {
            return new SuccessImpl<TToken, TOutput>(restStream, output);
        }

        public static bool Success<TToken, TOutput>(this IResult<TToken, TOutput> result)
        {
            return result.Match(
                success: (restStream, output) => true,
                failure: (restStream, error) => false);
        }

        public static IResult<TToken, TOutput> Failure<TToken, TOutput>(
            ITokenStream<TToken> restStream,
            IError error)
        {
            return new FailureImpl<TToken, TOutput>(restStream, error);
        }

        private sealed class FailureImpl<TToken, TOutput> : IResult<TToken, TOutput>
        {
            private readonly IError _error;
            private readonly ITokenStream<TToken> _restStream;

            internal FailureImpl(ITokenStream<TToken> restStream, IError error)
            {
                _error = error;
                _restStream = restStream;
            }

            public T Match<T>(
                Func<IError, T> failure,
                Func<TOutput, T> success)
            {
                return failure(_error);
            }

            public T Match<T>(
                Func<ITokenStream<TToken>, IError, T> failure,
                Func<ITokenStream<TToken>, TOutput, T> success)
            {
                return failure(_restStream, _error);
            }
        }

        private sealed class SuccessImpl<TToken, TOutput> : IResult<TToken, TOutput>
        {
            private readonly TOutput _output;
            private readonly ITokenStream<TToken> _restStream;

            internal SuccessImpl(ITokenStream<TToken> restStream, TOutput output)
            {
                _output = output;
                _restStream = restStream;
            }

            public T Match<T>(
                Func<IError, T> failure,
                Func<TOutput, T> success)
            {
                return success(_output);
            }

            public T Match<T>(
                Func<ITokenStream<TToken>, IError, T> failure,
                Func<ITokenStream<TToken>, TOutput, T> success)
            {
                return success(_restStream, _output);
            }
        }
    }
}
