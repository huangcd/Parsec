using System;

namespace Parsec.Core
{

    public interface IResult<out TToken, out TOutput, out TPos> : IEither<IError, TOutput>
    {
        T Match<T>(
            Func<ITokenStream<TToken, TPos>, IError, T> failure,
            Func<ITokenStream<TToken, TPos>, TOutput, T> success);
    }

    public static class Result
    {
        public static IResult<TToken, TOutput, TPos> Success<TToken, TOutput, TPos>(
            ITokenStream<TToken, TPos> restStream,
            TOutput output)
        {
            return new SuccessImpl<TToken, TOutput, TPos>(restStream, output);
        }

        public static IResult<TToken, TOutput, TPos> Failure<TToken, TOutput, TPos>(
            ITokenStream<TToken, TPos> restStream,
            IError error)
        {
            return new FailureImpl<TToken, TOutput, TPos>(restStream, error);
        }

        private sealed class FailureImpl<TToken, TOutput, TPos> : IResult<TToken, TOutput, TPos>
        {
            private readonly IError _error;
            private readonly ITokenStream<TToken, TPos> _restStream;

            internal FailureImpl(ITokenStream<TToken, TPos> restStream, IError error)
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
                Func<ITokenStream<TToken, TPos>, IError, T> failure, 
                Func<ITokenStream<TToken, TPos>, TOutput, T> success)
            {
                return failure(_restStream, _error);
            }
        }

        private sealed class SuccessImpl<TToken, TOutput, TPos> : IResult<TToken, TOutput, TPos>
        {
            private readonly TOutput _output;
            private readonly ITokenStream<TToken, TPos> _restStream;

            internal SuccessImpl(ITokenStream<TToken, TPos> restStream, TOutput output)
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
                Func<ITokenStream<TToken, TPos>, IError, T> failure, 
                Func<ITokenStream<TToken, TPos>, TOutput, T> success)
            {
                return success(_restStream, _output);
            }
        }
    }
}
