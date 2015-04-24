using System;

namespace Parsec.Core
{
    public interface IError
    {
        String Message { get; }
    }

    public interface IError<out TPos> : IError
    {
        /// <summary>
        /// Get the position which error happends
        /// </summary>
        TPos Pos { get; }
    }

    public static class Error
    {
        public static IError<TPos> Create<TPos>(TPos pos, String message)
        {
            return new ErrorImpl<TPos>(pos, message);
        }

        public static IError<Nothing> EndOfInput(String message)
        {
            return new EndOfInputImpl(message);
        }

        private sealed class EndOfInputImpl : IError<Nothing>
        {
            internal EndOfInputImpl(String message)
            {
                Message = message;
            }

            public string Message { get; private set; }

            public Nothing Pos { get { return Nothing.Instance; } }
        }

        private sealed class ErrorImpl<TPos> : IError<TPos>
        {
            internal ErrorImpl(TPos pos, String message)
            {
                Pos = pos;
                Message = message;
            }

            public TPos Pos { get; private set; }

            public string Message { get; private set; }
        }
    }
}